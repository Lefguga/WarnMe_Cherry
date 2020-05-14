using System;
using System.Windows.Controls;
using static WarnMe_Cherry.Global;
using static GLOBAL.CONFIG;

namespace WarnMe_Cherry.Steuerelemente.Sites.Home
{
    /// <summary>
    /// Interaktionslogik für Home_Site.xaml
    /// </summary>
    public partial class Home_Site : UserControl, Interfaces.IUpdateable
    {
        public delegate void ValueChange();
        public event ValueChange ValueUpdated;

        TimeSpan start;

        public Home_Site()
        {
#if TRACE
            INFO("Home_Site.Home_Site");
#endif
            InitializeComponent();
        }

        public void Update()
        {
#if TRACE
            INFO("Home_Site.Update");
#endif
            timeLine.Value = (NOW - WARNME_CONFIG.WORKINGDAYS[TODAY].StartZeit).TotalSeconds;
            timeLine.ToolTip = $"{WARNME_CONFIG.WORKINGDAYS[TODAY].DauerNetto}";
            //timeLine.Update();
        }

        private void StartTimeUpdated(object sender, TimeSpan value)
        {
#if TRACE
            INFO($"Home_Site.StartTimeUpdated with value {value}");
#endif
            if (value != start)
            {
                WARNME_CONFIG.WORKINGDAYS[TODAY].StartZeit = value;
                EndTimePicker.DateTime = value + WARNME_CONFIG.TIME.WORKTIME;
                MaxEndTimePicker.DateTime = value + WARNME_CONFIG.TIME.WORKLIMIT;

                start = value;
                UpdateEvent();
            }
        }

        private void UpdateEvent()
        {
            ValueUpdated?.Invoke();
        }
    }
}
