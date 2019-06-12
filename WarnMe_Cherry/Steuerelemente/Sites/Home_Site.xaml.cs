using System;
using System.Windows.Controls;
using WarnMe_Cherry.Datenbank;
using static WarnMe_Cherry.Global;

namespace WarnMe_Cherry.Steuerelemente.Sites
{
    /// <summary>
    /// Interaktionslogik für Home_Site.xaml
    /// </summary>
    public partial class Home_Site : UserControl, Interfaces.IUpdateable
    {
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
            throw new NotImplementedException();
        }

        private void StartTimeUpdated(object sender, TimeSpan value)
        {
#if TRACE
            INFO($"Home_Site.StartTimeUpdated with value {value}");
#endif
            if (value != start)
            {
                DATA.THIS.Heute_StartTime = value;
                start = value;

                EndTimePicker.DateTime = value + DATA.THIS.EINSTELLUNGEN.TOTAL_WORKTIME;
                MaxEndTimePicker.DateTime = value + DATA.THIS.EINSTELLUNGEN.TOTAL_WORKLIMIT;
            }
        }
    }
}
