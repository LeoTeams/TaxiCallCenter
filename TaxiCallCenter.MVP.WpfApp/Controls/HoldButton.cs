using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace TaxiCallCenter.MVP.WpfApp.Controls
{
    public class HoldButton : RepeatButton
    {
        private DispatcherTimer timer;
        private DateTime lastTimePressed = DateTime.UtcNow.AddYears(-1);

        public HoldButton()
        {
            this.Click += this.HandleClick;
        }

        public event EventHandler<EventArgs> HoldStarted;

        public event EventHandler<EventArgs> HoldReleased;

        protected virtual void OnHoldStarted()
        {
            this.HoldStarted?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnHoldReleased()
        {
            this.HoldReleased?.Invoke(this, EventArgs.Empty);
        }

        private void HandleClick(Object sender, RoutedEventArgs routedEventArgs)
        {
            if (this.timer == null)
            {
                this.timer = new DispatcherTimer();
                this.timer.Interval = TimeSpan.FromMilliseconds(50);
                this.timer.Tick += this.HandleTick;
            }

            if (!this.timer.IsEnabled)
            {
                this.timer.Start();
                this.OnHoldStarted();
            }

            this.lastTimePressed = DateTime.UtcNow;
        }

        private void HandleTick(Object sender, EventArgs e)
        {
            if (!this.IsPressed || DateTime.UtcNow - this.lastTimePressed > TimeSpan.FromMilliseconds(200))
            {
                this.timer.Stop();
                this.OnHoldReleased();
            }
        }
    }
}
