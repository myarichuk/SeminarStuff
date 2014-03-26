using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Extensions;
using Shared;

namespace Lazy
{
	class Program
	{
		static void Main(string[] args)
		{
			var laptops = Utils.GenerateLaptopData().ToList();

			using (var store = new DocumentStore
			{
				//Url = "http://ipv4.fiddler:8080", //use fiddler to see what is happening
				Url = "http://localhost:8080",
				DefaultDatabase = "FooDB"
			})
			{
				store.Initialize();
				store.DatabaseCommands.EnsureDatabaseExists("FooDB");

				using (var bulkInsertOperation = store.BulkInsert())
				{
					foreach (var laptop in laptops)
						bulkInsertOperation.Store(laptop);
				}

				using (var session = store.OpenSession())
				{
					var acerLaptops = session.Query<Laptop>()
											 .Where(x => x.Manufacturer.Equals("Acer"))
											 .Take(5)
											 .Lazily();

					var highEndLaptops = session.Query<Laptop>()
								   			    .Where(x => x.Cpu == CpuType.i7)											 
												.Take(3)
											    .Lazily();

					var lowEndLaptops = session.Query<Laptop>()
												.Where(x => x.Cpu == CpuType.i3 && x.RamSizeInMegabatyes < 2000)
												.Lazily(); 
										
					//when one of the queries is evaluated, all of them are going to be executed at server side
					//with a single call
					foreach(var laptop in highEndLaptops.Value)
						Console.WriteLine("Manufacturer : {0}, Ram: {1}, HDD: {2}", laptop.Manufacturer,
																					laptop.RamSizeInMegabatyes,
																					laptop.HDDSizeInGigabytes);

				}
			}
		}
	}
}
