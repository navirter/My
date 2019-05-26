using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace My
{
    public static class WaitSmoothly
    {
        public static bool Stop = false;
        public static bool Pause = false;

        public static TimeSpan Do(double secondsMin, double secondsMax)
        {
            int min = Convert.ToInt32(secondsMin);
            int max = Convert.ToInt32(secondsMax);
            int x = new Random().Next(min, max);
            return Do(x);
        }

        public static TimeSpan Do(double seconds)
        {
            DateTime start = DateTime.Now;
            DateTime target = start.AddSeconds(seconds);
            while (DateTime.Now < target)
            {
                Application.DoEvents();
                //Thread.Sleep(250);//instashutdown
                if (ThreadSeeker.Stop)
                    return DateTime.Now - start;
            }
            while (Pause)
            {
                if (ThreadSeeker.Stop)
                    return DateTime.Now - start;
                Application.DoEvents();
                //Thread.Sleep(250);//instashutdown
            }
            return DateTime.Now - start;
        }
    }
}
