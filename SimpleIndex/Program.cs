using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Abstractions.Data;
using Raven.Abstractions.Indexing;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using Raven.Client.Linq;
using Raven.Json.Linq;
using Raven.Storage.Esent.SchemaUpdates.Updates;
using Shared;

namespace SimpleIndex
{
	class Program
	{
	    public class UserSearchIndex : AbstractIndexCreationTask<User>
	    {
            public UserSearchIndex()
	        {
	            Map = users => from user in users
	                select new
	                {
	                    Query = new[]
	                    {
                            user.Name,
                            user.Email,
                            user.LastName,
                            user.Name + " " + user.LastName
	                    }.Concat(user.Email.Split('@')).ToArray()
	                };
	        }
	    }

	    public class UserSearchIndexWithDynamicFields: AbstractIndexCreationTask<User>
	    {
            //dynamic fields
	        public UserSearchIndexWithDynamicFields()
	        {
	            Map = users => from user in users
	                select new
	                {
                        user.Name,
                        user.Email,
                        //since Address is stored as Json object,
                        //and can be treated as tuple collection, we can do this
                        _ = user.Address.Select(x => CreateField(x.Key,x.Value))
	                };
	        }
	    }

		static void Main(string[] args)
		{
		    using (var store = new EmbeddableDocumentStore
		    {
		        RunInMemory = true
		    })
		    {
		        store.Initialize();
		        var users = GenerateUsers();

                using (var session = store.OpenSession())
		        {
		            foreach (var user in users)
		                session.Store(user);
                    session.SaveChanges();
		        }

                new UserSearchIndex().Execute(store);
                new UserSearchIndexWithDynamicFields().Execute(store);

		        using (var session = store.OpenSession())
		        {
		            var usersFromZhaoProvince = session.Advanced
                                                       .LuceneQuery<User, UserSearchIndexWithDynamicFields>()
                                                       .WaitForNonStaleResults()
                                                       .Search("Province","Zhao")
                                                       .ToList();
                    
                    Console.WriteLine("Users from Zhao province (dynamic field in the index)");
		            foreach (var user in usersFromZhaoProvince)
                        Console.WriteLine(user);

                    Console.WriteLine("\n users with email domain 'foo.com'");
                    var usersByEmailDomain = session.Query<User,UserSearchIndex>()
                                                    .Customize(x => x.WaitForNonStaleResults())
                                                    .Search(x => x.Query,"foo.com")
                                                    .ToList();

                    foreach (var user in usersByEmailDomain)
                        Console.WriteLine(user);

                    Console.WriteLine("\n users with email domain 'foo.com' and name Bob");
                    var usersByEmailDomainAndName = session.Query<User, UserSearchIndex>()
                                                           .Search(x => x.Query, "foo.com Bob", options: SearchOptions.And)
                                                           .ToList();

                    foreach (var user in usersByEmailDomainAndName)
                        Console.WriteLine(user);
                }
		    }
		}

	    private static IEnumerable<User> GenerateUsers()
	    {
            yield return new User
            {
                Name = "Bob Lee",
                Email = "foobar2@foo.com",
                Address = RavenJObject.FromObject(new
                {
                    State = "Arizona",
                    Area = 51
                })
            };

            yield return new User
            {
                Name = "Bob Levi",
                Email = "foobar1@foo.com",
                Address = RavenJObject.FromObject(new
                {
                    State = "Arizona",
                    Area = 51
                })
            };
            
            yield return new User
            {
                Name = "John",
                Email = "john@foo.com",
                Address = RavenJObject.FromObject(new
                {
                    City = "Paris",                    
                    Street = "Sesame str.",
                    Country = "France"
                })
            };

            yield return new User
            {
                Name = "Kenny",
                Email = "kenny@bar.com",
                Address = RavenJObject.FromObject(new
                {
                    Country = "Zimbabwe"
                })
            };

            yield return new User
            {
                Name = "Sun Tsu",
                Email = "sun.tsu@bar.com",
                Address = RavenJObject.FromObject(new
                {
                    Country = "China",
                    Province = "Hao"
                })
            };

            yield return new User
            {
                Name = "john woo",
                Email = "john.woo@bar.com",
                Address = RavenJObject.FromObject(new
                {
                    Country = "China",
                    Province = "Hao"
                })
            };

            yield return new User
            {
                Name = "Lao Tse",
                Email = "lao.tse@bar.com",
                Address = RavenJObject.FromObject(new
                {
                    Country = "China",
                    Province = "Zhao"
                })
            };
            yield return new User
            {
                Name = "Kong Fu-Tse",
                Email = "kong-fu.tse@bar.com",
                Address = RavenJObject.FromObject(new
                {
                    City = "FooBar",
                    Country = "China",
                    Province = "Zhao"
                })
            };
        }
	}
}
