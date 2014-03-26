using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using Shared;

namespace Facets
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
									laptop.Manufacturer,
									laptop.HDDSizeInGigabytes,
									laptop.RamSizeInMegabatyes
								};
			}
		}

		static void Main(string[] args)
		{
			var laptops = Utils.GenerateLaptopData();
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

			    Console.WriteLine();
			    Console.Write("Data is ready... entry query: ");
			    var searchTerm = Console.ReadLine();

				//query facets
				using (var session = store.OpenSession())
				{
				    var query = session.Query<Laptop, LaptopIndex>()
				        .Where(x => x.Manufacturer == searchTerm);

				    foreach (var laptop in query.ToList())
				    {
				        Console.WriteLine(laptop);
				    }

				    var facetResults = query
				        .ToFacets(new List<Facet>
				        {
				            new Facet<Laptop>
				            {
				                Name = x => x.Manufacturer,
				            },
				            new Facet<Laptop>
				            {
				                Name = x => x.RamSizeInMegabatyes,
				                Ranges =
				                {
				                    x => x.RamSizeInMegabatyes < 2000,
				                    x => x.RamSizeInMegabatyes > 2000 && x.RamSizeInMegabatyes < 4000,
				                    x => x.RamSizeInMegabatyes > 4000 && x.RamSizeInMegabatyes < 6000,
				                    x => x.RamSizeInMegabatyes > 6000 && x.RamSizeInMegabatyes < 8000
				                }
				            },
				        });

				    foreach (var facetResult in facetResults.Results)
				    {
				        Console.WriteLine(facetResult.Key);
				        foreach (var facetValue in facetResult.Value.Values)
				        {
				            Console.WriteLine("\t{0}:{1}", facetValue.Range, facetValue.Hits);
				        }
				    }
				}
			}
		}

	}
}
