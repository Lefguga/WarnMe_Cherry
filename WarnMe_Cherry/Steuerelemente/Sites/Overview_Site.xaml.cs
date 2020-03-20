using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using static WarnMe_Cherry.Global;

namespace WarnMe_Cherry.Steuerelemente.Sites
{
    /// <summary>
    /// Interaktionslogik für Overview_Site.xaml
    /// </summary>
    public partial class Overview_Site : UserControl, Interfaces.IUpdateable, Interfaces.IDataEntry
    {
        public string DATA_ID => "Overview";

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

        public void New()
        {

        }

        public void Load(JObject data)
        {
            if (data.TryGetValue(ZeitTabelle.DATA_ID, out JToken value))
            {
                ZeitTabelle.Load((JObject)value);
            }
            else
            {
                ZeitTabelle.New();
            }
        }

        public JObject Save()
        {
            JObject j = new JObject
            {
                { ZeitTabelle.DATA_ID, ZeitTabelle.Save() }
            };
            return j;
        }
    }
}
