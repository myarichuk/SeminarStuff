using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Raven.Client;

namespace Shared
{
	public static class Utils
	{
		public static IEnumerable<Laptop> GenerateLaptopData()
		{
			return GenerateLaptopDataPerManufacturer("Lenovo")
				.Concat(GenerateLaptopDataPerManufacturer("Acer"))
				.Concat(GenerateLaptopDataPerManufacturer("Toshiba"))
				.Concat(GenerateLaptopDataPerManufacturer("Sony"))
				.Concat(GenerateLaptopDataPerManufacturer("Intel"))
				.Concat(GenerateLaptopDataPerManufacturer("Foo"))
				.Concat(GenerateLaptopDataPerManufacturer("Bar"));
		}


		public static IEnumerable<Laptop> GenerateLaptopDataPerManufacturer(string manufacturer)
		{
			int cpuIndex = 0;
			for (int hddSize = 250; hddSize < 1000; hddSize += 250)
			{
				for (int ramSize = 256; ramSize < 4096 * 2; ramSize += 256)
				{
					cpuIndex++;
					CpuType cpuType;

					if (cpuIndex % 3 == 0)
						cpuType = CpuType.i7;
					else if (cpuIndex % 2 == 0)
						cpuType = CpuType.i5;
					else
						cpuType = CpuType.i3;

					yield return new Laptop
					{
						Manufacturer = manufacturer,
						Cpu = cpuType,
						RamSizeInMegabatyes = ramSize,
						HDDSizeInGigabytes = hddSize
					};
				}
			}
		}

	}
}
