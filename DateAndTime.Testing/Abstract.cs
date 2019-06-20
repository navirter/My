using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DateAndTime.Testing
{
    public abstract class Abstract
    {
        public readonly DateTime minDateTime = DateTime.MinValue;
        public readonly DateTime maxDateTime = DateTime.MaxValue;
        public readonly DateTime medDateTime = new DateTime(2000, 1, 1, 1, 1, 1);
        public readonly DateTime medDateTimeWithNoTime = new DateTime(2000, 1, 1, 0, 0, 0);
        public readonly string minDTWithNone = "0001.01.01", minDTWithWords = "0001г.01м.01д.", minDTWithTime = "0001.01.01: 00.00.00", minDTWithAll = "0001г.01м.01д.: 00ч.00м.00с.";
        public readonly string maxDTWithNone = "9999.12.31", maxDTWithWords = "9999г.12м.31д.", maxDTWithTime = "9999.12.31: 23.59.59", maxDTWithAll = "9999г.12м.31д.: 23ч.59м.59с.";
        public readonly string medDTWithNone = "2000.01.01", medDTWithWords = "2000г.01м.01д.", medDTWithTime = "2000.01.01: 01.01.01", medDTWithAll = "2000г.01м.01д.: 01ч.01м.01с.";

        public TimeSpan minTimeSpan = new TimeSpan();
        public TimeSpan maxTimeSpan = new TimeSpan(1000000, 0, 0, 0);
        public TimeSpan medTimeSpan = new TimeSpan(2000, 1, 1, 1);
        public readonly string minTSWithNone = "0000: 00.00.00", minTSWithWords = "0000д.: 00ч.00м.00с.";
        public readonly string maxTSWithNone = "1000000: 00.00.00", maxTSWithWords = "1000000д.: 00ч.00м.00с.";
        public readonly string medTSWithNone = "2000: 01.01.01", medTSWithWords = "2000д.: 01ч.01м.01с.";
    }
}
