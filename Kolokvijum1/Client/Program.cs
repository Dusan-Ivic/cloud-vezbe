using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("Gornja granica intervala: ");
                string input = Console.ReadLine();

                if (int.TryParse(input, out int intervalEnd))
                {
                    NetTcpBinding binding = new NetTcpBinding();
                    ChannelFactory<IJob> channelFactory = new ChannelFactory<IJob>(binding, new EndpointAddress("net.tcp://localhost:10100/InputRequest"));
                    IJob proxy = channelFactory.CreateChannel();
                    int totalSum = proxy.DoSum(intervalEnd);
                    Console.WriteLine($"Rezultat: {totalSum}");
                }
                else
                {
                    break;
                }
            }
        }
    }
}
