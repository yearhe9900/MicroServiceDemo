using Grpc.Core;
using MicroServiceDemo.Core.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroServiceDemo.Service.Infrastructure
{
    public class GRPCContext
    {
        /// <summary>
        /// Grpc服务初始化方法
        /// </summary>
        /// <returns></returns>
        public static Server ServerInitialize()
        {
            var Host = ConfigHelper.Host;
            int Prot = ConfigHelper.Post;

            Server server = new Server()
            {
                Services = {

                },
                Ports = { new ServerPort(Host, Prot, ServerCredentials.Insecure) }
            };

            Console.WriteLine($"CalendarAlertService Listening on port {Prot}");
            return server;
        }
    }
}
