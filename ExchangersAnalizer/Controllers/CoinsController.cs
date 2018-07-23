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
    using System.Linq;
    using System.Threading.Tasks;
    using Exchangers;
    using Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Services;

    [Route("api/[controller]")]
    public class CoinsController : Controller
    {
        private readonly ICoinInfoService _coinInfoService;
        private readonly MinExchangeUpbitAPI _exchangeGateApi;

        public CoinsController(
            ICoinInfoService coinInfoService,
            MinExchangeUpbitAPI exchangeGateApi)
        {
            _coinInfoService = coinInfoService;
            _exchangeGateApi = exchangeGateApi;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllCoins()
        {
            var coins = (await _coinInfoService.GetExchangerCoinInfoAsync()).ToList();
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