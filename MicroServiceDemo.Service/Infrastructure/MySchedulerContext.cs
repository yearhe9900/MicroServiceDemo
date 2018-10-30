using MicroServiceDemo.Core.QuartzNet.Center;
using MicroServiceDemo.Core.QuartzNet.Entity;
using MicroServiceDemo.Core.QuartzNet.Manager;
using MicroServiceDemo.Service.Jobs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroServiceDemo.Service.Infrastructure
{
    public class MySchedulerContext
    {
        public static void Initialize()
        {
            var scheduleEntity = new ScheduleEntity()
            {
                JobId = 1,
                JobGroup = "calender01",
                JobName = "calender01",
                Cron = "0/10 * * * * ? ",
                BeginTime = DateTime.Now,
                EndTime = DateTime.MaxValue,
            };

            scheduleEntity.Agrs = new Dictionary<string, object> { { "orderId", 1 } };

            List<ScheduleEntity> scheduleEntities = new List<ScheduleEntity>() { scheduleEntity };

            ScheduleManager.Instance.AddScheduleList(scheduleEntities);

            SchedulerCenter.Instance.RunScheduleJob<ScheduleManager, CalendarJob>(scheduleEntity.JobGroup, scheduleEntity.JobName).GetAwaiter();
        }
    }
}
