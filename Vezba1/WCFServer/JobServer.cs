using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCFServer
{
	class JobServer
	{
		private ServiceHost serviceHost;

		public JobServer()
		{
			Start();
		}

		public void Start()
		{
			serviceHost = new ServiceHost(typeof(HealthMonitoring));
			NetTcpBinding binding = new NetTcpBinding();
			serviceHost.AddServiceEndpoint(typeof(IHealthMonitoring), binding, new Uri("net.tcp://localhost:6000/HealthMonitoring"));
			serviceHost.Open();
			Console.WriteLine("Server ready and waiting for requests.");
		}

		public void Stop()
		{
			serviceHost.Close();
			Console.WriteLine("Server stopped.");
		}
	}
}
