// -----------------------------------------------------------------------
// <copyright file="DbInitializer.cs" company="SóiKA Apps">
//      All rights are reserved. Reproduction or transmission in whole or
//      in part, in any form or by any means, electronic, mechanical or
//      otherwise, is prohibited without the prior written consent of the 
//      copyright owner.
// </copyright>
// <summary>
//      Definition of the DbInitializer.cs class.
// </summary>
// -----------------------------------------------------------------------

namespace ExchangersAnalizer.Data
{
    using System.Linq;
    using System.Threading.Tasks;
    using Entities;
    using Microsoft.Extensions.Options;
    using Services;
    using Settings;

    public class DbInitializer
    {
        public static async Task InitSiteConfigurations(
            ApplicationDbContext context,
            IOptions<SiteSettings> siteSettings)
        {
            if (!context.Configs.Any())
            {
                var settings = siteSettings.Value;
                var config = new Config
                {
                    AllowIPs = settings.AllowIPs,
                    NumberOfCoinsToSend = settings.NumberOfCoinsToSend,
                    RefreshInMinutes = settings.RefreshInMinutes,
                    TelegramKey = settings.TelegramKey,
                    TelegramChatGroups = settings.AllowIPs
                };

                await context.Configs.AddAsync(config);
                await context.SaveChangesAsync();
            }
        }

        public static async Task InitSymbols(
            ApplicationDbContext context,
            ICoinInfoService coinInfoService)
        {
            if (!context.Symbols.Any())
            {
                var grabbedItems = await coinInfoService.GetExchangeSymbols();
                var symbols = grabbedItems.Select(
                    e => new Symbol
                    {
                        GlobalSymbol = e.GlobalSymbol,
                        Bittrex = e.Bittrex,
                        HitBtc = e.HitBtc,
                        Yobit = e.Yobit,
                        KuCoin = e.KuCoin,
                        Cryptopia = e.Cryptopia,
                        Binance = e.Binance
                    });

                await context.Symbols.AddRangeAsync(symbols);
                await context.SaveChangesAsync();
            }
        }
    }
}