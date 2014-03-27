using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Json.Linq;

namespace Shared
{
    public class User
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }

        public string[] Phones { get; set; }

        public string Email { get; set; }

        public RavenJObject Address { get; set; }

        public string Query { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {0}, Name: {1}, LastName: {2}, Phones: {3}, Email: {4}, Address: {5}", Id, Name, LastName, Phones, Email, Address);
        }
    }
}
