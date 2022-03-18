using InterroleContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobWorker
{
	class JobServerProvider : IJob
	{
		public int DoCalculus(int to)
		{
			Trace.WriteLine($"DoCalculus method called - interval [1, {to}]");

			int sum = 0;

			for (int i = 1; i <= to; i++)
				sum += i;

			return sum;
		}
	}
}
