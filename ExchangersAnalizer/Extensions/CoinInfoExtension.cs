// -----------------------------------------------------------------------
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
    using Enums;
    using ExchangeSharp;
    using Models;

    public static class CoinInfoExtension
    {
        public static List<CoinInfo> FillExchangePrice(
            this List<CoinInfo> coins,
            ExchangerEnum exchanger,
            List<KeyValuePair<string, ExchangeTicker>> marketTickers)
        {
            foreach (var coin in coins)
            {
                var ticker = marketTickers.FirstOrDefault(
                    t => t.Key.Equals(coin.ExchangeSymbol.Binance, StringComparison.OrdinalIgnoreCase)
                         || t.Key.Equals(coin.ExchangeSymbol.Bittrex, StringComparison.OrdinalIgnoreCase)
                         || t.Key.Equals(coin.ExchangeSymbol.HitBtc, StringComparison.OrdinalIgnoreCase)
                         || t.Key.Equals(coin.ExchangeSymbol.KuCoin, StringComparison.OrdinalIgnoreCase));
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
                        lastPrice = x.LastPrice
                    })
            };
        }
    }
}