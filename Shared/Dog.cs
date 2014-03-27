using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class Dog
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Breed { get; set; }

        public string[] OwnerIds { get; set; }

        public Gender Gender { get; set; }
    }

    public enum Gender
    {
        MaleDog,
        FemaleDog
    }
}
