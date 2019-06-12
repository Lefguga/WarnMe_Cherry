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

            SplashScreen splashScreen = new SplashScreen(typeof(App).Assembly.GetName().Version);
            splashScreen.Show();

            WarnMeVision vision = new WarnMeVision();
            vision.Loaded += (object o, RoutedEventArgs re) => { splashScreen.Close(); };
            vision.Show();
            splashScreen?.Close(); // prevent splashscreen from beeing open when vision fails
        }
    }
}
