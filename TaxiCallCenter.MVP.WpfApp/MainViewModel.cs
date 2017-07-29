using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using NAudio.Wave;
using TaxiCallCenter.MVP.WpfApp.Client;
using TaxiCallCenter.MVP.WpfApp.Events;
using TaxiCallCenter.MVP.WpfApp.Extensions;
using TaxiCallCenter.MVP.WpfApp.Models;

namespace TaxiCallCenter.MVP.WpfApp
{
    public class MainViewModel : BaseViewModel
    {
        private readonly Window window;
        private readonly SpeechKitClient speechKitClient = new SpeechKitClient("ceeefb0b-8573-4279-9180-898867ad72e1");
        private readonly Guid userId = Guid.NewGuid();

        private String speechTopic = "queries";
        private AudioDevice selectedInputDevice;
        private AudioDevice selectedOutputDevice;
        private TtsSpeaker selectedSpeaker;
        private TtsEmotion selectedEmotion;
        private String callerPhone = "+7 912 345 67 89";
        private Boolean acceptOrder;

        public event EventHandler<AccessForRecording> ListenAccessed;

        public MainViewModel(Window window)
        {
            this.window = window;

            this.AudioRecorder = new AudioRecorder(this);
            this.AudioPlayer = new AudioPlayer(this);
            this.AudioSaver = new AudioSaver();

            this.AudioRecorder.RecordingComplete += this.AudioRecorderOnRecordingComplete;
            this.ListenAccessed += this.AudioRecorder.AccessForListenRecived;

            var waveInDevices = WaveIn.DeviceCount;
            for (var deviceId = 0; deviceId < waveInDevices; deviceId++)
            {
                var deviceInfo = WaveIn.GetCapabilities(deviceId);
                this.InputDevices.Add(new AudioDevice
                {
                    Id = deviceId,
                    Name = deviceInfo.ProductName
                });
                ////this.Log.LogEvent($"Device {deviceId}: {deviceInfo.ProductName}, {deviceInfo.Channels} channels");
            }

            this.SelectedInputDevice = this.InputDevices.FirstOrDefault();

            this.OutputDevices.Add(new AudioDevice
            {
                Id = -1,
                Name = "Без синтеза речи"
            });
            var waveOutDevices = WaveOut.DeviceCount;
            for (var deviceId = 0; deviceId < waveOutDevices; deviceId++)
            {
                var deviceInfo = WaveOut.GetCapabilities(deviceId);
                this.OutputDevices.Add(new AudioDevice
                {
                    Id = deviceId,
                    Name = deviceInfo.ProductName
                });
                ////this.Log.LogEvent($"Device {deviceId}: {deviceInfo.ProductName}, {deviceInfo.Channels} channels");
            }

            this.SelectedOutputDevice = this.OutputDevices.Skip(1).FirstOrDefault();

            this.Speakers.Add(new TtsSpeaker { Name = "jane" });
            this.Speakers.Add(new TtsSpeaker { Name = "oksana" });
            this.Speakers.Add(new TtsSpeaker { Name = "alyss" });
            this.Speakers.Add(new TtsSpeaker { Name = "omazh" });
            this.SelectedSpeaker = this.Speakers[1];

            this.Emotions.Add(new TtsEmotion { Name = "good" });
            this.Emotions.Add(new TtsEmotion { Name = "evil" });
            this.Emotions.Add(new TtsEmotion { Name = "neutral" });
            this.SelectedEmotion = this.Emotions[0];

            this.OrderStateMachine = new OrderStateMachine(new SpeechSubsystem(this), new Logger(this), new OrdersService(this));
        }

        private async void AudioRecorderOnRecordingComplete(Object sender, RecordingCompleteEventArgs e)
        {
            // TODO: add button replay after speach 
            //this.AudioPlayer.PlayBytes(e.RecordingBytes);
            this.Log.LogEvent("Sending data for recognition");
            var recognitionResults = await this.RecognizeAsync(e.RecordingBytes);
           
            if (recognitionResults.Success && recognitionResults.Variants.Any())
            {
                var result = recognitionResults.Variants.First();
                this.Log.LogEvent($"Recognized text '{result.Text}' (confidence - {result.Confidence:N4})");
                await this.OrderStateMachine.ProcessResponseAsync(result.Text);
            }
            else
            {
                this.Log.LogEvent($"Recognition failed");
                await this.OrderStateMachine.ProcessRecognitionFailure();
            }

            
        }

        public ObservableCollection<LogEntry> Log { get; } = new ObservableCollection<LogEntry>();

        public ObservableCollection<AudioDevice> InputDevices { get; } = new ObservableCollection<AudioDevice>();

        public ObservableCollection<AudioDevice> OutputDevices { get; } = new ObservableCollection<AudioDevice>();

        public ObservableCollection<TtsSpeaker> Speakers { get; } = new ObservableCollection<TtsSpeaker>();

        public ObservableCollection<TtsEmotion> Emotions { get; } = new ObservableCollection<TtsEmotion>();

        public AudioDevice SelectedInputDevice
        {
            get => this.selectedInputDevice;
            set
            {
                if (Equals(value, this.selectedInputDevice)) return;
                this.OnPropertyChanging();
                this.selectedInputDevice = value;
                this.OnPropertyChanged();
            }
        }

        public AudioDevice SelectedOutputDevice
        {
            get => this.selectedOutputDevice;
            set
            {
                if (Equals(value, this.selectedOutputDevice)) return;
                this.OnPropertyChanging();
                this.selectedOutputDevice = value;
                this.OnPropertyChanged();
            }
        }

        public TtsSpeaker SelectedSpeaker
        {
            get => this.selectedSpeaker;
            set
            {
                if (Equals(value, this.selectedSpeaker)) return;
                this.OnPropertyChanging();
                this.selectedSpeaker = value;
                this.OnPropertyChanged();
            }
        }

        public TtsEmotion SelectedEmotion
        {
            get => this.selectedEmotion;
            set
            {
                if (Equals(value, this.selectedEmotion)) return;
                this.OnPropertyChanging();
                this.selectedEmotion = value;
                this.OnPropertyChanged();
            }
        }

        public String CallerPhone
        {
            get => this.callerPhone;
            set
            {
                if (value == this.callerPhone) return;
                this.OnPropertyChanging();
                this.callerPhone = value;
                this.OnPropertyChanged();
            }
        }

        public Boolean AcceptOrder
        {
            get => this.acceptOrder;
            set
            {
                if (value == this.acceptOrder) return;
                this.OnPropertyChanging();
                this.acceptOrder = value;
                this.OnPropertyChanged();
            }
        }

        public AudioRecorder AudioRecorder { get; }

        public AudioPlayer AudioPlayer { get; }

        public AudioSaver AudioSaver { get; }

        public OrderStateMachine OrderStateMachine { get; set; }

        public TaximeterService TaximeterService { get; set; }

        public void OnLisetenAccess()
        {
            this.ListenAccessed?.Invoke(this, new AccessForRecording());
        }

        public async Task InitializeTaximiter()
        {
            this.TaximeterService = new TaximeterService();
            await this.TaximeterService.InitializeAsync();
        }

        public async Task SpeakAsync(String text)
        {
            this.Log.LogEvent($"Syntesizing text '{text}'");
            if ((this.SelectedOutputDevice?.Id ?? -1) != -1)
            {
                var audio = await this.speechKitClient.GenerateAsync(this.SelectedSpeaker.Name, this.SelectedEmotion.Name, text);
                this.Log.LogEvent($"Received syntesized text: {audio.Length} bytes");
                this.AudioSaver.SaveBytes(this.userId, "Syntesized", Guid.NewGuid(), audio);
                this.AudioPlayer.PlayBytes(audio);
            }
            // TODO: Refactor lifecycle of view. Change this access to recognize and speach completed tasks.
            this.OnLisetenAccess();
        }

        public async Task<RecognitionResults> RecognizeAsync(Byte[] audioBytes)
        {
            try
            {
                var result = await this.speechKitClient.RecognizeAsync(this.userId, this.speechTopic, audioBytes);
                var xml = XElement.Parse(result);
                if (xml.Name != "recognitionResults")
                {
                    throw new InvalidOperationException();
                }

                var results = new RecognitionResults();
                if (xml.Attribute("success")?.Value == "1")
                {
                    results.Success = true;
                    foreach (var element in xml.Elements("variant"))
                    {
                        var confidence = element.Attribute("confidence")?.Value ?? "0";
                        confidence = confidence.Replace('.', ',');
                        var text = element.Value.Trim();
                        results.Variants.Add(new RecognitionVariant
                        {
                            Confidence = Double.Parse(confidence),
                            Text = text
                        });
                    }
                }
                return results;
            }
            catch (FormatException exception)
            {
                Log.LogEvent(exception.Message);
                var results = new RecognitionResults();
                return results;
            }
                
        }

        public async Task InitAsync()
        {
            this.OrderStateMachine = new OrderStateMachine(new SpeechSubsystem(this), new Logger(this), new OrdersService(this));
            await this.OrderStateMachine.Initialize(this.CallerPhone);
        }

        public async Task ProcessManualInput(String text)
        {
            this.Log.LogEvent($"Manual input: '{text}'");
            await this.OrderStateMachine.ProcessResponseAsync(text);
        }

        private class SpeechSubsystem : ISpeechSubsystem
        {
            private readonly MainViewModel mainViewModel;

            public SpeechSubsystem(MainViewModel mainViewModel)
            {
                this.mainViewModel = mainViewModel;
            }

            public Task SpeakAsync(String text)
            {
                return this.mainViewModel.SpeakAsync(text);
            }

            public void SetRecognitionMode(String mode)
            {
                this.mainViewModel.speechTopic = mode;
            }

            public void StopCommunication()
            {
            }
        }

        private class Logger : ILogger
        {
            private readonly MainViewModel mainViewModel;

            public Logger(MainViewModel mainViewModel)
            {
                this.mainViewModel = mainViewModel;
            }

            public void LogEvent(String eventText)
            {
                this.mainViewModel.Log.LogEvent(eventText);
            }
        }

        private class OrdersService : IOrdersService
        {
            private readonly MainViewModel mainViewModel;

            public OrdersService(MainViewModel mainViewModel)
            {
                this.mainViewModel = mainViewModel;
            }

            public async Task CreateOrderAsync(OrderInfo order)
            {
                if (this.mainViewModel.TaximeterService == null)
                {
                    this.mainViewModel.window.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(this.mainViewModel.window, $@"Откуда: {order.AddressFromStreet}, {order.AddressFromHouse}
Куда: {order.AddressToStreet}, {order.AddressToHouse}
Дата и время: {order.DateTime:yyyy-MM-dd HH:mm}
Телефон: {order.Phone}
Дополнительные пожелания: {order.AdditionalInfo}");
                    });
                }
                else
                {
                    await this.mainViewModel.TaximeterService.MakeOrderAsync(order, this.mainViewModel.AcceptOrder);
                }
            }
        }
    }
}
