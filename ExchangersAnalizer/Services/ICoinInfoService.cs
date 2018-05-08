﻿// -----------------------------------------------------------------------
// <copyright file="ICoinInfoService.cs" company="SóiKA Apps">
//      All rights are reserved. Reproduction or transmission in whole or
//      in part, in any form or by any means, electronic, mechanical or
//      otherwise, is prohibited without the prior written consent of the 
//      copyright owner.
// </copyright>
// <summary>
//      Definition of the ICoinInfoService.cs class.
// </summary>
// -----------------------------------------------------------------------

namespace ExchangersAnalizer.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface ICoinInfoService
    {
        Task<IEnumerable<CoinInfo>> GetExchangerCoinInfoAsync();

        Task<IEnumerable<ExchangeSymbol>> GetExchangeSymbols();
    }
}