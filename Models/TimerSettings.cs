using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quartets.Models
{
    public class TimerSettings(long totalTimeInMillSeconds, long intervalTimeInMillSeconds)
    {
        #region Properties

        public long TotalTimeInMillSeconds { get; set; } = totalTimeInMillSeconds;
        public long IntervalTimeInMillSeconds { get; set; } = intervalTimeInMillSeconds;

        #endregion
    }
}
