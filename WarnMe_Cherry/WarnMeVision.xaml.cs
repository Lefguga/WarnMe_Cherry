using System;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Hardcodet.Wpf.TaskbarNotification;
using WarnMe_Cherry.Steuerelemente.Sites.Overview;
using static WarnMe_Cherry.Global;
using static GLOBAL.CONFIG;

namespace WarnMe_Cherry
{
    /// <summary>
    /// Interaktionslogik für WarnMeVision.xaml
    /// </summary>
    public partial class WarnMeVision : Window
    {
        public delegate void DataModified();
        public event DataModified NewDataAvailable;

        private bool UpdaterInWork = false;

        BackgroundWorker Updater = new BackgroundWorker();
        
        WorkDayPropWindow notifyWorkDay = new WorkDayPropWindow();
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
            Home.StartTimePicker.DateTime = WARNME_CONFIG.WORKINGDAYS[TODAY].StartZeit;
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
            WARNME_CONFIG.WORKINGDAYS[TODAY].EndZeit = NOW;
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

            if (WARNME_CONFIG.SETTINGS.MINIMIZED)
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
            INFO("WarnMeVision: UpdateForm");
#endif
            // Update Timeline
            try
            {
                if (!WARNME_CONFIG.WORKINGDAYS.ContainsKey(TODAY))
                {
                    InitFormValues();
                }
                else
                {
                    WARNME_CONFIG.WORKINGDAYS[TODAY].EndZeit = NOW;
                }

            }
            catch (ResourceReferenceKeyNotFoundException)
            {
                InitFormValues();
            }

            switch (Tabs.SelectedIndex)
            {
                case 0: // Home Site
                    Home.Update();
                    break;
                // Refresh actual day
                case 1:
                    Übersicht.ZeitTabelle.UpdateWorkday(TODAY, WARNME_CONFIG.WORKINGDAYS[TODAY]);
                    break;
                // Set resttime as program usage
                case 2:
                    Einstellungen.Debug.Text = $"{NOW.Milliseconds} ms";
                    break;
                default:
                    break;
            }

            RefreshNotiToolTip();

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
            INFO("WarnMeVision: InitFormValues");
#endif

            // init new day of working
            if (!WARNME_CONFIG.WORKINGDAYS.ContainsKey(TODAY))
            {
                DateTime upTime = Extern.SystemUpTime;
                // check if uptime is today else add the actual time
                TimeSpan startZeit = (upTime.Date == TODAY ? upTime.TimeOfDay : NOW);
                startZeit -= WARNME_CONFIG.TIME.START_DELAY;
                WARNME_CONFIG.WORKINGDAYS.Add(TODAY, new Arbeitstag()
                {
                    StartZeit = startZeit,
                    EndZeit = NOW,
                    Bemerkung = ""
                });
            }

            Home.StartTimePicker.DateTime = WARNME_CONFIG.WORKINGDAYS[TODAY].StartZeit;

            Home.EndTimePicker.DateTime = WARNME_CONFIG.WORKINGDAYS[TODAY].StartZeit + WARNME_CONFIG.TIME.WORKTIME;
            Home.MaxEndTimePicker.DateTime = WARNME_CONFIG.WORKINGDAYS[TODAY].StartZeit + WARNME_CONFIG.TIME.WORKLIMIT;

            NewDataAvailable();

            // Color
            BorderBrush = new SolidColorBrush(WARNME_CONFIG.COLORS.ACCENT_COLOR);
            TitleGrid.Background = new SolidColorBrush(WARNME_CONFIG.COLORS.MAIN_COLOR);
            MainGrid.Background = new SolidColorBrush(WARNME_CONFIG.COLORS.MAIN_COLOR_WEAK);


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
            INFO("WarnMeVision: UpdateWorkingDay");
#endif
            WARNME_CONFIG.WORKINGDAYS[date] = arbeitstag;
            // Results in too many data writings
            // DATA.Commit();
        }

        private void RefreshNotiToolTip()
        {
            TimeSpan progress = WARNME_CONFIG.WORKINGDAYS[TODAY].Progress(NOW);
            TimeSpan total = WARNME_CONFIG.TIME.WORKTIME +
                WARNME_CONFIG.TIME.COFFEE.DURATION +
                WARNME_CONFIG.TIME.LUNCH.DURATION;
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
            INFO("WarnMeVision: PrepareShutdown");
#endif
            // Refresh "Heute"
            WARNME_CONFIG.WORKINGDAYS[TODAY].EndZeit = NOW;

            // Commit Values
            NewDataAvailable();

            // Clean up Taskbar Icon
            notifyWorkDay?.Close();
            icon.CloseBalloon();
            icon.Icon.Dispose();
            icon.Icon = null;
            icon.Dispose();
        }

        private void UpdateOccured()
        {
            NewDataAvailable();
        }

    }
}
