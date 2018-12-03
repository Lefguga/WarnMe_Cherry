using System;
using System.Windows.Controls;
using WarnMe_Cherry.Datenbank;

namespace WarnMe_Cherry.Steuerelemente.Sites
{
    /// <summary>
    /// Interaktionslogik für Setting_Site.xaml
    /// </summary>
    public partial class Setting_Site : UserControl
    {
        public const string STARTTIMEOFFSET = "StartTimeOffset";
        public const string ENDTIMEOFFSET = "EndTimeOffset";
        public const string HOUR3OFFSET = "Hour3Offset";
        public const string HOUR6OFFSET = "Hour6Offset";

        public delegate void SettingChange(string key, object value);
        public event SettingChange SettingChanged;

        private bool initReady = false;

        public Setting_Site()
        {
            InitializeComponent();
        }

        public void Update()
        {
            initReady = false;

            if (InternalVariables.Datenbank.TrySelect(InternalVariables.SETTINGS.NAME, STARTTIMEOFFSET, out string value))
                StarttimeOffset.DateTime = TimeSpan.Parse(value);
            if (InternalVariables.Datenbank.TrySelect(InternalVariables.SETTINGS.NAME, ENDTIMEOFFSET, out value))
                EndtimeOffset.DateTime = TimeSpan.Parse(value);
            if (InternalVariables.Datenbank.TrySelect(InternalVariables.SETTINGS.NAME, HOUR3OFFSET, out value))
                Hour3Offset.DateTime = TimeSpan.Parse(value);
            if (InternalVariables.Datenbank.TrySelect(InternalVariables.SETTINGS.NAME, HOUR6OFFSET, out value))
                Hour6Offset.DateTime = TimeSpan.Parse(value);

            initReady = true;
        }

        private void Starttime_Changed(TimeSpan value)
        {
            if (initReady)
               SettingChanged?.Invoke(STARTTIMEOFFSET, value);
        }

        private void Endtime_Changed(TimeSpan value)
        {
            if (initReady)
                SettingChanged?.Invoke(ENDTIMEOFFSET, value);
        }

        private void Hour3_Changed(TimeSpan value)
        {
            if (initReady)
                SettingChanged?.Invoke(HOUR3OFFSET, value);
        }

        private void Hour6_Changed(TimeSpan value)
        {
            if (initReady)
                SettingChanged?.Invoke(HOUR6OFFSET, value);
        }
    }
}
