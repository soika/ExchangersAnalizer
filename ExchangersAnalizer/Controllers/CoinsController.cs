// -----------------------------------------------------------------------
// <copyright file="CoinsController.cs" company="SóiKA Apps">
//      All rights are reserved. Reproduction or transmission in whole or
//      in part, in any form or by any means, electronic, mechanical or
//      otherwise, is prohibited without the prior written consent of the 
//      copyright owner.
// </copyright>
// <summary>
//      Definition of the CoinsController.cs class.
// </summary>
// -----------------------------------------------------------------------

namespace ExchangersAnalizer.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services;

    [Route("api/[controller]")]
    public class CoinsController : Controller
    {
        private readonly ICoinInfoService _coinInfoService;

        public CoinsController(ICoinInfoService coinInfoService)
        {
            _coinInfoService = coinInfoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCoins()
        {
            var coins = await _coinInfoService.GetExchangerCoinInfoAsync();
            return Ok(coins.Select(coin => coin.ToResponse()));
        }

        [HttpGet("symbols")]
        public async Task<IActionResult> GetSymbols()
        {
            var symbols = await _coinInfoService.GetExchangeSymbols();
            return Ok(symbols);
        }
    }
}