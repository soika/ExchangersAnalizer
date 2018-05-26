// -----------------------------------------------------------------------
// <copyright file="TelegramBotService.cs" company="SóiKA Apps">
//      All rights are reserved. Reproduction or transmission in whole or
//      in part, in any form or by any means, electronic, mechanical or
//      otherwise, is prohibited without the prior written consent of the 
//      copyright owner.
// </copyright>
// <summary>
//      Definition of the TelegramBotService.cs class.
// </summary>
// -----------------------------------------------------------------------

namespace ExchangersAnalizer.Services
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using Models;
    using Settings;
    using Telegram.Bot;

    public class TelegramBotService : ITelegramBotService
    {
        private readonly IOptions<TelegramBotSettings> _telegramBotSettings;

        public TelegramBotService(IOptions<TelegramBotSettings> telegramBotSettings)
        {
            _telegramBotSettings = telegramBotSettings;
        }

        /// <inheritdoc />
        public async Task SendGroupMessagesAsync(params GroupMessage[] messages)
        {
            var botClient = new TelegramBotClient(_telegramBotSettings.Value.AccessApiToken);
            foreach (var message in messages)
            {
                await botClient.SendTextMessageAsync(_telegramBotSettings.Value.GroupIds[1], message.Content, message.ParseMode);
            }
        }
    }
}