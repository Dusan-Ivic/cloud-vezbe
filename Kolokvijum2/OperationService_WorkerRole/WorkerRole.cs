using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Contracts;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using OperationService_Data;

namespace OperationService_WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private OperationServer operationServer = new OperationServer();

        public override void Run()
        {
            Trace.TraceInformation("OperationService_WorkerRole is running");

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
            operationServer.Open();

            Trace.TraceInformation("OperationService_WorkerRole has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("OperationService_WorkerRole is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            operationServer.Close();
            base.OnStop();

            Trace.TraceInformation("OperationService_WorkerRole has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            OperationDataRepository repo = new OperationDataRepository();
            CloudQueue queue = QueueHelper.GetQueueReference("vezba");
            BlobHelper blobHelper = new BlobHelper();
            
            while (!cancellationToken.IsCancellationRequested)
            {
                CloudQueueMessage message = queue.GetMessage();
                if (message != null)
                {
                    int result = int.Parse(message.AsString.Split(',')[0]);
                    string arithmeticOperator = message.AsString.Split(',')[1];

                    if (arithmeticOperator == "+")
                    {
                        Trace.TraceInformation("Sabiranje!");
                    }
                    else if (arithmeticOperator == "-")
                    {
                        Trace.TraceInformation("Oduzimanje! Poruka se prosledjuje bratskoj instanci!");

                        int currentInstanceIndex = 0;
                        if (!int.TryParse(RoleEnvironment.CurrentRoleInstance.Id.Substring(RoleEnvironment.CurrentRoleInstance.Id.LastIndexOf(".") + 1), out currentInstanceIndex))
                        {
                            int.TryParse(RoleEnvironment.CurrentRoleInstance.Id.Substring(RoleEnvironment.CurrentRoleInstance.Id.LastIndexOf("_") + 1), out currentInstanceIndex);
                        }

                        int nextInstanceIndex = (currentInstanceIndex + 1) % RoleEnvironment.CurrentRoleInstance.Role.Instances.Count;
                        RoleInstance nextInstance = RoleEnvironment.CurrentRoleInstance.Role.Instances[nextInstanceIndex];
                        IPEndPoint endpoint = nextInstance.InstanceEndpoints["InternalRequest"].IPEndpoint;

                        NetTcpBinding binding = new NetTcpBinding();

                        ChannelFactory<IReceiveResult> channelFactory
                            = new ChannelFactory<IReceiveResult>(binding, new EndpointAddress($"net.tcp://{endpoint}/InternalRequest"));
                        IReceiveResult proxy = channelFactory.CreateChannel();
                        proxy.ReceiveResult(result);

                        int totalSum = 0;
                        repo.RetrieveAllEntities().ToList().ForEach(entity => {
                            if (entity.Operator == "-")
                                totalSum += entity.Result;
                        });

                        CloudBlockBlob blob = blobHelper.GetBlockBlobReference("vezba", "snapshot");
                        blobHelper.UploadText($"{totalSum}", blob);
                    }

                    queue.DeleteMessage(message);
                    Trace.TraceInformation("Poruka je obradjena i obrisana iz reda!");
                }
                else
                {
                    Trace.TraceInformation("Trenutno nema poruka u redu!");
                }

                await Task.Delay(5000);
            }
        }
    }
}
