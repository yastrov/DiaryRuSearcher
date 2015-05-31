using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryAPI
{
    public class DiaryStopWatcher
    {
        private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        /// <summary>
        /// API need re authorization after 30 minute (1800000)
        /// </summary>
        /// <returns></returns>
        public bool IsActiveTimer()
        {
            if (stopwatch.ElapsedMilliseconds == 0)
                return false;
            return stopwatch.ElapsedMilliseconds - 1799990 < 0;
        }
        public void ResetTimer()
        {
            stopwatch.Restart();
        }
        public void StartTimer()
        {
            stopwatch.Start();
        }

        public void StartOrResetTimer()
        {
            if (stopwatch.IsRunning)
                stopwatch.Reset();
            else
                stopwatch.Start();
        }
    }
}
