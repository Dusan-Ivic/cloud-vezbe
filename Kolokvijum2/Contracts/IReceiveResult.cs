using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract]
    public interface IReceiveResult
    {
        [OperationContract]
        void ReceiveResult(int result);
    }
}
