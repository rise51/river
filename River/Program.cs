using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace River
{
    class Program
    {
        //internal static int totalLimit = 1000;

        //internal static int totalRun = 0;

        //internal static ConcurrentQueue<int> concurrentQueue = new ConcurrentQueue<int>();

        //String str = ConfigurationManager.AppSettings["xxx"];
        static void Main(string[] args)
        {
            /*
             * 1、初始化入参
             * 2、初始化配置参数
             * 3、启动获取资源服务
             * 4、启动消耗资源服务
             */
            #region 测试代码
            //int total = int.Parse(args[0].ToString());
            //for (int i = 0; i < 1000; i++)
            //{
            //    concurrentQueue.Enqueue(i);
            //}
            //while (true && totalRun < totalLimit)
            //{


            //    Task.Run(() =>
            //        {
            //            if (concurrentQueue.Count > 0)
            //            {
            //                int temp;
            //                concurrentQueue.TryDequeue(out temp);
            //                totalRun++;
            //                Console.WriteLine(string.Format("Total:{0} index:{1}", totalLimit, totalRun));
            //            }

            //        });
            //}
            #endregion
            string inputStr = Console.ReadLine();
            int total = int.Parse(inputStr);
            //Console.WriteLine(total);
            //Console.ReadKey();
            Singleton.Instance.RequestTotal = total;
            Singleton.Instance.DoWork();
            //Console.WriteLine("");
            //Console.ReadKey();
        }

    }
}
