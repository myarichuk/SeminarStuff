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


public class Index_Auto_2fLaptops_2fByCpu : Raven.Database.Linq.AbstractViewGenerator
{
	public Index_Auto_2fLaptops_2fByCpu()
	{
		this.ViewText = @"from doc in docs.Laptops
select new { Cpu = doc.Cpu }";
		this.ForEntityNames.Add("Laptops");
		this.AddMapDefinition(docs => 
			from doc in docs
			where string.Equals(doc["@metadata"]["Raven-Entity-Name"], "Laptops", System.StringComparison.InvariantCultureIgnoreCase)
			select new {
				Cpu = doc.Cpu,
				__document_id = doc.__document_id
			});
		this.AddField("Cpu");
		this.AddField("__document_id");
		this.AddQueryParameterForMap("Cpu");
		this.AddQueryParameterForMap("__document_id");
		this.AddQueryParameterForReduce("Cpu");
		this.AddQueryParameterForReduce("__document_id");
	}
}
