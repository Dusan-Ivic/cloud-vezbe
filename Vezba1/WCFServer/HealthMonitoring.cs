using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFServer
{
	class HealthMonitoring : IHealthMonitoring
	{
		public void IAmAlive()
		{
			Console.WriteLine("Worker role checked in.");
		}
	}
}
