using System.Windows;
using static GLOBAL.CONFIG;
using static WarnMe_Cherry.Global;

namespace WarnMe_Cherry
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        internal Datenbank.Datenbank datenbank;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            WarnMeLoader splashScreen = new WarnMeLoader(typeof(App).Assembly.GetName().Version);
            splashScreen.Show();

            datenbank = new Datenbank.Datenbank(SAVE_CONFIG.CONFIG_FILE_NAME);

            WarnMeVision vision = new WarnMeVision();
            vision.Loaded += (object o, RoutedEventArgs re) => { splashScreen.Hide(); };
            vision.NewDataAvailable += () =>
            {
#if TRACE
                INFO("Application: Data Updated.");
#endif
                datenbank.Commit(); 
            };
            vision.Show();
            splashScreen?.Close(); // prevent splashscreen stay open when vision fails
        }

    }
}
