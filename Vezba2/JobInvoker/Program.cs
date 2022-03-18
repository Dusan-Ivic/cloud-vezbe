using InterroleContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace JobInvoker
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.Write("Gornja granica intervala: ");
			int n = Convert.ToInt32(Console.ReadLine());

			NetTcpBinding binding = new NetTcpBinding();
			ChannelFactory<IJob> channelFactory
				= new ChannelFactory<IJob>(binding, new EndpointAddress("net.tcp://localhost:10100/InputRequest"));
			IJob proxy = channelFactory.CreateChannel();

			int result = proxy.DoCalculus(n);

			Console.WriteLine($"Rezultat je {result}.");
			Console.ReadKey();
		}
	}
}
