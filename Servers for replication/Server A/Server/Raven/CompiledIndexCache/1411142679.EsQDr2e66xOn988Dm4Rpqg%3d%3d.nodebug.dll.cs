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


public class Index_Auto_2fLaptops_2fByManufacturer : Raven.Database.Linq.AbstractViewGenerator
{
	public Index_Auto_2fLaptops_2fByManufacturer()
	{
		this.ViewText = @"from doc in docs.Laptops
select new { Manufacturer = doc.Manufacturer }";
		this.ForEntityNames.Add("Laptops");
		this.AddMapDefinition(docs => 
			from doc in docs
			where string.Equals(doc["@metadata"]["Raven-Entity-Name"], "Laptops", System.StringComparison.InvariantCultureIgnoreCase)
			select new {
				Manufacturer = doc.Manufacturer,
				__document_id = doc.__document_id
			});
		this.AddField("Manufacturer");
		this.AddField("__document_id");
		this.AddQueryParameterForMap("Manufacturer");
		this.AddQueryParameterForMap("__document_id");
		this.AddQueryParameterForReduce("Manufacturer");
		this.AddQueryParameterForReduce("__document_id");
	}
}
