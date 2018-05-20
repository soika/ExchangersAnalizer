// -----------------------------------------------------------------------
// <copyright file="Symbol.cs" company="SóiKA Apps">
//      All rights are reserved. Reproduction or transmission in whole or
//      in part, in any form or by any means, electronic, mechanical or
//      otherwise, is prohibited without the prior written consent of the 
//      copyright owner.
// </copyright>
// <summary>
//      Definition of the Symbol.cs class.
// </summary>
// -----------------------------------------------------------------------

namespace ExchangersAnalizer.Entities
{
    public class Symbol
    {
        public int Id { get; set; }

        public string GlobalSymbol { get; set; }

        public string Binance { get; set; }

        public string Bittrex { get; set; }

        public string HitBtc { get; set; }

        public string KuCoin { get; set; }

        public string Yobit { get; set; }

        public string Cryptopia { get; set; }
    }
}