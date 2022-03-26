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
			NetTcpBinding binding = new NetTcpBinding();
			ChannelFactory<IJob> channelFactory
				= new ChannelFactory<IJob>(binding, new EndpointAddress("net.tcp://localhost:10100/InputRequest"));

			string option = "";
			do
			{
				Console.Write("Broj: ");
				int num = Convert.ToInt32(Console.ReadLine());

				IJob proxy = channelFactory.CreateChannel();

				int result = proxy.DoCalculus(num);
				Console.WriteLine($"Rezultat: {result}");

				Console.WriteLine("Zavrsi? <da/ne>");
				option = Console.ReadLine();
			}
			while (option.ToLower() != "da");

			Console.ReadLine();
		}
	}
}
