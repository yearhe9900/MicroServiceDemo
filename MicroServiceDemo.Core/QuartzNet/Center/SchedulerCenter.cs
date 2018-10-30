using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Quartz.Impl.Matchers;
using MicroServiceDemo.Core.QuartzNet.Entity;
using MicroServiceDemo.Core.QuartzNet.Manager;
using MicroServiceDemo.Core.Common.Helpers;
using MicroServiceDemo.Core.QuartzNet.Listener;

namespace MicroServiceDemo.Core.QuartzNet.Center
{
    /// <summary>
	/// 任务调度中心
	/// </summary>
	public class SchedulerCenter
    {
        #region  初始化

        /// <summary>
        /// 任务调度对象
        /// </summary>
        public static readonly SchedulerCenter Instance;
        static SchedulerCenter()
        {
            Instance = new SchedulerCenter();
        }
        private Task<IScheduler> _scheduler;

        /// <summary>
        /// 创建任务计划调度器
        /// </summary>
        /// <returns></returns>
        private Task<IScheduler> Scheduler
        {
            get
            {
                if (this._scheduler != null)
                {
                    return this._scheduler;
                }
                // 从Factory中获取Scheduler实例
                NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" },
                };
                StdSchedulerFactory factory = new StdSchedulerFactory(props);
                return this._scheduler = factory.GetScheduler();
            }
        }

        #endregion

        #region 运行计划

        /// <summary>
        /// 运行指定的计划(泛型指定IJob实现类)
        /// </summary>
        /// <param name="jobGroup">任务分组</param>
        /// <param name="jobName">任务名称</param>
        /// <returns></returns>
        public async Task<BaseQuartzNetResult> RunScheduleJob<T, V>(string jobGroup, string jobName) where T : ScheduleManager, new() where V : IJob
        {
            BaseQuartzNetResult result;
            //开启调度器
            await this.Scheduler.Result.Start();
            //创建指定泛型类型参数指定的类型实例
            T t = Activator.CreateInstance<T>();
            //获取任务实例
            ScheduleEntity scheduleModel = t.GetScheduleModel(jobGroup, jobName);
            //添加任务
            var addResult = AddScheduleJob<V>(scheduleModel).Result;
            if (addResult.Code == 1000)
            {
                scheduleModel.Status = EnumType.JobStatus.Activated;//已启用
                t.UpdateScheduleStatus(scheduleModel);
                //用给定的密钥恢复（取消暂停）IJobDetail
                await this.Scheduler.Result.ResumeJob(new JobKey(jobName, jobGroup));
                result = new BaseQuartzNetResult
                {
                    Code = 1000,
                    Msg = "启动成功"
                };
            }
            else
            {
                result = new BaseQuartzNetResult
                {
                    Code = -1,
                    Msg = addResult.Msg
                };
            }
            return result;
        }

        /// <summary>
        /// 运行指定的计划(映射处理IJob实现类)
        /// </summary>
        /// <param name="jobGroup">任务分组</param>
        /// <param name="jobName">任务名称</param>
        /// <returns></returns>
        public async Task<BaseQuartzNetResult> RunScheduleJob<T>(string jobGroup, string jobName) where T : ScheduleManager
        {
            BaseQuartzNetResult result;
            //开启调度器
            await this.Scheduler.Result.Start();
            //创建指定泛型类型参数指定的类型实例
            T t = Activator.CreateInstance<T>();
            //获取任务实例
            ScheduleEntity scheduleModel = t.GetScheduleModel(jobGroup, jobName);
            //添加任务
            var addResult = AddScheduleJob(scheduleModel).Result;
            if (addResult.Code == 1000)
            {
                scheduleModel.Status = EnumType.JobStatus.Activated;
                t.UpdateScheduleStatus(scheduleModel);
                //用给定的密钥恢复（取消暂停）IJobDetail
                await this.Scheduler.Result.ResumeJob(new JobKey(jobName, jobGroup));
                result = new BaseQuartzNetResult
                {
                    Code = 1000,
                    Msg = "启动成功"
                };
            }
            else
            {
                result = new BaseQuartzNetResult
                {
                    Code = -1
                };
            }
            return result;
        }

        #endregion

        #region 暂停或恢复计划

        /// <summary>
        /// 暂停指定的计划
        /// </summary>
        /// <param name="jobGroup">任务分组</param>
        /// <param name="jobName">任务名称</param>
        /// <param name="isDelete">停止并删除任务</param>
        /// <returns></returns>
        public async Task<BaseQuartzNetResult> StopScheduleJob<T>(string jobGroup, string jobName, bool isDelete = false) where T : ScheduleManager, new()
        {
            var result = new BaseQuartzNetResult();
            try
            {
                //检查任务是否存在
                var jk = new JobKey(jobName, jobGroup);
                if (!await Scheduler.Result.CheckExists(jk))
                {
                    return new BaseQuartzNetResult
                    {
                        Code = -1,
                        Msg = jobName + "任务未运行"
                    };
                }
                await this.Scheduler.Result.PauseJob(jk);
                if (isDelete)
                {
                    Activator.CreateInstance<T>().RemoveScheduleModel(jobGroup, jobName);
                }
                result = new BaseQuartzNetResult
                {
                    Code = 1000,
                    Msg = "停止任务成功！"
                };
            }
            catch (Exception ex)
            {
                result = new BaseQuartzNetResult
                {
                    Code = -1,
                    Msg = "停止任务失败"
                };
            }
            return result;
        }

        /// <summary>
        /// 恢复运行暂停的任务
        /// </summary>
        /// <param name="jobName">任务名称</param>
        /// <param name="jobGroup">任务分组</param>
        public async Task<BaseQuartzNetResult> ResumeJob(string jobName, string jobGroup)
        {
            var result = new BaseQuartzNetResult();
            try
            {
                //检查任务是否存在
                var jk = new JobKey(jobName, jobGroup);
                if (!await Scheduler.Result.CheckExists(jk))
                {
                    return new BaseQuartzNetResult
                    {
                        Code = -1,
                        Msg = jobName + "任务未运行"
                    };
                }
                //任务已经存在则恢复运行暂停任务
                await this.Scheduler.Result.ResumeJob(jk);
                result = new BaseQuartzNetResult
                {
                    Code = 1000,
                    Msg = "恢复运行任务成功！"
                };
            }
            catch (Exception ex)
            {
                result = new BaseQuartzNetResult
                {
                    Code = -1,
                    Msg = "恢复运行任务失败"
                };
            }
            return result;
        }

        #endregion

        #region 停止计划

        /// <summary>
        /// 停止任务调度
        /// </summary>
        public async void StopScheduleAsync()
        {
            try
            {
                //判断调度是否已经关闭
                if (!this.Scheduler.Result.IsShutdown)
                {
                    //等待任务运行完成
                    await this.Scheduler.Result.Shutdown();
                    await Console.Out.WriteLineAsync("任务调度停止！");
                }
            }
            catch (Exception ex)
            {
              
            }
        }

        #endregion

        #region 私有方法

        #region 添加计划

        /// <summary>
        /// 添加一个工作调度（映射程序集指定IJob实现类）
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private async Task<BaseQuartzNetResult> AddScheduleJob(ScheduleEntity m)
        {
            var result = new BaseQuartzNetResult();
            try
            {
                //检查任务是否已存在
                var jk = new JobKey(m.JobName, m.JobGroup);
                if (await this.Scheduler.Result.CheckExists(jk))
                {
                    //删除已经存在任务
                    await this.Scheduler.Result.DeleteJob(jk);
                }
                //反射获取任务执行类
                var jobType = ClassHelper.GetAbsolutePath(m.AssemblyName, m.AssemblyName + "." + m.ClassName);
                // 定义这个工作，并将其绑定到我们的IJob实现类
                IJobDetail job = new JobDetailImpl(m.JobName, m.JobGroup, jobType);
                //IJobDetail job = JobBuilder.CreateForAsync<T>().WithIdentity(m.JobName, m.JobGroup).Build();
                // 创建触发器
                ITrigger trigger;
                //校验是否正确的执行周期表达式
                if (!string.IsNullOrEmpty(m.Cron) && CronExpression.IsValidExpression(m.Cron))
                {
                    trigger = CreateCronTrigger(m);
                }
                else
                {
                    trigger = CreateSimpleTrigger(m);
                }
                // 设置监听器
                JobListener listener = new JobListener();
                // IMatcher<JobKey> matcher = KeyMatcher<JobKey>.KeyEquals(job.Key);
                this.Scheduler.Result.ListenerManager.AddJobListener(listener, GroupMatcher<JobKey>.AnyGroup());
                // 告诉Quartz使用我们的触发器来安排作业
                await this.Scheduler.Result.ScheduleJob(job, trigger);

                result.Code = 1000;
            }
            catch (Exception ex)
            {
                result.Code = 1001;
                result.Msg = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 添加任务调度（指定IJob实现类）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="m"></param>
        /// <returns></returns>
        private async Task<BaseQuartzNetResult> AddScheduleJob<T>(ScheduleEntity m) where T : IJob
        {
            var result = new BaseQuartzNetResult();
            try
            {
                //检查任务是否已存在
                var jk = new JobKey(m.JobName, m.JobGroup);
                if (await this.Scheduler.Result.CheckExists(jk))
                {
                    //删除已经存在任务
                    await this.Scheduler.Result.DeleteJob(jk);
                }
                // 定义这个工作，并将其绑定到我们的IJob实现类
                IJobDetail job = JobBuilder.CreateForAsync<T>().WithIdentity(m.JobName, m.JobGroup).Build();
                // 创建触发器
                ITrigger trigger;
                //校验是否正确的执行周期表达式
                if (!string.IsNullOrEmpty(m.Cron) && CronExpression.IsValidExpression(m.Cron))
                {
                    trigger = CreateCronTrigger(m);
                }
                else
                {
                    trigger = CreateSimpleTrigger(m);
                }
                // 设置监听器
                JobListener listener = new JobListener();
                // IMatcher<JobKey> matcher = KeyMatcher<JobKey>.KeyEquals(job.Key);
                this.Scheduler.Result.ListenerManager.AddJobListener(listener, GroupMatcher<JobKey>.AnyGroup());
                // 告诉Quartz使用我们的触发器来安排作业
                await this.Scheduler.Result.ScheduleJob(job, trigger);

                result.Code = 1000;
            }
            catch (Exception ex)
            {
                result.Code = 1001;
                result.Msg = ex.Message;
            }
            return result;
        }

        #endregion

        #region 创建触发器

        /// <summary>
        /// 创建类型Simple的触发器
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private ITrigger CreateSimpleTrigger(ScheduleEntity m)
        {
            //作业触发器
            if (m.RunTimes > 0)
            {
                return TriggerBuilder.Create()
               .WithIdentity(m.JobName, m.JobGroup)
               .StartAt(m.BeginTime)//开始时间
               .EndAt(m.EndTime)//结束数据
               .WithSimpleSchedule(x => x
                   .WithIntervalInSeconds(m.IntervalSecond)//执行时间间隔，单位秒
                   .WithRepeatCount(m.RunTimes))//执行次数、默认从0开始
                   .ForJob(m.JobName, m.JobGroup)//作业名称
               .Build();
            }
            else
            {
                return TriggerBuilder.Create()
               .WithIdentity(m.JobName, m.JobGroup)
               .StartAt(m.BeginTime)//开始时间
               .EndAt(m.EndTime)//结束数据
               .WithSimpleSchedule(x => x
                   .WithIntervalInSeconds(m.IntervalSecond)//执行时间间隔，单位秒
                   .RepeatForever())//无限循环
                   .ForJob(m.JobName, m.JobGroup)//作业名称
               .Build();
            }
        }

        /// <summary>
        /// 创建类型Cron的触发器
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private ITrigger CreateCronTrigger(ScheduleEntity m)
        {
            // 作业触发器
            return TriggerBuilder.Create()
                   .WithIdentity(m.JobName, m.JobGroup)
                   .StartAt(m.BeginTime)//开始时间
                   .EndAt(m.EndTime)//结束数据
                   .WithCronSchedule(m.Cron)//指定cron表达式
                   .ForJob(m.JobName, m.JobGroup)//作业名称
                   .Build();
        }

        #endregion

        #endregion
    }
}