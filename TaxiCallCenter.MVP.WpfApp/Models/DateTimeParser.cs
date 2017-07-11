using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TaxiCallCenter.MVP.WpfApp.Models
{
    public static class DateTimeParser
    {
        private static readonly Regex TimePattern1 = new Regex(@"^(?<hour>\d{1,2})(?: час\w*)? (?:(?<min>\d{1,2})|(?<min>0) 0)(?: мин\w*)?$");
        private static readonly Regex TimePattern2 = new Regex(@"^(?<hour>\d{1,2})(?: час\w*)?$");

        private static readonly Regex DateTimePattern1 = new Regex(@"^(?<day>\d{1,2}) (?<month>января|февраля|марта|апреля|мая|июня|июля|августа|сентября|октября|ноября|декабря) (?<hour>\d{1,2})(?: час\w*)? (?:(?<min>\d{1,2})|(?<min>0) 0)(?: мин\w*)?$");
        private static readonly Regex DateTimePattern2 = new Regex(@"^(?<day>\d{1,2}) (?<month>января|февраля|марта|апреля|мая|июня|июля|августа|сентября|октября|ноября|декабря) (?<hour>\d{1,2})(?: час\w*)?$");
        private static readonly Regex DateTimePattern3 = new Regex(@"^завтра (?<hour>\d{1,2})(?: час\w*)? (?:(?<min>\d{1,2})|(?<min>0) 0)(?: мин\w*)?$");
        private static readonly Regex DateTimePattern4 = new Regex(@"^завтра (?<hour>\d{1,2})(?: час\w*)?$");
        private static readonly Regex DateTimePattern5 = new Regex(@"^послезавтра (?<hour>\d{1,2})(?: час\w*)? (?:(?<min>\d{1,2})|(?<min>0) 0)(?: мин\w*)?$");
        private static readonly Regex DateTimePattern6 = new Regex(@"^послезавтра (?<hour>\d{1,2})(?: час\w*)?$");

        public static String ParseTime(String input)
        {
            Match match;
            if ((match = DateTimeParser.TimePattern1.Match(input)).Success)
            {
                var hour = Int32.Parse(match.Groups["hour"].Value);
                var min = Int32.Parse(match.Groups["min"].Value);

                if (hour < 0 || hour > 23) return null;
                if (min < 0 || min > 59) return null;

                return $"{hour:D2}:{min:D2}";
            }

            if ((match = DateTimeParser.TimePattern2.Match(input)).Success)
            {
                var hour = Int32.Parse(match.Groups["hour"].Value);

                if (hour < 0 || hour > 23) return null;

                return $"{hour:D2}:00";
            }

            return null;
        }

        public static String ParseDateTime(String input)
        {
            Match match;
            var months = new CultureInfo("ru-RU").DateTimeFormat.MonthGenitiveNames;
            if ((match = DateTimeParser.DateTimePattern1.Match(input)).Success)
            {
                var year = DateTime.Now.Year;
                var day = Int32.Parse(match.Groups["day"].Value);
                var month = match.Groups["month"].Value;
                var monthNo = Array.IndexOf(months, month) + 1;

                var hour = Int32.Parse(match.Groups["hour"].Value);
                var min = Int32.Parse(match.Groups["min"].Value);

                var date = new DateTime(year, monthNo, day, hour, min, 0);
                return date.ToString("yyyy-MM-dd HH:mm");
            }

            if ((match = DateTimeParser.DateTimePattern2.Match(input)).Success)
            {
                var year = DateTime.Now.Year;
                var day = Int32.Parse(match.Groups["day"].Value);
                var month = match.Groups["month"].Value;
                var monthNo = Array.IndexOf(months, month) + 1;

                var hour = Int32.Parse(match.Groups["hour"].Value);
                
                var date = new DateTime(year, monthNo, day, hour, 0, 0);
                return date.ToString("yyyy-MM-dd HH:mm");
            }

            if ((match = DateTimeParser.DateTimePattern3.Match(input)).Success)
            {
                var tommorow = DateTime.Now.AddDays(1);

                var hour = Int32.Parse(match.Groups["hour"].Value);
                var min = Int32.Parse(match.Groups["min"].Value);

                var date = new DateTime(tommorow.Year, tommorow.Month, tommorow.Day, hour, min, 0);
                return date.ToString("yyyy-MM-dd HH:mm");
            }

            if ((match = DateTimeParser.DateTimePattern4.Match(input)).Success)
            {
                var tommorow = DateTime.Now.AddDays(1);

                var hour = Int32.Parse(match.Groups["hour"].Value);

                var date = new DateTime(tommorow.Year, tommorow.Month, tommorow.Day, hour, 0, 0);
                return date.ToString("yyyy-MM-dd HH:mm");
            }

            if ((match = DateTimeParser.DateTimePattern5.Match(input)).Success)
            {
                var tommorow = DateTime.Now.AddDays(2);

                var hour = Int32.Parse(match.Groups["hour"].Value);
                var min = Int32.Parse(match.Groups["min"].Value);

                var date = new DateTime(tommorow.Year, tommorow.Month, tommorow.Day, hour, min, 0);
                return date.ToString("yyyy-MM-dd HH:mm");
            }

            if ((match = DateTimeParser.DateTimePattern6.Match(input)).Success)
            {
                var tommorow = DateTime.Now.AddDays(2);

                var hour = Int32.Parse(match.Groups["hour"].Value);

                var date = new DateTime(tommorow.Year, tommorow.Month, tommorow.Day, hour, 0, 0);
                return date.ToString("yyyy-MM-dd HH:mm");
            }

            return null;
        }
    }
}
