using System;
using System.Windows.Controls;

namespace WarnMe_Cherry.Steuerelemente.Sites.Overview
{
    /// <summary>
    /// Interaktionslogik für Workday.xaml
    /// </summary>
    public partial class Workday : UserControl
    {
        public delegate void ValueChange();
        public event ValueChange ValueUpdated;

        private DateTime dateTime;
        public DateTime DateTime
        {
            get => dateTime;
            set
            {
                dateTime = value;
                if (dateTime != null)
                    DayOfMonth.Text = dateTime.Day.ToString();
            }
        }

        private Arbeitstag arbeitstag;
        public Arbeitstag Arbeitstag
        {
            get => arbeitstag;
            set
            {
                arbeitstag = value;
                if (!(arbeitstag is null))
                {
                    starttime.Content = arbeitstag.StartZeit;
                    endtime.Content = arbeitstag.EndZeit;
                    duration.Content = arbeitstag.Duration;
                    comment.Text = arbeitstag.Bemerkung;
                    ValueUpdated?.Invoke();
                }
            }
        }

        public Workday()
        {
            InitializeComponent();
        }

        public Workday(DateTime datetime, Arbeitstag arbeitstag)
        {
            DateTime = datetime;
            Arbeitstag = arbeitstag;
        }
    }
}
