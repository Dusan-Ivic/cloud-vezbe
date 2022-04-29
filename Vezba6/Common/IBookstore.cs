using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface IBookstore : ITransaction
    {
        [OperationContract]
        void ListAvailableItems();

        [OperationContract]
        void EnlistPurchase(string bookID, int count);

        [OperationContract]
        double GetItemPrice(string bookID);
    }
}
