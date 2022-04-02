using InterroleContracts;
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
	class PartialJobServerProvider : IPartialJob
	{
		public void SendMessageInternal(string message, string id1)
		{
			Trace.TraceInformation($"Server 2 received: {message}");

			IPEndPoint endpoint
				= RoleEnvironment.Roles["JobWorker2"].Instances[0].InstanceEndpoints["InternalRequest"].IPEndpoint;

			NetTcpBinding binding = new NetTcpBinding();
			ChannelFactory<IJob2> channelFactory
				= new ChannelFactory<IJob2>(binding, new EndpointAddress($"net.tcp://{endpoint}/InternalRequest"));
			IJob2 proxy = channelFactory.CreateChannel();

			proxy.SaveMessage(message, id1, RoleEnvironment.CurrentRoleInstance.Id);
		}
	}
}
