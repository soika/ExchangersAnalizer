// -----------------------------------------------------------------------
// <copyright file="ProgramExtension.cs" company="SóiKA Apps">
//      All rights are reserved. Reproduction or transmission in whole or
//      in part, in any form or by any means, electronic, mechanical or
//      otherwise, is prohibited without the prior written consent of the 
//      copyright owner.
// </copyright>
// <summary>
//      Definition of the ProgramExtension.cs class.
// </summary>
// -----------------------------------------------------------------------

namespace ExchangersAnalizer.Extensions.Programs
{
    using Data;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Services;
    using Settings;

    public static class ProgramExtension
    {
        public static IWebHost InitializeData(this IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var context = serviceProvider.GetService<ApplicationDbContext>();
                var siteSettings = serviceProvider.GetService<IOptions<SiteSettings>>();
                var coinService = serviceProvider.GetService<ICoinInfoService>();

                DbInitializer.InitSiteConfigurations(context, siteSettings).Wait();
                DbInitializer.InitSymbols(context, coinService).Wait();
            }

            return host;
        }
    }
}