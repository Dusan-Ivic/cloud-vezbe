using InterroleContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobWorker2
{
	class JobServerProvider : IJob2
	{
		public List<Data> messageCollection = new List<Data>();

		public void SaveMessage(string message, string id1, string id2)
		{
			Data data = new Data(message, id1, id2);
			messageCollection.Add(data);

			Trace.TraceInformation($"Message saved: {message}");
		}
	}
}
