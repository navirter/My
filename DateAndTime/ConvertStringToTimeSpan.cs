using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace My.DateAndTime
{
    public static class ConvertStringToTimeSpan
    {
        /// <summary>
        /// Converts "dddd.HH.mm.ss" to TimeSpan. Ignores all exept for digits, "." and ":"
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TimeSpan Do(string source)
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
            catch { throw new Exception("can't read timespan(dddd.HH.mm.ss or dd: HH.mm.ss) " + source); }
        }
    }
}
