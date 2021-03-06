﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WarnMe_Cherry.Steuerelemente.Subparts
{
    /// <summary>
    /// Interaktionslogik für TimeOfDay.xaml
    /// </summary>
    public partial class TimeOfDay : UserControl
    {
        public delegate void TimeChange(TimeSpan time);
        public event TimeChange TimeChanged;

        int hour, minute, second;
        public int Hour
        {
            get => hour;
            set
            {
                hour = value % 24;
                Hours.Text = hour.ToString("D2");
                TriggerTimeUpdate();
            }
        }
        public int Minute
        {
            get => minute;
            set
            {
                minute = value % 60;
                Minutes.Text = minute.ToString("D2");
                TriggerTimeUpdate();
            }
        }
        public int Second
        {
            get => second;
            set
            {
                second = value % 60;
                Seconds.Text = second.ToString("D2");
                TriggerTimeUpdate();
            }
        }

        private void TriggerTimeUpdate() => TimeChanged?.Invoke(new TimeSpan(Hour, Minute, Second));

        public Brush FocusBrush { get; set; } = new LinearGradientBrush(new GradientStopCollection(3)
        {
            new GradientStop(Color.FromArgb(0x9F, 0x00, 0x60, 0xFF), 0.16),
            new GradientStop(Color.FromArgb(0x00, 0x00, 0x60, 0xFF), 0.15)
        }, 90);

        public TimeSpan Value
        {
            get => new TimeSpan(Hour, Minute, Second);
            set { Hour = value.Hours; Minute = value.Minutes; Second = value.Seconds; }
        }

        public TimeOfDay()
        {
            InitializeComponent();
        }

        private void Control_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBlock)
            {
                ((TextBlock)sender).Background = FocusBrush;
            }
        }

        private void Control_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBlock)
                ((TextBlock)sender).Background = Brushes.Transparent;
        }

        private void Hours_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ((UIElement)sender).Focus();
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
