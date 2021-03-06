﻿// -----------------------------------------------------------------------
// <copyright file="CoinInfoExtension.cs" company="SóiKA Apps">
//      All rights are reserved. Reproduction or transmission in whole or
//      in part, in any form or by any means, electronic, mechanical or
//      otherwise, is prohibited without the prior written consent of the 
//      copyright owner.
// </copyright>
// <summary>
//      Definition of the CoinInfoExtension.cs class.
// </summary>
// -----------------------------------------------------------------------

namespace ExchangersAnalizer.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Enums;
    using ExchangeSharp;
    using Models;
    using Telegram.Bot.Types.Enums;

    public static class CoinInfoExtension
    {
        public static List<CoinInfo> FillCoinPrices(
            this List<CoinInfo> coins,
            ExchangerEnum exchanger,
            List<KeyValuePair<string, ExchangeTicker>> marketTickers)
        {
            if (marketTickers == null || !marketTickers.Any())
            {
                return coins;
            }

            foreach (var coin in coins)
            {
                var ticker = marketTickers.FirstOrDefault(
                    t => t.Key.Equals(coin.ExchangeSymbol.Binance, StringComparison.OrdinalIgnoreCase)
                         || t.Key.Equals(coin.ExchangeSymbol.Bittrex, StringComparison.OrdinalIgnoreCase)
                         || t.Key.Equals(coin.ExchangeSymbol.HitBtc, StringComparison.OrdinalIgnoreCase)
                         || t.Key.Equals(coin.ExchangeSymbol.KuCoin, StringComparison.OrdinalIgnoreCase)
                         || t.Key.Equals(coin.ExchangeSymbol.Cryptopia, StringComparison.OrdinalIgnoreCase)
                         || t.Key.Equals(coin.ExchangeSymbol.Okex, StringComparison.OrdinalIgnoreCase)
                         || t.Key.Equals(coin.ExchangeSymbol.Gate, StringComparison.OrdinalIgnoreCase)
                         || t.Key.Equals(coin.ExchangeSymbol.Huobi, StringComparison.OrdinalIgnoreCase)
                         || t.Key.Equals(coin.ExchangeSymbol.Upbit, StringComparison.OrdinalIgnoreCase)
                         || t.Key.Equals(coin.ExchangeSymbol.Yobit, StringComparison.OrdinalIgnoreCase));

                if (ticker.Key == null)
                {
                    continue;
                }

                switch (exchanger)
                {
                    case ExchangerEnum.Binance:
                    {
                        coin.ExchangePrices.Add(
                            new ExchangePrice
                            {
                                Exchanger = ExchangerEnum.Binance,
                                LastPrice = ticker.Value.Last
                            });

                        break;
                    }

                    case ExchangerEnum.Bittrex:
                    {
                        coin.ExchangePrices.Add(
                            new ExchangePrice
                            {
                                Exchanger = ExchangerEnum.Bittrex,
                                LastPrice = ticker.Value.Last
                            });

                        break;
                    }

                    case ExchangerEnum.HitBtc:
                    {
                        coin.ExchangePrices.Add(
                            new ExchangePrice
                            {
                                Exchanger = ExchangerEnum.HitBtc,
                                LastPrice = ticker.Value.Last
                            });

                        break;
                    }

                    case ExchangerEnum.KuCoin:
                    {
                        coin.ExchangePrices.Add(
                            new ExchangePrice
                            {
                                Exchanger = ExchangerEnum.KuCoin,
                                LastPrice = ticker.Value.Last
                            });

                        break;
                    }

                    case ExchangerEnum.Cryptopia:
                    {
                        coin.ExchangePrices.Add(
                            new ExchangePrice
                            {
                                Exchanger = ExchangerEnum.Cryptopia,
                                LastPrice = ticker.Value.Last
                            });

                        break;
                    }

                    case ExchangerEnum.Yobit:
                    {
                        coin.ExchangePrices.Add(
                            new ExchangePrice
                            {
                                Exchanger = ExchangerEnum.Yobit,
                                LastPrice = ticker.Value.Last
                            });

                        break;
                    }

                    case ExchangerEnum.Okex:
                    {
                        coin.ExchangePrices.Add(
                            new ExchangePrice
                            {
                                Exchanger = ExchangerEnum.Okex,
                                LastPrice = ticker.Value.Last
                            });

                        break;
                    }

                    case ExchangerEnum.Gate:
                    {
                        coin.ExchangePrices.Add(
                            new ExchangePrice
                            {
                                Exchanger = ExchangerEnum.Gate,
                                LastPrice = ticker.Value.Last
                            });

                        break;
                    }

                    case ExchangerEnum.Huobi:
                    {
                        coin.ExchangePrices.Add(
                            new ExchangePrice
                            {
                                Exchanger = ExchangerEnum.Huobi,
                                LastPrice = ticker.Value.Last
                            });

                        break;
                    }

                    case ExchangerEnum.Upbit:
                    {
                        coin.ExchangePrices.Add(
                            new ExchangePrice
                            {
                                Exchanger = ExchangerEnum.Upbit,
                                LastPrice = ticker.Value.Last
                            });

                        break;
                    }
                }
            }

            return coins;
        }

        public static List<CoinInfo> ToAnalizedExchangePrice(
            this List<CoinInfo> coins)
        {
            coins.RemoveAll(x => x.ExchangePrices.All(p => p.LastPrice == 0));
            foreach (var coin in coins)
            {
                var items = coin.ExchangePrices.Where(p => p.LastPrice > 0);
                if (!items.Any())
                {
                   continue;
                }

                var lowestPrice = coin.ExchangePrices.Where(p => p.LastPrice > 0).Min(c => c.LastPrice);

                var compareToExchanger = ExchangerEnum.Binance;

                foreach (var coinExchangePrice in coin.ExchangePrices)
                {
                    coinExchangePrice.Percent = (coinExchangePrice.LastPrice - lowestPrice) / lowestPrice * 100;
                    if (coinExchangePrice.Percent == 0)
                    {
                        compareToExchanger = coinExchangePrice.Exchanger;
                    }
                }

                foreach (var coinExchangePrice in coin.ExchangePrices)
                {
                    coinExchangePrice.CompareToExchanger = compareToExchanger;
                }
            }

            return coins;
        }

        public static object ToResponse(this CoinInfo info)
        {
            return new
            {
                symbol = info.ExchangeSymbol.GlobalSymbol,
                currency = info.Currency.ToString(),
                prices = info.ExchangePrices.Select(
                    x => new
                    {
                        exchanger = x.Exchanger.ToString(),
                        lastPrice = x.LastPrice,
                        percent = x.Percent
                    })
            };
        }

        public static GroupMessage ToTelegramMessage(
            this List<CoinInfo> coins,
            ListBaseEnum listBase = ListBaseEnum.GreaterThanBinance)
        {
            var message = new GroupMessage
            {
                ParseMode = ParseMode.Html
            };
            var strBuilder = new StringBuilder("<strong>TOP COIN PRICES COMPARE</strong>").Append("\n\n");

            foreach (var coin in coins)
            {
                strBuilder.Append($"<strong>{coin.ExchangeSymbol.GlobalSymbol}:</strong> ").Append("\n");

                foreach (var price in coin.ExchangePrices)
                {
                    if (price.Percent == 0)
                    {
                        strBuilder.Append(
                                $"{price.Exchanger}: {string.Format("{0:0.########}", price.LastPrice)} BTC.")
                            .Append("\n");
                        continue;
                    }

                    var priceBold = price.Percent >= 4
                        ? $"<strong>{string.Format("{0:0.###}", price.Percent)}</strong>"
                        : $"{string.Format("{0:0.###}", price.Percent)}";
                    strBuilder.Append(
                            $"{price.Exchanger}: {string.Format("{0:0.########}", price.LastPrice)} BTC " +
                            $"({priceBold} % compare to <strong>{price.CompareToExchanger.ToString()}</strong>)")
                        .Append("\n");
                }

                strBuilder.Append("\n");
            }

            message.Content = strBuilder.ToString();
            return message;
        }
    }
}