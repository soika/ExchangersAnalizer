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
    using Microsoft.Extensions.Options;
    using Services;
    using Settings;

    public class TelegramBotTask : IScheduledTask
    {
        private readonly ICoinInfoService _coinInfoService;
        private readonly ITelegramBotService _telegramBotService;
        private readonly IOptions<SiteSettings> _options;

        public TelegramBotTask(
            ITelegramBotService telegramBotService,
            ICoinInfoService coinInfoService,
            IOptions<SiteSettings> options)
        {
            _telegramBotService = telegramBotService;
            _coinInfoService = coinInfoService;
            _options = options;
        }

        /// <inheritdoc />
        public string Schedule => $"*/{_options.Value.SendMessageInMinutes} * * * *";

        /// <inheritdoc />
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var greaterPriceCoins = (await _coinInfoService.GetExchangerCoinInfoAsync()).ToList();
            var msg1 = greaterPriceCoins.Take(_options.Value.NumberOfCoinsToSend).ToList().ToTelegramMessage();
            await _telegramBotService.SendGroupMessagesAsync(msg1);
        }
    }
}