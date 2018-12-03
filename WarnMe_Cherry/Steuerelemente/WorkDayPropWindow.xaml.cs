using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WarnMe_Cherry.Steuerelemente
{
    /// <summary>
    /// Interaktionslogik für WorkDayPropWindow.xaml
    /// </summary>
    public partial class WorkDayPropWindow : Window
    {
        public delegate void WorkdayUpdate(DateTime date, Arbeitstag arbeitstag);
        public event WorkdayUpdate WorkdayUpdated;

        private DateTime date;
        private bool block = false;
        private int input = 0;
        private bool setinput = false;

        public WorkDayPropWindow()
        {
            InitializeComponent();
        }

        public void Show(Arbeitstag workday, DateTime date)
        {
            input = 0;
            setinput = false;
            block = true;
            this.date = date;

            starttime.Value = workday.StartZeit;
            endtime.Value = workday.EndZeit;
            duration.Value = workday.Duration;
            comment.Text = workday.Bemerkung;
            Show();
        }

        private void KeyInput(object sender, KeyEventArgs e)
        {
            if (block)
                block = false;
            if (e.Key == Key.Escape)
            {
                block = true;
                Hide();
            }
            else if (e.Key == Key.Enter)
            {
                Hide();
            }
            else if (e.Key.IsDigit())
            {
                TimeOfDay tod;
                if (starttime.IsKeyboardFocusWithin)
                    tod = starttime;
                else if (endtime.IsKeyboardFocusWithin)
                    tod = endtime;
                else
                    return;
                
                if (setinput)
                {
                    setinput = false;
                    tod.SetValue(input * 10 + e.Key.GetDigit());
                }
                else
                {
                    setinput = true;
                    input = e.Key.GetDigit();
                }
            }
        }

        private void LoseFocus(object sender, EventArgs e)
        {
            if (!block)
                WorkdayUpdated?.Invoke(date, new Arbeitstag()
                {
                    StartZeit = starttime.Value,
                    EndZeit = endtime.Value,
                    Bemerkung = comment.Text
                });
            Hide();
        }
    }
}
