using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroServiceDemo.Core.Common.Helpers
{
    public class CodeWatchHelper
    {
        public static void CodeStopWatch(Action action, string writeLineString = null)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start(); //  开始监视代码

            //要执行的函数
            action.Invoke();

            stopwatch.Stop(); //  停止监视
            TimeSpan timeSpan = stopwatch.Elapsed; //  获取总时间

            System.Diagnostics.Debug.WriteLine( writeLineString+ "代码执行时间：{0}(毫秒)", timeSpan.TotalMilliseconds);  //总毫秒数
        }
    }
}
