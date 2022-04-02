using InterroleContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCFClient
{
	class Program
	{
		static void Main(string[] args)
		{
			while (true)
			{
				Console.Write("Message: ");
				string message = Console.ReadLine();

				if (message == "end")
					break;

				NetTcpBinding binding = new NetTcpBinding();
				ChannelFactory<IJob> channelFactory
					= new ChannelFactory<IJob>(binding, new EndpointAddress("net.tcp://localhost:10100/InputRequest"));
				IJob proxy = channelFactory.CreateChannel();

				proxy.SendMessage(message);
			}
		}
	}
}
