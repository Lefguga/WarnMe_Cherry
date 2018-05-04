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
            public struct Korrektur
            {
                public struct TIME_A_DAY
                {
                    public const string NAME = "Workingtime";
                    public TimeSpan Value { get; set; }
                }
                public struct MAX_TIME_A_DAY
                {
                    public const string Name = "MaxWorkingtime";
                    public TimeSpan Value { get; set; }
                }
            }
        }
        public struct OTHER
        {
            public const string NAME = "Sonstiges";
            public const string SYSTEM_UP_TIME = "SystemUpTime";
        }
        

        // Shortcuts
        string HeuteString => DateTime.Now.ToShortDateString();
        TimeSpan Jetzt => DateTime.Now.TimeOfDay;

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

        private void InitFormValues()
        {

            // init new day of working
            if (!Datenbank.Exists(WORKINGDAYS.NAME, HeuteString))
            {
                DateTime upTime = Extern.SystemUpTime;
                Datenbank.Insert(WORKINGDAYS.NAME, HeuteString, new Arbeitstag()
                {
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

            /*
            if (dictionaryAT.Count > 1)
            {
                foreach (var item in dictionaryAT.Reverse())
                {
                    Viewbox day = new Viewbox
                    {
                        Margin = new Thickness(0, 3, 0, 3),
                        Child = new TextBlock() { Text = item.Key }
                    },
                    start = new Viewbox
                    {
                        Margin = new Thickness(0, 3, 0, 3),
                        Child = new Steuerelemente.DateTimePicker() { DateTime = item.Value.StartZeit, AllowInputs = true }
                    },
                    end = new Viewbox
                    {
                        Margin = new Thickness(0, 3, 0, 3),
                        Child = new Steuerelemente.DateTimePicker() { DateTime = item.Value.EndZeit, AllowInputs = true }
                    },
                    duration = new Viewbox
                    {
                        Margin = new Thickness(0, 3, 0, 3),
                        Child = new Steuerelemente.DateTimePicker() { DateTime = item.Value.Duration, AllowInputs = true }
                    },
                    comment = new Viewbox
                    {
                        Margin = new Thickness(0, 3, 0, 3),
                        Child = new TextBox() {
                            Text = item.Value.Bemerkung,
                            BorderBrush = null,
                            Background = null,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            Foreground = MainWindow.Foreground,
                            MinWidth = 150.0d
                        }
                    };

                    Uebersicht.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
                    Uebersicht.Children.Add(day);
                    Uebersicht.Children.Add(start);
                    Uebersicht.Children.Add(end);
                    Uebersicht.Children.Add(duration);
                    Uebersicht.Children.Add(comment);

                    Grid.SetRow(day, Uebersicht.RowDefinitions.Count - 1);
                    Grid.SetRow(start, Uebersicht.RowDefinitions.Count - 1);
                    Grid.SetRow(end, Uebersicht.RowDefinitions.Count - 1);
                    Grid.SetRow(duration, Uebersicht.RowDefinitions.Count - 1);
                    Grid.SetRow(comment, Uebersicht.RowDefinitions.Count - 1);

                    Grid.SetColumn(start, 1);
                    Grid.SetColumn(end, 2);
                    Grid.SetColumn(duration, 3);
                    Grid.SetColumn(comment, 4);
                }
            }
            */
        }

        private void UpdateForm(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            // Update Timeline
            timeLine.Value = (Jetzt - Heute.StartZeit).TotalSeconds;

            // Restart Updater
            Updater.RunWorkerAsync();
        }

        private void ActivateDragMove(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }

        private void CloseClick(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void MaximizeClick(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void MinimizeClick(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ((UIElement)e.OriginalSource).Focus();
        }

        private void HomeBT_Click(object sender, MouseButtonEventArgs e)
        {
            Tabs.SelectedIndex = 0; // Home
        }

        private void ÜbersichtBT_Click(object sender, MouseButtonEventArgs e)
        {
            Tabs.SelectedIndex = 1; // View
        }

        private void EinstellungenBT_Click(object sender, MouseButtonEventArgs e)
        {
            Tabs.SelectedIndex = 2; // Settings
        }

        private void TestWebsite(object sender, MouseButtonEventArgs e)
        {
            Extensions.Wetter wetter = new Extensions.Wetter();
        }

        private void ShutdownWarnMe(object sender, System.ComponentModel.CancelEventArgs e)
        {
            PrepareShutdown();
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
