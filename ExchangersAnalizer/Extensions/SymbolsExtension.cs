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

    public static class SymbolsExtension
    {
        public static IEnumerable<string> ToGlobalSymbols(this IEnumerable<string> symbols)
        {
            return symbols.Select(symbol => symbol.Replace("-", string.Empty)).ToList();
        }
    }
}