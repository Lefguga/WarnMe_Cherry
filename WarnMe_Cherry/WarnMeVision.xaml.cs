using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WarnMe_Cherry.Steuerelemente;

namespace WarnMe_Cherry
{
    /// <summary>
    /// Interaktionslogik für WarnMeVision.xaml
    /// </summary>
    public partial class WarnMeVision : Window
    {
        System.ComponentModel.BackgroundWorker Updater = new System.ComponentModel.BackgroundWorker();

        const string WORKINGDAYS_TABLE_NAME = "Arbeitstage";
        const string WECKER_TABLE_NAME = "Wecker";
        const string SETTINGS_TABLE_NAME = "Einstellungen";

        // Shortcuts
        string HeuteString => DateTime.Now.ToShortDateString();
        TimeSpan Jetzt => DateTime.Now.TimeOfDay;

        Arbeitstag Heute
        {
            get
            {
                Arbeitstag heute;
                if (Datenbank.Exists(WORKINGDAYS_TABLE_NAME, HeuteString))
                {
                    heute = Datenbank.Select<Arbeitstag>(WORKINGDAYS_TABLE_NAME, HeuteString);
                    heute.EndZeit = DateTime.Now.TimeOfDay;
                }
                else
                {
                    heute = new Arbeitstag()
                    {
                        StartZeit = DateTime.Now.TimeOfDay - Extern.UpDuration,
                        EndZeit = DateTime.Now.TimeOfDay
                    };
                    Datenbank.Insert(WORKINGDAYS_TABLE_NAME, HeuteString, heute);
                }
                return heute;
            }
            set
            {
                Datenbank.Update(WORKINGDAYS_TABLE_NAME, HeuteString, value);
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
            Datenbank = new Datenbank.Datenbank(@".\Configuration.json");
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
            timeLine.Value = (Jetzt - Heute.StartZeit).TotalSeconds;

            // Restart Updater
            Updater.RunWorkerAsync();
        }

        private void InitFormValues()
        {

            // init new day of working
            Arbeitstag heute = Heute;

            StartTimePicker.DateTime = heute.StartZeit;

            EndTimePicker.DateTime = heute.StartZeit + new TimeSpan(7, 45, 0);
            MaxEndTimePicker.DateTime = heute.StartZeit + new TimeSpan(10, 45, 0);

            Datenbank.Update("Sonstiges", "SystemUpTime", Extern.UpDuration);
            Datenbank.Commit();
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
