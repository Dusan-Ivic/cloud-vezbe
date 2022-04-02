using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobWorker2
{
	public class Data
	{
		public string Message { get; set; }
		public string Id1 { get; set; }
		public string Id2 { get; set; }

		public Data(string message, string id1, string id2)
		{
			Message = message;
			Id1 = id1;
			Id2 = id2;
		}
	}
}
