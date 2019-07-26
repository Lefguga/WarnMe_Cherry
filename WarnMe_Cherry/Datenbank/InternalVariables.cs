using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;

namespace WarnMe_Cherry.Datenbank
{
    internal class THIS
    {
        #region STRUCTS
        internal struct INTERNAL
        {
            internal const string CONFIG_FILE_NAME = @".\Configuration.json";
            internal string BACKUP_FILE_NAME { get; set; }
        }

        [Newtonsoft.Json.JsonProperty(PropertyName = "Workdays")]
        public Dictionary<DateTime, Arbeitstag> WORKINGDAYS { get; set; } = new Dictionary<DateTime, Arbeitstag>();

        [Newtonsoft.Json.JsonProperty(PropertyName = "Clocks")]
        public Dictionary<string, Steuerelemente.Wecker> WECKER { get; set; } = new Dictionary<string, Steuerelemente.Wecker>();

        [Newtonsoft.Json.JsonProperty(PropertyName = "Settings")]
        public EINSTELLUNGEN EINSTELLUNGEN = new EINSTELLUNGEN();

        [Newtonsoft.Json.JsonProperty(PropertyName = "Colors")]
        public COLORS COLORS = new COLORS();

        //[Newtonsoft.Json.JsonProperty(PropertyName = "Other")]
        //public OTHER OTHER = new OTHER();
        #endregion

        internal DateTime Date { get; set; }
        internal TimeSpan Heute_StartTime
        {
            get => WORKINGDAYS[DateNow].StartZeit;
            set
            {
                Arbeitstag _tag = WORKINGDAYS[DateNow];
                _tag.StartZeit = value;
                WORKINGDAYS[DateNow] = _tag;
            }
        }
        internal TimeSpan Heute_EndTime
        {
            get => WORKINGDAYS[DateNow].EndZeit;
            set
            {
                Arbeitstag _tag = WORKINGDAYS[DateNow];
                _tag.EndZeit = value;
                WORKINGDAYS[DateNow] = _tag;
            }
        }
        internal Arbeitstag Heute
        {
            get
            {
                if (WORKINGDAYS.ContainsKey(DateNow))
                    return WORKINGDAYS[DateNow];
                else
                    throw new DatenbankBaseException("Key not found");
            }
            set
            {
                if (WORKINGDAYS.ContainsKey(DateNow))
                    WORKINGDAYS[DateNow] = value;
                else
                    throw new DatenbankBaseException("Key not found");
            }
        }
        internal static DateTime DateNow => DateTime.Now.Date;
        /// <summary>
        /// Gibt einen <see cref="string"/> mit <see cref="DateTime.Now"/>.ToShortDateString() Wert mit dem aktuellen Datum zurück.
        /// </summary>
        //internal static string StringNow => DateTime.Now.ToShortDateString();
        /// <summary>
        /// Gibt einen <see cref="DateTime.TimeOfDay"/> Wert mit der aktuellen Uhrzeit zurück.
        /// </summary>
        internal static TimeSpan TimeNow => DateTime.Now.TimeOfDay;
        //internal static Datenbank Datenbank { get; set; }

    }

    internal class OTHER
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "Uptime")]
        public TimeSpan SYSTEM_UP_TIME { get; set; } = new TimeSpan();

        [Newtonsoft.Json.JsonProperty(PropertyName = "Update")]
        public DateTime SYSTEM_UP_DATE { get; set; } = new DateTime();
    }

    internal class EINSTELLUNGEN
    {
        // TABLE

        // Workingtime
        [Newtonsoft.Json.JsonProperty(PropertyName = "Workingtime")]
        public TimeSpan WORKDAY_NORMAL { get; set; } = new TimeSpan(7, 0, 0);

        [Newtonsoft.Json.JsonProperty(PropertyName = "Workinglimit")]
        public TimeSpan WORKDAY_LIMIT { get; set; } = new TimeSpan(10, 0, 0);

        // Offstes
        [Newtonsoft.Json.JsonProperty(PropertyName = "StartDelay")]
        public TimeSpan OFFSET_START_TIME { get; set; } = new TimeSpan(0, 5, 0);

        [Newtonsoft.Json.JsonProperty(PropertyName = "EndDelay")]
        public TimeSpan OFFSET_END_TIME { get; set; } = new TimeSpan(0, 5, 0);

        // Coffee
        [Newtonsoft.Json.JsonProperty(PropertyName = "Coffee")]
        public TIME COFFEE { get; set; } = new TIME
        {
            START = new TimeSpan(3, 0, 0),
            DURATION = new TimeSpan(0, 15, 0)
        };

        // Lunch

        [Newtonsoft.Json.JsonProperty(PropertyName = "Lunch")]
        public TIME LUNCH { get; set; } = new TIME
        {
            START = new TimeSpan(6, 0, 0),
            DURATION = new TimeSpan(0, 30, 0)
        };

        [Newtonsoft.Json.JsonProperty(PropertyName = "StartMinimized")]
        public bool MINIMIZED { get; set; } = true;



        internal TimeSpan TOTAL_WORKTIME => WORKDAY_NORMAL + COFFEE.DURATION + LUNCH.DURATION;
        internal TimeSpan TOTAL_WORKLIMIT => WORKDAY_LIMIT + COFFEE.DURATION + LUNCH.DURATION;
    }

    internal class COLORS
    {
        public Color ACCENT_COLOR { get; set; } = Color.FromArgb(255, 0, 0, 204);
        public Color MAIN_COLOR { get; set; } = Color.FromArgb(255, 38, 38, 38);
        internal Color MAIN_COLOR_WEAK => Color.Multiply(MAIN_COLOR, AVG_COLOR(MAIN_COLOR) < 128 ? 1.125f : 0.875f);
        public Color FONT_COLOR { get; set; } = Color.FromArgb(255, 234, 234, 234);

        private static byte AVG_COLOR(Color c)
        {
            return (byte)((c.R + c.G + c.B) / 3);
        }
    }

    internal class TIME
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "Starttime")]
        public TimeSpan START { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "Duration")]
        public TimeSpan DURATION { get; set; }
    }
}
