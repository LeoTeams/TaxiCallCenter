using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TaxiCallCenter.MVP.WpfApp.Extensions;
using TaxiCallCenter.MVP.WpfApp.Models;

namespace TaxiCallCenter.MVP.WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Int32 count;

        public MainWindow()
        {
            this.ViewModel = new MainViewModel(this);
            this.DataContext = this.ViewModel;
            this.InitializeComponent();
        }

        protected MainViewModel ViewModel { get; }

        private void PushToTalk_OnClick(Object sender, RoutedEventArgs e)
        {
            this.count++;
            this.Title = $"Listening... {this.count}";
        }

        private void PushToTalk_OnHoldStarted(Object sender, EventArgs e)
        {
            this.count = 0;
            this.Title = $"Listening... {this.count}";
            this.ViewModel.AudioRecorder.StartRecording();
        }

        private void PushToTalk_OnHoldReleased(Object sender, EventArgs e)
        {
            this.ViewModel.AudioRecorder.StopRecording();
            this.Title = $"Waiting...";
        }

        private async void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            //await this.ViewModel.InitAsync();
        }

        private async void Send_Click(Object sender, RoutedEventArgs e)
        {
            await this.ProcessSend();
        }

        private async void Chat_KeyUp(Object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                await this.ProcessSend();
            }
        }

        private async Task ProcessSend()
        {
            await this.ViewModel.ProcessManualInput(this.ChatTextBox.Text);
            this.ChatTextBox.Text = "";
        }

        private async void StartButton_Click(Object sender, RoutedEventArgs e)
        {
            await this.ViewModel.InitAsync();
        }
    }
}
