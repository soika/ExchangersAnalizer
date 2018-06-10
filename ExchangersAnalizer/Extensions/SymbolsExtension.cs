// -----------------------------------------------------------------------
// <copyright file="SymbolsExtension.cs" company="SóiKA Apps">
//      All rights are reserved. Reproduction or transmission in whole or
//      in part, in any form or by any means, electronic, mechanical or
//      otherwise, is prohibited without the prior written consent of the 
//      copyright owner.
// </copyright>
// <summary>
//      Definition of the SymbolsExtension.cs class.
// </summary>
// -----------------------------------------------------------------------

namespace ExchangersAnalizer.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Enums;
    using Models;
    using Settings;

    public static class SymbolsExtension
    {
        public static List<ExchangeSymbol> FillExchangerSymbols(
            this List<ExchangeSymbol> globalSymbols,
            ExchangerEnum exchanger,
            List<string> symbols)
        {
            switch (exchanger)
            {
                default:
                {
                    return globalSymbols;
                }

                case ExchangerEnum.Binance:
                {
                    foreach (var globalSymbol in globalSymbols)
                    {
                        foreach (var symbol in symbols)
                        {
                            if (SymbolHelper.ToGlobalSymbol(symbol, ExchangerEnum.Binance)
                                .Equals(globalSymbol.GlobalSymbol))
                            {
                                globalSymbol.Binance = symbol;
                            }
                        }
                    }

                    return globalSymbols;
                }

                case ExchangerEnum.Bittrex:
                {
                    foreach (var globalSymbol in globalSymbols)
                    {
                        foreach (var symbol in symbols)
                        {
                            if (SymbolHelper.ToGlobalSymbol(symbol, ExchangerEnum.Bittrex)
                                .Equals(globalSymbol.GlobalSymbol))
                            {
                                globalSymbol.Bittrex = symbol;
                            }
                        }
                    }

                    return globalSymbols;
                }

                case ExchangerEnum.HitBtc:
                {
                    foreach (var globalSymbol in globalSymbols)
                    {
                        foreach (var symbol in symbols)
                        {
                            if (SymbolHelper.ToGlobalSymbol(symbol, ExchangerEnum.HitBtc)
                                .Equals(globalSymbol.GlobalSymbol))
                            {
                                globalSymbol.HitBtc = symbol;
                            }
                        }
                    }

                    return globalSymbols;
                }

                case ExchangerEnum.KuCoin:
                {
                    foreach (var globalSymbol in globalSymbols)
                    {
                        foreach (var symbol in symbols)
                        {
                            if (SymbolHelper.ToGlobalSymbol(symbol, ExchangerEnum.KuCoin)
                                .Equals(globalSymbol.GlobalSymbol))
                            {
                                globalSymbol.KuCoin = symbol;
                            }
                        }
                    }

                    return globalSymbols;
                }

                case ExchangerEnum.Cryptopia:
                {
                    foreach (var globalSymbol in globalSymbols)
                    {
                        foreach (var symbol in symbols)
                        {
                            if (SymbolHelper.ToGlobalSymbol(symbol, ExchangerEnum.Cryptopia)
                                .Equals(globalSymbol.GlobalSymbol))
                            {
                                globalSymbol.Cryptopia = symbol;
                            }
                        }
                    }

                    return globalSymbols;
                }

                case ExchangerEnum.Yobit:
                {
                    foreach (var globalSymbol in globalSymbols)
                    {
                        foreach (var symbol in symbols)
                        {
                            if (SymbolHelper.ToGlobalSymbol(symbol, ExchangerEnum.Yobit)
                                .Equals(globalSymbol.GlobalSymbol))
                            {
                                globalSymbol.Yobit = symbol;
                            }
                        }
                    }

                    return globalSymbols;
                }

                case ExchangerEnum.Okex:
                {
                    foreach (var globalSymbol in globalSymbols)
                    {
                        foreach (var symbol in symbols)
                        {
                            if (SymbolHelper.ToGlobalSymbol(symbol, ExchangerEnum.Okex)
                                .Equals(globalSymbol.GlobalSymbol))
                            {
                                globalSymbol.Okex = symbol;
                            }
                        }
                    }

                    return globalSymbols;
                }
            }
        }

        public static List<ExchangeSymbol> IgnoreSymbols(this List<ExchangeSymbol> symbols, string[] ignoreSymbols)
        {
            return symbols.Where(s => !ignoreSymbols.Contains(s.GlobalSymbol)).ToList();
        }

        public static List<ExchangeSymbol> FilterByBaseCurency(
            this List<ExchangeSymbol> globalSymbols,
            BaseCurrencyEnum currency = BaseCurrencyEnum.BTC)
        {
            return globalSymbols
                .Where(
                    s => !string.IsNullOrEmpty(s.Bittrex)
                         || !string.IsNullOrEmpty(s.HitBtc)
                         || !string.IsNullOrEmpty(s.KuCoin))
                .Where(s => s.GlobalSymbol.EndsWith(currency.ToString())).ToList();
        }

        public static List<string> FilterByBaseCurency(
            this List<string> symbols,
            BaseCurrencyEnum currency = BaseCurrencyEnum.BTC)
        {
            return symbols.Where(s => s.ToUpper().EndsWith(currency.ToString())).ToList();
        }

        public static List<string> FilterByIgnoreCoins(
            this List<string> symbols,
            IgnoreCoinSettings[] ignoreCoinSettings,
            ExchangerEnum exchanger)
        {
            var ignoreSymbol = ignoreCoinSettings.FirstOrDefault(ss => ss.Exchanger.Equals(exchanger.ToString()));
            if (ignoreSymbol == null)
            {
                return symbols;
            }

            symbols.RemoveAll(s => ignoreSymbol.Symbols.Contains(SymbolHelper.ToGlobalSymbol(s, exchanger)));
            return symbols;
        }
    }
}