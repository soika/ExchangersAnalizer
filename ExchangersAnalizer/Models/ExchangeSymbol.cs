// -----------------------------------------------------------------------
// <copyright file="ExchangeSymbol.cs" company="SóiKA Apps">
//      All rights are reserved. Reproduction or transmission in whole or
//      in part, in any form or by any means, electronic, mechanical or
//      otherwise, is prohibited without the prior written consent of the 
//      copyright owner.
// </copyright>
// <summary>
//      Definition of the ExchangeSymbol.cs class.
// </summary>
// -----------------------------------------------------------------------

namespace ExchangersAnalizer.Models
{
    public class ExchangeSymbol
    {
        public string GlobalSymbol { get; set; }

        public string Binance { get; set; }

        public string Okex { get; set; }

        public string HitBtc { get; set; }

        public string Bittrex { get; set; }

        public string Bithumb { get; set; }
    }
}