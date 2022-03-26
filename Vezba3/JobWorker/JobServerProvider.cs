using InterroleContracts;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace JobWorker
{
	class JobServerProvider : IJob
	{
		private string internalEndpointName = "InternalRequest";

		public int DoCalculus(int to)
		{
			List<EndpointAddress> internalEndpoints
				= new List<EndpointAddress>();
			foreach (RoleInstance instance in RoleEnvironment.Roles[RoleEnvironment.CurrentRoleInstance.Role.Name].Instances)
			{
				if (instance.Id != RoleEnvironment.CurrentRoleInstance.Id)
				{
					internalEndpoints.Add(
						new EndpointAddress(String.Format("net.tcp://{0}/{1}",
						instance.InstanceEndpoints[internalEndpointName].IPEndpoint.ToString(),
						internalEndpointName))
					);
				}
			}

			Trace.WriteLine($"DoCalculus method called - interval [1, {to}]");

			int sum = 0;
			int start = 1;
			int interval = to / internalEndpoints.Count;

			for (int i = 0; i < internalEndpoints.Count; i++)
			{
				NetTcpBinding binding = new NetTcpBinding();
				ChannelFactory<IPartialJob> channelFactory
					= new ChannelFactory<IPartialJob>(binding, new EndpointAddress(internalEndpoints[i].Uri));
				IPartialJob proxy = channelFactory.CreateChannel();

				if (i < internalEndpoints.Count - 1)
				{
					sum += proxy.DoSum(start, start + interval - 1);
					start = start + interval;
				}
				else
				{
					sum += proxy.DoSum(start, to);
				}
			}

			return sum;
		}
	}
}
