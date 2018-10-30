using System;
using Grpc.Core;
using MicroServiceDemo.Core.Common.Helpers;
using Autofac.Engine;
using AutoMapper;
using MicroServiceDemo.Core.Common.Enums;
using MongoDB.Bson;
using Quartz;
using Quartz.Impl;
using System.Threading.Tasks;
using MicroServiceDemo.Service.Jobs;
using MicroServiceDemo.Core.QuartzNet.Entity;
using System.Threading;
using MicroServiceDemo.Service.Infrastructure;

namespace MicroServiceDemo.Service
{
    class Program
    {
        private static readonly ManualResetEvent manualResetEvent = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            ////Autofac初始化
            //EngineContext.Initialize();

            ////AutoMapper初始化
            //Mapper.Initialize(MapConfiguration.AddProfile);

            ////Grpc服务初始化
            //Server server = GRPCContext.ServerInitialize();
            //server.Start();
            //server.ShutdownTask.Wait();

            MySchedulerContext.Initialize();

            RedisContext.Initialize();

            // 进入阻塞状态，开始等待唤醒信号。（防止程序直接退出）
            manualResetEvent.WaitOne();
        }
    }
}
