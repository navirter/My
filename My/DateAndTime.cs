using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My
{
    public static class DateAndTime
    {
        public static readonly string dateTimeFormat = "yyyy.MM.dd: hh.mm.ss";
        public static readonly string dateFormat = "yyyy.MM.dd";
        public static readonly string timeSpanFormat = "dd: hh.mm.ss";

        /// <summary>
        /// Converts "yyyy.MM.dd: HH.mm.ss" or "yyyy.MM.dd" to DateTime. Ignores all exept for digits, "." and ":"
        /// </summary>
        /// <param name="source"></param>
        /// <param name="withTime"></param>
        /// <returns></returns>
        public static DateTime convertStringToDateTime(string source, bool withTime = true)
        {
            try
            {

                string nsource = new string(source.ToCharArray().Where(s => Char.IsDigit(s) || s == '.' || s == ':').ToArray());
                string[] parts = nsource.Split(new[] { ".", ":" }, StringSplitOptions.RemoveEmptyEntries);
                int year = int.Parse(parts[0]);
                int month = int.Parse(parts[1]);
                int day = int.Parse(parts[2]);
                int hour = 0; int minute = 0; int second = 0;
                if (withTime)
                {
                    int.TryParse(parts[3], out hour);
                    int.TryParse(parts[4], out minute);
                    try { int.TryParse(parts[5], out second); } catch { }
                }
                DateTime res = new DateTime(year, month, day, hour, minute, second);
                return res;
            }
            catch { string full = ""; if (withTime) full = "full"; throw new Exception("can't " + full + " read datetime " + source); }
        }
        /// <summary>
        /// Converts "dd.HH.mm.ss" or "yyyy.MM.dd" to TimeSpan. Ignores all exept for digits, "." and ":"
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TimeSpan convertStringToTimeSpan(string source)
        {
            try
            {
                string nsource = new string(source.ToCharArray().Where(s => Char.IsDigit(s) || s == '.' || s == ':').ToArray());
                string[] parts = nsource.Split(new[] { ".", ":" }, StringSplitOptions.RemoveEmptyEntries);
                int day = Convert.ToInt32(double.Parse(parts[0]));
                int hour = 0; int.TryParse(parts[1], out hour);
                int minute = 0; int.TryParse(parts[2], out minute);
                int second = 0; try { int.TryParse(parts[3], out second); } catch { }
                return new TimeSpan(day, hour, minute, second);
            }
            catch { throw new Exception("can't read timespan " + source); }
        }
        /// <summary>
        /// Format: yyyy.MM.dd: hh.mm.ss
        /// </summary>
        /// <param name="time"></param>
        /// <param name="withTime"></param>
        /// <returns></returns>
        public static string convertDateTimeToString(DateTime time, bool withTime = true, bool with_words = false)
        {
            string year = time.Year.ToString();
            while (year.Length < 4) year = "0" + year;
            string month = time.Month.ToString();
            if (month.Length == 1) month = "0" + month;
            string day = time.Day.ToString();
            if (day.Length == 1) day = "0" + day;
            string hour = time.Hour.ToString();
            if (hour.Length == 1) hour = "0" + hour;
            string minute = time.Minute.ToString();
            if (minute.Length == 1) minute = "0" + minute;
            string second = time.Second.ToString();
            if (second.Length == 1) second = "0" + second;

            if (!with_words)
            {
                string res = year + "." + month + "." + day;
                if (withTime) res += ": " + hour + "." + minute + "." + second;
                return res;
            }
            else
            {
                string res = year + "г." + month + "м." + day + "д.";
                if (withTime) res += ": " + hour + "ч." + minute + "м." + second + "с.";
                return res;
            }
        }
        /// <summary>
        /// Format: dd: hh.mm.ss
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string convertTimeSpanToString(TimeSpan time, bool with_words = false)
        {
            string day = Convert.ToInt32(Math.Truncate(time.TotalDays)).ToString();
            while (day.Length < 4) day = "0" + day;
            string hour = time.Hours.ToString();
            if (hour.Length == 1) hour = "0" + hour;
            string minute = time.Minutes.ToString();
            if (minute.Length == 1) minute = "0" + minute;
            string second = time.Seconds.ToString();
            if (second.Length == 1) second = "0" + second;
            if (!with_words)
            {
                string res = day + ": " + hour + "." + minute + "." + second;
                return res;
            }
            else
            {
                string res = day + "д.: " + hour + "ч." + minute + "м." + second + "с.";
                return res;
            }
        }
    }
}
