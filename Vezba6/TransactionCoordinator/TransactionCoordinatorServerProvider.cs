using Common;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TransactionCoordinator
{
    public class TransactionCoordinatorServerProvider : IPurchase
    {
        private string internalEndpointName = "InternalRequest";

        public bool OrderItem(string bookID, string userID)
        {
            EndpointAddress bankEndpointAddress = null;
            EndpointAddress bookstoreEndpointAddress = null;

            foreach (var role in RoleEnvironment.Roles)
            {
                if (role.Key == "Bank")
                {
                    bankEndpointAddress = new EndpointAddress(String.Format("net.tcp://{0}/{1}", 
                        RoleEnvironment.Roles[role.Key].Instances[0].InstanceEndpoints[internalEndpointName].IPEndpoint.ToString(), 
                        internalEndpointName));
                }
                else if (role.Key == "Bookstore")
                {
                    bookstoreEndpointAddress = new EndpointAddress(String.Format("net.tcp://{0}/{1}",
                        RoleEnvironment.Roles[role.Key].Instances[0].InstanceEndpoints[internalEndpointName].IPEndpoint.ToString(),
                        internalEndpointName));
                }
            }

            if (bankEndpointAddress != null & bookstoreEndpointAddress != null)
            {
                NetTcpBinding binding = new NetTcpBinding();

                ChannelFactory<IBookstore> channelFactoryBookstore
                    = new ChannelFactory<IBookstore>(binding, new EndpointAddress(bookstoreEndpointAddress.Uri));
                ChannelFactory<IBank> channelFactoryBank
                    = new ChannelFactory<IBank>(binding, new EndpointAddress(bankEndpointAddress.Uri));

                IBookstore proxyBookstore = channelFactoryBookstore.CreateChannel();
                IBank proxyBank = channelFactoryBank.CreateChannel();

                proxyBookstore.ListAvailableItems();
                proxyBank.ListClients();

                double bookPrice = proxyBookstore.GetItemPrice(bookID);

                proxyBookstore.EnlistPurchase(bookID, 1);
                proxyBank.EnlistMoneyTransfer(userID, bookPrice);

                bool isBookstorePrepared = proxyBookstore.Prepare();
                bool isBankPrepared = proxyBank.Prepare();

                if (isBookstorePrepared && isBankPrepared)
                {
                    proxyBookstore.Commit();
                    proxyBank.Commit();
                    return true;
                }
                else
                {
                    proxyBookstore.Rollback();
                    proxyBank.Rollback();
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
