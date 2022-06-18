using Contracts;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace OperationService_WorkerRole
{
    public class OperationServer
    {
        private ServiceHost serviceHost;
        private string internalEndpointName = "InternalRequest";

        public OperationServer()
        {
            RoleInstanceEndpoint internalEndpoint
                = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints[internalEndpointName];

            string endpoint = String.Format("net.tcp://{0}/{1}", internalEndpoint.IPEndpoint, internalEndpointName);

            NetTcpBinding binding = new NetTcpBinding();

            serviceHost = new ServiceHost(typeof(OperationServerProvider));

            serviceHost.AddServiceEndpoint(typeof(IReceiveResult), binding, endpoint);
        }

        public void Open()
        {
            serviceHost.Open();
        }

        public void Close()
        {
            serviceHost.Close();
        }
    }
}
