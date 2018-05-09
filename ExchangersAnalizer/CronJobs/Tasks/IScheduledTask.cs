// -----------------------------------------------------------------------
// <copyright file="IScheduledTask.cs" company="SóiKA Apps">
//      All rights are reserved. Reproduction or transmission in whole or
//      in part, in any form or by any means, electronic, mechanical or
//      otherwise, is prohibited without the prior written consent of the 
//      copyright owner.
// </copyright>
// <summary>
//      Definition of the IScheduledTask.cs class.
// </summary>
// -----------------------------------------------------------------------

namespace ExchangersAnalizer.CronJobs.Tasks
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IScheduledTask
    {
        string Schedule { get; }

        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}