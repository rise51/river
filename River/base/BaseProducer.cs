using System;
using System.Collections.Concurrent;
using System.Threading;

namespace River
{
    public class BaseProducer <T>: IWorker
    {

        protected ConcurrentQueue<T> _conQueue;
        //protected readonly ILogger _logger = LoggerFactory.GetChannelLog(typeof(BaseProducer<T>));
        bool isWaitFlag = true;

        public Thread _thread;
        public Thread Thread
        {
            get { return _thread; }
        }

        string _name;
        public string Name
        {
            get { return _name; }
        }

        int sleepSecond = 50000;
        public int Sleepses
        {
            get { return sleepSecond; }
            set { sleepSecond = value; }
        }

        int queueMax = 50000;
        public int QueueMax
        {
            get { return queueMax; }
            set { queueMax = value; }
        }

        public BaseProducer(string name, ConcurrentQueue<T> conQueue)
        {
            _name = name;
            _conQueue = conQueue;
            _thread = new Thread(Start);
            _thread.Name = string.Format("prothr{0}", _name);
        }

        public void Start()
        {
            while (isWaitFlag)
            {
                if (!IsWait())
                {
                    Process();
                    isWaitFlag = false;
                }
                else
                {
                    Thread.Sleep(sleepSecond);
                }
            }
        }

        public bool IsWait()
        {
            return _conQueue.Count > queueMax;
        }

        /// <summary>
        /// 执行过程
        /// </summary>
        /// <param name="filter"></param>
        public virtual void Process(Func<string, dynamic, bool> filter = null)
        {
        }

    }
}