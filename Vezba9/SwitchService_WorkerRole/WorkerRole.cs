using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using SwitchService_Data;

namespace SwitchService_WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private PartialJobServer partialJobServer = new PartialJobServer();

        public override void Run()
        {
            Trace.TraceInformation("SwitchService_WorkerRole is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();
            partialJobServer.Open();

            Trace.TraceInformation("SwitchService_WorkerRole has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("SwitchService_WorkerRole is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();
            partialJobServer.Close();

            Trace.TraceInformation("SwitchService_WorkerRole has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            SwitchDataRepository repo = new SwitchDataRepository();
            CloudQueue queue = QueueHelper.GetQueueReference("vezba");

            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                //Trace.TraceInformation("Working");

                int currentInstanceIndex = 0;
                if (!int.TryParse(RoleEnvironment.CurrentRoleInstance.Id.Substring(RoleEnvironment.CurrentRoleInstance.Id.LastIndexOf(".") + 1), out currentInstanceIndex))
                {
                    int.TryParse(RoleEnvironment.CurrentRoleInstance.Id.Substring(RoleEnvironment.CurrentRoleInstance.Id.LastIndexOf("_") + 1), out currentInstanceIndex);
                }

                if (currentInstanceIndex == 0)
                {
                    CloudQueueMessage message = queue.GetMessage();
                    if (message == null)
                    {
                        Trace.TraceInformation("Trenutno nema poruka u redu!");
                    }
                    else
                    {
                        Trace.TraceInformation($"Poruka: {message.AsString}");

                        string name = message.AsString.Split(':')[0];
                        string state = message.AsString.Split(':')[1];

                        if (state == "otvoren")
                        {
                            IPEndPoint endpoint
                                = RoleEnvironment.CurrentRoleInstance.Role.Instances[currentInstanceIndex + 1].InstanceEndpoints["InternalRequest"].IPEndpoint;

                            NetTcpBinding binding = new NetTcpBinding();

                            ChannelFactory<IPartialJob> channelFactory
                                = new ChannelFactory<IPartialJob>(binding, new EndpointAddress($"net.tcp://{endpoint}/InternalRequest"));

                            IPartialJob proxy = channelFactory.CreateChannel();

                            proxy.SendForward(message.AsString);

                            Trace.TraceInformation("Poruka je prosledjena bratskoj instanci!");
                        }
                        else if (state == "zatvoren")
                        {
                            SwitchService_Data.Switch entry = new SwitchService_Data.Switch(DateTime.Now.ToString())
                            {
                                Name = name,
                                State = state
                            };

                            repo.AddSwitch(entry);

                            Trace.TraceInformation("Prekidac je uspesno dodat u tabelu!");
                        }

                        queue.DeleteMessage(message);
                        Trace.TraceInformation("Poruka je obradjena i obrisana iz reda!");
                    }
                }

                await Task.Delay(5000);
            }
        }
    }
}
