using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WarnMe_Cherry.Extensions;

namespace WarnMe_Cherry.Wecker
{
    /// <summary>
    /// Interaktionslogik für Wecker.xaml
    /// </summary>
    [Serializable]
    public partial class Wecker : UserControl
    {
        public DayMatcher ActiveDays { get ; set; }
        
        public TimeSpan Time { get; set; }

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
