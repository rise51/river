
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autohome.Club.Framework;

namespace River
{
    public class Producer : Base<IPItem>
    {
        List<string> cache;

        public Singleton internalSingleton;

        /// <summary>
        /// 开始时间
        /// </summary>
        string beginTime = "00:00:00";
        public string BeginTime
        {
            get { return beginTime; }
            private set { beginTime = value; }
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        string endTime = "23:59:59";
        public string EndTime
        {
            get { return endTime; }
            private set { endTime = value; }
        }

        /// <summary>
        /// 车辆执行时间间隔
        /// </summary>
        int listInterval = 4000;
        public int ListInteval
        {
            get { return listInterval; }
            private set { listInterval = value; }
        }

        int runthreshold = 600;
        internal int Runthreshold
        {
            get { return runthreshold; }
            private set { runthreshold = value; }
        }

        int runCount = 0;
        internal int RunCount
        {
            get { return runCount; }
            set { runCount = value; }
        }

        public Producer(string name, ConcurrentQueue<IPItem> conQueue, Singleton singleton)
            : base(name, conQueue)
        {
            cache = new List<string>();
            this.internalSingleton = singleton;
        }

        public override bool IsWait()
        {
            return _conQueue.Count > QueueMax;
        }

        public override void Process(Func<string, dynamic, bool> filter = null)
        {
            ProcessListData(filter);
        }

        /// <summary>
        /// 优信拍
        /// </summary>
        /// <param name="filter"></param>
        private void ProcessListData(Func<string, dynamic, bool> filter)
        {
            while (true)
            {
                //if (this.internalSingleton.RequestIpCount > this.internalSingleton.RequestTotal)
                //{
                //    /*
                //     * 请求资源达到目标值，不在请求资源
                //     */
                //}
                //else
                //{
                try
                {
                    #region 芝麻IP
                    //string listStr = Z.WebRequest("http://http.tiqu.alicdns.com/getip3?num=30&type=1&pro=&city=0&yys=0&port=1&pack=110766&ts=0&ys=0&cs=0&lb=1&sb=0&pb=4&mr=1&regions=&gm=4", "GET", "UTF-8");
                    //if (!string.IsNullOrWhiteSpace(listStr))
                    //{

                    //    Array temp = listStr.Split('\r');
                    //    if (temp != null && temp.Length > 0)
                    //    {
                    //        foreach (string item in temp)
                    //        {
                    //            try
                    //            {

                    //                //符合过滤条件，跳过
                    //                //if (filter != null && filter(publishiId, item)) continue;
                    //                ////if (state != "3") continue;
                    //                //bool notExist = _conQueue.Any(queItem => string.Compare(queItem.ID, publishiId, true) == 0);
                    //                //bool notCache = cache.Any(listItem => string.Compare(listItem, publishiId, true) == 0);
                    //                //if (!notExist && !notCache)
                    //                //{
                    //                //_logger.Info("优信拍抓取列表数据，新增 {0}", item.ToString());
                    //                string tempipwithport = item.Trim('\n').ToString();
                    //                if (string.IsNullOrWhiteSpace(tempipwithport)) continue;
                    //                string ipport = item.Trim('\n').ToString();
                    //                _conQueue.Enqueue(new IPItem()
                    //                {
                    //                    ipwithport = ipport,
                    //                    outip = ipport.Split(':').FirstOrDefault()
                    //                });
                    //                this.internalSingleton.RequestIpCount++;
                    //                //缓存发布ID
                    //                //    cache.Add(publishiId);

                    //                //}
                    //            }
                    //            catch (Exception e)
                    //            {
                    //                //_logger.Error("优信拍抓取列表数据，入队列异常：{0} itme{1}", e, item.ToString());
                    //                continue;
                    //            }
                    //        }
                    //    }


                    //}
                    #endregion

                    #region 中联IP
                    string listStr = Z.WebRequest("http://ip.jiuyuanxx.com/getip?num=150&time=10", "GET", "UTF-8");
                    if (!string.IsNullOrWhiteSpace(listStr))
                    {
                        JObject jo = (JObject)JsonConvert.DeserializeObject(listStr);
                        string strData = jo["data"].ToString();
                        if (listStr.Contains("data") && !string.IsNullOrWhiteSpace(strData))
                        {
                            JArray jArray = JArray.Parse(strData);
                            if (jArray != null && jArray.Count > 0)
                            {
                                foreach (var item in jArray)
                                {
                                    try
                                    {
                                        #region 返回结果示例
                                        /*
                                         * {
                                                "code": 0,
                                                "success": true,
                                                    "data": [
                                                                {
                                                                    "ip": "183.129.206.210",
                                                                     "port": 18010,
                                                                        "expire_time": "2020-10-09 17:04:32",
                                                                        "outip": "117.60.106.122"
                                                                    },
                                                                {
                                                                        "ip": "183.129.206.210",
                                                                        "port": 18014,
                                                                         "expire_time": "2020-10-09 17:04:32",
                                                                            "outip": "222.246.229.15"
                                                                                },
                                                                             {
                                                                        "ip": "183.129.206.210",
                                                                        "port": 18012,
                                                                        "expire_time": "2020-10-09 17:04:32",
                                                                            "outip": "139.213.142.151"
                                                                             }
                                                                ]
                                                                }
                                         * 
                                         */
                                        #endregion

                                        string ip = ((JObject)item)["ip"].ToString();
                                        string port = ((JObject)item)["port"].ToString();
                                        if (string.IsNullOrWhiteSpace(ip) || string.IsNullOrWhiteSpace(port)) continue;
                                        _conQueue.Enqueue(new IPItem()
                                        {
                                            ipwithport = string.Format("{0}:{1}", ip, port),
                                            outip = ((JObject)item)["outip"].ToString()
                                        });
                                        this.internalSingleton.RequestIpCount++;
                                        //缓存发布ID
                                        //    cache.Add(publishiId);

                                        //}
                                    }
                                    catch (Exception e)
                                    {
                                        //_logger.Error("优信拍抓取列表数据，入队列异常：{0} itme{1}", e, item.ToString());
                                        continue;
                                    }
                                }
                            }
                        }

                    }
                    #endregion
                    DelayStrategy();
                }
                catch (Exception ex)
                {
                    //_logger.Error("优信拍抓取列表数据异常：{0} ", ex);
                }
                //}

            }
        }

        /// <summary>
        /// 设置睡眠策略
        /// </summary>
        private void DelayStrategy()
        {
            Thread.Sleep(5000);
            //try
            //{
            //    string now = DateTime.Now.ToLongTimeString();
            //    if (string.Compare(now, beginTime, true) > 0 && string.Compare(endTime, now, true) > 0)
            //    {
            //        Thread.Sleep(listInterval);
            //    }
            //    else
            //    {
            //        string[] tArray = beginTime.Split(':');
            //        DateTime mt = DateTime.Now.Date.AddDays(1);
            //        DateTime mtTime = new DateTime(mt.Year, mt.Month, mt.Day, int.Parse(tArray[0]), int.Parse(tArray[1]), int.Parse(tArray[2]));
            //        TimeSpan ts = (TimeSpan)(mtTime - DateTime.Now);
            //        Thread.Sleep(ts);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    //_logger.Error("优信拍 抓取列表数据 设置睡眠策略异常:{0}", ex);
            //    Thread.Sleep(listInterval);
            //}
        }
    }
}