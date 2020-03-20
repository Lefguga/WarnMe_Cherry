using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WarnMe_Cherry.Datenbank;
using Hardcodet.Wpf.TaskbarNotification;
using static WarnMe_Cherry.Global;
using System.Windows.Media;

namespace WarnMe_Cherry
{
    /// <summary>
    /// Interaktionslogik für WarnMeVision.xaml
    /// </summary>
    public partial class WarnMeVision : Window
    {
        private bool UpdaterInWork = false;

        BackgroundWorker Updater = new BackgroundWorker();
        
        Steuerelemente.WorkDayPropWindow notifyWorkDay = new Steuerelemente.WorkDayPropWindow();
        ContextMenu contextMenu;

        TaskbarIcon icon;

        bool DeviceLocked = false;
        
        /// <summary>
        /// 
        /// </summary>
        public WarnMeVision()
        {
#if TRACE
            INFO("Generate WarnMeVision()");
#endif
            InitializeComponent();
            Übersicht.ZeitTabelle.ValueUpdated += ZeitTabelle_ValueUpdated;
            Microsoft.Win32.SystemEvents.SessionSwitch += SaveCurrentSessionState;
        }

        /// <summary>
        /// Saves Sessionstate on change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveCurrentSessionState(object sender, Microsoft.Win32.SessionSwitchEventArgs e)
        {
#if TRACE
            INFO($"SaveCurrentSessionState is {e.Reason}");
#endif
            switch (e.Reason)
            {
                case Microsoft.Win32.SessionSwitchReason.SessionLock: DeviceLocked = true; break;
                case Microsoft.Win32.SessionSwitchReason.SessionUnlock: DeviceLocked = false; break;
            }
        }

        /// <summary>
        /// Provides possibility to drag the window with an <see cref="FrameworkElement"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ActivateDragMove(object sender, MouseButtonEventArgs e)
        {
#if TRACE
            INFO("ActivateDragMove");
#endif
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }

        /// <summary>
        /// Simply closes the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseClick(object sender, MouseButtonEventArgs e) => Close();

        /// <summary>
        /// Simply maximizes the window 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaximizeClick(object sender, MouseButtonEventArgs e) => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

        /// <summary>
        /// Simply minimizes the window and hides it in the taskbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinimizeClick(object sender, MouseButtonEventArgs e) => Hide();

        /// <summary>
        /// Set a <see cref="FrameworkElement"/>s fokus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrameworkElement_MouseDown_Focus(object sender, MouseButtonEventArgs e) => ((FrameworkElement)sender).Focus();

        /// <summary>
        /// Handle the closing procedere
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
#if TRACE
            INFO("OnClosing");
#endif
            PrepareShutdown();
            // notyIcon?.Dispose();
            base.OnClosing(e);
        }

        protected new void Show()
        {
            //base.Show();
            WindowState = WindowState.Normal;
            Focus();
            Activate();
            ShowInTaskbar = true;
        }

        protected new void Hide()
        {
            //base.Hide();
            WindowState = WindowState.Minimized;
            ShowInTaskbar = false;
        }

        // *********************************************************************************
        // HOME EVENTS
        // *********************************************************************************

        private void HomeBT_Click(object sender, MouseButtonEventArgs e)
        {
#if TRACE
            INFO("HomeBT_Click");
#endif
            Home.StartTimePicker.DateTime = DATA.THIS.Heute.StartZeit;
            Tabs.SelectedIndex = 0; //Home
        }

        // *********************************************************************************
        // ÜBERSICHT EVENTS
        // *********************************************************************************

        private void ÜbersichtBT_Click(object sender, MouseButtonEventArgs e)
        {
#if TRACE
            INFO("ÜbersichtBT_Click");
#endif
            // Konvertierung zur sortierten Tabelle
            Arbeitstag _heute = DATA.THIS.Heute;
            _heute.EndZeit = WARNME_CONFIG.TimeNow;
            DATA.THIS.Heute = _heute;
            Übersicht.Update();
            Tabs.SelectedIndex = 1; //Übersicht
        }

        private void ZeitTabelle_ValueUpdated(DateTime date, Arbeitstag arbeitstag)
        {
#if TRACE
            INFO("ZeitTabelle_ValueUpdated");
#endif
            UpdateWorkingDay(date, arbeitstag);
        }

        // *********************************************************************************
        // EINSTELLUNG EVENTS
        // *********************************************************************************

        private void EinstellungenBT_Click(object sender, MouseButtonEventArgs e)
        {
#if TRACE
            INFO("EinstellungenBT_Click");
#endif
            Tabs.SelectedIndex = 2; //Einstellungen
        }

        private void TestWebsite(object sender, MouseButtonEventArgs e)
        {
#if TRACE
            INFO("TestWebsite");
#endif
            Extensions.Wetter wetter = new Extensions.Wetter();
        }

        // *********************************************************************************
        // TRIGGER EVENTS
        // *********************************************************************************

        /// <summary>
        /// ===== INITIALIZISE =====
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AfterFormInitialized(object sender, RoutedEventArgs e)
        {
#if TRACE
            INFO("AfterFormInitialized");
#endif

            //Load Datenbank
            // DATA = new Datenbank.Datenbank(INTERNAL.CONFIG_FILE_NAME);  NOT NEEDED WITH STATIC ALTERNATIVE

            //Set up Variables and Settings
            InitFormValues();

            if (DATA.THIS.EINSTELLUNGEN.MINIMIZED)
                Hide();

            //Set Menuitem
            MenuItem show = new MenuItem() { Header = "Show" };
            show.Click += (object o, RoutedEventArgs me) => { Show(); };
            MenuItem hide = new MenuItem() { Header = "Hide" };
            hide.Click += (object o, RoutedEventArgs me) => { Hide(); };
            MenuItem close = new MenuItem() { Header = "Close" };
            close.Click += (object o, RoutedEventArgs me) => { Close(); };

            contextMenu = new ContextMenu()
            {
                Items =
                {
                    show,
                    hide,
                    new Separator(),
                    close
                }
            };

            //Set Notification Icon
            icon = new TaskbarIcon
            {
                Icon = (System.Drawing.Icon)Properties.Resources.IconCherry.Clone(),
                //ToolTip = notifyWorkDay,
                ContextMenu = contextMenu
            };
            icon.TrayMouseDoubleClick += (object o, RoutedEventArgs me) => { Show(); };

            //Set up Updater
            Updater.DoWork += (object o, DoWorkEventArgs arg) =>
            {
                System.Threading.Thread.Sleep(1000 - DateTime.Now.Millisecond);
            };
            Updater.RunWorkerCompleted += Update;

            //Set up TimeLine
            Home.timeLine.MaxValue = new TimeSpan(10, 45, 0).TotalSeconds; // Datenbank.Select<TimeSpan>("Parameter", "MaxWorkingTime").TotalSeconds;

            //Start Updater
            Updater.RunWorkerAsync();
        }
        
        /// <summary>
        /// Updater, verantwortlich für das Aufrufen aller anderen Updater
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Update(object sender, RunWorkerCompletedEventArgs e)
        {
#if DEBUG
            INFO("Update");
#endif
            if (!UpdaterInWork)
            {

                if (!DeviceLocked)
                {
                    UpdaterInWork = true;

                    //Ruft Update Funktionen auf
                    UpdateForm();

                    UpdaterInWork = false;
                }

                // Restart Updater
                Updater.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Updater für alle windows form Anwendungen
        /// </summary>
        private void UpdateForm()
        {
#if DEBUG
            INFO("UpdateForm");
#endif
            // Update Timeline
            try
            {
                if (!DATA.THIS.WORKINGDAYS.ContainsKey(WARNME_CONFIG.DateNow))
                {
                    InitFormValues();
                }
                else
                {
                    DATA.THIS.Heute_EndTime = WARNME_CONFIG.TimeNow;
                }

            }
            catch (ResourceReferenceKeyNotFoundException)
            {
                InitFormValues();
            }

            Home.timeLine.Value = (WARNME_CONFIG.TimeNow - DATA.THIS.Heute.StartZeit).TotalSeconds;
            Home.timeLine.ToolTip = $"{DATA.THIS.Heute.DauerNetto}";

            RefreshNotiToolTip();

            // Set resttime as program usage
            Einstellungen.Debug.Text = $"{WARNME_CONFIG.TimeNow.Milliseconds} ms";
        }
        
        // private void ShutdownWarnMe(object sender, System.ComponentModel.CancelEventArgs e) => PrepareShutdown();

        // *********************************************************************************
        // PRIVATE FUNCTIONS
        // *********************************************************************************

        /// <summary>
        /// Setzt alle Werte auf Standard Einstllungen zurück.
        /// Erstellt einen neuen Tag wenn es noch keinen gibt.
        /// </summary>
        private void InitFormValues()
        {
#if TRACE
            INFO("InitFormValues");
#endif

            // init new day of working
            if (!DATA.THIS.WORKINGDAYS.ContainsKey(WARNME_CONFIG.DateNow))
            {
                DateTime upTime = Extern.SystemUpTime;
                // check if uptime is today else add the actual time
                TimeSpan startZeit = (upTime.Date == DateTime.Now.Date ? upTime.TimeOfDay : WARNME_CONFIG.TimeNow);
                startZeit -= DATA.THIS.EINSTELLUNGEN.OFFSET_START_TIME;
                DATA.THIS.WORKINGDAYS.Add(WARNME_CONFIG.DateNow, new Arbeitstag()
                {
                    StartZeit = startZeit,
                    EndZeit = WARNME_CONFIG.TimeNow,
                    Bemerkung = ""
                });
            }
            DATA.THIS.Date = WARNME_CONFIG.DateNow;
            DATA.THIS.Heute_EndTime = WARNME_CONFIG.TimeNow;

            Home.StartTimePicker.DateTime = DATA.THIS.Heute.StartZeit;

            Home.EndTimePicker.DateTime = DATA.THIS.Heute.StartZeit + DATA.THIS.EINSTELLUNGEN.TOTAL_WORKTIME;
            Home.MaxEndTimePicker.DateTime = DATA.THIS.Heute.StartZeit + DATA.THIS.EINSTELLUNGEN.TOTAL_WORKLIMIT;
            
            DATA.Commit();

            // Color
            BorderBrush = new SolidColorBrush(DATA.THIS.COLORS.ACCENT_COLOR);
            TitleGrid.Background = new SolidColorBrush(DATA.THIS.COLORS.MAIN_COLOR);
            MainGrid.Background = new SolidColorBrush(DATA.THIS.COLORS.MAIN_COLOR_WEAK);


            // WORKINGDAY TABLE
            Übersicht.Update();

            Einstellungen.Init();
            Einstellungen.Update();

        }

        /// <summary>
        /// Updated den Arbeitstag Eintrag in der Datenbank mit dem neuen Tag
        /// </summary>
        /// <param name="date">Das Datum des Arbeitstages (benutzt <see cref="DateTime.ToShortDateString"/> als Schlüssel)</param>
        /// <param name="arbeitstag"></param>
        private void UpdateWorkingDay(DateTime date, Arbeitstag arbeitstag)
        {
#if TRACE
            INFO("UpdateWorkingDay");
#endif
            DATA.THIS.WORKINGDAYS[date] = arbeitstag;
            // Results in too many data writings
            // DATA.Commit();
        }

        private void RefreshNotiToolTip()
        {
            TimeSpan progress = DATA.THIS.Heute.Progress(WARNME_CONFIG.TimeNow);
            TimeSpan total = DATA.THIS.EINSTELLUNGEN.WORKDAY_NORMAL +
                DATA.THIS.EINSTELLUNGEN.COFFEE.DURATION +
                DATA.THIS.EINSTELLUNGEN.LUNCH.DURATION;
            if (progress < total)
            {
                icon.ToolTipText = string.Format("Done: {0}\nTodo: {1}",
                  progress.ToString(@"hh\:mm\:ss"),
                  (total - progress).ToString(@"hh\:mm\:ss"));
            }
            else
            {
                icon.ToolTipText = string.Format("Done: {0}\nOver: {1}",
                  progress.ToString(@"hh\:mm\:ss"),
                  (total - progress).ToString(@"hh\:mm\:ss"));
            }
        }

        /// <summary>
        /// Updated 
        /// </summary>
        private void PrepareShutdown()
        {
#if TRACE
            INFO("PrepareShutdown");
#endif
            // Refresh "Heute"
            DATA.THIS.Heute_EndTime = WARNME_CONFIG.TimeNow;

            // Commit Values
            DATA.Commit();

            // Close Forms
            Steuerelemente.WorkTable.window.Close();

            // Clean up Taskbar Icon
            notifyWorkDay?.Close();
            icon.CloseBalloon();
            icon.Icon.Dispose();
            icon.Icon = null;
            icon.Dispose();
        }

//        private void SettingChanged(string key, object value)
//        {
//#if TRACE
//            INFO("SettingChanged");
//#endif
//#if GEN_LIBRARY
//            THIS.Datenbank.Update(EINSTELLUNGEN.NAME, key, value);
//#else
//            throw new NotImplementedException("Generic Library not availabel.");
//#endif
//            // THIS.Datenbank.Commit();
//        }
    }
}
