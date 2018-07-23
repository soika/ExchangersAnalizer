// -----------------------------------------------------------------------
// <copyright file="MinExchangeGateAPI.cs" company="SóiKA Apps">
//      All rights are reserved. Reproduction or transmission in whole or
//      in part, in any form or by any means, electronic, mechanical or
//      otherwise, is prohibited without the prior written consent of the 
//      copyright owner.
// </copyright>
// <summary>
//      Definition of the MinExchangeGateAPI.cs class.
// </summary>
// -----------------------------------------------------------------------

namespace ExchangersAnalizer.Exchangers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using ExchangeSharp;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class MinExchangeGateAPI
    {
        private readonly IHttpClientFactory httpClientFactory;

        public MinExchangeGateAPI(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<string>> GetSymbolsAsync()
        {
            var client = httpClientFactory.CreateClient("Gate");
            var response = await client.GetAsync("pairs");
            var jsonContent = await response.Content.ReadAsStringAsync();
            var symbols = JsonConvert.DeserializeObject<IEnumerable<string>>(jsonContent);
            return symbols.Where(s => s.EndsWith("_BTC"));
        }

        public async Task<IEnumerable<KeyValuePair<string, ExchangeTicker>>> GetTickersAsync()
        {
            var tickers = new List<KeyValuePair<string, ExchangeTicker>>();
            var client = httpClientFactory.CreateClient("Gate");
            var response = await client.GetAsync("tickers");
            var jsonContent = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(jsonContent);
            var items = jObject.Properties();

            foreach (var item in items)
            {
                if (item.Name.ToUpper().EndsWith("_BTC"))
                {
                    tickers.Add(
                        new KeyValuePair<string, ExchangeTicker>(item.Name, ParseTicker(item.Name, item.Value)));
                }
            }

            return tickers;
        }

        private ExchangeTicker ParseTicker(string name, JToken jtoken)
        {
            var quoteVolume = jtoken["quoteVolume"].ConvertInvariant(new decimal());
            var lastPrice = jtoken["last"].ConvertInvariant(new decimal());

            return new ExchangeTicker
            {
                Last = lastPrice,
                Bid = jtoken["highestBid"].ConvertInvariant(new decimal()),
                Ask = jtoken["lowestAsk"].ConvertInvariant(new decimal()),
                Volume = new ExchangeVolume
                {
                    BaseSymbol = name,
                    ConvertedSymbol = name,
                    BaseVolume = jtoken["baseVolume"].ConvertInvariant(new decimal()),
                    ConvertedVolume = quoteVolume * lastPrice,
                    Timestamp = DateTime.Now
                }
            };
        }
    }
}