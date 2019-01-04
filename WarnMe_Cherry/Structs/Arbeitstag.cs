﻿using System;
using System.Collections.Generic;
using static WarnMe_Cherry.Datenbank.InternalVariables;

namespace WarnMe_Cherry
{
    [Serializable]
    public struct Arbeitstag : IEquatable<Arbeitstag>
    {
        TimeSpan startZeit;
        TimeSpan endZeit;

        public static readonly Arbeitstag Zero = new Arbeitstag();

        [Newtonsoft.Json.JsonProperty(PropertyName = "come")]
        public TimeSpan StartZeit { get => new TimeSpan(startZeit.Days, startZeit.Hours, startZeit.Minutes, startZeit.Seconds); set => startZeit = value; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "go")]
        public TimeSpan EndZeit { get => new TimeSpan(endZeit.Days, endZeit.Hours, endZeit.Minutes, endZeit.Seconds); set => endZeit = value; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "comment")]
        public string Bemerkung { get; set; }
        internal TimeSpan Duration { get => EndZeit - StartZeit; }

        public override bool Equals(object obj)
        {
            return obj is Arbeitstag && Equals((Arbeitstag)obj);
        }

        public bool Equals(Arbeitstag other)
        {
            return StartZeit.Equals(other.StartZeit) &&
                   EndZeit.Equals(other.EndZeit) &&
                   Bemerkung == other.Bemerkung;
        }

        public override int GetHashCode()
        {
            var hashCode = -499075268;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<TimeSpan>.Default.GetHashCode(StartZeit);
            hashCode = hashCode * -1521134295 + EqualityComparer<TimeSpan>.Default.GetHashCode(EndZeit);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Bemerkung);
            return hashCode;
        }

        public static bool operator==(Arbeitstag a1, Arbeitstag a2)
        {
            return a1.startZeit == a2.startZeit && a1.endZeit == a2.endZeit && a1.Bemerkung == a2.Bemerkung;
        }

        public static bool operator!=(Arbeitstag a1, Arbeitstag a2)
        {
            return a1.startZeit != a2.startZeit || a1.endZeit != a2.endZeit || a1.Bemerkung != a2.Bemerkung;
        }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Datenbank.Datenbank.DefaultSetting);
        }
    }
}
