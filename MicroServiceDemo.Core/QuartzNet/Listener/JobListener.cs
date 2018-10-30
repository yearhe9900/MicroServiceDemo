using MicroServiceDemo.Core.QuartzNet.Manager;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MicroServiceDemo.Core.QuartzNet.Listener
{
    public class JobListener : IJobListener
    {
        public string Name => "JobListener";
        public static int count = 0;

        //job开始执行之前调用
        public async Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
               await Console.Out.WriteLineAsync("job开始执行之前调用");
        }

        //job每次执行之后调用
        public async Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            await Console.Out.WriteLineAsync("job每次执行之后调用");
        }

        //job执行结束之后调用
        public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default(CancellationToken))
        {
            count++;
            var manage = new ScheduleManager();
            var model = manage.GetScheduleModel(context.JobDetail.Key.Group, context.JobDetail.Key.Name);
            await Console.Out.WriteLineAsync("job执行结束之后调用  " + count);
            if (model.RunTimes != 0 && count == model.RunTimes)
            {
                count = 0;
            }
        }
    }
}
