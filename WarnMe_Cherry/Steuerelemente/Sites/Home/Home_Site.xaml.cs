using System;
using System.Windows.Controls;
using System.Windows.Media;
using static WarnMe_Cherry.Global;
using static GLOBAL.CONFIG;

namespace WarnMe_Cherry.Steuerelemente.Sites.Home
{
    /// <summary>
    /// Interaktionslogik für Home_Site.xaml
    /// </summary>
    public partial class Home_Site : UserControl, Interfaces.IUpdateable
    {
        public delegate void ValueChange(DateTime date, Arbeitstag wDay);
        public event ValueChange ValueUpdated;

        TimeSpan start;

        public Home_Site()
        {
#if TRACE
            INFO("Home_Site.Home_Site");
#endif
            InitializeComponent();
        }

        internal void Init()
        {
            
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
                TimeSpan totalBrakeTime = WARNME_CONFIG.TIME.COFFEE.DURATION + WARNME_CONFIG.TIME.LUNCH.DURATION;
                WARNME_CONFIG.WORKINGDAYS[TODAY].StartZeit = value;
                EndTimePicker.DateTime = value + WARNME_CONFIG.TIME.WORKTIME + totalBrakeTime;
                MaxEndTimePicker.DateTime = value + WARNME_CONFIG.TIME.WORKLIMIT + totalBrakeTime;

                start = value;
                ValueUpdated?.Invoke(TODAY, WARNME_CONFIG.WORKINGDAYS[TODAY]);
            }
        }
    }
}
