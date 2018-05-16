// -----------------------------------------------------------------------
// <copyright file="ExchangePrice.cs" company="SóiKA Apps">
//      All rights are reserved. Reproduction or transmission in whole or
//      in part, in any form or by any means, electronic, mechanical or
//      otherwise, is prohibited without the prior written consent of the 
//      copyright owner.
// </copyright>
// <summary>
//      Definition of the ExchangePrice.cs class.
// </summary>
// -----------------------------------------------------------------------

namespace ExchangersAnalizer.Models
{
    using Enums;

    public class ExchangePrice
    {
        public ExchangerEnum Exchanger { get; set; }

        public decimal LastPrice { get; set; }

        public decimal Percent { get; set; }
    }
}