﻿// -----------------------------------------------------------------------
// <copyright file="SymbolHelper.cs" company="SóiKA Apps">
//      All rights are reserved. Reproduction or transmission in whole or
//      in part, in any form or by any means, electronic, mechanical or
//      otherwise, is prohibited without the prior written consent of the 
//      copyright owner.
// </copyright>
// <summary>
//      Definition of the SymbolHelper.cs class.
// </summary>
// -----------------------------------------------------------------------

namespace ExchangersAnalizer.Extensions
{
    using System;
    using Enums;

    public static class SymbolHelper
    {
        public static string ToGlobalSymbol(string symbol, ExchangerEnum exchanger)
        {
            switch (exchanger)
            {
                default:
                {
                    return symbol;
                }

                case ExchangerEnum.Binance:
                {
                    return symbol.Replace("-", string.Empty);
                }

                case ExchangerEnum.Bittrex:
                {
                    // BTC-LTC => LTCBTC
                    var items = symbol.Split(new[] {'-'}, StringSplitOptions.RemoveEmptyEntries);
                    return $"{items[1]}{items[0]}";
                }

                case ExchangerEnum.Okex:
                {
                    // btc_ltc => BTCLTC
                    return symbol.Replace("_", string.Empty).ToUpper();
                }

                case ExchangerEnum.HitBtc:
                case ExchangerEnum.Bithumb:
                {
                    return symbol;
                }

                case ExchangerEnum.KuCoin:
                {
                    return symbol.Replace("-", string.Empty);
                }

                case ExchangerEnum.Cryptopia:
                {
                    return symbol.Replace("/", string.Empty);
                }

                case ExchangerEnum.Yobit:
                {
                    return symbol.Replace("_", string.Empty).ToUpper();
                }

                case ExchangerEnum.Gate:
                {
                    // eth_btc => BTCETH
                    var items = symbol.Split(new[] {'_'}, StringSplitOptions.RemoveEmptyEntries);
                    return $"{items[0].ToUpper()}{items[1].ToUpper()}";
                }

                case ExchangerEnum.Huobi:
                {
                    return $"{symbol.Replace("btc", string.Empty).ToUpper()}BTC";
                }

                case ExchangerEnum.Upbit:
                {
                    var items = symbol.Split(new[] {'-'}, StringSplitOptions.RemoveEmptyEntries);
                    return $"{items[1].ToUpper()}{items[0].ToUpper()}";
                }
            }
        }
    }
}