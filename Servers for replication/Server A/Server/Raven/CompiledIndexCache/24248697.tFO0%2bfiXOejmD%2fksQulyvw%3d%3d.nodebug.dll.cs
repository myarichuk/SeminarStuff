using Raven.Abstractions;
using Raven.Database.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System;
using Raven.Database.Linq.PrivateExtensions;
using Lucene.Net.Documents;
using System.Globalization;
using System.Text.RegularExpressions;
using Raven.Database.Indexing;


public class Index_Auto_2fLaptops_2fByCpuAndRamSizeInMegabatyes_RangeSortByRamSizeInMegabatyes : Raven.Database.Linq.AbstractViewGenerator
{
	public Index_Auto_2fLaptops_2fByCpuAndRamSizeInMegabatyes_RangeSortByRamSizeInMegabatyes()
	{
		this.ViewText = @"from doc in docs.Laptops
select new { RamSizeInMegabatyes = doc.RamSizeInMegabatyes, Cpu = doc.Cpu }";
		this.ForEntityNames.Add("Laptops");
		this.AddMapDefinition(docs => 
			from doc in docs
			where string.Equals(doc["@metadata"]["Raven-Entity-Name"], "Laptops", System.StringComparison.InvariantCultureIgnoreCase)
			select new {
				RamSizeInMegabatyes = doc.RamSizeInMegabatyes,
				Cpu = doc.Cpu,
				__document_id = doc.__document_id
			});
		this.AddField("RamSizeInMegabatyes");
		this.AddField("Cpu");
		this.AddField("__document_id");
		this.AddQueryParameterForMap("RamSizeInMegabatyes");
		this.AddQueryParameterForMap("Cpu");
		this.AddQueryParameterForMap("__document_id");
		this.AddQueryParameterForReduce("RamSizeInMegabatyes");
		this.AddQueryParameterForReduce("Cpu");
		this.AddQueryParameterForReduce("__document_id");
	}
}
