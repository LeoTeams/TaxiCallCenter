using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using TaxiCallCenter.MVP.WpfApp.Events;
using TaxiCallCenter.MVP.WpfApp.Extensions;
using TaxiCallCenter.MVP.WpfApp.Models;

namespace TaxiCallCenter.MVP.WpfApp
{
    public class AudioRecorder
    {
        private readonly MainViewModel mainViewModel;

        private WaveIn waveIn;
        private WaveFileWriter waveWriter;
        private MemoryStream waveStream;
        private DateTime recordingStarted;

        public AudioRecorder(MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
        }

        public event EventHandler<RecordingCompleteEventArgs> RecordingComplete;

        public void StartRecording()
        {
            if (this.waveIn != null)
            {
                return;
            }

            this.mainViewModel.Log.LogEvent("Recording started");
            this.waveIn = new WaveIn();
            this.waveIn.DeviceNumber = this.mainViewModel.SelectedInputDevice?.Id ?? 0;
            this.waveIn.WaveFormat = new WaveFormat(16000, 16, 1);
            this.waveIn.DataAvailable += this.WaveInHandleDataAvailable;
            this.waveIn.RecordingStopped += this.WaveInHandleRecordingStopped;

            this.waveStream = new MemoryStream();
            this.waveWriter = new WaveFileWriter(this.waveStream, this.waveIn.WaveFormat);

            this.recordingStarted = DateTime.UtcNow;
            this.waveIn.StartRecording();
        }

        private void WaveInHandleRecordingStopped(Object sender, StoppedEventArgs stoppedEventArgs)
        {
            this.waveIn.Dispose();
            this.waveIn = null;
            this.waveWriter.Dispose();
            this.waveWriter = null;
            var recordingBytes = this.waveStream.ToArray();
            this.waveStream = null;
            this.mainViewModel.Log.LogEvent($"Recording stopped. Duration: {(DateTime.UtcNow - this.recordingStarted).TotalMilliseconds:N0} ms. Size: {recordingBytes.Length} bytes");
            this.OnRecordingComplete(recordingBytes);
        }

        private void WaveInHandleDataAvailable(Object sender, WaveInEventArgs e)
        {
            if (this.waveWriter != null)
            {
                this.waveWriter.Write(e.Buffer, 0, e.BytesRecorded);
                this.waveWriter.Flush();
            }
        }

        public void StopRecording()
        {
            this.waveIn.StopRecording();
        }

        protected virtual void OnRecordingComplete(Byte[] recordingBytes)
        {
            this.RecordingComplete?.Invoke(this, new RecordingCompleteEventArgs(recordingBytes));
        }
    }
}
