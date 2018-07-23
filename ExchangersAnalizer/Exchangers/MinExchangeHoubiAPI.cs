// -----------------------------------------------------------------------
// <copyright file="MinExchangeHoubiAPI.cs" company="SóiKA Apps">
//      All rights are reserved. Reproduction or transmission in whole or
//      in part, in any form or by any means, electronic, mechanical or
//      otherwise, is prohibited without the prior written consent of the 
//      copyright owner.
// </copyright>
// <summary>
//      Definition of the MinExchangeHoubiAPI.cs class.
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

    public class MinExchangeHoubiAPI
    {
        private readonly IHttpClientFactory httpClientFactory;

        public MinExchangeHoubiAPI(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<string>> GetSymbolsAsync()
        {
            var client = httpClientFactory.CreateClient("Houbi");
            var response = await client.GetAsync("v1/common/symbols");
            var jsonContent = await response.Content.ReadAsStringAsync();

            var parsedData = JsonConvert.DeserializeObject<HuobiSymbolResponse>(jsonContent);
            return parsedData.Data?.Where(x => x.QuoteCurrency.Equals("btc")).Select(x => x.Symbol);
        }

        public async Task<IEnumerable<KeyValuePair<string, ExchangeTicker>>> GetTickersAsync()
        {
            var tickers = new List<KeyValuePair<string, ExchangeTicker>>();
            var client = httpClientFactory.CreateClient("Houbi");
            var response = await client.GetAsync("market/tickers");
            var jsonContent = await response.Content.ReadAsStringAsync();
            var parsedData = JsonConvert.DeserializeObject<HuobiTickerResponse>(jsonContent);

            var tickerData = parsedData.Data;
            foreach (var huobiTicker in tickerData)
            {
                tickers.Add(new KeyValuePair<string, ExchangeTicker>(huobiTicker.Symbol, ParseTicker(huobiTicker)));
            }

            return tickers;
        }

        private ExchangeTicker ParseTicker(HuobiTicker huobiTicker)
        {
            return new ExchangeTicker
            {
                Last = huobiTicker.Close,
                Bid = huobiTicker.High,
                Ask = huobiTicker.Low,
                Volume = new ExchangeVolume
                {
                    BaseSymbol = huobiTicker.Symbol,
                    ConvertedSymbol = huobiTicker.Symbol,
                    BaseVolume = huobiTicker.Volume,
                    ConvertedVolume = huobiTicker.Amount * huobiTicker.Close,
                    Timestamp = DateTime.Now
                }
            };
        }
    }

    public class HuobiSymbolResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("data")]
        public HuobiSymbol[] Data { get; set; }
    }

    public class HuobiTickerResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("ts")]
        public double Timestamp { get; set; }

        [JsonProperty("data")]
        public HuobiTicker[] Data { get; set; }
    }

    public class HuobiSymbol
    {
        [JsonProperty("base-currency")]
        public string BaseCurrency { get; set; }

        [JsonProperty("quote-currency")]
        public string QuoteCurrency { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }
    }

    public class HuobiTicker
    {
        [JsonProperty("open")]
        public decimal Open { get; set; }

        [JsonProperty("close")]
        public decimal Close { get; set; }

        [JsonProperty("low")]
        public decimal Low { get; set; }

        [JsonProperty("high")]
        public decimal High { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("count")]
        public decimal Count { get; set; }

        [JsonProperty("vol")]
        public decimal Volume { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }
    }
}