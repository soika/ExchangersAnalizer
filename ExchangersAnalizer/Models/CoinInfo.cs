// -----------------------------------------------------------------------
// <copyright file="CoinInfo.cs" company="SóiKA Apps">
//      All rights are reserved. Reproduction or transmission in whole or
//      in part, in any form or by any means, electronic, mechanical or
//      otherwise, is prohibited without the prior written consent of the 
//      copyright owner.
// </copyright>
// <summary>
//      Definition of the CoinInfo.cs class.
// </summary>
// -----------------------------------------------------------------------

namespace ExchangersAnalizer.Models
{
    using System.Collections.Generic;
    using Enums;

    public class CoinInfo
    {
        public IList<ExchangePrice> ExchangePrices { get; set; }

        public BaseCurrencyEnum Currency { get; set; }

        public ExchangeSymbol ExchangeSymbol { get; set; }
    }
}