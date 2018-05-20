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
    using Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Models;
    using Services;
    using Settings;

    [Route("api/[controller]")]
    public class CoinsController : Controller
    {
        private readonly ICoinInfoService _coinInfoService;
        private readonly ITelegramBotService _telegramBotService;

        public CoinsController(
            ICoinInfoService coinInfoService,
            ITelegramBotService telegramBotService)
        {
            _coinInfoService = coinInfoService;
            _telegramBotService = telegramBotService;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllCoins()
        {
            var coins = await _coinInfoService.GetExchangerCoinInfoAsync();
            foreach (var coin in coins)
            {
                await _telegramBotService.SendGroupMessagesAsync(
                    new GroupMessage
                    {
                        GroupId = "-1001277886153",
                        Content = $"Coin {coin.ExchangeSymbol.GlobalSymbol}: \n" +
                                  $"{coin.ExchangePrices.ElementAt(1).Exchanger} {coin.ExchangePrices.ElementAt(1).Percent}%"
                    });
            }

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