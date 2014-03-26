using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Raven.Client.Document;

namespace ReplicationDemo
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var store = new DocumentStore
			{
				Url = "http://localhost:8080",
				DefaultDatabase = "TestA"
			})
			{
				//uncomment this row to enable load-balancing 
				store.Conventions.FailoverBehavior = FailoverBehavior.ReadFromAllServers | FailoverBehavior.AllowReadsFromSecondariesAndWritesToSecondaries;

				store.Initialize();
				var finishFetchingEvent = new ManualResetEventSlim();


				Task.Run(() =>
				{
					do
					{
// ReSharper disable once AccessToDisposedClosure
						using (var session = store.OpenSession())
						{
							var timer = Stopwatch.StartNew();
							var userRecord = session.Load<dynamic>("users/1");
							timer.Stop();
							Console.WriteLine("fetched user, Name = {0}, latency = {1}ms", userRecord.Name,timer.ElapsedMilliseconds);
						}
					} while (!finishFetchingEvent.Wait(TimeSpan.FromMilliseconds(250)));
				});

				Console.ReadKey();
				finishFetchingEvent.Set();
			}
		}
	}
}
