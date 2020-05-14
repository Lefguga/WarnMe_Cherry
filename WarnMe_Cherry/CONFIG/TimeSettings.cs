using System;
using System.Collections.Generic;
using WarnMe_Cherry.Steuerelemente;

namespace WarnMe_Cherry.CONFIG
{
    internal class TimeSettings
    {
        public TimeSpan WORKTIME = new TimeSpan(7, 0, 0);
        public TimeSpan WORKLIMIT = new TimeSpan(10, 0, 0);
        public TimeSpan START_DELAY = new TimeSpan(0, 5, 0);
        public TimeSpan END_DELAY = new TimeSpan(0, 5, 0);

        public Brake COFFEE = new Brake { START = new TimeSpan(3, 0, 0), DURATION = new TimeSpan(0, 15, 0) };
        public Brake LUNCH = new Brake { START = new TimeSpan(6, 15, 0), DURATION = new TimeSpan(0, 30, 0) };

    }
}
