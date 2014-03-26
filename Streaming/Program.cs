using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using Shared;

namespace Streaming
{
	class Program
	{
		public class LaptopIndex : AbstractIndexCreationTask<Laptop>
		{
			public LaptopIndex()
			{
				Map = laptops => from laptop in laptops
								 select new
								 {
									 laptop.Id,
									 laptop.Cpu,
									 laptop.Manufacturer,
									 laptop.HDDSizeInGigabytes,
									 laptop.RamSizeInMegabatyes
								 };
			}
		}

		private static void Main(string[] args)
		{
			var laptops = Utils.GenerateLaptopData().ToList();

			using (var store = new EmbeddableDocumentStore
			{
				RunInMemory = true
			})
			{				
				store.Initialize();
				new LaptopIndex().Execute(store);

				//store all data in the db
				using (var session = store.OpenSession())
				{
					foreach (var laptop in laptops)
						session.Store(laptop);
					session.SaveChanges();
				}

				Console.WriteLine("Initial objects count: "+ laptops.Count);

				using (var session = store.OpenSession())
				{
					var regularQueryResultCount = session.Query<Laptop>()
														 .Customize(x => x.WaitForNonStaleResults())
													     .ToList();
					Console.WriteLine("Regular query result count: " + regularQueryResultCount.Count);
				}
				
				using (var session = store.OpenSession())
				{
					//cannot stream on dynamic indexes!!! 
					//regular query can work on dynamic indexes
					using (var streamingQuery = session.Advanced.Stream(session.Query<Laptop, LaptopIndex>()))
					{
						var streamingResults = new List<Laptop>();
						while (streamingQuery.MoveNext())
							streamingResults.Add(streamingQuery.Current.Document);

						Console.WriteLine("Streaming results count: " + streamingResults.Count);
					}
				}
			}
		}
	}
}
