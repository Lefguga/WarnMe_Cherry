using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WarnMe_Cherry.Steuerelemente
{
    /// <summary>
    /// Interaktionslogik für TimeOfDay.xaml
    /// </summary>
    public partial class TimeOfDay : UserControl
    {
        public delegate void TimeUpdates(TimeSpan time);
        public event TimeUpdates TimeUpdated;

        int hour, minute, second;
        public int Hour
        {
            get => hour;
            set
            {
                if (value >= 0)
                {
                    hour = value % 24;
                }
                else
                {
                    hour = 23;
                }
                Hours.Text = hour.ToString("D2");
                TriggerTimeUpdate();
            }
        }
        public int Minute
        {
            get => minute;
            set
            {
                if (value >= 0)
                {
                    minute = value % 60;
                }
                else
                {
                    minute = 59;
                }
                Minutes.Text = minute.ToString("D2");
                TriggerTimeUpdate();
            }
        }
        public int Second
        {
            get => second;
            set
            {
                if (value >= 0)
                {
                    second = value % 60;
                }
                else
                {
                    second = 59;
                }
                Seconds.Text = second.ToString("D2");
                TriggerTimeUpdate();
            }
        }

        private void TriggerTimeUpdate() => TimeUpdated?.Invoke(new TimeSpan(Hour, Minute, Second));

        Brush focusBrush = new LinearGradientBrush(new GradientStopCollection(3)
        {
            new GradientStop(Color.FromArgb(0x9F, 0x00, 0x60, 0xFF), 0.16),
            new GradientStop(Color.FromArgb(0x00, 0x00, 0x60, 0xFF), 0.15)
        },90);
        public Brush FocusBrush { get => focusBrush; set => focusBrush = value; }
        public TimeSpan Value { get => new TimeSpan(Hour, Minute, Second); internal set { Hour = value.Hours; Minute = value.Minutes; Second = value.Seconds; } }

        public TimeOfDay()
        {
            InitializeComponent();
        }

        private void Control_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBlock)
                ((TextBlock)sender).Background = FocusBrush;
        }

        private void Control_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBlock)
                ((TextBlock)sender).Background = Brushes.Transparent;
        }

        public void AddValue(int v)
        {
            // Get value in TextBlock and increase the value

            if (Hours.IsFocused)
            {
                Hour += v;
            }
            else if (Minutes.IsFocused)
            {
                Minute += v;
            }
            else if (Seconds.IsFocused)
            {
                Second += v;
            }
        }

        public void SetValue(int v)
        {
            // Get value in TextBlock and increase the value

            if (Hours.IsFocused)
            {
                Hour = v;
            }
            else if (Minutes.IsFocused)
            {
                Minute = v;
            }
            else if (Seconds.IsFocused)
            {
                Second = v;
            }
        }
    }
}
