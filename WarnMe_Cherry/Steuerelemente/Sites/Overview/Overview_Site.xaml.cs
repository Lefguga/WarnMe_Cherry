using System;
using System.Windows.Controls;
using System.Windows.Input;
using static WarnMe_Cherry.Global;

namespace WarnMe_Cherry.Steuerelemente.Sites.Overview
{
    /// <summary>
    /// Interaktionslogik für Overview_Site.xaml
    /// </summary>
    public partial class Overview_Site : UserControl, Interfaces.IUpdateable
    {
        public delegate void ValueChange(DateTime date, Arbeitstag wDay);
        public event ValueChange ValueUpdated;

        public Overview_Site()
        {
#if TRACE
            INFO("Generate Overview_Site");
#endif
            InitializeComponent();
        }

        private void PrevMonth_Click(object sender, MouseButtonEventArgs e)
        {
#if TRACE
            INFO("Overview_Site.PrevMonth_Click");
#endif
            if (ZeitTabelle.Month == 1)
            {
                ZeitTabelle.Month = 12;
                ZeitTabelle.Year--;
            }
            else
            {
                ZeitTabelle.Month--;
            }
            ZeitTabelle.UpdateTable();
            MonthTitle.Text = ZeitTabelle.MonthString;
        }

        private void NextMonth_Click(object sender, MouseButtonEventArgs e)
        {
#if TRACE
            INFO("Overview_Site.NextMonth_Click");
#endif
            if (ZeitTabelle.Month == 12)
            {
                ZeitTabelle.Month = 1;
                ZeitTabelle.Year++;
            }
            else
            {
                ZeitTabelle.Month++;
            }
            ZeitTabelle.UpdateTable();
            MonthTitle.Text = ZeitTabelle.MonthString;
        }

        private void ResetMonth_Click(object sender, MouseButtonEventArgs e)
        {
#if TRACE
            INFO("Overview_Site.ResetMonth_Click");
#endif
            ZeitTabelle.Month = DateTime.Now.Month;
            ZeitTabelle.Year = DateTime.Now.Year;
            ZeitTabelle.UpdateTable();
            MonthTitle.Text = ZeitTabelle.MonthString;
        }

        public void Update()
        {
#if TRACE
            INFO("Overview_Site.Update");
#endif
            ZeitTabelle.Update();
        }

        private void UpdateEvent(DateTime date, Arbeitstag wDay)
        {
            ValueUpdated?.Invoke(date, wDay);
        }
    }
}
