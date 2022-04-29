using Common;
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
            NetTcpBinding binding = new NetTcpBinding();

            ChannelFactory<IPurchase> channelFactory
                = new ChannelFactory<IPurchase>(binding, new EndpointAddress("net.tcp://localhost:10100/InputRequest"));

            IPurchase proxy = channelFactory.CreateChannel();

            bool orderSuccessful = proxy.OrderItem("B1", "U1");
            Console.WriteLine("Order from user U1 for book B1: " + (orderSuccessful ? "SUCCESSFUL" : "FAILED")); // SUCCESSFUL

            orderSuccessful = proxy.OrderItem("B1", "U2");
            Console.WriteLine("Order from user U2 for book B1: " + (orderSuccessful ? "SUCCESSFUL" : "FAILED")); //FAILED

            orderSuccessful = proxy.OrderItem("B2", "U1");
            Console.WriteLine("Order from user U1 for book B2: " + (orderSuccessful ? "SUCCESSFUL" : "FAILED")); //SUCCESSFUL

            Console.ReadKey();
        }
    }
}
