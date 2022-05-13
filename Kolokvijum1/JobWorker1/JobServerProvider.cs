using Contracts;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace JobWorker1
{
    public class JobServerProvider : IJob
    {
        public int DoSum(int intervalEnd)
        {
            Trace.TraceInformation($"[JobWorker1] Racunanje UKUPNE sume u intervalu [0, {intervalEnd}]");

            int totalSum = 0;

            if (intervalEnd % 2 == 0)
            {
                // Samo instance sa PARNIM indeksom iz JobWorker2 i JobWorker3

                foreach (RoleInstance instance in RoleEnvironment.Roles["JobWorker2"].Instances)
                {
                    int instanceIndex = 0;
                    if (!int.TryParse(instance.Id.Substring(instance.Id.LastIndexOf(".") + 1), out instanceIndex))
                    {
                        int.TryParse(instance.Id.Substring(instance.Id.LastIndexOf("_") + 1), out instanceIndex);
                    }

                    if (instanceIndex % 2 == 0)
                    {
                        IPEndPoint endpoint = instance.InstanceEndpoints["InternalRequest"].IPEndpoint;
                        NetTcpBinding binding = new NetTcpBinding();
                        ChannelFactory<IPartialJob> channelFactory = new ChannelFactory<IPartialJob>(binding, new EndpointAddress($"net.tcp://{endpoint}/InternalRequest"));
                        IPartialJob proxy = channelFactory.CreateChannel();
                        int partialSum = proxy.DoSumPartial(0, intervalEnd / 2);
                        totalSum += partialSum;
                    }
                }

                foreach (RoleInstance instance in RoleEnvironment.Roles["JobWorker3"].Instances)
                {
                    int instanceIndex = 0;
                    if (!int.TryParse(instance.Id.Substring(instance.Id.LastIndexOf(".") + 1), out instanceIndex))
                    {
                        int.TryParse(instance.Id.Substring(instance.Id.LastIndexOf("_") + 1), out instanceIndex);
                    }

                    if (instanceIndex % 2 == 0)
                    {
                        IPEndPoint endpoint = instance.InstanceEndpoints["InternalRequest"].IPEndpoint;
                        NetTcpBinding binding = new NetTcpBinding();
                        ChannelFactory<IPartialJob> channelFactory = new ChannelFactory<IPartialJob>(binding, new EndpointAddress($"net.tcp://{endpoint}/InternalRequest"));
                        IPartialJob proxy = channelFactory.CreateChannel();
                        int partialSum = proxy.DoSumPartial((intervalEnd / 2) + 1, intervalEnd);
                        totalSum += partialSum;
                    }
                }
            }
            else
            {
                // Samo instance sa NEPARNIM indeksom iz JobWorker2 i JobWorker3

                foreach (RoleInstance instance in RoleEnvironment.Roles["JobWorker2"].Instances)
                {
                    int instanceIndex = 0;
                    if (!int.TryParse(instance.Id.Substring(instance.Id.LastIndexOf(".") + 1), out instanceIndex))
                    {
                        int.TryParse(instance.Id.Substring(instance.Id.LastIndexOf("_") + 1), out instanceIndex);
                    }

                    if (instanceIndex % 2 != 0)
                    {
                        IPEndPoint endpoint = instance.InstanceEndpoints["InternalRequest"].IPEndpoint;
                        NetTcpBinding binding = new NetTcpBinding();
                        ChannelFactory<IPartialJob> channelFactory = new ChannelFactory<IPartialJob>(binding, new EndpointAddress($"net.tcp://{endpoint}/InternalRequest"));
                        IPartialJob proxy = channelFactory.CreateChannel();
                        int partialSum = proxy.DoSumPartial(0, intervalEnd / 2);
                        totalSum += partialSum;
                    }
                }

                foreach (RoleInstance instance in RoleEnvironment.Roles["JobWorker3"].Instances)
                {
                    int instanceIndex = 0;
                    if (!int.TryParse(instance.Id.Substring(instance.Id.LastIndexOf(".") + 1), out instanceIndex))
                    {
                        int.TryParse(instance.Id.Substring(instance.Id.LastIndexOf("_") + 1), out instanceIndex);
                    }

                    if (instanceIndex % 2 != 0)
                    {
                        IPEndPoint endpoint = instance.InstanceEndpoints["InternalRequest"].IPEndpoint;
                        NetTcpBinding binding = new NetTcpBinding();
                        ChannelFactory<IPartialJob> channelFactory = new ChannelFactory<IPartialJob>(binding, new EndpointAddress($"net.tcp://{endpoint}/InternalRequest"));
                        IPartialJob proxy = channelFactory.CreateChannel();
                        int partialSum = proxy.DoSumPartial((intervalEnd / 2) + 1, intervalEnd);
                        totalSum += partialSum;
                    }
                }
            }

            return totalSum;
        }
    }
}
