using System;
using System.Runtime.InteropServices;

namespace WarnMe_Cherry
{
    public static class Extern
    {
        [DllImport("kernel32")]
        extern static UInt64 GetTickCount64();

        public static TimeSpan SystemUpDuration
        {
            get
            {
                GetTickCount64(); // run 2 times to be sure
                return TimeSpan.FromMilliseconds(GetTickCount64());
            }
        }

        public static DateTime SystemUpTime
        {
            get
            {
               return DateTime.Now - SystemUpDuration;
            }
        }
    }
}
