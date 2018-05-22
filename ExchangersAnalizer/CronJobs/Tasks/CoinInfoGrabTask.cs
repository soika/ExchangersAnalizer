﻿// -----------------------------------------------------------------------
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
    using Services;

    public class CoinInfoGrabTask : IScheduledTask
    {
        private readonly ICoinInfoService _coinInfoService;

        public CoinInfoGrabTask(ICoinInfoService coinInfoService)
        {
            this._coinInfoService = coinInfoService;
        }

        /// <inheritdoc />
        public string Schedule => "*/5 * * * *";

        /// <inheritdoc />
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await _coinInfoService.ForceUpdateCoinInfoAsync(); 
        }
    }
}