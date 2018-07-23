// -----------------------------------------------------------------------
// <copyright file="MinExchangeUpbitAPI.cs" company="SóiKA Apps">
//      All rights are reserved. Reproduction or transmission in whole or
//      in part, in any form or by any means, electronic, mechanical or
//      otherwise, is prohibited without the prior written consent of the 
//      copyright owner.
// </copyright>
// <summary>
//      Definition of the MinExchangeUpbitAPI.cs class.
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

    public class MinExchangeUpbitAPI
    {
        private readonly IHttpClientFactory httpClientFactory;

        public MinExchangeUpbitAPI(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<string>> GetSymbolsAsync()
        {
            var client = httpClientFactory.CreateClient("Upbit");
            var response = await client.GetAsync("market/all");
            var jsonContent = await response.Content.ReadAsStringAsync();

            var parsedData = JsonConvert.DeserializeObject<IEnumerable<UpbitSymbol>>(jsonContent);
            return parsedData.Where(x => x.Symbol.StartsWith("BTC-")).Select(x => x.Symbol);
        }

        public async Task<IEnumerable<KeyValuePair<string, ExchangeTicker>>> GetTickersAsync(params string[] symbols)
        {
            var tickers = new List<KeyValuePair<string, ExchangeTicker>>();
            var client = httpClientFactory.CreateClient("Upbit");
            var response = await client.GetAsync($"ticker?markets={string.Join(',', symbols)}");
            var jsonContent = await response.Content.ReadAsStringAsync();
            var parsedData = JsonConvert.DeserializeObject<IEnumerable<UpbitTicker>>(jsonContent);

            foreach (var item in parsedData)
            {
                tickers.Add(
                    new KeyValuePair<string, ExchangeTicker>(item.Symbol, ParseTicker(item)));
            }

            return tickers;
        }

        private ExchangeTicker ParseTicker(UpbitTicker ticker)
        {
            return new ExchangeTicker
            {
                Last = ticker.Close,
                Bid = ticker.Low,
                Ask = ticker.High,
                Volume = new ExchangeVolume
                {
                    BaseSymbol = ticker.Symbol,
                    ConvertedSymbol = ticker.Symbol,
                    BaseVolume = ticker.Volume,
                    ConvertedVolume = ticker.Amount * ticker.Close,
                    Timestamp = DateTime.Now
                }
            };
        }
    }

    public class UpbitSymbol
    {
        [JsonProperty("market")]
        public string Symbol { get; set; }

        [JsonProperty("korean_name")]
        public string KoreanName { get; set; }

        [JsonProperty("english_name")]
        public string EnglishName { get; set; }
    }

    public class UpbitTicker
    {
        [JsonProperty("opening_price")]
        public decimal Open { get; set; }

        [JsonProperty("trade_price")]
        public decimal Close { get; set; }

        [JsonProperty("low_price")]
        public decimal Low { get; set; }

        [JsonProperty("high_price")]
        public decimal High { get; set; }

        [JsonProperty("acc_trade_volume_24h")]
        public decimal Amount { get; set; }

        [JsonProperty("acc_trade_volume")]
        public decimal Volume { get; set; }

        [JsonProperty("market")]
        public string Symbol { get; set; }
    }
}