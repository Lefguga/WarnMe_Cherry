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
            //Workday w = new Workday()
            //{
            //    Margin = new System.Windows.Thickness(0, 0, 648, 215)
            //};
            //Data.Children.Add(w);
            //Grid.SetRow(w, 1);
        }

        public void Update()
        {
#if TRACE
            INFO("Home_Site.Update");
#endif
            throw new NotImplementedException();
        }

        private void StartTimeUpdated(TimeSpan value)
        {
#if TRACE
            INFO($"Home_Site.StartTimeUpdated with value {value}");
#endif
            if (value != start)
            {
                Arbeitstag heute = InternalVariables.Heute;
                heute.StartZeit = value;
                InternalVariables.Heute = heute;
                start = value;

                EndTimePicker.DateTime = value + new TimeSpan(7, 45, 00);
                MaxEndTimePicker.DateTime = value + new TimeSpan(10, 45, 00);
            }
        }
    }
}
