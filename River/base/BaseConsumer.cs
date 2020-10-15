using System;
using System.Collections.Concurrent;
using System.Threading;

namespace River
{
    public class BaseConsumer : IWorker
    {
        protected ConcurrentQueue<DataItem> _conQueue;
        //protected readonly ILogger _logger = LoggerFactory.GetChannelLog(typeof(BaseConsumer));

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

        bool isWaitFlag = true;

        public BaseConsumer(string name, ConcurrentQueue<DataItem> conQueue)
        {
            _name = name;
            _conQueue = conQueue;
            _thread = new Thread(Start);
            _thread.Name = string.Format("conthr{0}", _name);
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
            return _conQueue.IsEmpty;
        }

        public virtual void Process(Func<string, dynamic, bool> filter = null)
        {
            throw new NotImplementedException();
        }

    }
}