using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace River
{
    public class Consumer : Base<IPMetaDataItem>
    {

        protected IStorer _storer/*存储器*/;
        int taskThreshold /*任务个数临界值*/= 0;
        Object objec/*队列取数锁对象*/ = new Object();

        /// <summary>
        /// 批量存储数据个数
        /// </summary>
        int databatchCount = 10;
        public int DataBatchCount
        {
            get { return databatchCount; }
            set { databatchCount = value; }
        }

        /// <summary>
        /// 任务最大数量
        /// </summary>
        int taskMaxScale = 1;
        public int TaskScale
        {
            get { return taskMaxScale; }
            set { taskMaxScale = value; }
        }

        /// <summary>
        /// 主流程执行时间间隔
        /// </summary>
        int procInterval = 5000;
        public int ProInteval
        {
            get { return procInterval; }
            private set { procInterval = value; }
        }

        public Consumer(string name, ConcurrentQueue<IPMetaDataItem> storeQueue, IStorer storer)
            : base(name, storeQueue)
        {
            _storer = storer;
        }

        public override bool IsWait()
        {
            return _conQueue.IsEmpty;
        }

        public override void Process(Func<string, dynamic, bool> filter = null)
        {
            ProcessStorer();
        }

        private void ProcessStorer()
        {
            while (true)
            {
                if (_conQueue.Count > databatchCount && taskThreshold < taskMaxScale)
                {
                    try
                    {
                        //List<YouxinpaiStoreModel> youxinpaiList = new List<YouxinpaiStoreModel>();
                        //lock (objec)
                        //{
                        //    MetaDataItem tempMdi = null;
                        //    for (int i = 0; i < databatchCount; i++)
                        //    {
                        //        if (_conQueue.TryDequeue(out tempMdi))
                        //        {
                        //            youxinpaiList.Add(tempMdi.Convert2YouxinipaiModel());
                        //        }
                        //    }
                        //}
                        //Task.Run(
                        //                () =>
                        //                {
                        //                    taskThreshold++;
                        //                    try
                        //                    {
                        //                        _storer.Save(youxinpaiList);
                        //                    }
                        //                    catch (Exception e)
                        //                    {
                        //                        //_logger.Error("优信拍 数据持久化任务异常:{0},youxinpaiList{1}", e, JsonConvert.SerializeObject(youxinpaiList));
                        //                    }
                        //                    finally
                        //                    {
                        //                        taskThreshold--;
                        //                    }
                        //                }
                        //                );
                    }
                    catch (Exception ex)
                    {
                        //_logger.Error("优信拍 数据存储主流程异常:{0}", ex);
                    }
                }
                else
                {
                    Thread.Sleep(procInterval);
                }
            }
        }
    }
}