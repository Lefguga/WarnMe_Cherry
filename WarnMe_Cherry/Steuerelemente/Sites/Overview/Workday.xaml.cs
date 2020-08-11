using System;
using System.Windows.Controls;
#if TRACE
using static WarnMe_Cherry.Global;
#endif

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
                    duration.Content = arbeitstag.DauerNetto;
                    comment.Text = arbeitstag.Bemerkung;
                    UpdateEvent();
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

        private void UpdateStartTimeEvent(object sender, TimeSpan value)
        {
#if TRACE
            INFO($"Workday: starttime of {dateTime} updated to {value}");
#endif
            arbeitstag.StartZeit = value;
            UpdateEvent();
        }

        private void UpdateEndTimeEvent(object sender, TimeSpan value)
        {
#if TRACE
            INFO($"Workday: endtime of {dateTime} updated to {value}");
#endif
            arbeitstag.EndZeit = value;
            UpdateEvent();
        }

        private void UpdateEvent()
        {
#if TRACE
            INFO("Workday: UpdateEvent triggered.");
#endif
            ValueUpdated?.Invoke();
        }
    }
}
