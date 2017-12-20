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
    }
}
