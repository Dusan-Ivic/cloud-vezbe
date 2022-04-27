using Contracts;
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
            NetTcpBinding binding = new NetTcpBinding();
            ChannelFactory<IStudent> channelFactory
                = new ChannelFactory<IStudent>(binding, new EndpointAddress("net.tcp://localhost:10100/InputRequest"));
            IStudent proxy = channelFactory.CreateChannel();

            proxy.AddStudent("PR 1-2019", "Petar", "Petrovic");
            proxy.AddStudent("PR 2-2019", "Marko", "Markovic");

            var students = proxy.RetrieveAllStudents();
            foreach (var student in students)
            {
                Console.WriteLine($"{student.Name} {student.LastName}");
            }

            Console.ReadKey();
        }
    }
}
