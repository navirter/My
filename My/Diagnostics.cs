using System;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace My
{

    public static class Diagnostics
    {
        public static TimeSpan getIdleTime()
        {
            return IdleTimeFinder.getTimeSpent(IdleTimeFinder.GetIdleTime());
        }
        #region getting idle time

        internal struct LASTINPUTINFO
        {
            public uint cbSize;

            public uint dwTime;
        }
        /// <summary>
        /// Helps to find the idle time, (in milliseconds) spent since the last user input
        /// </summary>
        class IdleTimeFinder
        {
            [DllImport("User32.dll")]
            private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

            [DllImport("Kernel32.dll")]
            private static extern ulong GetLastError();

            public static uint GetIdleTime()
            {
                LASTINPUTINFO lastInPut = new LASTINPUTINFO();
                lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
                GetLastInputInfo(ref lastInPut);

                return ((uint)Environment.TickCount - lastInPut.dwTime);
            }
            /// <summary>
            /// Get the Last input time in milliseconds
            /// </summary>
            /// <returns></returns>
            public static long GetLastInputTime()
            {
                LASTINPUTINFO lastInPut = new LASTINPUTINFO();
                lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
                if (!GetLastInputInfo(ref lastInPut))
                {
                    throw new Exception(GetLastError().ToString());
                }
                return lastInPut.dwTime;
            }
            public static TimeSpan getTimeSpent(long ticks)
            {
                return TimeSpan.FromMilliseconds(ticks);
            }
        }

        #endregion

        public static int get_cpu_percent_usage(Process p)
        {
            PerformanceCounter pc = new PerformanceCounter("Process", "% Processor Time", p.ProcessName);
            float fcpu = pc.NextValue();
            int cpu = Convert.ToInt32(fcpu);
            return cpu;
        }

        public static long get_memory_mb_usage(Process p)
        {
            PerformanceCounter pc = new PerformanceCounter("Process", "Working Set", p.ProcessName);
            long memory = pc.RawValue / 1024 / 1024;
            return memory;
        }
    }
}
