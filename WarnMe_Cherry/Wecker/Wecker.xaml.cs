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

namespace WarnMe_Cherry.Wecker
{
    /// <summary>
    /// Interaktionslogik für Wecker.xaml
    /// </summary>
    [Serializable]
    public partial class Wecker : UserControl
    {
        public struct DaysOfWeek
        {
            public bool Montag { get; set; }
            public bool Dienstag { get; set; }
            public bool Mittwoch { get; set; }
            public bool Donnerstag { get; set; }
            public bool Freitag { get; set; }
            public bool Samstag { get; set; }
            public bool Sonntag { get; set; }
        }

        DaysOfWeek activeDays;
        public DaysOfWeek ActiveDays { get => activeDays; set => activeDays = value; }
        
        TimeSpan time = new TimeSpan();
        public TimeSpan Time { get => time; set => time = value; }

        public bool IsActiveted
        {
            get => Switch.SwitchValue;
            set
            {
                Switch.SwitchValue = value;
            }
        }

        public Wecker()
        {
            InitializeComponent();
        }

        private void Titel_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox editText = new TextBox()
            {
                Focusable=true,
                Text = Titel.Text,
                BorderBrush=null,
                Background = Brushes.Transparent,
                Margin = new Thickness(0, 0, 0, -5),
                FontSize = Titel.FontSize + 1,
                Foreground = Foreground
            };
            editText.KeyDown += delegate (object o, KeyEventArgs a)
            {
                if (a.Key == Key.Enter)
                {
                    Titel.Text = ((TextBox)o).Text;
                    ContentGrid.Children.Remove((TextBox)o);
                    Titel.Visibility = Visibility.Visible;
                }
                else if (a.Key == Key.Escape)
                {
                    ContentGrid.Children.Remove((TextBox)o);
                    Titel.Visibility = Visibility.Visible;
                }
            };
            editText.LostFocus += delegate (object o, RoutedEventArgs a)
            {
                    Titel.Text = ((TextBox)o).Text;
                    ContentGrid.Children.Remove((TextBox)o);
                    Titel.Visibility = Visibility.Visible;
            };

            Titel.Visibility = Visibility.Hidden;
            editText.SetValue(Grid.RowProperty, 1);
            ContentGrid.Children.Add(editText);
            editText.Focus();
            editText.SelectAll();
        }
    }
}
