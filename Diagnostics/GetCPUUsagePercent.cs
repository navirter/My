using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.Diagnostics
{
    public static class GetCPUUsagePercent
    {
        public static int Do(Process p)
        {
            PerformanceCounter pc = new PerformanceCounter("Process", "% Processor Time", p.ProcessName);
            float fcpu = pc.NextValue();
            int cpu = Convert.ToInt32(fcpu);
            return cpu;
        }
    }
}
