using System;
using System.Windows.Controls;
using WarnMe_Cherry.Datenbank;

namespace WarnMe_Cherry.Steuerelemente.Sites
{
    /// <summary>
    /// Interaktionslogik für Home_Site.xaml
    /// </summary>
    public partial class Home_Site : UserControl
    {
        TimeSpan start;

        public Home_Site()
        {
            InitializeComponent();
            //Workday w = new Workday()
            //{
            //    Margin = new System.Windows.Thickness(0, 0, 648, 215)
            //};
            //Data.Children.Add(w);
            //Grid.SetRow(w, 1);
        }
    
        private void StartTimeUpdated(TimeSpan value)
        {
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
