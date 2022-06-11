using Common;
using SwitchService_Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchService_WorkerRole
{
    public class PartialJobServerProvider : IPartialJob
    {
        private SwitchDataRepository repo = new SwitchDataRepository();

        public void SendForward(string message)
        {
            Trace.TraceInformation($"Poruka: {message}");

            string name = message.Split(':')[0];
            string state = message.Split(':')[1];

            SwitchService_Data.Switch entry = new SwitchService_Data.Switch(DateTime.Now.ToString())
            {
                Name = name,
                State = state
            };

            repo.AddSwitch(entry);

            Trace.TraceInformation("Prekidac je uspesno dodat u tabelu!");
        }
    }
}
