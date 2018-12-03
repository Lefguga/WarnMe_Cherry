using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WarnMe_Cherry.Datenbank;
using static WarnMe_Cherry.Datenbank.InternalVariables;

namespace WarnMe_Cherry
{
    /// <summary>
    /// Interaktionslogik für WarnMeVision.xaml
    /// </summary>
    public partial class WarnMeVision : Window
    {
        BackgroundWorker Updater = new BackgroundWorker();

        ContextMenu contextMenu = new ContextMenu()
        {
            Items =
            {
                new MenuItem() { Header="Show" },
                new MenuItem() { Header="Hide" },
                new Separator(),
                new MenuItem() { Header="Close" }
            }
        };

        bool DeviceLocked = false;
        struct Arbeitstag_Heute
        {
            public static DateTime Date;
            public static Arbeitstag Arbeitstag;
        };

        #region Shortcuts
        string HeuteString => DateTime.Now.ToShortDateString();
        /// <summary>
        /// Gibt einen <see cref="DateTime.TimeOfDay"/> Wert mit der aktuellen Uhrzeit zurück.
        /// </summary>
        TimeSpan Jetzt => DateTime.Now.TimeOfDay;
        #endregion
        
        public WarnMeVision()
        {
            //System.Threading.Thread.Sleep(1000);
            InitializeComponent();
            Übersicht.ZeitTabelle.ValueUpdated += ZeitTabelle_ValueUpdated;
            Microsoft.Win32.SystemEvents.SessionSwitch += SaveCurrentSessionState;
        }

        private void SaveCurrentSessionState(object sender, Microsoft.Win32.SessionSwitchEventArgs e)
        {
            switch(e.Reason)
            {
                case Microsoft.Win32.SessionSwitchReason.SessionLock: DeviceLocked = true; break;
                case Microsoft.Win32.SessionSwitchReason.SessionUnlock: DeviceLocked = false; break;
            }
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

        protected override void OnClosing(CancelEventArgs e)
        {
            PrepareShutdown();
            // notyIcon?.Dispose();
            base.OnClosing(e);
        }

        // *********************************************************************************
        // HOME EVENTS
        // *********************************************************************************

        private void HomeBT_Click(object sender, MouseButtonEventArgs e)
        {
            Home.StartTimePicker.DateTime = Heute.StartZeit;
            Tabs.SelectedIndex = 0; //Home
        }

        // *********************************************************************************
        // ÜBERSICHT EVENTS
        // *********************************************************************************

        private void ÜbersichtBT_Click(object sender, MouseButtonEventArgs e)
        {
            Tabs.SelectedIndex = 1; //Übersicht
        }

        private void ZeitTabelle_ValueUpdated(DateTime date, Arbeitstag arbeitstag)
        {
            UpdateWorkingDay(date, arbeitstag);
        }

        // *********************************************************************************
        // EINSTELLUNG EVENTS
        // *********************************************************************************

        private void EinstellungenBT_Click(object sender, MouseButtonEventArgs e)
        {
            Tabs.SelectedIndex = 2; //Einstellungen
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
            // Init Notificytion Icon
            //notyIcon = new NotificationIcon.NotificationIcon
            //{
            //    ContextMenu = contextMenu.ContextMenu,
            //    ToolTipText = "Hallo"
            //};

            //Load Datenbank
            InternalVariables.Datenbank = new Datenbank.Datenbank(SETTINGS.CONFIG_FILE_NAME);

            //Set up Variables and Settings
            InitFormValues();

            //Set up Updater
            Updater.DoWork += (object o, DoWorkEventArgs arg) =>
            {
                System.Threading.Thread.Sleep(1000 - DateTime.Now.Millisecond);
            };
            Updater.RunWorkerCompleted += UpdateForm;

            //Set up TimeLine
            Home.timeLine.MaxValue = new TimeSpan(10, 45, 0).TotalSeconds; // Datenbank.Select<TimeSpan>("Parameter", "MaxWorkingTime").TotalSeconds;

            //Start Updater
            Updater.RunWorkerAsync();
        }

        private void UpdateForm(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!DeviceLocked)
            {

                // Update Timeline
                try
                {
                    if (Arbeitstag_Heute.Date != DateTime.Now.Date)
                    {
                        UpdateWorkingDay(Arbeitstag_Heute.Date, Arbeitstag_Heute.Arbeitstag);
                        InitFormValues();
                    }
                    else
                    {
                        Arbeitstag_Heute.Arbeitstag.EndZeit = Jetzt;
                    }

                }
                catch (ResourceReferenceKeyNotFoundException)
                {
                    InitFormValues();
                }

                Home.timeLine.Value = (Jetzt - Arbeitstag_Heute.Arbeitstag.StartZeit).TotalSeconds;
                Home.timeLine.Update();

            }

            // Restart Updater
            Updater.RunWorkerAsync();
        }

        // private void ShutdownWarnMe(object sender, System.ComponentModel.CancelEventArgs e) => PrepareShutdown();

        // *********************************************************************************
        // PRIVATE FUNCTIONS
        // *********************************************************************************

        private void InitFormValues()
        {

            // init new day of working
            if (!InternalVariables.Datenbank.Exists(WORKINGDAYS.NAME, HeuteString))
            {
                DateTime upTime = Extern.SystemUpTime;
                InternalVariables.Datenbank.Insert(WORKINGDAYS.NAME, HeuteString, new Arbeitstag()
                {
                    // check if uptime is today else add the actual time
                    StartZeit = (upTime.Date == DateTime.Now.Date ? upTime.TimeOfDay : Jetzt),
                    EndZeit = Jetzt,
                    Bemerkung = ""
                });
            }
            Arbeitstag_Heute.Date = DateTime.Now.Date;
            Arbeitstag_Heute.Arbeitstag = Heute;

            Home.StartTimePicker.DateTime = Arbeitstag_Heute.Arbeitstag.StartZeit;

            Home.EndTimePicker.DateTime = Arbeitstag_Heute.Arbeitstag.StartZeit + new TimeSpan(7, 45, 0);
            Home.MaxEndTimePicker.DateTime = Arbeitstag_Heute.Arbeitstag.StartZeit + new TimeSpan(10, 45, 0);

            InternalVariables.Datenbank.Update(OTHER.NAME, OTHER.SYSTEM_UP_TIME, Extern.SystemUpTime);
            InternalVariables.Datenbank.Commit();

            // WORKINGDAY TABLE
            System.Collections.Generic.Dictionary<DateTime, Arbeitstag> dictionaryAT = InternalVariables.Datenbank.Select<Arbeitstag>(WORKINGDAYS.NAME).ToDictionary(item => DateTime.Parse(item.Key), item => item.Value);

            Übersicht.ZeitTabelle.TableData = new System.Collections.Generic.SortedDictionary<DateTime, Arbeitstag>(dictionaryAT);

            Übersicht.ZeitTabelle.UpdateTable();

            Einstellungen.Update();

        }

        private void UpdateWorkingDay(DateTime date, Arbeitstag arbeitstag)
        {
            InternalVariables.Datenbank.Update(WORKINGDAYS.NAME, date.ToShortDateString(), arbeitstag);
            InternalVariables.Datenbank.Commit();
        }

        private void PrepareShutdown()
        {
            // Refresh "Heute"
            Arbeitstag_Heute.Arbeitstag.EndZeit = Jetzt;
            Heute = Arbeitstag_Heute.Arbeitstag;

            // Commit Values
            InternalVariables.Datenbank.Commit();

            // Close Forms
            Steuerelemente.WorkTable.window.Close();
        }

        private void SettingChanged(string key, object value)
        {
            InternalVariables.Datenbank.Update(SETTINGS.NAME, key, value);
            InternalVariables.Datenbank.Commit();
        }
    }
}
