// -----------------------------------------------------------------------
// <copyright file="ITelegramBotService.cs" company="SóiKA Apps">
//      All rights are reserved. Reproduction or transmission in whole or
//      in part, in any form or by any means, electronic, mechanical or
//      otherwise, is prohibited without the prior written consent of the 
//      copyright owner.
// </copyright>
// <summary>
//      Definition of the ITelegramBotService.cs class.
// </summary>
// -----------------------------------------------------------------------

namespace ExchangersAnalizer.Services
{
    using System.Threading.Tasks;
    using Models;
    using Telegram.Bot;

    public interface ITelegramBotService
    {
        Task SendGroupMessagesAsync(params GroupMessage[] messages);
    }
}