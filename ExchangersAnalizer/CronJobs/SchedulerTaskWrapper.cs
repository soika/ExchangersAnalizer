// -----------------------------------------------------------------------
// <copyright file="SchedulerTaskWrapper.cs" company="SóiKA Apps">
//      All rights are reserved. Reproduction or transmission in whole or
//      in part, in any form or by any means, electronic, mechanical or
//      otherwise, is prohibited without the prior written consent of the 
//      copyright owner.
// </copyright>
// <summary>
//      Definition of the SchedulerTaskWrapper.cs class.
// </summary>
// -----------------------------------------------------------------------

namespace ExchangersAnalizer.CronJobs
{
    using System;
    using Crons;
    using Tasks;

    public class SchedulerTaskWrapper
    {
        public CrontabSchedule Schedule { get; set; }

        public IScheduledTask Task { get; set; }

        public DateTime LastRunTime { get; set; }

        public DateTime NextRunTime { get; set; }

        public void Increment()
        {
            LastRunTime = NextRunTime;
            NextRunTime = Schedule.GetNextOccurrence(NextRunTime);
        }

        public bool ShouldRun(DateTime currentTime)
        {
            return NextRunTime < currentTime && LastRunTime != NextRunTime;
        }
    }
}