using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WarnMe_Cherry
{
    /// <summary>
    /// Interaktionslogik für WarnMeVision.xaml
    /// </summary>
    public partial class WarnMeVision : Window
    {
        System.ComponentModel.BackgroundWorker Updater = new System.ComponentModel.BackgroundWorker();

        #region STRUCTS
        public struct WORKINGDAYS
        {
            public const string NAME = "Arbeitstage";
            public System.Collections.Generic.Dictionary<string, Arbeitstag> VALUE { get; set; }
        }
        public struct WECKER
        {
            public const string NAME = "Wecker";
            public System.Collections.Generic.Dictionary<string, Wecker.Wecker> VALUE { get; set; }
        }
        public struct SETTINGS
        {
            public const string CONFIG_FILE_NAME = @".\Configuration.json";
            // TABLE
            public const string NAME = "Einstellungen";
            public struct Zeiten
            {
                public const string NAME = "Zeiten";
                public struct TIME_A_DAY
                {
                    public const string NAME = "Workingtime";
                    public TimeSpan Value { get; set; }
                }
                public struct MAX_TIME_A_DAY
                {
                    public const string NAME = "MaxWorkingtime";
                    public TimeSpan Value { get; set; }
                }
                public struct KORREKTUR
                {
                    public const string NAME = "Korrektur";
                    public TimeSpan Value { get; set; }
                }
            }
        }
        public struct OTHER
        {
            public const string NAME = "Sonstiges";
            public const string SYSTEM_UP_TIME = "SystemUpTime";
        }
        #endregion

        #region Shortcuts
        string HeuteString => DateTime.Now.ToShortDateString();
        TimeSpan Jetzt => DateTime.Now.TimeOfDay;
        #endregion

        Arbeitstag Heute
        {
            get
            {
                if (Datenbank.Exists(WORKINGDAYS.NAME, HeuteString))
                {
                    return Datenbank.Select<Arbeitstag>(WORKINGDAYS.NAME, HeuteString);
                }
                throw new ResourceReferenceKeyNotFoundException("Data not found", HeuteString);
            }
            set
            {
                Datenbank.Update(WORKINGDAYS.NAME, HeuteString, value);
            }
        }

        Datenbank.Datenbank Datenbank;

        public WarnMeVision()
        {
            //System.Threading.Thread.Sleep(1000);
            InitializeComponent();
        }

        private void ActivateDragMove(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }

        private void CloseClick(object sender, MouseButtonEventArgs e) => Close();

        private void MaximizeClick(object sender, MouseButtonEventArgs e) => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

        private void MinimizeClick(object sender, MouseButtonEventArgs e) => WindowState = WindowState.Minimized;

        private void FrameworkElement_MouseDown_Focus(object sender, MouseButtonEventArgs e) => ((FrameworkElement)sender).Focus();

        // *********************************************************************************
        // HOME EVENTS
        // *********************************************************************************

        private void HomeBT_Click(object sender, MouseButtonEventArgs e)
        {
            Home.Visibility = Visibility.Visible; // Home
            Settings.Visibility = Übersicht.Visibility = Visibility.Hidden;
        }

        // *********************************************************************************
        // ÜBERSICHT EVENTS
        // *********************************************************************************

        private void ÜbersichtBT_Click(object sender, MouseButtonEventArgs e)
        {
            Übersicht.Visibility = Visibility.Visible; // Home
            Settings.Visibility = Home.Visibility = Visibility.Hidden;
        }

        private void PrevMonth_Click(object sender, MouseButtonEventArgs e)
        {
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
            ZeitTabelle.Month = DateTime.Now.Month;
            ZeitTabelle.Year = DateTime.Now.Year;
            ZeitTabelle.UpdateTable();
            MonthTitle.Text = ZeitTabelle.MonthString;
        }

        // *********************************************************************************
        // EINSTELLUNG EVENTS
        // *********************************************************************************

        private void EinstellungenBT_Click(object sender, MouseButtonEventArgs e)
        {
            Settings.Visibility = Visibility.Visible; // Home
            Home.Visibility = Übersicht.Visibility = Visibility.Hidden;
        }

        private void TestWebsite(object sender, MouseButtonEventArgs e)
        {
            Extensions.Wetter wetter = new Extensions.Wetter();
        }

        // *********************************************************************************
        // TRIGGER EVENTS
        // *********************************************************************************

        private void AfterFormInitialized(object sender, RoutedEventArgs e)
        {
            //Load Datenbank
            Datenbank = new Datenbank.Datenbank(SETTINGS.CONFIG_FILE_NAME);

            //Set up Variables and Settings
            InitFormValues();

            //Set up Updater
            Updater.DoWork += (object o, System.ComponentModel.DoWorkEventArgs arg) =>
            {
                System.Threading.Thread.Sleep(1000 - DateTime.Now.Millisecond);
            };
            Updater.RunWorkerCompleted += UpdateForm;

            //Set up TimeLine
            timeLine.MaxValue = new TimeSpan(10, 45, 0).TotalSeconds; // Datenbank.Select<TimeSpan>("Parameter", "MaxWorkingTime").TotalSeconds;

            //Start Updater
            Updater.RunWorkerAsync();
        }

        private void UpdateForm(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            // Update Timeline
            Arbeitstag _heute;
            try
            {
                _heute = Heute;
            }
            catch (ResourceReferenceKeyNotFoundException)
            {
                InitFormValues();
                _heute = Heute;
            }
            timeLine.Value = (Jetzt - _heute.StartZeit).TotalSeconds;

            // Restart Updater
            Updater.RunWorkerAsync();
        }

        private void ShutdownWarnMe(object sender, System.ComponentModel.CancelEventArgs e) => PrepareShutdown();

        // *********************************************************************************
        // PRIVATE FUNCTIONS
        // *********************************************************************************

        private void InitFormValues()
        {

            // init new day of working
            if (!Datenbank.Exists(WORKINGDAYS.NAME, HeuteString))
            {
                DateTime upTime = Extern.SystemUpTime;
                Datenbank.Insert(WORKINGDAYS.NAME, HeuteString, new Arbeitstag()
                {
                    // check if uptime is today else add the actual time
                    StartZeit = (upTime.Date == DateTime.Now.Date ? upTime.TimeOfDay : Jetzt),
                    EndZeit = Jetzt,
                    Bemerkung = ""
                });
            }
            Arbeitstag heute = Heute;

            StartTimePicker.DateTime = heute.StartZeit;

            EndTimePicker.DateTime = heute.StartZeit + new TimeSpan(7, 45, 0);
            MaxEndTimePicker.DateTime = heute.StartZeit + new TimeSpan(10, 45, 0);

            Datenbank.Update(OTHER.NAME, OTHER.SYSTEM_UP_TIME, Extern.SystemUpTime);
            Datenbank.Commit();

            // WORKINGDAY TABLE
            System.Collections.Generic.Dictionary<DateTime, Arbeitstag> dictionaryAT = Datenbank.Select<Arbeitstag>(WORKINGDAYS.NAME).ToDictionary(item => DateTime.Parse(item.Key), item => item.Value);

            ZeitTabelle.TableData = new System.Collections.Generic.SortedDictionary<DateTime, Arbeitstag>(dictionaryAT);

            ZeitTabelle.UpdateTable();

        }

        private void PrepareShutdown()
        {
            // Refresh "Heute"
            Arbeitstag heute = Heute;
            heute.EndZeit = Jetzt;
            Heute = heute;

            // Commit Values
            Datenbank.Commit();
        }

        private void StartTimeUpdated(TimeSpan value)
        {
            Arbeitstag heute = Heute;
            heute.StartZeit = value;
            Heute = heute;

            EndTimePicker.DateTime = value + new TimeSpan(7, 45, 00);
            MaxEndTimePicker.DateTime = value + new TimeSpan(10, 45, 00);
        }
    }
}
