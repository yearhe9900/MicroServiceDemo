using MicroServiceDemo.Core.Common.Enums;
using MicroServiceDemo.Core.Common.Helpers;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroServiceDemo.Service.Infrastructure
{
    public class RedisContext
    {
        public static void Initialize()
        {
            RedisHelper redis = new RedisHelper();

            var channelStr = RedisPublishChannelEnum.Test.ToString();

            void action(RedisChannel channel, RedisValue message)
            {
              
            }

            redis.SubscribeMessage(channelStr, action);
        }
    }
}
