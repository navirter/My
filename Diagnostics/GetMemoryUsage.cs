using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.Diagnostics
{
    public static class GetMemoryUsage
    {
        public static long DoMegabytes(Process p)
        {
            PerformanceCounter pc = new PerformanceCounter("Process", "Working Set", p.ProcessName);
            long memory = pc.RawValue / 1024 / 1024;
            return memory;
        }
    }
}
