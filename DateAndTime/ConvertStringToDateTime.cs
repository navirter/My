using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace My.DateAndTime
{
    public static class ConvertStringToDateTime
    {
        /// <summary>
     /// Converts "yyyy.MM.dd: HH.mm.ss" or "yyyy.MM.dd" to DateTime. Ignores all exept for digits, "." and ":"
     /// </summary>
     /// <param name="source"></param>
     /// <param name="withTime"></param>
     /// <returns></returns>
        public static DateTime Do(string source, bool withTime = true)
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
            catch { string full = ""; if (withTime) full = "full "; throw new Exception("can't " + full + "read datetime(yyyy.MM.dd: HH.mm.ss) " + source); }
        }
    }
}
