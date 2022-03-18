using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Contracts;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;

namespace JobWorker
{
	public class WorkerRole : RoleEntryPoint
	{
		private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

		private IHealthMonitoring proxy;

		public override void Run()
		{
			Trace.TraceInformation("JobWorker is running");

			try
			{
				this.RunAsync(this.cancellationTokenSource.Token).Wait();
			}
			finally
			{
				this.runCompleteEvent.Set();
			}
		}

		public void Connect()
		{
			NetTcpBinding binding = new NetTcpBinding();
			ChannelFactory<IHealthMonitoring> channelFactory
				= new ChannelFactory<IHealthMonitoring>(binding, new EndpointAddress("net.tcp://localhost:6000/HealthMonitoring"));
			proxy = channelFactory.CreateChannel();
		}

		public override bool OnStart()
		{
			Connect();

			// Set the maximum number of concurrent connections
			ServicePointManager.DefaultConnectionLimit = 12;

			// For information on handling configuration changes
			// see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

			bool result = base.OnStart();

			Trace.TraceInformation("JobWorker has been started");

			return result;
		}

		public override void OnStop()
		{
			Trace.TraceInformation("JobWorker is stopping");

			this.cancellationTokenSource.Cancel();
			this.runCompleteEvent.WaitOne();

			base.OnStop();

			Trace.TraceInformation("JobWorker has stopped");
		}

		private async Task RunAsync(CancellationToken cancellationToken)
		{
			// TODO: Replace the following with your own logic.
			while (!cancellationToken.IsCancellationRequested)
			{
				proxy.IAmAlive();
				Trace.WriteLine("tekst_poruke", "Information");
				Trace.TraceInformation("Working");
				await Task.Delay(1000);
			}
		}
	}
}
