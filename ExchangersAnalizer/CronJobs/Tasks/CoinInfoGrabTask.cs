// -----------------------------------------------------------------------
// <copyright file="CoinInfoGrabTask.cs" company="SóiKA Apps">
//      All rights are reserved. Reproduction or transmission in whole or
//      in part, in any form or by any means, electronic, mechanical or
//      otherwise, is prohibited without the prior written consent of the 
//      copyright owner.
// </copyright>
// <summary>
//      Definition of the CoinInfoGrabTask.cs class.
// </summary>
// -----------------------------------------------------------------------

namespace ExchangersAnalizer.CronJobs.Tasks
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using Services;
    using Settings;

    public class CoinInfoGrabTask : IScheduledTask
    {
        private readonly ICoinInfoService _coinInfoService;
        private readonly IOptions<SiteSettings> _options;

        public CoinInfoGrabTask(
            ICoinInfoService coinInfoService,
            IOptions<SiteSettings> options)
        {
            this._coinInfoService = coinInfoService;
            _options = options;
        }

        /// <inheritdoc />
        public string Schedule => $"*/{_options.Value.RefreshInMinutes} * * * *";

        /// <inheritdoc />
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await _coinInfoService.ForceUpdateCoinInfoAsync(); 
        }
    }
}