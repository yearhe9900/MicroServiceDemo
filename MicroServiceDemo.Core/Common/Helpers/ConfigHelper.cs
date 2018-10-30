using System;
using System.Collections.Generic;
using System.Text;

namespace MicroServiceDemo.Core.Common.Helpers
{
    public class ConfigHelper
    {
        /// <summary>
        /// 服务器名称
        /// </summary>
        public static string HostName { get; set; } = JsonHelper.GetAppSetting("HostName");

        /// <summary>
        /// 日志服务地址
        /// </summary>
        public static string LogServerHost { get; set; } = JsonHelper.GetAppSetting("LogServerHost");

        /// <summary>
        /// GRPC Host
        /// </summary>
        public static string Host { get; set; } = JsonHelper.GetAppSetting("Host");

        /// <summary>
        /// GRPC Post
        /// </summary>
        public static int Post { get; set; } = Convert.ToInt32(JsonHelper.GetAppSetting("Post"));

        /// <summary>
        /// Redis链接地址
        /// </summary>
        public static string RedisUrl { get; set; } = JsonHelper.GetAppSetting("RedisUrl");

        /// <summary>
        /// 任务Mongo链接地址
        /// </summary>
        public static string MongoMdTaskUrl { get; set; } = JsonHelper.GetAppSetting("MongoTaskUrl");

        /// <summary>
        /// 日程Sql链接地址
        /// </summary>
        public static string SqlMDCalendarUrl { get; set; } = JsonHelper.GetAppSetting("MDCalendarUrl");
    }
}
