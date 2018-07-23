// -----------------------------------------------------------------------
// <copyright file="MinExchangeOkexAPI.cs" company="SóiKA Apps">
//      All rights are reserved. Reproduction or transmission in whole or
//      in part, in any form or by any means, electronic, mechanical or
//      otherwise, is prohibited without the prior written consent of the 
//      copyright owner.
// </copyright>
// <summary>
//      Definition of the MinExchangeOkexAPI.cs class.
// </summary>
// -----------------------------------------------------------------------

namespace ExchangersAnalizer.Exchangers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using ExchangeSharp;
    using Newtonsoft.Json.Linq;

    public class MinExchangeOkexAPI : ExchangeAPI
    {
        public MinExchangeOkexAPI()
        {
            RequestContentType = "application/x-www-form-urlencoded";
        }

        public override string BaseUrl { get; set; } = "https://www.okex.com/api/v1";

        public string BaseUrlV2 { get; set; } = "https://www.okex.com/v2/spot";

        public override string BaseUrlWebSocket { get; set; } = "wss://real.okex.com:10441/websocket";

        public override string Name => "Okex";

        public override string NormalizeSymbol(string symbol)
        {
            return (symbol ?? string.Empty).ToLowerInvariant().Replace('-', '_');
        }

        protected override async Task ProcessRequestAsync(HttpWebRequest request, Dictionary<string, object> payload)
        {
            var exchangeOkexApi = this;
            if (!exchangeOkexApi.CanMakeAuthenticatedRequest(payload))
            {
                return;
            }

            payload.Remove("nonce");
            payload["api_key"] = exchangeOkexApi.PublicApiKey.ToUnsecureString();
            var str1 = string.Join(
                "&",
                new SortedSet<string>(
                    CryptoUtility.GetFormForPayload(payload, false).Split('&', StringSplitOptions.None),
                    StringComparer.Ordinal));
            var str2 = CryptoUtility.MD5Sign(str1 + "&secret_key=" + exchangeOkexApi.PrivateApiKey.ToUnsecureString());
            await request.WriteToRequestAsync(str1 + "&sign=" + str2);
        }

        private async Task<Tuple<JToken, string>> MakeRequestOkexAsync(
            string symbol,
            string subUrl,
            string baseUrl = null)
        {
            var exchangeOkexApi = this;
            symbol = exchangeOkexApi.NormalizeSymbol(symbol);
            return new Tuple<JToken, string>(
                await exchangeOkexApi.MakeJsonRequestAsync<JToken>(
                    subUrl.Replace("$SYMBOL$", symbol ?? string.Empty),
                    baseUrl,
                    null,
                    null),
                symbol);
        }

        protected override async Task<IEnumerable<string>> OnGetSymbolsAsync()
        {
            return (await GetSymbolsMetadataAsync()).Select(x => x.MarketName);
        }

        protected override async Task<IEnumerable<ExchangeMarket>> OnGetSymbolsMetadataAsync()
        {
            var exchangeOkexApi = this;
            if (exchangeOkexApi.ReadCache("GetSymbolsMetadata", out List<ExchangeMarket> markets))
            {
                return markets;
            }

            markets = new List<ExchangeMarket>();
            foreach (var jtoken in await exchangeOkexApi.MakeJsonRequestAsync<JToken>(
                "/markets/products",
                exchangeOkexApi.BaseUrlV2,
                null,
                null))
            {
                var stringLowerInvariant = jtoken["symbol"].ToStringLowerInvariant();
                var strArray = stringLowerInvariant.Split('_', StringSplitOptions.None);
                var exchangeMarket = new ExchangeMarket
                {
                    MarketName = stringLowerInvariant,
                    IsActive = jtoken["online"].ConvertInvariant(false),
                    BaseCurrency = strArray[1],
                    MarketCurrency = strArray[0]
                };
                var num1 = Math.Pow(10.0, -jtoken["quotePrecision"].ConvertInvariant(0.0));
                exchangeMarket.QuantityStepSize = num1.ConvertInvariant(new decimal());
                var num2 = Math.Pow(10.0, jtoken["maxSizeDigit"].ConvertInvariant(0.0));
                exchangeMarket.MaxTradeSize = num2.ConvertInvariant(new decimal()) - new decimal(10, 0, 0, false, 1);
                exchangeMarket.MinTradeSize = jtoken["minTradeSize"].ConvertInvariant(new decimal());
                exchangeMarket.PriceStepSize = jtoken["quoteIncrement"].ConvertInvariant(new decimal());
                exchangeMarket.MinPrice = exchangeMarket.PriceStepSize.Value;
                var num3 = Math.Pow(10.0, jtoken["maxPriceDigit"].ConvertInvariant(0.0));
                exchangeMarket.MaxPrice = num3.ConvertInvariant(new decimal()) - new decimal(10, 0, 0, false, 1);
                markets.Add(exchangeMarket);
            }

            exchangeOkexApi.WriteCache("GetSymbolsMetadata", TimeSpan.FromMinutes(60.0), markets);
            return markets;
        }

        protected override async Task<ExchangeTicker> OnGetTickerAsync(string symbol)
        {
            var tuple = await MakeRequestOkexAsync(symbol, "/ticker.do?symbol=$SYMBOL$", null);
            return ParseTicker(tuple.Item2, tuple.Item1);
        }

        protected override async Task<IEnumerable<KeyValuePair<string, ExchangeTicker>>> OnGetTickersAsync()
        {
            var tuple = await MakeRequestOkexAsync(null, "/markets/index-tickers?limit=100000000", BaseUrlV2);
            var keyValuePairList = new List<KeyValuePair<string, ExchangeTicker>>();
            foreach (var ticker in tuple.Item1)
            {
                var stringInvariant = ticker["symbol"].ToStringInvariant();
                keyValuePairList.Add(
                    new KeyValuePair<string, ExchangeTicker>(stringInvariant, ParseTickerV2(stringInvariant, ticker)));
            }

            return keyValuePairList;
        }

        protected override IDisposable OnGetTradesWebSocket(
            Action<KeyValuePair<string, ExchangeTrade>> callback,
            params string[] symbols)
        {
            if (callback == null || symbols == null || symbols.Length == 0)
            {
                return null;
            }

            var normalizedSymbol = NormalizeSymbol(symbols[0]);
            return ConnectWebSocket(
                string.Empty,
                (msg, _socket) =>
                {
                    try
                    {
                        var token = JToken.Parse(msg.UTF8String())[0];
                        var stringInvariant = token["channel"].ToStringInvariant();
                        if (stringInvariant.EqualsWithOption("addChannel", StringComparison.OrdinalIgnoreCase))
                        {
                            return;
                        }

                        var sArray = stringInvariant.Split('_', StringSplitOptions.None);
                        var key = sArray[3] + "_" + sArray[4];
                        foreach (var exchangeTrade in (IEnumerable<ExchangeTrade>) ParseWebSocket(sArray, token))
                        {
                            callback(new KeyValuePair<string, ExchangeTrade>(key, exchangeTrade));
                        }
                    }
                    catch
                    {
                    }
                },
                _socket =>
                {
                    var message = string.Format(
                        "{{'event':'addChannel','channel':'{0}'}}",
                        string.Format("ok_sub_spot_{0}_deals", normalizedSymbol));
                    _socket.SendMessage(message);
                });
        }

        protected override async Task<ExchangeOrderBook> OnGetOrderBookAsync(string symbol, int maxCount = 100)
        {
            var tuple = await MakeRequestOkexAsync(symbol, "/depth.do?symbol=$SYMBOL$", null);
            var exchangeOrderBook = new ExchangeOrderBook();
            foreach (var jToken in tuple.Item1["asks"])
            {
                var jarray = (JArray) jToken;
                var exchangeOrderPrice = new ExchangeOrderPrice
                {
                    Amount = jarray[1].ConvertInvariant(new decimal()),
                    Price = jarray[0].ConvertInvariant(new decimal())
                };
                exchangeOrderBook.Asks[exchangeOrderPrice.Price] = exchangeOrderPrice;
            }

            foreach (JArray jarray in tuple.Item1["bids"])
            {
                var exchangeOrderPrice = new ExchangeOrderPrice
                {
                    Amount = jarray[1].ConvertInvariant(new decimal()),
                    Price = jarray[0].ConvertInvariant(new decimal())
                };
                exchangeOrderBook.Bids[exchangeOrderPrice.Price] = exchangeOrderPrice;
            }

            return exchangeOrderBook;
        }

        protected override async Task OnGetHistoricalTradesAsync(
            Func<IEnumerable<ExchangeTrade>, bool> callback,
            string symbol,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var allTrades = new List<ExchangeTrade>();
            foreach (var jtoken in (await MakeRequestOkexAsync(symbol, "/trades.do?symbol=$SYMBOL$", null)).Item1)
            {
                allTrades.Add(
                    new ExchangeTrade
                    {
                        Amount = jtoken["amount"].ConvertInvariant(new decimal()),
                        Price = jtoken["price"].ConvertInvariant(new decimal()),
                        Timestamp = ((double) jtoken["date_ms"].ConvertInvariant(0L))
                            .UnixTimeStampToDateTimeMilliseconds(),
                        Id = jtoken["tid"].ConvertInvariant(0L),
                        IsBuy = jtoken["type"].ToStringInvariant() == "buy"
                    });
            }

            var num = callback(allTrades) ? 1 : 0;
        }

        protected override async Task<IEnumerable<MarketCandle>> OnGetCandlesAsync(
            string symbol,
            int periodSeconds,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? limit = null)
        {
            var exchangeOkexApi = this;
            var candles = new List<MarketCandle>();
            symbol = exchangeOkexApi.NormalizeSymbol(symbol);
            var str = "/kline.do?symbol=" + symbol;
            if (startDate.HasValue)
            {
                str = str + "&since=" + (long) startDate.Value.UnixTimestampFromDateTimeMilliseconds();
            }

            if (limit.HasValue)
            {
                str = str + "&size=" + limit.Value.ToStringInvariant();
            }

            var periodStringLong = CryptoUtility.SecondsToPeriodStringLong(periodSeconds);
            var url = str + "&type=" + periodStringLong;
            foreach (JArray jarray in await exchangeOkexApi.MakeJsonRequestAsync<JToken>(url, null, null, null))
            {
                var num1 = jarray[4].ConvertInvariant(new decimal());
                var num2 = jarray[5].ConvertInvariant(0.0);
                candles.Add(
                    new MarketCandle
                    {
                        Timestamp = ((double) jarray[0].ConvertInvariant(0L)).UnixTimeStampToDateTimeMilliseconds(),
                        OpenPrice = jarray[1].ConvertInvariant(new decimal()),
                        HighPrice = jarray[2].ConvertInvariant(new decimal()),
                        LowPrice = jarray[3].ConvertInvariant(new decimal()),
                        ClosePrice = num1,
                        BaseVolume = num2,
                        ConvertedVolume = (double) num1 * num2,
                        ExchangeName = exchangeOkexApi.Name,
                        Name = symbol,
                        PeriodSeconds = periodSeconds
                    });
            }

            return candles;
        }

        protected override async Task<Dictionary<string, decimal>> OnGetAmountsAsync()
        {
            var exchangeOkexApi = this;
            var amounts = new Dictionary<string, decimal>();
            var noncePayloadAsync = await exchangeOkexApi.OnGetNoncePayloadAsync();
            foreach (JProperty jproperty in (await exchangeOkexApi.MakeJsonRequestAsync<JToken>(
                "/userinfo.do",
                exchangeOkexApi.BaseUrl,
                noncePayloadAsync,
                "POST"))["info"]["funds"])
            {
                exchangeOkexApi.ParseAmounts(jproperty.Value, amounts);
            }

            return amounts;
        }

        protected override async Task<Dictionary<string, decimal>> OnGetAmountsAvailableToTradeAsync()
        {
            var exchangeOkexApi = this;
            var amounts = new Dictionary<string, decimal>();
            var noncePayloadAsync = await exchangeOkexApi.OnGetNoncePayloadAsync();
            var jtoken1 = (await exchangeOkexApi.MakeJsonRequestAsync<JToken>(
                "/userinfo.do",
                exchangeOkexApi.BaseUrl,
                noncePayloadAsync,
                "POST"))["info"]["funds"];
            var jtoken2 = jtoken1["free"];
            return exchangeOkexApi.ParseAmounts(jtoken1["free"], amounts);
        }

        protected override async Task<ExchangeOrderResult> OnPlaceOrderAsync(ExchangeOrderRequest order)
        {
            var exchangeOkexApi = this;
            var symbol = exchangeOkexApi.NormalizeSymbol(order.Symbol);
            var payload = await exchangeOkexApi.OnGetNoncePayloadAsync();
            payload["symbol"] = symbol;
            payload["type"] = order.IsBuy ? "buy" : "sell";
            var outputQuantity = await exchangeOkexApi.ClampOrderQuantity(symbol, order.Amount);
            var outputPrice = await exchangeOkexApi.ClampOrderPrice(symbol, order.Price);
            if (order.OrderType == OrderType.Market)
            {
                throw new NotSupportedException(
                    "Okex confuses price with amount while sending a market order, so market orders are disabled for now");
            }

            payload["price"] = outputPrice;
            payload["amount"] = outputQuantity;
            order.ExtraParameters.CopyTo(payload);
            var token = await exchangeOkexApi.MakeJsonRequestAsync<JToken>(
                "/trade.do",
                exchangeOkexApi.BaseUrl,
                payload,
                "POST");
            order.Amount = outputQuantity;
            order.Price = outputPrice;
            return exchangeOkexApi.ParsePlaceOrder(token, order);
        }

        protected override async Task OnCancelOrderAsync(string orderId, string symbol = null)
        {
            var exchangeOkexApi = this;
            var noncePayloadAsync = await exchangeOkexApi.OnGetNoncePayloadAsync();
            if (string.IsNullOrEmpty(symbol))
            {
                throw new InvalidOperationException("Okex cancel order request requires symbol");
            }

            noncePayloadAsync[nameof(symbol)] = exchangeOkexApi.NormalizeSymbol(symbol);
            noncePayloadAsync["order_id"] = orderId;
            var jtoken = await exchangeOkexApi.MakeJsonRequestAsync<JToken>(
                "/cancel_order.do",
                exchangeOkexApi.BaseUrl,
                noncePayloadAsync,
                "POST");
        }

        protected override async Task<ExchangeOrderResult> OnGetOrderDetailsAsync(string orderId, string symbol = null)
        {
            var exchangeOkexApi = this;
            var orders = new List<ExchangeOrderResult>();
            var noncePayloadAsync = await exchangeOkexApi.OnGetNoncePayloadAsync();
            if (string.IsNullOrEmpty(symbol))
            {
                throw new InvalidOperationException("Okex single order details request requires symbol");
            }

            noncePayloadAsync[nameof(symbol)] = exchangeOkexApi.NormalizeSymbol(symbol);
            noncePayloadAsync["order_id"] = orderId;
            foreach (var token in (await exchangeOkexApi.MakeJsonRequestAsync<JToken>(
                "/order_info.do",
                exchangeOkexApi.BaseUrl,
                noncePayloadAsync,
                "POST"))["orders"])
            {
                orders.Add(exchangeOkexApi.ParseOrder(token));
            }

            return orders[0];
        }

        protected override async Task<IEnumerable<ExchangeOrderResult>> OnGetOpenOrderDetailsAsync(string symbol)
        {
            var exchangeOkexApi = this;
            var orders = new List<ExchangeOrderResult>();
            var noncePayloadAsync = await exchangeOkexApi.OnGetNoncePayloadAsync();
            noncePayloadAsync[nameof(symbol)] = symbol;
            noncePayloadAsync["order_id"] = -1;
            foreach (var token in (await exchangeOkexApi.MakeJsonRequestAsync<JToken>(
                "/order_info.do",
                exchangeOkexApi.BaseUrl,
                noncePayloadAsync,
                "POST"))["orders"])
            {
                orders.Add(exchangeOkexApi.ParseOrder(token));
            }

            return orders;
        }

        private ExchangeTicker ParseTicker(string symbol, JToken data)
        {
            var jtoken = data["ticker"];
            var num1 = jtoken["last"].ConvertInvariant(new decimal());
            var num2 = jtoken["vol"].ConvertInvariant(new decimal());
            return new ExchangeTicker
            {
                Ask = jtoken["sell"].ConvertInvariant(new decimal()),
                Bid = jtoken["buy"].ConvertInvariant(new decimal()),
                Last = num1,
                Volume = new ExchangeVolume
                {
                    BaseVolume = num2,
                    BaseSymbol = symbol,
                    ConvertedVolume = num2 * num1,
                    ConvertedSymbol = symbol,
                    Timestamp = ((double) data["date"].ConvertInvariant(0L)).UnixTimeStampToDateTimeSeconds()
                }
            };
        }

        private ExchangeTicker ParseTickerV2(string symbol, JToken ticker)
        {
            var num1 = ticker["last"].ConvertInvariant(new decimal());
            var num2 = ticker["volume"].ConvertInvariant(new decimal());
            return new ExchangeTicker
            {
                Ask = ticker["sell"].ConvertInvariant(new decimal()),
                Bid = ticker["buy"].ConvertInvariant(new decimal()),
                Last = num1,
                Volume = new ExchangeVolume
                {
                    BaseVolume = num2,
                    BaseSymbol = symbol,
                    ConvertedVolume = num2 * num1,
                    ConvertedSymbol = symbol,
                    Timestamp = ((double) ticker["createdDate"].ConvertInvariant(0L))
                        .UnixTimeStampToDateTimeMilliseconds()
                }
            };
        }

        private Dictionary<string, decimal> ParseAmounts(JToken token, Dictionary<string, decimal> amounts)
        {
            foreach (JProperty jproperty in token)
            {
                var num = jproperty.Value.ConvertInvariant(new decimal());
                if (!(num == decimal.Zero))
                {
                    if (amounts.ContainsKey(jproperty.Name))
                    {
                        amounts[jproperty.Name] += num;
                    }
                    else
                    {
                        amounts[jproperty.Name] = num;
                    }
                }
            }

            return amounts;
        }

        private ExchangeOrderResult ParsePlaceOrder(JToken token, ExchangeOrderRequest order)
        {
            var exchangeOrderResult = new ExchangeOrderResult();
            exchangeOrderResult.Amount = order.Amount;
            exchangeOrderResult.Price = order.Price;
            exchangeOrderResult.IsBuy = order.IsBuy;
            exchangeOrderResult.OrderId = token["order_id"].ToStringInvariant();
            exchangeOrderResult.Symbol = order.Symbol;
            exchangeOrderResult.AveragePrice = exchangeOrderResult.Price;
            exchangeOrderResult.Result = ExchangeAPIOrderResult.Pending;
            return exchangeOrderResult;
        }

        private ExchangeAPIOrderResult ParseOrderStatus(int status)
        {
            switch (status)
            {
                case -1:
                    return ExchangeAPIOrderResult.Canceled;
                case 0:
                    return ExchangeAPIOrderResult.Pending;
                case 1:
                    return ExchangeAPIOrderResult.FilledPartially;
                case 2:
                    return ExchangeAPIOrderResult.Filled;
                case 4:
                    return ExchangeAPIOrderResult.PendingCancel;
                default:
                    return ExchangeAPIOrderResult.Unknown;
            }
        }

        private ExchangeOrderResult ParseOrder(JToken token)
        {
            return new ExchangeOrderResult
            {
                Amount = token["amount"].ConvertInvariant(new decimal()),
                AmountFilled = token["deal_amount"].ConvertInvariant(new decimal()),
                Price = token["price"].ConvertInvariant(new decimal()),
                AveragePrice = token["avg_price"].ConvertInvariant(new decimal()),
                IsBuy = token["type"].ToStringInvariant().StartsWith("buy"),
                OrderDate = ((double) token["create_date"].ConvertInvariant(0L)).UnixTimeStampToDateTimeMilliseconds(),
                OrderId = token["order_id"].ToStringInvariant(),
                Symbol = token["symbol"].ToStringInvariant(),
                Result = ParseOrderStatus(token["status"].ConvertInvariant(0))
            };
        }

        private object ParseWebSocket(string[] sArray, JToken token)
        {
            var s = sArray[5];
            if (s == "depth")
            {
                return ParseOrderBookWebSocket(token);
            }

            if (s == "deals")
            {
                return ParseTradesWebSocket(token["data"]);
            }

            return null;
        }

        private ExchangeOrderBook ParseOrderBookWebSocket(JToken token)
        {
            var exchangeOrderBook = new ExchangeOrderBook();
            foreach (JArray jarray in token["asks"])
            {
                var exchangeOrderPrice = new ExchangeOrderPrice
                {
                    Price = jarray[0].ConvertInvariant(new decimal()),
                    Amount = jarray[1].ConvertInvariant(new decimal())
                };
                exchangeOrderBook.Asks[exchangeOrderPrice.Price] = exchangeOrderPrice;
            }

            foreach (JArray jarray in token["bids"])
            {
                var exchangeOrderPrice = new ExchangeOrderPrice
                {
                    Price = jarray[0].ConvertInvariant(new decimal()),
                    Amount = jarray[1].ConvertInvariant(new decimal())
                };
                exchangeOrderBook.Bids[exchangeOrderPrice.Price] = exchangeOrderPrice;
            }

            return exchangeOrderBook;
        }

        private IEnumerable<ExchangeTrade> ParseTradesWebSocket(JToken token)
        {
            var exchangeTradeList = new List<ExchangeTrade>();
            foreach (var jtoken in token)
            {
                var timeSpan = TimeSpan.Parse(jtoken[3].ToStringInvariant());
                var dateTime = DateTime.Today;
                dateTime = dateTime.Add(timeSpan);
                var universalTime = dateTime.ToUniversalTime();
                var exchangeTrade = new ExchangeTrade
                {
                    Id = jtoken[0].ConvertInvariant(0L),
                    Price = jtoken[1].ConvertInvariant(new decimal()),
                    Amount = jtoken[2].ConvertInvariant(new decimal()),
                    Timestamp = universalTime,
                    IsBuy = jtoken[4].ToStringInvariant().EqualsWithOption("bid", StringComparison.OrdinalIgnoreCase)
                };
                exchangeTradeList.Add(exchangeTrade);
            }

            return exchangeTradeList;
        }
    }
}