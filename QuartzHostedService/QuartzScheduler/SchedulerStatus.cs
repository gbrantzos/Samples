using System;
using System.Collections.Generic;

namespace QuartzHostedService.QuartzScheduler
{
    public class SchedulerStatus
    {
        public bool Active { get; set; }
        public List<SchedulerJobStatus> Jobs { get; set; }
    }

    public class SchedulerJobStatus
    {
        public string Name { get; set; }
        public bool Active { get; set; }
        public string CronExpression { get; set; }
        public string CronExpressionDescription { get; set; }
        public string Jobtype { get; set; }
        public DateTime? PreviousFireTime { get; set; }
        public DateTime? NextFireTime { get; set; }
    }

}
