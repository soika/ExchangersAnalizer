// -----------------------------------------------------------------------
// <copyright file="TelegramBotTask.cs" company="SóiKA Apps">
//      All rights are reserved. Reproduction or transmission in whole or
//      in part, in any form or by any means, electronic, mechanical or
//      otherwise, is prohibited without the prior written consent of the 
//      copyright owner.
// </copyright>
// <summary>
//      Definition of the TelegramBotTask.cs class.
// </summary>
// -----------------------------------------------------------------------

namespace ExchangersAnalizer.CronJobs.Tasks
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Enums;
    using Extensions;
    using Services;

    public class TelegramBotTask : IScheduledTask
    {
        private readonly ICoinInfoService _coinInfoService;
        private readonly ITelegramBotService _telegramBotService;

        public TelegramBotTask(ITelegramBotService telegramBotService, ICoinInfoService coinInfoService)
        {
            _telegramBotService = telegramBotService;
            _coinInfoService = coinInfoService;
        }

        /// <inheritdoc />
        public string Schedule => "*/10 * * * *";

        /// <inheritdoc />
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var greaterPriceCoins = (await _coinInfoService.GetExchangerCoinInfoAsync()).ToList();
            var lessPriceCoins = greaterPriceCoins.ToPriceOrderByDescendingList();

            var msg1 = greaterPriceCoins.Take(10).ToList().ToTelegramMessage();
            var msg2 = lessPriceCoins.Take(10).ToList().ToTelegramMessage(ListBaseEnum.LessThanBinance);

            await _telegramBotService.SendGroupMessagesAsync(msg1, msg2);
        }
    }
}