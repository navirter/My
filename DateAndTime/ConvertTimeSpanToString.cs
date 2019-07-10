using System;
using System.Collections.Generic;
using System.Text;

namespace My.DateAndTime
{
    public static class ConvertTimeSpanToString
    {
        /// <summary>
        /// Format: dddd: hh.mm.ss
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string Do(TimeSpan time, bool with_words = false)
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
