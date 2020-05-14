using System;
using System.Collections.Generic;
using WarnMe_Cherry.CONFIG;
using WarnMe_Cherry.Steuerelemente.Subparts;

namespace WarnMe_Cherry.CONFIG
{
    [Serializable]
    public class WARNME_CONFIG
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "ApplicationSettings")]
        internal ApplicationSettings SETTINGS = new ApplicationSettings();

        [Newtonsoft.Json.JsonProperty(PropertyName = "TimeSettings")]
        internal TimeSettings TIME = new TimeSettings();

        [Newtonsoft.Json.JsonProperty(PropertyName = "Colors")]
        internal COLORS COLORS = new COLORS();

        [Newtonsoft.Json.JsonProperty(PropertyName = "Alarm")]
        internal Dictionary<string, Wecker> WECKER = new Dictionary<string, Wecker>();

        [Newtonsoft.Json.JsonProperty(PropertyName = "Workdays")]
        internal Dictionary<DateTime, Arbeitstag> WORKINGDAYS = new Dictionary<DateTime, Arbeitstag>();
    }

}

namespace GLOBAL
{
    public static class CONFIG
    {
        public static WARNME_CONFIG WARNME_CONFIG = new WARNME_CONFIG();
        public struct SAVE_CONFIG
        {
            internal const string CONFIG_FILE_NAME = @".\Configuration.json";
            // internal string BACKUP_FILE_NAME { get; set; }
        }

        // shortcuts
        public static DateTime TODAY => DateTime.Today.Date;
        public static TimeSpan NOW => DateTime.Now.TimeOfDay;
    }
}
