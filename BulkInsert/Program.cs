using Raven.Client.Embedded;
using Shared;
using System;
using System.Linq;

namespace BulkInsert
{
	class Program
	{
		private static void Main(string[] args)
		{
			var laptops = Utils.GenerateLaptopData().ToList();

			using (var store = new EmbeddableDocumentStore
			{
				RunInMemory = true
			})
			{
				store.Initialize();

				Console.WriteLine("Object count before storing with Bulk Insert: " + laptops.Count);
				using (var bulkInsertOperation = store.BulkInsert())
				{
					foreach (var laptop in laptops)
						bulkInsertOperation.Store(laptop);					
				}

				using (var session = store.OpenSession())
				{
					var countOfObjectsStored = session.Query<Laptop>().Customize(x => x.WaitForNonStaleResults()).Count();
					Console.WriteLine("Count of objects stored with Bulk Insert: " + countOfObjectsStored);					
				}
			}
		}
	}
}
