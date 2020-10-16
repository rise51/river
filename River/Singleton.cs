using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace River
{
    public class Singleton
    {
        private static volatile Singleton instance;
        private static object syncRoot = new Object();
        /// <summary>
        /// 当前执行数量
        /// </summary>
        public int RequestCount { get; set; }

        /// <summary>
        /// 执行总数
        /// </summary>
        public int RequestTotal { get; set; }

        /// <summary>
        /// /请求IP数量
        /// </summary>
        public int RequestIpCount { get; set; }

        /// <summary>
        /// /消耗IP数量
        /// </summary>
        public int ConsumerIpCount { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        private Singleton() { this.BeginTime = DateTime.Now; }
        public static Singleton Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new Singleton();
                    }
                }
                return instance;
            }
        }

        private static ConcurrentQueue<IPItem> listConQueue = new ConcurrentQueue<IPItem>();
        private static ConcurrentQueue<IPMetaDataItem> detailConQueue = new ConcurrentQueue<IPMetaDataItem>();
        public void DoWork()
        {
            //Task.Run(() => 
            //{
                Producer yxpProducer = new Producer("优信拍列表页", listConQueue,this);
                Intermediary yxpMiddleWorker = new Intermediary("优信拍详情页", listConQueue, detailConQueue,this);
                Consumer yxpStrorer = new Consumer("优信拍存储", detailConQueue, new DataStorer());
                yxpProducer.Thread.Start();
                yxpMiddleWorker.Thread.Start();
                yxpStrorer.Start();
            //});
        }
    }
}
