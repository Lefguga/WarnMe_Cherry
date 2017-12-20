using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WarnMe_Cherry.Steuerelemente
{
    /// <summary>
    /// Interaktionslogik für TimeOfDay.xaml
    /// </summary>
    public partial class TimeOfDay : UserControl
    {
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
            }
        }

        public TimeOfDay()
        {
            InitializeComponent();
        }

        private void Control_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBlock)
                ((TextBlock)sender).Background = new LinearGradientBrush(Color.FromArgb(0x00, 0x00, 0x50, 0xF0), Color.FromArgb(0xA0, 0x00, 0x50, 0xF0), 90);
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
