using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using Raven.Client.Linq;
using Raven.Database.Indexing;
using Shared;

namespace MapReduceIndex
{
	class Program
	{
	    public class DogsByBreed
        {
            public string Breed { get; set; }

            public int Count { get; set; }

	        public override string ToString()
	        {
	            return string.Format("Breed: {0}, Count: {1}", Breed, Count);
	        }
        }

	    public class DogsByBreedIndex : AbstractIndexCreationTask<Dog,DogsByBreed>
	    {
	        public DogsByBreedIndex()
	        {
                //note: Map and Reduce transformation end-result must be the same
                Map = dogs => from dog in dogs
	                select new
	                {
	                    dog.Breed,
	                    Count = 1
	                };
               
	            Reduce = results => from result in results
	                group result by result.Breed
	                into g
	                select new
	                {
	                    Breed = g.Key,
	                    Count = g.Sum(x => x.Count)
	                };
	        }
	    }

		static void Main()
		{
		    var dogs = GenerateDogData();
		    using (var store = new EmbeddableDocumentStore
		    {
		        RunInMemory = true
		    })
		    {
		        store.Initialize();
                new DogsByBreedIndex().Execute(store);

		        using (var session = store.OpenSession())
		        {
		            foreach (var dog in dogs)
                        session.Store(dog);
                    
                    session.SaveChanges();
                }

		        using (var session = store.OpenSession())
		        {
		            var dogsByBreedQuery = session.Query<DogsByBreed,DogsByBreedIndex>()
                        .Customize(x => x.WaitForNonStaleResults())
		                .Where(x => x.Count > 1)
		                .ToList();

		            foreach (var dogsByBreed in dogsByBreedQuery)
                        Console.WriteLine(dogsByBreed);
		        }
		    }
		}

	    private static IEnumerable<Dog> GenerateDogData()
	    {
            yield return new Dog
            {
                Name = "Rex",
                Breed = "German Sheperd",
                Gender = Gender.MaleDog
            };

            yield return new Dog
            {
                Name = "Bonnie",
                Breed = "Golder Retriever",
                Gender = Gender.FemaleDog
            };

            yield return new Dog
            {
                Name = "Fang",
                Breed = "German Sheperd",
                Gender = Gender.MaleDog
            };

            yield return new Dog
            {
                Name = "Beethoven",
                Breed = "Saint-Bernard",
                Gender = Gender.MaleDog
            };

            yield return new Dog
            {
                Name = "Chewbacca",
                Breed = "Mixed",
                Gender = Gender.MaleDog
            };

            yield return new Dog
            {
                Name = "Beethoven Jr.",
                Breed = "Saint-Bernard",
                Gender = Gender.MaleDog
            };
        }
	}
}
