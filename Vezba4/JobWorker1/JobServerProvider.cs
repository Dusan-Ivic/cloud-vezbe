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
	class JobServerProvider : IJob
	{
		public void SendMessage(string message)
		{
			Trace.TraceInformation($"Server 1 received: {message}");

			int currentInstanceIndex = int.Parse(RoleEnvironment.CurrentRoleInstance.Id.Split('_')[2]);

			int instanceCount = RoleEnvironment.CurrentRoleInstance.Role.Instances.Count;

			int nextInstanceIndex = (currentInstanceIndex + 1) % instanceCount;

			IPEndPoint endpoint
				= RoleEnvironment.CurrentRoleInstance.Role.Instances[nextInstanceIndex].InstanceEndpoints["InternalRequest"].IPEndpoint;

			foreach (RoleInstance instance in RoleEnvironment.Roles["JobWorker1"].Instances)
			{
				int index = int.Parse(instance.Id.Split('_')[2]);
				if (index == nextInstanceIndex)
				{
					endpoint = instance.InstanceEndpoints["InternalRequest"].IPEndpoint;
					break;
				}
			}

			NetTcpBinding binding = new NetTcpBinding();
			ChannelFactory<IPartialJob> channelFactory
				= new ChannelFactory<IPartialJob>(binding, new EndpointAddress($"net.tcp://{endpoint}/InternalRequest"));
			IPartialJob proxy = channelFactory.CreateChannel();

			proxy.SendMessageInternal(message, RoleEnvironment.CurrentRoleInstance.Id);
		}
	}
}
