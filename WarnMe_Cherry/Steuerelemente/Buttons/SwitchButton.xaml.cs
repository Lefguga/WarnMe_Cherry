using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WarnMe_Cherry.Steuerelemente
{
    /// <summary>
    /// Interaktionslogik für SwitchButton.xaml
    /// </summary>
    public partial class SwitchButton : UserControl
    {
        public delegate void ValueChange(bool value);
        public event ValueChange ValueChanged;

        bool switchValue = false;
        public bool SwitchValue
        {
            get => switchValue;
            set
            {
                switchValue = value;

                Switch.HorizontalAlignment = SwitchValue ? HorizontalAlignment.Left : HorizontalAlignment.Right;
                SwitchBG.BorderBrush = SwitchValue ? onColor : offColor;
                ValueChanged?.Invoke(SwitchValue);
            }
        }

        SolidColorBrush offColor = new SolidColorBrush(Color.FromArgb(0x7F, 0xE0, 0xE0, 0xE0));
        public SolidColorBrush OffColor { get => offColor; set => offColor = value; }

        SolidColorBrush onColor = new SolidColorBrush(Color.FromArgb(0x7F, 0x00, 0x55, 0xAA));
        public SolidColorBrush OnColor { get => onColor; set => onColor = value; }

        public SwitchButton()
        {
            InitializeComponent();
        }

        private void Switch_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SwitchValue = (SwitchValue == false);
        }
    }
}
