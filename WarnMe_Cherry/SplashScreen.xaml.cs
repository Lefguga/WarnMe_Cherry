using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace WarnMe_Cherry
{
    /// <summary>
    /// Interaktionslogik für SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        internal System.Windows.Media.ImageSource Source
        {
            get => loading.Source;
            set => loading.Source = value;
        }

        private BackgroundWorker updater = new BackgroundWorker();
        private Random random = new Random(1337);
        
        private bool readyToClose = false;

        public SplashScreen(Version version)
        {
            InitializeComponent();
            RunningVersion.Text = $"Version: {version}";
            this.Title          = $"WarnMe {version}";
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            // awaits an amount of time
            updater.DoWork += Updater_DoWork;
            updater.RunWorkerCompleted += RunFakeProgress;
            updater.RunWorkerAsync(100);
            
        }

        private void Updater_DoWork(object o, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep((int)e.Argument);
        }

        private void RunFakeProgress(object o, RunWorkerCompletedEventArgs e)
        {
            if (progress.Value < 100)
            {
                progress.Value += random.NextDouble() * 20d;
#if Release
                updater.RunWorkerAsync(random.Next(500));
#else
                updater.RunWorkerAsync(random.Next(2000));
#endif
            }
            else
            {
                Close();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (readyToClose)
            {
                base.OnClosing(e);
            }
            else
            {
                readyToClose = true;
                e.Cancel = true;
            }
        }

        private void MediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show(e.ToString(), e.ErrorException.ToString());
        }
    }
}
