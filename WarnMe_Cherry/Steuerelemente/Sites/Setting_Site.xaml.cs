using System;
using System.Windows.Controls;
using static WarnMe_Cherry.Global;
using static WarnMe_Cherry.Datenbank.WARNME_CONFIG;

namespace WarnMe_Cherry.Steuerelemente.Sites
{
    /// <summary>
    /// Interaktionslogik für Setting_Site.xaml
    /// </summary>
    public partial class Setting_Site : UserControl, Interfaces.IUpdateable
    {
        public enum CHANGED_TYPE {OFFSET, TIME, SETTING};
        //public delegate void SettingChange(CHANGED_TYPE key, object value);
        //public event SettingChange SettingChanged;

        private bool initReady = false;

        public Setting_Site()
        {
            InitializeComponent();
        }

        public void Init()
        {
            // Mini
            StartMinimized.SwitchValue = DATA.THIS.EINSTELLUNGEN.MINIMIZED;

            // Offsets time
            StarttimeOffset.DateTime = DATA.THIS.EINSTELLUNGEN.OFFSET_START_TIME;
            EndtimeOffset.DateTime = DATA.THIS.EINSTELLUNGEN.OFFSET_END_TIME;

            // Offset breaks
            Offset1Start.DateTime = DATA.THIS.EINSTELLUNGEN.COFFEE.START;
            Offset1Duration.DateTime = DATA.THIS.EINSTELLUNGEN.COFFEE.DURATION;
            Offset2Start.DateTime = DATA.THIS.EINSTELLUNGEN.LUNCH.START;
            Offset2Duration.DateTime = DATA.THIS.EINSTELLUNGEN.LUNCH.DURATION;
        }

        public void Update()
        {
            initReady = false;
            // DO WORK
            initReady = true;
        }

        private void MinimizedChanged(bool value)
        {
            if (initReady)
                //SettingChanged?.Invoke(CHANGED_TYPE.SETTING, value);
                DATA.THIS.EINSTELLUNGEN.MINIMIZED = value;
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
                            DATA.THIS.EINSTELLUNGEN.OFFSET_START_TIME = value;
                            break;
                        case 1:
                            DATA.THIS.EINSTELLUNGEN.OFFSET_END_TIME = value;
                            break;
                        case 2:
                            DATA.THIS.EINSTELLUNGEN.COFFEE.START = value;
                            break;
                        case 3:
                            DATA.THIS.EINSTELLUNGEN.COFFEE.DURATION = value;
                            break;
                        case 4:
                            DATA.THIS.EINSTELLUNGEN.LUNCH.START = value;
                            break;
                        case 5:
                            DATA.THIS.EINSTELLUNGEN.LUNCH.DURATION = value;
                            break;
                        default:
                            throw new Exception("A new unhandled Usercontrol triggered an change event.");
                    }
                }
            }
        }
    }
}
