using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WarnMe_Cherry.Steuerelemente
{
    /// <summary>
    /// Interaktionslogik für DateTimePicker.xaml
    /// </summary>
    public partial class DateTimePicker : UserControl
    {
        public delegate void TimeUpdates(TimeSpan value);
        public event TimeUpdates TimeChanged;

        public bool AllowInputs { get; set; } = false;
        public Visibility ShowUpDown { get; set; } = Visibility.Hidden;
        int LastInput = 0;
        
        public TimeSpan DateTime
        {
            get => new TimeSpan(Hour, Minute, Second);
            set
            {
                Hour = value.Hours;
                Minute = value.Minutes;
                Second = value.Seconds;
            }
        }
        
        public int Hour
        {
            get => Time.Hour;
            set => Time.Hour = value;
        }
        public int Minute
        {
            get => Time.Minute;
            set => Time.Minute = value;
        }
        public int Second
        {
            get => Time.Second;
            set => Time.Second = value;
        }

        public DateTimePicker()
        {
            InitializeComponent();

            Loaded += delegate (object o, RoutedEventArgs e)
            {
                Buttons.Visibility = ShowUpDown;
            };

            Time.TimeChanged += delegate (TimeSpan timeSpan)
            {
                TimeChanged?.Invoke(DateTime);
            };
        }

        private void IncreaseValue(object sender, MouseButtonEventArgs e)
        {
            Time.AddValue(1);
        }

        private void DecreaseValue(object sender, MouseButtonEventArgs e)
        {
            Time.AddValue(-1);
        }

        private void HandleKeyInput(object sender, KeyEventArgs e)
        {
            if (AllowInputs)
            {
                if (e.Key.IsDigit())
                {
                    Time.SetValue(LastInput * 10 + e.Key.GetDigit());
                    LastInput = e.Key.GetDigit();
                }
                else if (e.Key == Key.Enter)
                {
                    //UpdateDateTime();
                    LastInput = 0;
                }
                else if (e.Key == Key.Escape)
                {
                    //UpdateString();
                    LastInput = 0;
                }
                else if (e.Key == Key.Add || e.Key == Key.OemPlus)
                {
                    Time.AddValue(1);
                    LastInput = (LastInput + 1) % 10;
                }
                else if (e.Key == Key.Subtract || e.Key == Key.OemMinus)
                {
                    Time.AddValue(-1);
                    LastInput = (LastInput + 9) % 10;
                }
            }
        }
    }

    public static class ValueExtensions
    {
        public static bool IsDigit(this Key key)
        {
            if (Key.D0 <= key && key <= Key.D9 || Key.NumPad0 <= key && key <= Key.NumPad9)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Return the Digit Value from Keyinput
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetDigit(this Key key)
        {
            if (Key.D0 <= key && key <= Key.D9)
                return key - Key.D0;
            if (Key.NumPad0 <= key && key <= Key.NumPad9)
                return key - Key.NumPad0;
            throw new InvalidCastException(string.Format("Could not convert into digit: {0}", key));
        }
    }
}
