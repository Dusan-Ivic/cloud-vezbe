using Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobWorker2
{
    public class JobServerProvider : IPartialJob
    {
        public int DoSumPartial(int intervalStart, int intervalEnd)
        {
            Trace.TraceInformation($"[JobWorker2] Racunanje PARCIJALNE sume u intervalu [{intervalStart}, {intervalEnd}]");

            int partialSum = 0;

            for (int i = intervalStart; i <= intervalEnd; i++)
            {
                partialSum += i;
            }

            return partialSum;
        }
    }
}
