using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace InterroleContracts
{
	[ServiceContract]
	public interface IJob2
	{
		[OperationContract]
		void SaveMessage(string message, string id1, string id2);
	}
}
