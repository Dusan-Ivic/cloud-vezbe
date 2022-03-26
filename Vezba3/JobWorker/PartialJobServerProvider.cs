using InterroleContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobWorker
{
	class PartialJobServerProvider : IPartialJob
	{
		public int DoSum(int from, int to)
		{
			Trace.WriteLine($"DoSum method called - interval [{from}, {to}]");

			int sum = 0;

			for (int i = from; i <= to; i++)
			{
				sum += i;
			}

			return sum;
		}
	}
}
