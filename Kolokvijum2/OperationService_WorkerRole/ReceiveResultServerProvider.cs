using Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationService_WorkerRole
{
    public class OperationServerProvider : IReceiveResult
    {
        public void ReceiveResult(int result)
        {
            Trace.TraceInformation($"Rezultat: {result}");
        }
    }
}
