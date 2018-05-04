using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace WarnMe_Cherry.Extensions
{
    public class DayMatcher
    {
        public string MATCH_STRING
        {
            get
            {
                StringBuilder @string = new StringBuilder("");
                if (!string.IsNullOrWhiteSpace(pat.Year))
                    @string.AppendFormat("{0}{1}", ID.Year, pat.Year);
                if (!string.IsNullOrWhiteSpace(pat.Month))
                    @string.AppendFormat("{0}{1}", ID.Month, pat.Month);
                if (!string.IsNullOrWhiteSpace(pat.Week))
                    @string.AppendFormat("{0}{1}", ID.Week, pat.Week);
                if (!string.IsNullOrWhiteSpace(pat.Day))
                    @string.AppendFormat("{0}{1}", ID.Day, pat.Day);
                return @string.ToString();
            }
            set
            {
                Regex r = new Regex(string.Format(@"({0}|{1}|{2}|{3})([^A-Z]+)", ID.Year, ID.Month, ID.Week, ID.Day));
                foreach (Match match in r.Matches(value))
                {
                    if (match.Groups.Count > 2)
                        switch (match.Groups[1].Value)
                        {
                            case ID.Year:
                                pat.Year = match.Groups[2].Value;
                                break;
                            case ID.Month:
                                pat.Month = match.Groups[2].Value;
                                break;
                            case ID.Week:
                                pat.Week = match.Groups[2].Value;
                                break;
                            case ID.Day:
                                pat.Day = match.Groups[2].Value;
                                break;
                            default:
                                break;
                        }
                }
            }
        }

        string DateString => DateTime.Now.Date.ToString("dd.MM.yy");

        string Year => DateTime.Now.Year.ToString();
        string Month => DateTime.Now.Month.ToString();
        string Week => System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar.GetWeekOfYear(DateTime.Now, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday).ToString();
        string Day => ((int)DateTime.Now.DayOfWeek).ToString();

        struct ID
        {
            public const string Year = "Year";
            public const string Month = "Month";
            public const string Week = "Week";
            public const string Day = "Day";
        }
        // const string allIDs = "YMWD";
        
        struct Pattern
        {
            public string Year;
            public string Month;
            public string Week;
            public string Day;
        }
        Pattern pat = new Pattern();

        public bool MatchToday()
        {
            return MatchYear() & MatchMonth() & MatchWeek() & MatchDay();
        }

        public void SetEveryWeek(DayOfWeek day)
        {
            pat.Day = ((int)day).ToString();
        }

        public void SetEverySecondWeek(DayOfWeek day)
        {
            pat.Week = ".?[02468]";
            pat.Day = ((int)day).ToString();
        }

        private bool MatchYear()
        {
            if (string.IsNullOrWhiteSpace(pat.Year))
                return true; // exclusive pattern
            return new Regex(pat.Year).IsMatch(Year);

            throw new NotImplementedException();
        }

        private bool MatchMonth()
        {
            if (string.IsNullOrWhiteSpace(pat.Month))
                return true; // exclusive pattern
            return new Regex(pat.Month).IsMatch(Month);

            throw new NotImplementedException();
        }

        private bool MatchWeek()
        {
            if (string.IsNullOrWhiteSpace(pat.Week))
                return true; // exclusive pattern
            return new Regex(pat.Week).IsMatch(Week);

            throw new NotImplementedException();
        }

        private bool MatchDay()
        {
            if (string.IsNullOrWhiteSpace(pat.Day))
                return true; // exclusive pattern
            return new Regex(pat.Day).IsMatch(Day);

            throw new NotImplementedException();
        }
    }
}
