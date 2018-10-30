namespace MicroServiceDemo.Core.QuartzNet.Entity
{
    /// <summary>
    /// 枚举类型
    /// </summary>
    public class EnumType
    {
        /// <summary>
        /// 作业状态
        /// </summary>
        public enum JobStatus
        {
            /// <summary>
            /// 已启用
            /// </summary>
            Activated = 1,

            /// <summary>
            /// 已停用
            /// </summary>
            Unactivated = 0
        }

        /// <summary>
        /// 作业步骤
        /// </summary>
        public enum JobStep
        {
            /// <summary>
            /// 执行完成
            /// </summary>
            ExecutedSuccessfully = 1,

            /// <summary>
            /// 执行任务计划中
            /// </summary>
            Executing = 0
        }

        /// <summary>
        /// 作业执行状态
        /// </summary>
        public enum JobRunStatus
        {
            /// <summary>
            /// 执行中
            /// </summary>
            Running = 1,

            /// <summary>
            /// 待执行
            /// </summary>
            ReadyToRun = 0
        }
    }
}
