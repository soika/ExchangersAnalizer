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
    public class CoinInfo
    {
        public string Symbol { get; set; }

        public string Name { get; set; }

        public string Exchanger { get; set; }

        public double AskPrice { get; set; }

        public double BidPrice { get; set; }

        public double AskPercent { get; set; }

        public double BidPercent { get; set; }

        public bool DefaultExchanger { get; set; }
    }
}