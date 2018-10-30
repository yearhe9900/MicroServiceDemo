using MicroServiceDemo.Core.Common.Helpers;
using MicroServiceDemo.Core.QuartzNet.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroServiceDemo.Core.QuartzNet.Manager
{
    /// <summary>
    /// 任务管理类
    /// </summary>
    public class ScheduleManager
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public static readonly ScheduleManager Instance;
        static ScheduleManager()
        {
            Instance = new ScheduleManager();
        }

        /// <summary>
        /// 任务计划列表
        /// </summary>
        public static List<ScheduleEntity> ScheduleList = new List<ScheduleEntity>();

        /// <summary>
        /// 任务调度详情列表
        /// </summary>
        public static IList<ScheduleDetailsEntity> ScheduleDetailList = new List<ScheduleDetailsEntity>();

        /// <summary>
        /// 添加任务列表
        /// </summary>
        /// <param name="scheduleEntity"></param>
        public virtual void AddScheduleList(ScheduleEntity scheduleEntity)
        {
            try
            {
                ScheduleList.Remove(ScheduleList.Where(o => o.JobName.Equals(scheduleEntity.JobName))?.FirstOrDefault());
                ScheduleList.Add(scheduleEntity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 添加任务列表
        /// </summary>
        /// <param name="scheduleEntity"></param>
        public virtual void AddScheduleList(List<ScheduleEntity> scheduleEntities)
        {
            foreach (var entity in scheduleEntities)
            {
                AddScheduleList(entity);
            }
        }

        /// <summary>
        /// 获取任务实例
        /// </summary>
        /// <param name="jobGroup">任务分组</param>
        /// <param name="jobName">任务名称</param>
        /// <returns></returns>
        public virtual ScheduleEntity GetScheduleModel(string jobGroup, string jobName)
        {
            return ScheduleList.Where(w => w.JobName == jobName && w.JobGroup == jobGroup).FirstOrDefault();
        }

        /// <summary>
        /// 移除任务
        /// </summary>
        /// <param name="jobGroup"></param>
        /// <param name="jobName"></param>
        /// <returns></returns>
        public virtual ScheduleEntity RemoveScheduleModel(string jobGroup, string jobName)
        {
            ScheduleEntity scheduleModel = this.GetScheduleModel(jobGroup, jobName);
            if (scheduleModel != null)
            {
                ScheduleList.Remove(scheduleModel);
            }
            return scheduleModel;
        }

        /// <summary>
        /// 修改任务执行状态
        /// </summary>
        /// <param name="entity"></param>
        public virtual void UpdateScheduleRunStatus(ScheduleEntity entity)
        {
            ScheduleList.Where(w => w.JobName == entity.JobName && w.JobGroup == entity.JobGroup).FirstOrDefault().RunStatus = entity.RunStatus;
        }

        /// <summary>
        /// 添加任务调度详情
        /// </summary>
        /// <param name="detail"></param>
        public virtual void AddScheduleDetails(ScheduleDetailsEntity detail)
        {
            ScheduleDetailList.Add(detail);
        }

        /// <summary>
        /// 修改下一次执行时间
        /// </summary>
        /// <param name="entity"></param>
        public virtual void UpdateScheduleNextTime(ScheduleEntity entity)
        {
            ScheduleList.Where(w => w.JobName == entity.JobName && w.JobGroup == entity.JobGroup).FirstOrDefault().NextTime = entity.NextTime;
        }

        /// <summary>
        /// 修改任务状态
        /// </summary>
        /// <param name="entity"></param>
        public virtual void UpdateScheduleStatus(ScheduleEntity entity)
        {
            ScheduleList.Where(w => w.JobName == entity.JobName && w.JobGroup == entity.JobGroup).FirstOrDefault().Status = entity.Status;
        }
    }
}
