using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarnMe_Cherry
{
    public struct Arbeitstag
    {
        TimeSpan startZeit;
        TimeSpan endZeit;
        public TimeSpan StartZeit { get => new TimeSpan(startZeit.Days, startZeit.Hours, startZeit.Minutes, startZeit.Seconds); set => startZeit = value; }
        public TimeSpan EndZeit { get => new TimeSpan(endZeit.Days, endZeit.Hours, endZeit.Minutes, endZeit.Seconds); set => endZeit = value; }
    }
}
