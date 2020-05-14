using System;
using System.Windows.Controls;
using WarnMe_Cherry.Steuerelemente.Subparts;
using static GLOBAL.CONFIG;

namespace WarnMe_Cherry.Steuerelemente.Sites.Settings
{
    /// <summary>
    /// Interaktionslogik für TimingsList.xaml
    /// </summary>
    public partial class TimingsList : UserControl, Interfaces.IUpdateable
    {
        public delegate void ValueChange();
        public event ValueChange ValueUpdated;

        private bool initReady = false;

        public TimingsList()
        {
            InitializeComponent();
        }

        public void Update()
        {
            if (!initReady)
            {
                initReady = true;
            }
        }

        private void MinimizedChanged(bool value)
        {
            if (initReady)
            {
                WARNME_CONFIG.SETTINGS.MINIMIZED = value;
                UpdateEvent();
            }
        }

        private void TimeSetting_Changed(object sender, TimeSpan value)
        {
            if (initReady)
            {
                if (sender is DateTimePicker)
                {
                    // TODO: its bit of a hack to do it like this
                    switch (Grid.GetRow(sender as DateTimePicker))
                    {
                        case 0:
                            WARNME_CONFIG.TIME.START_DELAY = value;
                            break;
                        case 1:
                            WARNME_CONFIG.TIME.END_DELAY = value;
                            break;
                        case 2:
                            WARNME_CONFIG.TIME.COFFEE.START = value;
                            break;
                        case 3:
                            WARNME_CONFIG.TIME.COFFEE.DURATION = value;
                            break;
                        case 4:
                            WARNME_CONFIG.TIME.LUNCH.START = value;
                            break;
                        case 5:
                            WARNME_CONFIG.TIME.LUNCH.DURATION = value;
                            break;
                        default:
                            throw new Exception("A new unhandled Usercontrol triggered an change event.");
                    }
                }
                UpdateEvent();
            }
        }

        private void UpdateEvent()
        {
            ValueUpdated?.Invoke();
        }
    }
}
