using System;
using System.Collections.Generic;
using System.Text;

namespace My.DateAndTime
{
    public static class ConvertDateTimeToString
    {
        /// <summary>
        /// Format: yyyy.MM.dd: hh.mm.ss
        /// </summary>
        /// <param name="time"></param>
        /// <param name="withTime"></param>
        /// <returns></returns>
        public static string Do(DateTime time, bool withTime = true, bool withWords = false)
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

            if (!withWords)
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
    }
}
