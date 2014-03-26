using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
	public enum CpuType
	{
		i3,
		i5,
		i7
	}

	public class Laptop
	{
		public string Id { get; set; }

		public string Manufacturer { get; set; }

		public int RamSizeInMegabatyes { get; set; }

		public CpuType Cpu { get; set; }

		public int HDDSizeInGigabytes { get; set; }

	    public override string ToString()
	    {
	        return string.Format("Id: {0}, Manufacturer: {1}, RamSizeInMegabatyes: {2}, Cpu: {3}, HDDSizeInGigabytes: {4}", Id, Manufacturer, RamSizeInMegabatyes, Cpu, HDDSizeInGigabytes);
	    }
	}
}
