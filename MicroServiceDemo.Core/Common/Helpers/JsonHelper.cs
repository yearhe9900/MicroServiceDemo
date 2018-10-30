using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MicroServiceDemo.Core.Common.Helpers
{
    public class JsonHelper
    {
        /// <summary>
        /// 获取项目配置文件信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetAppSetting(string key)
        {
            var builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("Config/appsettings.json");
            var config = builder.Build();

            return config.GetSection(key).Value;
        }
    }
}
