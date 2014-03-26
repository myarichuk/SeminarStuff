using Raven.Client.Document;
using Raven.Client.Extensions;

namespace MinimalProject
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var store = new DocumentStore
			{
				Url = "http://localhost:8080"
			})
			{
				store.Initialize();
				store.DatabaseCommands.EnsureDatabaseExists("FooDB");

				using (var session = store.OpenSession(database: "FooDB"))
				{
					
				}
			}			
		}
	}
}
