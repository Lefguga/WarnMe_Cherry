using System;
using System.Windows;

namespace WarnMe_Cherry.Datenbank
{
    internal static class InternalVariables
    {
        #region STRUCTS
        public struct WORKINGDAYS
        {
            public const string NAME = "Arbeitstage";
            public System.Collections.Generic.Dictionary<DateTime, Arbeitstag> VALUE { get; set; }
        }
        public struct WECKER
        {
            public const string NAME = "Wecker";
            public System.Collections.Generic.Dictionary<string, Steuerelemente.Wecker> VALUE { get; set; }
        }
        public struct SETTINGS
        {
            public const string CONFIG_FILE_NAME = @".\Configuration.json";
            // TABLE
            public const string NAME = "Einstellungen";
            public struct WorkingdayNormal
            {
                public const string NAME = "Arbeitstag";
                public TimeSpan Value { get; set; }
            }
            public struct WorkingdayLimit
            {
                public const string NAME = "Stundenlimit";
                public TimeSpan Value { get; set; }
            }
            public struct StartTimeOffset
            {
                public const string NAME = "StartTimeOffset";
                public TimeSpan Value { get; set; }
            }
            public struct EndTimeOffset
            {
                public const string NAME = "EndTimeOffset";
                public TimeSpan Value { get; set; }
            }
        }
        public struct OTHER
        {
            public const string NAME = "Sonstiges";
            public const string SYSTEM_UP_TIME = "SystemUpTime";
        }
        #endregion
        
        public static Arbeitstag Heute
        {
            get
            {
                if (Datenbank.Exists(WORKINGDAYS.NAME, HeuteString))
                {
                    return Datenbank.Select<Arbeitstag>(WORKINGDAYS.NAME, HeuteString);
                }
                throw new ResourceReferenceKeyNotFoundException("Data not found", HeuteString);
            }
            set
            {
                Datenbank.Update(WORKINGDAYS.NAME, HeuteString, value);
            }
        }

        static string HeuteString => DateTime.Now.ToShortDateString();
        static TimeSpan Jetzt => DateTime.Now.TimeOfDay;
        internal static Datenbank Datenbank { get; set; }

    }
}
