using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.IO;

namespace IDE.Core.BOM
{

    /// <summary>
    /// Octopart provider. If we will have multiple providers, make a base class
    /// </summary>
    public class BomProvider
    {
        public BomProvider()
        {
//#if DEBUG
//            ApiKey = API_Key;
//#endif
        }

        //octopart key
        //const string API_Key = "39e9c5b8";//my personal
        //const string API_Key = "e273fa8a37db721fb6a6";
        const int BATCH_Limit = 20;
        const string octopartUrlBase = "http://octopart.com/api/v3";  // Octopart API url
        const string octopartMatchUrlEndpoint = "parts/match";             // Octopart search type
        const string octopartSearchUrlEndpoint = "parts/search";

        // string[] suppliers = new[] { "Digi-Key", "Mouser", "Farnell" };

        public string ApiKey { get; set; }

        public List<string> Suppliers { get; set; } = new List<string>();

        public bool FilterSuppliers { get; set; } = true;

        public bool ShowInStock { get; set; } = true;

        /// <summary>
        /// having an existing BOM list, execute a BOM match search
        /// </summary>
        /// <param name="bomItems"></param>
        /// <returns></returns>
        public IList<object> BomMatchQuoteSearch(IList<BomItem> bomItems, string currency = "USD")
        {
            List<Dictionary<string, string>> line_items = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> queries = new List<Dictionary<string, string>>();

            foreach (var bom in bomItems)
            {
                if (!string.IsNullOrEmpty(bom.MPN) && !string.IsNullOrEmpty(bom.Manufacturer))
                {
                    //line_items.Add(Enumerable.Range(0, csv.FieldHeaders.Length).ToDictionary(i => csv.FieldHeaders[i], i => csv.CurrentRecord[i]));
                    line_items.Add(new Dictionary<string, string>()
                    {
                        { nameof(BomItem.MPN), bom.MPN },
                        { nameof(BomItem.Manufacturer), bom.Manufacturer }
                    });
                    queries.Add(new Dictionary<string, string> {
                                {"mpn", bom.MPN},
                                {"brand", bom.Manufacturer},
                                {"reference", (line_items.Count - 1).ToString()}
                            });
                }
            }

            //query API. use batching, since octopart can accept 20 items at a time
            List<dynamic> results = new List<dynamic>();
            for (int i = 0; i < queries.Count; i += BATCH_Limit)
            {
                // Batch queries in groups of 20, query limit of
                // parts match endpoint
                var batched_queries = queries.GetRange(i, Math.Min(BATCH_Limit, queries.Count - i));

                // Create the search request
                string queryString = JsonConvert.SerializeObject(batched_queries);//(new JavaScriptSerializer()).Serialize(batched_queries);
                var client = new RestClient(octopartUrlBase);
                var req = new RestRequest(octopartMatchUrlEndpoint, Method.GET)
                            .AddParameter("apikey", ApiKey)
                            .AddParameter("queries", queryString);

                // Perform the search and obtain results
                var data = client.Execute(req).Content;
                var response = JsonConvert.DeserializeObject<dynamic>(data);
                results.AddRange(response["results"]);
            }

            // Analyze results sent back Octopart API
            Debug.WriteLine("Found " + line_items.Count + " line items in BOM.");
            // Price BOM
            int hits = 0;
            double total_avg_price = 0.0;
            foreach (var result in results)
            {
                var line_item = line_items[(int)result["reference"]];
                if (!result["items"].HasValues)
                {
                    Debug.WriteLine(string.Format("Did not find a match on line item #{0} ({1})", (int)result["reference"] + 1, string.Join(" ", line_item.Values.ToArray())));
                    continue;
                }

                // Get pricing from the first item for desired quantity
                int quantity = int.Parse(line_item["Qty"]);
                List<double> prices = new List<double>();
                foreach (var offer in result["items"][0]["offers"])
                {
                    if (offer["prices"][currency] == null)
                        continue;
                    double price = 0;
                    foreach (var price_tuple in offer["prices"][currency])
                    {
                        // Find correct price break
                        if (price_tuple[0] > quantity)
                            break;
                        price = price_tuple[1];
                    }
                    if (price != 0)
                    {
                        prices.Add(price);
                    }
                }

                if (prices.Count == 0)
                {
                    Debug.WriteLine(string.Format("Did not find {2} pricing on line item #{0} ({1})", (int)result["reference"] + 1,
                                                                                                       string.Join(" ", line_item.Values.ToArray()),
                                                                                                       currency));
                    continue;
                }

                double avg_price = quantity * prices.Sum() / prices.Count;
                total_avg_price += avg_price;
                hits++;
            }

            Debug.WriteLine(string.Format("Matched on {0:0.0}% of BOM, total average prices is {2} {1:0.00}.", (hits / (float)line_items.Count) * 100, total_avg_price, currency));


            return null;
        }

        public IList<Result> SearchByQuery(string query)
        {
            // Create the search request
            var client = new RestClient(octopartUrlBase);
            var req = new RestRequest(octopartSearchUrlEndpoint, Method.GET)
                        .AddParameter("apikey", ApiKey)
                        .AddParameter("q", query)
                        //.AddParameter("filter[queries][]", "offers.seller.name:Digi-Key")
                        //.AddParameter("filter[fields][offers.seller.name][]", "Digi-Key")
                        //.AddParameter("filter[queries][]", "offer.seller.name:Digi-Key")
                        .AddParameter("include[]", "specs")
                        .AddParameter("include[]", "datasheets")
                        .AddParameter("include[]", "imagesets");

            // Perform the search and obtain results
            var resp = client.Execute(req);
            var responseObject = JsonConvert.DeserializeObject<dynamic>(resp.Content);

            var userCurrency = (string)responseObject.user_currency;

            var resultList = new List<Result>();

            foreach (var result in responseObject.results)
            {
                var item = result.item;
                foreach (var offer in item.offers)
                {
                    // skip offers that are not from digikey and have less than qty 1000 in stock
                    //if (((int)offer.in_stock_quantity < 1000)) continue;
                    var rr = new Result();
                    var b = BuildBomItem(result, item, offer, rr);
                    if (!b)
                        continue;

                    BuildSpecs(item, rr);
                    BuildPricingInfo(userCurrency, offer, rr);

                    BuildDatasheeets(item, rr);

                    rr.Finish();
                    if (rr.prices.Count > 0) resultList.Add(rr);

                }
            }
            return resultList;

        }

        private bool BuildBomItem(dynamic result, dynamic item, dynamic offer, Result rr)
        {
            rr.seller = (string)offer.seller.name;
            if (FilterSuppliers)
                if (!Suppliers.Contains(rr.seller))//only some suppliers
                    return false;
            if (item.manufacturer != null)
                rr.manufacturer = item.manufacturer.name;
            rr.description = StripTagsRegex((string)result.snippet);//.ToString());//desc
            rr.mpn = (string)item.mpn;
            try { rr.package = (string)item.specs.case_package.value[0]; } catch { }
            rr.sku = (string)offer.sku;
            rr.stock = (int)offer.in_stock_quantity;
            if (ShowInStock && rr.stock <= 0)
                return false;
            rr.packaging = (string)offer.packaging;
            // see if we got a url for an image
            try { rr.imageURLSmall = (string)item.imagesets[0].small_image.url; } catch { }
            try { rr.imageURLMedium = (string)item.imagesets[0].medium_image.url; } catch { }
            if (string.IsNullOrEmpty(rr.imageURLMedium))
                rr.imageURLMedium = rr.imageURLSmall;
            try { rr.rohs = (string)item.specs.rohs_status.display_value; } catch { }

            return true;
        }

        private void BuildSpecs(dynamic item, Result rr)
        {
            // now pull out everything else
            var list = ((JObject)item.specs).Properties().Select(p => p.Name).ToList();
            foreach (var key in list)
            {
                try
                {
                    rr.specs.Add((string)item.specs[key].metadata.name, (string)item.specs[key].display_value);
                }
                catch { }
            }
        }

        private void BuildPricingInfo(string userCurrency, dynamic offer, Result rr)
        {
            // pull pricing information
            try
            {
                //these could have multiple currencies
                string currency = ((JObject)offer.prices).Properties().Select(p => p.Name).FirstOrDefault();
                if (!string.IsNullOrEmpty(currency))
                {
                    if (offer.prices[currency] != null)
                    {
                        foreach (var price in offer.prices[currency])
                        {
                            rr.prices.Add((int)price[0], (float)price[1]);
                        }

                        var actualCurrency = currency;
                        if (!string.IsNullOrEmpty((string)offer.eligible_region) && !string.IsNullOrEmpty(userCurrency))
                        {
                            actualCurrency = userCurrency;
                        }
                        rr.currency = actualCurrency;
                    }
                }
            }
            catch { }
        }

        private void BuildDatasheeets(dynamic item, Result rr)
        {
            //datasheets
            if (item.datasheets != null)
            {
                //we want datasheets from the same supplier (we could use the manufacturer as well; we could make an option)
                //we could group sheets by source
                foreach (var datasheet in item.datasheets)
                {
                    try
                    {
                        string srcName = datasheet.attribution.sources[0].name;
                        if (rr.seller == srcName)
                        {
                            var url = (string)datasheet.url;
                            Uri uri = new Uri(url);
                            var filename = Path.GetFileName(uri.LocalPath);
                            if (!rr.datasheets.ContainsKey(filename))
                                rr.datasheets.Add(filename, url);
                        }
                    }
                    catch { }
                }
            }
        }

        string StripTagsRegex(string source)
        {
            return Regex.Replace(source, "<.*?>", string.Empty);
        }
    }
    public class Result
    {
        public string description;
        public Dictionary<int, float> prices = new Dictionary<int, float>();
        public Dictionary<string, string> specs = new Dictionary<string, string>();
        public List<int> qtyList = new List<int>();
        public Dictionary<string, string> datasheets = new Dictionary<string, string>();
        public string sku;
        public string mpn;
        public int stock;
        //public float value;
        //public float voltageRating;
        //public string tolerance;
        public string package;
        public string packaging;
        public string manufacturer;
        // public string dielectric;
        public string imageURLSmall;
        public string imageURLMedium;
        public string seller;//supplier
        public string rohs;
        public string currency;

        public string GetSpec(string key)
        {
            if (specs.ContainsKey(key)) return specs[key];
            return "";
        }

        //this will return the price if present, if not the price
        //for the next lower quantity, else 0 if not a valid quantity
        public float GetPrice(int qty)
        {
            int i;
            if (prices.ContainsKey(qty)) return prices[qty];
            for (i = 0; i < qtyList.Count; i++) if (qtyList[i] >= qty) break;
            if (i == qtyList.Count) return 0f;
            if (i > 0) return prices[qtyList[i - 1]];
            return 0f;
        }

        public void Finish()
        {
            //generate ordered list of quantities
            foreach (int x in prices.Keys.OrderBy(x => x).ToArray()) qtyList.Add(x);
        }
    }

}
