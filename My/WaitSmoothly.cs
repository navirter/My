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
        public static void Do(double seconds, ref bool stop)
        {
            for (double i = 0.1; i < seconds; i += 0.1)
            {
                Application.DoEvents();
                Thread.Sleep(100);
                if (stop)
                    return;
            }
            while (ThreadSeeker.pause)
            {
                if (stop)
                    return;
                Application.DoEvents();
                Thread.Sleep(100);
            }
        }

        public static void Do(double seconds)
        {
            for (double i = 0.1; i < seconds; i += 0.1)
            {
                Application.DoEvents();
                Thread.Sleep(100);
            }
            while (ThreadSeeker.pause)
            {
                Application.DoEvents();
                Thread.Sleep(100);
            }
        }
    }
}
