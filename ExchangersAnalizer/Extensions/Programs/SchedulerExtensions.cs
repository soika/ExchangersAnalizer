// -----------------------------------------------------------------------
// <copyright file="SchedulerExtensions.cs" company="SóiKA Apps">
//      All rights are reserved. Reproduction or transmission in whole or
//      in part, in any form or by any means, electronic, mechanical or
//      otherwise, is prohibited without the prior written consent of the 
//      copyright owner.
// </copyright>
// <summary>
//      Definition of the SchedulerExtensions.cs class.
// </summary>
// -----------------------------------------------------------------------

namespace ExchangersAnalizer.Extensions.Programs
{
    using System;
    using System.Threading.Tasks;
    using CronJobs;
    using CronJobs.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public static class SchedulerExtensions
    {
        public static IServiceCollection AddScheduler(this IServiceCollection services)
        {
            return services.AddSingleton<IHostedService, SchedulerHostedService>();
        }

        public static IServiceCollection AddScheduler(
            this IServiceCollection services,
            EventHandler<UnobservedTaskExceptionEventArgs> unobservedTaskExceptionHandler)
        {
            return services.AddSingleton<IHostedService, SchedulerHostedService>(
                serviceProvider =>
                {
                    var instance = new SchedulerHostedService(serviceProvider.GetServices<IScheduledTask>());
                    instance.UnobservedTaskException += unobservedTaskExceptionHandler;
                    return instance;
                });
        }
    }
}