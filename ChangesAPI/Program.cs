using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Abstractions.Data;
using Raven.Client.Embedded;
using Raven.Client.Util;
using Shared;

namespace ChangesAPI
{
	class Program
	{
		public class User
		{
			public string Name { get; set; }
		}
		static void Main(string[] args)
		{
			var laptops = Utils.GenerateLaptopData().Take(10).ToList();

			using (var store = new EmbeddableDocumentStore
			{
				RunInMemory = true
			})
			{
				store.Initialize();

				store.Changes()
					.ForDocumentsStartingWith("laptop")
					.Subscribe(DisplayNotificationInfo);

				using (var session = store.OpenSession())
				{
					session.Store(new User { Name = "john" });
					session.Store(new User { Name = "bob" });

					//note - the act of calling Store() on an entity  updates the object with generated key
					//before calling Store() the Id property of each object == null
					foreach (var laptop in laptops)
						session.Store(laptop);
					session.SaveChanges();					
				}

				using (var session = store.OpenSession())
				{
					//this row loads all laptop objects to session cache and retrieves them
					var fetchedLaptops = session.Load<Laptop>(laptops.Select(l => l.Id));

					//after loading, the laptop references are tracked by session,
					//so they can be deleted
					foreach (var laptop in fetchedLaptops)
						session.Delete(laptop);
					session.SaveChanges();
				}
			}
		}

		private static void DisplayNotificationInfo(DocumentChangeNotification changeInfo)
		{
			switch (changeInfo.Type)
			{
					case DocumentChangeTypes.Put:
					Console.WriteLine("Document Id = {0} was added/updated",changeInfo.Id);
					break;
					case DocumentChangeTypes.Delete:
					Console.WriteLine("Document Id = {0} was deleted",changeInfo.Id);
					break;
					//...
			}
		}
	}
}
