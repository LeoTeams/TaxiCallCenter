using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace TaxiCallCenter.MVP.WpfApp
{
    public class AudioPlayer
    {
        private readonly MainViewModel mainViewModel;

        public AudioPlayer(MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
        }

        public void PlayBytes(Byte[] bytes)
        {
            var deviceId = this.mainViewModel.SelectedOutputDevice?.Id ?? -1;
            if (deviceId != -1)
            {
                var recordingStream = new MemoryStream(bytes);
                var reader = new WaveFileReader(recordingStream);
                var waveOut = new WaveOut();
                waveOut.PlaybackStopped += (sender2, args) =>
                {
                    waveOut.Dispose();
                    reader.Dispose();
                    recordingStream.Dispose();
                };
                waveOut.DeviceNumber = deviceId;
                waveOut.Init(reader);
                waveOut.Play();
            }
        }
    }
}
