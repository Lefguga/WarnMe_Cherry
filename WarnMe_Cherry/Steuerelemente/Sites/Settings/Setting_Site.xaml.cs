using System;
using System.Windows.Controls;
using WarnMe_Cherry.Steuerelemente.Subparts;
using static GLOBAL.CONFIG;

namespace WarnMe_Cherry.Steuerelemente.Sites.Settings
{
    /// <summary>
    /// Interaktionslogik für Setting_Site.xaml
    /// </summary>
    public partial class Setting_Site : UserControl, Interfaces.IUpdateable
    {
        public delegate void ValueChange();
        public event ValueChange ValueUpdated;

        public Setting_Site()
        {
            InitializeComponent();
        }

        public void Init()
        {
            // Mini
            Timings.StartMinimized.SwitchValue = WARNME_CONFIG.SETTINGS.MINIMIZED;

            // Offsets time
            Timings.StarttimeOffset.DateTime = WARNME_CONFIG.TIME.START_DELAY;
            Timings.EndtimeOffset.DateTime = WARNME_CONFIG.TIME.END_DELAY;

            // Offset breaks
            Timings.Offset1Start.DateTime = WARNME_CONFIG.TIME.COFFEE.START;
            Timings.Offset1Duration.DateTime = WARNME_CONFIG.TIME.COFFEE.DURATION;
            Timings.Offset2Start.DateTime = WARNME_CONFIG.TIME.LUNCH.START;
            Timings.Offset2Duration.DateTime = WARNME_CONFIG.TIME.LUNCH.DURATION;
        }

        public void Update()
        {
            // DO WORK
            Timings.Update();
        }

        public void UpdateEvent()
        {
            ValueUpdated?.Invoke();
        }
        private void SwitchToTimings(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SettingsTab.SelectedIndex = 0;
        }

        private void SwitchToDmar(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SettingsTab.SelectedIndex = 1;
        }        
    }
}
