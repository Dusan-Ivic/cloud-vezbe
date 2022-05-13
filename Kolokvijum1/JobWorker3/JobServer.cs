using Contracts;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace JobWorker3
{
    public class JobServer
    {
        private ServiceHost serviceHost;
        private string internalEndpointName = "InternalRequest";

        public JobServer()
        {
            RoleInstanceEndpoint internalEndpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints[internalEndpointName];
            string endpoint = String.Format("net.tcp://{0}/{1}", internalEndpoint.IPEndpoint, internalEndpointName);
            serviceHost = new ServiceHost(typeof(JobServerProvider));
            NetTcpBinding binding = new NetTcpBinding();
            serviceHost.AddServiceEndpoint(typeof(IPartialJob), binding, endpoint);
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
