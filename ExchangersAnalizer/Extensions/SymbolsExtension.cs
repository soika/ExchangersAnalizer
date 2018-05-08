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

    public static class SymbolsExtension
    {
        public static IEnumerable<string> ToGlobalSymbols(this IEnumerable<string> symbols)
        {
            return symbols.Select(symbol => symbol.Replace("-", string.Empty))
                .ToList();
        }

        public static string ToGlobalSymbol(this string symbol, ExchangerEnum exchanger)
        {
            switch (exchanger)
            {
                default:
                {
                    return symbol;
                }

                case ExchangerEnum.Binance:
                case ExchangerEnum.Bittrex:
                case ExchangerEnum.Bithumb:
                {
                    return symbol.Replace("-", string.Empty);
                }

                case ExchangerEnum.Okex:
                {
                    return symbol.Replace("_", string.Empty)
                        .ToUpper();
                }

                case ExchangerEnum.Hitbtc:
                {
                    return symbol.Replace("/", string.Empty);
                }
            }
        }
    }
}