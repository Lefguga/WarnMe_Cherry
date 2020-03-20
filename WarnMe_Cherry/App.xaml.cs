using System.Windows;

namespace WarnMe_Cherry
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            WarnMeLoader splashScreen = new WarnMeLoader(typeof(App).Assembly.GetName().Version);
            splashScreen.Show();

            Datenbank.Datenbank datenbank = new Datenbank.Datenbank(WARNME_CONFIG.SAVE_CONFIG.CONFIG_FILE_NAME);

            WarnMeVision vision = new WarnMeVision();
            vision.Loaded += (object o, RoutedEventArgs re) => { splashScreen.Hide(); };
            vision.Show();
            splashScreen?.Close(); // prevent splashscreen stay open when vision fails
        }
    }
}
