﻿using Autohome.Club.Framework;
using Pegasus.utils;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Pegasus
{
    public class Intermediary : Base<IPItem>
    {
        //static FormBrower browser = new FormBrower();
        static int successCount = 0;
        int taskThreshold = 0;
        object objec = new object();
        ConcurrentQueue<IPMetaDataItem> _storeQueue;
        static Random random = new Random(15);
        static Random random1 = new Random(538);
        static Random random2 = new Random(3293);
        string guidStr = "EC51C026-F4BB-4AB0-9872-993F6CFF34A8";
        string guidStr1 = "52CD437F-1054-4AD5-A7F4-345B803C41AA";
        GuidMapper[] guidMapper = new GuidMapper[] {
            new GuidMapper(){ guid="EC51C026-F4BB-4AB0-9872-993F6CFF34A8",guid1="52CD437F-1054-4AD5-A7F4-345B803C41AA"},
            new GuidMapper(){ guid="633BA35B-8163-4E8F-A9B5-8CC19CC5DC30",guid1="ED45C0B4-14DB-48C2-8214-10198263E503"},
            new GuidMapper(){ guid="EC51C026-F4BB-4AB0-9872-993F6CFF34A8",guid1="2B52B9C2-0697-4014-A648-2A006F2A014B"},
            new GuidMapper(){ guid="06104009-FAC0-4562-A83B-58DA8626DA1A",guid1="69468F9B-0DD3-460C-865D-7F481C9E66F5"},
            new GuidMapper(){ guid="B636EDDC-8C45-4701-8865-AA5AB4FC634A",guid1="E7681A6C-F535-4B3F-9225-AF2E854396FD"},
            new GuidMapper(){ guid="5B599ABD-23BB-4804-BABE-3E85C38FAF45",guid1="0F3206F4-7D85-4EDB-AEB7-27E4EEC11EDF"},
            new GuidMapper(){ guid="32B38BAF-E56E-451C-B31C-E0449AC02965",guid1="C73D0DE3-DEA3-4CBB-901C-C6673359B1AB"},
            new GuidMapper(){ guid="2E98E588-793D-4F90-89E6-375C48812CC0",guid1="AF8C446B-FECA-4D96-8086-9EE29E7C3FD2"},
        };
        string[] broswer = new string[] {
            "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.{0}.77 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:83.0) Gecko/20100101 Firefox/83.0 {0}",
            "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4{0}.67 Mobile Safari/537.36 Edg/87.0.664.55",
            "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.25 Safari/537.36 Core/1.70.3{0}.400 QQBrowser/10.6.4208.400"
        };

        public Singleton internalSingleton;

        /// <summary>
        /// 主流程执行时间间隔
        /// </summary>
        int procInterval = 1000;
        public int ProInteval
        {
            get { return procInterval; }
            private set { procInterval = value; }
        }

        /// <summary>
        /// 交易执行时间间隔
        /// </summary>
        int tradeInterval = 1000;
        public int TradeInteval
        {
            get { return tradeInterval; }
            private set { tradeInterval = value; }
        }

        /// <summary>
        /// 车辆执行时间间隔
        /// </summary>
        int carInterval = 1000;
        public int CarInteval
        {
            get { return carInterval; }
            private set { carInterval = value; }
        }

        int taskScale = 20;
        public int TaskScale
        {
            get { return taskScale; }
            private set { taskScale = value; }
        }

        public Intermediary(string name, ConcurrentQueue<IPItem> inputConQueue,
            ConcurrentQueue<IPMetaDataItem> outputConQueue, Singleton singleton)
            : base(name, inputConQueue)
        {
            _storeQueue = outputConQueue;
            internalSingleton = singleton;
            guidStr = Guid.NewGuid().ToString().ToUpper();
            if (ConfigUtls.intermediary_taskscale > 0)
            {
                this.taskScale = ConfigUtls.intermediary_taskscale;
            }
        }

        public override bool IsWait()
        {
            return _conQueue.IsEmpty;
        }

        public override void Process(Func<string, dynamic, bool> filter = null)
        {
            /*--------------------------------------------------------------
            1、获取当前交易数据
            2、即时抓取交易数据
            3、即时抓取车辆数据
             4、队列存储
            */
            if (ConfigUtls.process_multi > 0)
            {
                ProcessTradeDataPegasus();
            }
            else
            {
                ProcessTradeData();
            }
            
        }

        private void ProcessTradeData()
        {
            while (true)
            {
                if (_conQueue.Count > 0 && taskThreshold < taskScale)
                {
                    try
                    {
                        IPMetaDataItem mdi = null;
                        IPItem di = null;
                        lock (objec)
                        {
                            if (_conQueue.TryDequeue(out di))
                            {
                                mdi = di.Convert2IPMetaDataItem();
                            }
                        }
                        Task.Run(
                                   () =>
                                   {
                                       taskThreshold++;
                                       try
                                       {
                                           SpiderProcess(mdi);
                                       }
                                       catch (Exception ex1)
                                       {
                                           //_logger.Error("优信拍 数据抓取任务异常：{0} mdi{1}", ex1, JsonConvert.SerializeObject(mdi));
                                       }
                                       finally
                                       {
                                           taskThreshold--;
                                       }
                                   }
                                   );
                        //Thread.Sleep(30);
                        if (ConfigUtls.proxy_rate_open == 0)
                        {
                            Thread.Sleep(random.Next(30));
                        }

                    }
                    catch (Exception ex)
                    {
                        //_logger.Error("优信拍 抓取主过程 异常：{0}", ex);
                    }
                    if (internalSingleton.RequestCount == internalSingleton.RequestTotal || internalSingleton.RequestCount > internalSingleton.RequestTotal)
                    {
                        Console.ReadKey();
                    }
                }
                else
                {
                    Thread.Sleep(procInterval);
                }
            }
        }

        private void ProcessTradeDataMulti()
        {
            while (true)
            {
                if (_conQueue.Count > 0 && taskThreshold < taskScale)
                {
                    try
                    {
                        IPMetaDataItem mdi = null;
                        IPItem di = null;
                        lock (objec)
                        {
                            if (_conQueue.TryDequeue(out di))
                            {
                                mdi = di.Convert2IPMetaDataItemMulti();
                            }
                        }
                        Task.Run(
                                   () =>
                                   {
                                       taskThreshold++;
                                       try
                                       {
                                           SpiderProcessMulti(mdi);
                                       }
                                       catch (Exception ex1)
                                       {
                                           //_logger.Error("优信拍 数据抓取任务异常：{0} mdi{1}", ex1, JsonConvert.SerializeObject(mdi));
                                       }
                                       finally
                                       {
                                           taskThreshold--;
                                       }
                                   }
                                   );
                        //Thread.Sleep(30);
                        if (ConfigUtls.proxy_rate_open == 0)
                        {
                            Thread.Sleep(random.Next(30));
                        }

                    }
                    catch (Exception ex)
                    {
                        //_logger.Error("优信拍 抓取主过程 异常：{0}", ex);
                    }
                    //if (internalSingleton.RequestCount == internalSingleton.RequestTotal || internalSingleton.RequestCount > internalSingleton.RequestTotal)
                    //{
                    //    Console.ReadKey();
                    //}
                }
                else
                {
                    Thread.Sleep(procInterval);
                }
            }
        }

        private void ProcessTradeDataPegasus()
        {
            while (true)
            {
                if (_conQueue.Count > 0 && taskThreshold < taskScale)
                {
                    try
                    {
                        IPMetaDataItem mdi = null;
                        IPItem di = null;
                        lock (objec)
                        {
                            if (_conQueue.TryDequeue(out di))
                            {
                                mdi = di.Convert2IPMetaDataItemMulti();
                            }
                        }
                        Task.Run(
                                   () =>
                                   {
                                       taskThreshold++;
                                       try
                                       {
                                           SpiderProcessPegasus(mdi);
                                       }
                                       catch (Exception ex1)
                                       {
                                           //_logger.Error("优信拍 数据抓取任务异常：{0} mdi{1}", ex1, JsonConvert.SerializeObject(mdi));
                                       }
                                       finally
                                       {
                                           taskThreshold--;
                                       }
                                   }
                                   );
                        //Thread.Sleep(30);
                        if (ConfigUtls.proxy_rate_open == 0)
                        {
                            Thread.Sleep(random.Next(30));
                        }

                    }
                    catch (Exception ex)
                    {
                        //_logger.Error("优信拍 抓取主过程 异常：{0}", ex);
                    }
                    //if (internalSingleton.RequestCount == internalSingleton.RequestTotal || internalSingleton.RequestCount > internalSingleton.RequestTotal)
                    //{
                    //    Console.ReadKey();
                    //}
                }
                else
                {
                    Thread.Sleep(procInterval);
                }
            }
        }

        /// <summary>
        /// 浏览器请求详情页
        /// </summary>
        /// <param name="dataItem"></param>
        private void SpiderProcess(DataItem dataItem)
        {
            //if (dataItem != null&&!string.IsNullOrWhiteSpace(dataItem.jump_url))
            //{
            //    browser.RequestWeb(dataItem.jump_url);
            //    successCount++;
            //}
        }

        private void SpiderProcess(IPMetaDataItem mdi)
        {
            if (mdi != null)
            {
                if (string.IsNullOrWhiteSpace(mdi.ipwithport) || mdi.ipwithport.Contains("{"))
                {
                    return;
                }
                //string proxy = mdi.ipwithport;
                //RefreshIESettings(proxy);
                //IEProxy ie = new IEProxy(proxy);
                //browser.RequestWeb(mdi.requesturl);
                //if (GatherTradeData(mdi, null))
                //{
                //    GatherCarData(mdi, null);
                //    _storeQueue.Enqueue(mdi);
                //
                //if (internalSingleton.RequestIpCount > internalSingleton.RequestTotal &&
                //        (_conQueue.Count == 0))
                //{
                //    return;
                //}
                if (!string.IsNullOrWhiteSpace(mdi.ipwithport))
                {
                    HttpClient httpClient = GetProxyHttpClient(mdi);
                    if (ConfigUtls.proxy_rate_open > 0)
                    {
                        if (mdi.requesturls != null && mdi.requesturls.Length > 0)
                        {
                            try
                            {
                                foreach (var item in mdi.requesturls)
                                {
                                    if (!string.IsNullOrWhiteSpace(item))
                                    {
                                        string httpResult = "";
                                        string tempRequest = HttpUtility.UrlDecode(item, System.Text.Encoding.UTF8);
                                        try
                                        {
                                            httpClient.GetAsync(tempRequest);
                                            httpResult = "dddd" + httpResult;
                                            this.internalSingleton.RequestCount++;
                                            //Console.WriteLine(string.Format("*执行总数totalCount:{0}/{1}当前执行数量 /获取资源数量{2}/消耗资源数量{5}/开始时间{3} 结束时间{4}",
                                            //     this.internalSingleton.RequestTotal,
                                            //     internalSingleton.RequestCount,
                                            //     internalSingleton.RequestIpCount,
                                            //     internalSingleton.BeginTime.ToString("yyyy-MM-dd hh:mm:ss fff"),
                                            //     DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff"),
                                            //     this.internalSingleton.ConsumerIpCount));
                                        }
                                        catch (Exception e1)
                                        {
                                            //Console.WriteLine(string.Format("{0}\n{1}", mdi.ipwithport, e1.StackTrace.ToString()));
                                        }
                                        
                                    }
                                    //Thread.Sleep(20);
                                    Thread.Sleep(ConfigUtls.time_space);
                                }
                                string tzResult = "";
                                //if (DateTime.Now.Minute % 9 == 0)
                                //{
                                //    string tempRequest = HttpUtility.UrlDecode("http://wetopic.api.autohome.com.cn/api/test", System.Text.Encoding.UTF8);
                                //    var response = httpClient.GetAsync(tempRequest).Result;
                                //    if (response.IsSuccessStatusCode)
                                //    {
                                //        tzResult = response.ToString();
                                //    }
                                //}
                                Console.WriteLine(string.Format("*执行总数totalCount:{0}/{1}当前执行数量 /获取资源数量{2}/消耗资源数量{5}/队列长度{7}/开始时间{3} 结束时间{4} Result:{6}",
                                                this.internalSingleton.RequestTotal,
                                                internalSingleton.RequestCount,
                                                internalSingleton.RequestIpCount,
                                                internalSingleton.BeginTime.ToString("yyyy-MM-dd hh:mm:ss fff"),
                                                DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff"),
                                                this.internalSingleton.ConsumerIpCount, tzResult, this._conQueue.Count));
                            }
                            catch (Exception)
                            {
                            }
                            finally
                            {
                                this.internalSingleton.ConsumerIpCount++;
                                httpClient.Dispose();
                                httpClient = null;
                                //mdi.Dispose();
                                mdi = null;
                            }

                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(mdi.requesturl))
                        {
                            string httpResult = "";
                            string tempRequest = HttpUtility.UrlDecode(mdi.requesturl, System.Text.Encoding.UTF8);
                            try
                            {
                                httpClient.GetAsync(tempRequest);
                                httpResult = "dddd" + httpResult;
                                this.internalSingleton.RequestCount++;
                                Console.WriteLine(string.Format("*执行总数totalCount:{0}/{1}当前执行数量 /获取资源数量{2}/消耗资源数量{5}/队列长度{6}/开始时间{3} 结束时间{4}",
                                                 this.internalSingleton.RequestTotal,
                                                 internalSingleton.RequestCount,
                                                 internalSingleton.RequestIpCount,
                                                 internalSingleton.BeginTime.ToString("yyyy-MM-dd hh:mm:ss fff"),
                                                 DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff"),
                                                 this.internalSingleton.ConsumerIpCount, this._conQueue.Count));
                            }
                            catch (Exception e2)
                            {
                            }
                            finally
                            {
                                this.internalSingleton.ConsumerIpCount++;
                                httpClient.Dispose();
                                httpClient = null;
                                //mdi.Dispose();
                                mdi = null;
                            }

                        }
                    }
                }

            }
        }

        private void SpiderProcessMulti(IPMetaDataItem mdi)
        {
            if (mdi != null)
            {
                if (string.IsNullOrWhiteSpace(mdi.ipwithport) || mdi.ipwithport.Contains("{"))
                {
                    return;
                }
                //string proxy = mdi.ipwithport;
                //RefreshIESettings(proxy);
                //IEProxy ie = new IEProxy(proxy);
                //browser.RequestWeb(mdi.requesturl);
                //if (GatherTradeData(mdi, null))
                //{
                //    GatherCarData(mdi, null);
                //    _storeQueue.Enqueue(mdi);
                //
                //if (internalSingleton.RequestIpCount > internalSingleton.RequestTotal &&
                //        (_conQueue.Count == 0))
                //{
                //    return;
                //}
                if (!string.IsNullOrWhiteSpace(mdi.ipwithport))
                {
                    HttpClient httpClient = GetProxyHttpClientMulti(mdi);
                    if (ConfigUtls.proxy_rate_open > 0)
                    {
                        if (mdi.requestUrlAndReferers != null && mdi.requestUrlAndReferers.Count > 0)
                        {
                            try
                            {
                                foreach (var item in mdi.requestUrlAndReferers)
                                {
                                    if (item != null && !string.IsNullOrWhiteSpace(item.requesturl))
                                    {
                                        string httpResult = "";
                                        string tempRequest = HttpUtility.UrlDecode(item.requesturl, System.Text.Encoding.UTF8);
                                        try
                                        {
                                            SetCookie(httpClient, item, mdi);
                                            httpClient.GetAsync(tempRequest);
                                            httpResult = "dddd" + httpResult;
                                            this.internalSingleton.RequestCount++;
                                            //Console.WriteLine(string.Format("*执行总数totalCount:{0}/{1}当前执行数量 /获取资源数量{2}/消耗资源数量{5}/开始时间{3} 结束时间{4}",
                                            //    this.internalSingleton.RequestTotal,
                                            //    internalSingleton.RequestCount,
                                            //    internalSingleton.RequestIpCount,
                                            //    internalSingleton.BeginTime.ToString("yyyy-MM-dd hh:mm:ss fff"),
                                            //    DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff"),
                                            //    this.internalSingleton.ConsumerIpCount));
                                        }
                                        catch (Exception e1)
                                        {
                                            //Console.WriteLine(string.Format("{0}\n{1}", mdi.ipwithport, e1.StackTrace.ToString()));
                                        }
                                        //参照间隔时间Thread.Sleep(20);
                                        //Thread.Sleep(6);
                                        Thread.Sleep(ConfigUtls.time_space_multi);
                                    }
                                }
                                if (ConfigUtls.probe_switch > 0&&DateTime.Now.Minute==29 && 28<DateTime.Now.Second && DateTime.Now.Second<31)
                                {
                                    string tempRequest = HttpUtility.UrlDecode("http://wetopic.api.autohome.com.cn/api/test", System.Text.Encoding.UTF8);
                                    var response = httpClient.GetAsync(tempRequest).Result;
                                    mdi.result = string.Format("Ipport {3} -{4} Datetime {2} IsSuccessStatusCode {0} StatusCode {1}"
                                        , response.IsSuccessStatusCode, response.StatusCode,DateTime.Now,mdi.ipwithport,mdi.outip);
                                    this._storeQueue.Enqueue(mdi);
                                }
                                Console.WriteLine(string.Format("*执行总数totalCount:{0}/{1}当前执行数量 /获取资源数量{2}/消耗资源数量{5}/队列长度{6}/开始时间{3} 结束时间{4}",
                                                this.internalSingleton.RequestTotal,
                                                internalSingleton.RequestCount,
                                                internalSingleton.RequestIpCount,
                                                internalSingleton.BeginTime.ToString("yyyy-MM-dd hh:mm:ss fff"),
                                                DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff"),
                                                this.internalSingleton.ConsumerIpCount,this._conQueue.Count));
                            }
                            catch (Exception)
                            {
                            }
                            finally
                            {
                                this.internalSingleton.ConsumerIpCount++;
                                httpClient.Dispose();
                                httpClient = null;
                                mdi = null;
                            }

                        }
                    }
                    else
                    {
                        if (mdi.requestUrlAndReferers!=null && mdi.requestUrlAndReferers.Count>0)
                        {
                            try
                            {
                                foreach (var item in mdi.requestUrlAndReferers)
                                {
                                    if (item != null && !string.IsNullOrWhiteSpace(item.requesturl))
                                    {
                                        string httpResult = "";
                                        string tempRequest = HttpUtility.UrlDecode(item.requesturl, System.Text.Encoding.UTF8);
                                        try
                                        {
                                            SetCookie(httpClient, item, mdi);
                                            httpClient.GetAsync(tempRequest);
                                            httpResult = "dddd" + httpResult;
                                            this.internalSingleton.RequestCount++;
                                            //Console.WriteLine(string.Format("*执行总数totalCount:{0}/{1}当前执行数量 /获取资源数量{2}/消耗资源数量{5}/开始时间{3} 结束时间{4}",
                                            //     this.internalSingleton.RequestTotal,
                                            //     internalSingleton.RequestCount,
                                            //     internalSingleton.RequestIpCount,
                                            //     internalSingleton.BeginTime.ToString("yyyy-MM-dd hh:mm:ss fff"),
                                            //     DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff"), 
                                            //     this.internalSingleton.ConsumerIpCount));
                                        }
                                        catch (Exception e1)
                                        {
                                            //Console.WriteLine(string.Format("{0}\n{1}", mdi.ipwithport, e1.StackTrace.ToString()));
                                        }

                                    }

                                }
                                Console.WriteLine(string.Format("*执行总数totalCount:{0}/{1}当前执行数量 /获取资源数量{2}/消耗资源数量{5}/队列长度{6}/开始时间{3} 结束时间{4}",
                                                 this.internalSingleton.RequestTotal,
                                                 internalSingleton.RequestCount,
                                                 internalSingleton.RequestIpCount,
                                                 internalSingleton.BeginTime.ToString("yyyy-MM-dd hh:mm:ss fff"),
                                                 DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff"),
                                                 this.internalSingleton.ConsumerIpCount,this._conQueue.Count));
                            }
                            catch (Exception)
                            {
                            }
                            finally
                            {
                                this.internalSingleton.ConsumerIpCount++;
                                httpClient.Dispose();
                                httpClient = null;
                                mdi = null;
                            }

                        }
                    }
                }

            }
        }

        private void SpiderProcessPegasus(IPMetaDataItem mdi)
        {
            if (mdi != null)
            {
                if (string.IsNullOrWhiteSpace(mdi.ipwithport) || mdi.ipwithport.Contains("{"))
                {
                    return;
                }
                //string proxy = mdi.ipwithport;
                //RefreshIESettings(proxy);
                //IEProxy ie = new IEProxy(proxy);
                //browser.RequestWeb(mdi.requesturl);
                //if (GatherTradeData(mdi, null))
                //{
                //    GatherCarData(mdi, null);
                //    _storeQueue.Enqueue(mdi);
                //
                //if (internalSingleton.RequestIpCount > internalSingleton.RequestTotal &&
                //        (_conQueue.Count == 0))
                //{
                //    return;
                //}
                if (!string.IsNullOrWhiteSpace(mdi.ipwithport))
                {
                    HttpClient httpClient = GetProxyHttpClientPegasus(mdi);
                    if (ConfigUtls.proxy_rate_open > 0)
                    {
                        //if (mdi.requestUrlAndReferers != null && mdi.requestUrlAndReferers.Count > 0)
                        //{
                            try
                            {
                            //foreach (var item in mdi.requestUrlAndReferers)
                            //{
                            //if (item != null && !string.IsNullOrWhiteSpace(item.requesturl))
                            //{
                            string request = string.Format(@"https://log.m.sm.cn/0.gif?pt=dl_shichang_mingzhenhuati&hid=76514d6eedf163f0a6fbb43704c837c4&sid=76514d6eedf163f0a6fbb43704c837c4&ip={0}&qt=1609375192&l=2&c=634424&from=kk%40quark_mingzhentopic_11&cp=main&ext=name:index_share;&log_type=2",
                                mdi.outip);
                               
                                        string httpResult = "";
                                        string tempRequest = HttpUtility.UrlDecode(request, System.Text.Encoding.UTF8);
                                        try
                                        {
                                            //SetCookie(httpClient, item, mdi);
                                            httpClient.PostAsync(tempRequest,null);
                                            httpResult = "dddd" + httpResult;
                                            this.internalSingleton.RequestCount++;
                                            //Console.WriteLine(string.Format("*执行总数totalCount:{0}/{1}当前执行数量 /获取资源数量{2}/消耗资源数量{5}/开始时间{3} 结束时间{4}",
                                            //    this.internalSingleton.RequestTotal,
                                            //    internalSingleton.RequestCount,
                                            //    internalSingleton.RequestIpCount,
                                            //    internalSingleton.BeginTime.ToString("yyyy-MM-dd hh:mm:ss fff"),
                                            //    DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff"),
                                            //    this.internalSingleton.ConsumerIpCount));
                                        }
                                        catch (Exception e1)
                                        {
                                            //Console.WriteLine(string.Format("{0}\n{1}", mdi.ipwithport, e1.StackTrace.ToString()));
                                        }
                                        //参照间隔时间Thread.Sleep(20);
                                        //Thread.Sleep(6);
                                        Thread.Sleep(ConfigUtls.time_space_multi);
                                    //}
                                //}
                                //if (ConfigUtls.probe_switch > 0 && DateTime.Now.Minute == 29 && 28 < DateTime.Now.Second && DateTime.Now.Second < 31)
                                //{
                                //    string tempRequest = HttpUtility.UrlDecode("http://wetopic.api.autohome.com.cn/api/test", System.Text.Encoding.UTF8);
                                //    var response = httpClient.GetAsync(tempRequest).Result;
                                //    mdi.result = string.Format("Ipport {3} -{4} Datetime {2} IsSuccessStatusCode {0} StatusCode {1}"
                                //        , response.IsSuccessStatusCode, response.StatusCode, DateTime.Now, mdi.ipwithport, mdi.outip);
                                //    this._storeQueue.Enqueue(mdi);
                                //}
                                Console.WriteLine(string.Format("*执行总数totalCount:{0}/{1}当前执行数量 /获取资源数量{2}/消耗资源数量{5}/队列长度{6}/开始时间{3} 结束时间{4}",
                                                this.internalSingleton.RequestTotal,
                                                internalSingleton.RequestCount,
                                                internalSingleton.RequestIpCount,
                                                internalSingleton.BeginTime.ToString("yyyy-MM-dd hh:mm:ss fff"),
                                                DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff"),
                                                this.internalSingleton.ConsumerIpCount, this._conQueue.Count));
                            }
                            catch (Exception)
                            {
                            }
                            finally
                            {
                                this.internalSingleton.ConsumerIpCount++;
                                httpClient.Dispose();
                                httpClient = null;
                                mdi = null;
                            }

                        //}
                    }
                    else
                    {
                        if (mdi.requestUrlAndReferers != null && mdi.requestUrlAndReferers.Count > 0)
                        {
                            try
                            {
                                foreach (var item in mdi.requestUrlAndReferers)
                                {
                                    if (item != null && !string.IsNullOrWhiteSpace(item.requesturl))
                                    {
                                        string httpResult = "";
                                        string tempRequest = HttpUtility.UrlDecode(item.requesturl, System.Text.Encoding.UTF8);
                                        try
                                        {
                                            SetCookie(httpClient, item, mdi);
                                            httpClient.GetAsync(tempRequest);
                                            httpResult = "dddd" + httpResult;
                                            this.internalSingleton.RequestCount++;
                                            //Console.WriteLine(string.Format("*执行总数totalCount:{0}/{1}当前执行数量 /获取资源数量{2}/消耗资源数量{5}/开始时间{3} 结束时间{4}",
                                            //     this.internalSingleton.RequestTotal,
                                            //     internalSingleton.RequestCount,
                                            //     internalSingleton.RequestIpCount,
                                            //     internalSingleton.BeginTime.ToString("yyyy-MM-dd hh:mm:ss fff"),
                                            //     DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff"), 
                                            //     this.internalSingleton.ConsumerIpCount));
                                        }
                                        catch (Exception e1)
                                        {
                                            //Console.WriteLine(string.Format("{0}\n{1}", mdi.ipwithport, e1.StackTrace.ToString()));
                                        }

                                    }

                                }
                                Console.WriteLine(string.Format("*执行总数totalCount:{0}/{1}当前执行数量 /获取资源数量{2}/消耗资源数量{5}/队列长度{6}/开始时间{3} 结束时间{4}",
                                                 this.internalSingleton.RequestTotal,
                                                 internalSingleton.RequestCount,
                                                 internalSingleton.RequestIpCount,
                                                 internalSingleton.BeginTime.ToString("yyyy-MM-dd hh:mm:ss fff"),
                                                 DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff"),
                                                 this.internalSingleton.ConsumerIpCount, this._conQueue.Count));
                            }
                            catch (Exception)
                            {
                            }
                            finally
                            {
                                this.internalSingleton.ConsumerIpCount++;
                                httpClient.Dispose();
                                httpClient = null;
                                mdi = null;
                            }

                        }
                    }
                }

            }
        }

        #region 代理IP
        public struct Struct_INTERNET_PROXY_INFO
        {
            public int dwAccessType;
            public IntPtr proxy;
            public IntPtr proxyBypass;
        };
        //strProxy为代理IP:端口
        private void RefreshIESettings(string strProxy)
        {
            const int INTERNET_OPTION_PROXY = 38;
            const int INTERNET_OPEN_TYPE_PROXY = 3;
            const int INTERNET_OPEN_TYPE_DIRECT = 1;

            Struct_INTERNET_PROXY_INFO struct_IPI;
            // Filling in structure
            struct_IPI.dwAccessType = INTERNET_OPEN_TYPE_PROXY;
            struct_IPI.proxy = Marshal.StringToHGlobalAnsi(strProxy);
            struct_IPI.proxyBypass = Marshal.StringToHGlobalAnsi("local");

            // Allocating memory
            IntPtr intptrStruct = Marshal.AllocCoTaskMem(Marshal.SizeOf(struct_IPI));
            if (string.IsNullOrEmpty(strProxy) || strProxy.Trim().Length == 0)
            {
                strProxy = string.Empty;
                struct_IPI.dwAccessType = INTERNET_OPEN_TYPE_DIRECT;

            }
            // Converting structure to IntPtr
            Marshal.StructureToPtr(struct_IPI, intptrStruct, true);

            bool iReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_PROXY, intptrStruct, Marshal.SizeOf(struct_IPI));
        }

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);
        public class IEProxy
        {
            private const int INTERNET_OPTION_PROXY = 38;
            private const int INTERNET_OPEN_TYPE_PROXY = 3;
            private const int INTERNET_OPEN_TYPE_DIRECT = 1;

            private string ProxyStr;


            [DllImport("wininet.dll", SetLastError = true)]

            private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);

            public struct Struct_INTERNET_PROXY_INFO
            {
                public int dwAccessType;
                public IntPtr proxy;
                public IntPtr proxyBypass;
            }

            private bool InternetSetOption(string strProxy)
            {
                int bufferLength;
                IntPtr intptrStruct;
                Struct_INTERNET_PROXY_INFO struct_IPI;

                if (string.IsNullOrEmpty(strProxy) || strProxy.Trim().Length == 0)
                {
                    strProxy = string.Empty;
                    struct_IPI.dwAccessType = INTERNET_OPEN_TYPE_DIRECT;
                }
                else
                {
                    struct_IPI.dwAccessType = INTERNET_OPEN_TYPE_PROXY;
                }
                struct_IPI.proxy = Marshal.StringToHGlobalAnsi(strProxy);
                struct_IPI.proxyBypass = Marshal.StringToHGlobalAnsi("local");
                bufferLength = Marshal.SizeOf(struct_IPI);
                intptrStruct = Marshal.AllocCoTaskMem(bufferLength);
                Marshal.StructureToPtr(struct_IPI, intptrStruct, true);
                return InternetSetOption(IntPtr.Zero, INTERNET_OPTION_PROXY, intptrStruct, bufferLength);

            }
            public IEProxy(string strProxy)
            {
                this.ProxyStr = strProxy;
            }
            //设置代理
            public bool RefreshIESettings()
            {
                return InternetSetOption(this.ProxyStr);
            }
            //取消代理
            public bool DisableIEProxy()
            {
                return InternetSetOption(string.Empty);
            }
        }
        #endregion

        #region 代理


        /// <summary>
        /// 获取动态代理
        /// </summary>
        /// <param name="proxyIPport"></param>
        /// <returns></returns>
        private HttpClient GetProxyHttpClient(IPMetaDataItem mdi)
        {
            var proxy = new WebProxy(string.Format("http://{0}", mdi.ipwithport));
            HttpClientHandler httpClientHandler = new HttpClientHandler()
            {
                Proxy = proxy
            };
            var httpCient = new HttpClient(httpClientHandler);
            // 增加头部
            httpCient.DefaultRequestHeaders.Add("Accept", "image/webp,image/apng,image/*,*/*;q=0.8");
            httpCient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            httpCient.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
            string cookie = string.Format("fvlid={1}; " +
                "sessionid=C0B882EA-FEDC-43CA-8E9F-B322F35528C8%7C%7C2020-07-25+12%3A38%3A50.381%7C%7C0; " +
                "autoid=c933fac8868713f3f0e2d3d4b83f16b0; " +
                "ahpau=1; __ah_uuid_ng=c_C0B882EA-FEDC-43CA-8E9F-B322F35528C8; " +
                "sessionip={0}; " +
                "sessionvid=681B9A1B-2EC5-4194-8437-33DEFB091DEA; " +
                "area=119999; v_no=0; visit_info_ad=C0B882EA-FEDC-43CA-8E9F-B322F35528C8||681B9A1B-2EC5-4194-8437-33DEFB091DEA||-1||-1||4; ref=0%7C0%7C0%7C0%7C2020-08-02+08%3A00%3A14.918%7C2020-07-25+12%3A38%3A50.381"
                , mdi.outip, mdi.fvlid);
            //string cookie = string.Format("fvlid={1}; " +
            //  "sessionid=C0B882EA-FEDC-43CA-8E9F-B322F35528C8%7C%7C2020-07-25+12%3A38%3A50.381%7C%7C0; " +
            //  "autoid=c933fac8868713f3f0e2d3d4b83f16b0; " +
            //  "ahpau=1; __ah_uuid_ng=c_C0B882EA-FEDC-43CA-8E9F-B322F35528C8; " +
            //  "sessionip={0}; " +
            //  "sessionvid=681B9A1B-2EC5-4194-8437-33DEFB091DEA; " +
            //  "area=119999; v_no=0; visit_info_ad=C0B882EA-FEDC-43CA-8E9F-B322F35528C8||681B9A1B-2EC5-4194-8437-33DEFB091DEA||-1||-1||4; ref=0%7C0%7C0%7C0%7C2020-08-02+08%3A00%3A14.918%7C2020-07-25+12%3A38%3A50.381"
            //  , mdi.outip, mdi.fvlid);
            httpCient.DefaultRequestHeaders.Add("Cookie", cookie);
            httpCient.DefaultRequestHeaders.Add("Host", "al.autohome.com.cn");
            httpCient.DefaultRequestHeaders.Add("Referer", ConfigUtls.mda_pv_init_referer);
            httpCient.DefaultRequestHeaders.Add("User-Agent",string.Format("Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.{0}.77 Safari/537.36",3000+ random1.Next()));
            return httpCient;
        }

        /// <summary>
        /// 获取动态代理
        /// </summary>
        /// <param name="proxyIPport"></param>
        /// <returns></returns>
        private HttpClient GetProxyHttpClientMulti(IPMetaDataItem mdi)
        {
            var proxy = new WebProxy(string.Format("http://{0}", mdi.ipwithport));
            HttpClientHandler httpClientHandler = new HttpClientHandler()
            {
                Proxy = proxy
            };
            var httpCient = new HttpClient(httpClientHandler);
            // 增加头部
            httpCient.DefaultRequestHeaders.Add("Accept", "image/webp,image/apng,image/*,*/*;q=0.8");
            httpCient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            httpCient.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");

            ////string guidStr = Guid.NewGuid().ToString().ToUpper();
            ////string guidStr1 = Guid.NewGuid().ToString().ToUpper();
            //string dateTime = DateTime.Now.AddDays(-random.Next(10)).ToString();
            //string cookie = string.Format("fvlid={1}; " +
            //    "sessionid={2}{6} " +
            //    "autoid={7}; " +
            //    "ahpau=1; __ah_uuid_ng=c_{3}; " +
            //    "sessionip={0}; " +
            //    "sessionvid=681B9A1B-2EC5-4194-8437-33DEFB091DEA; " +
            //    "area=119999; v_no=0; visit_info_ad={4}||681B9A1B-2EC5-4194-8437-33DEFB091DEA||-1||-1||4; ref={5}"
            //    , mdi.outip, mdi.fvlid,
            //    this.guidStr, this.guidStr, this.guidStr,
            //    HttpUtility.UrlEncode(string.Format("0|0|0|0|{0}|{1}", DateTime.Now.ToString(), dateTime))
            //    , HttpUtility.UrlEncode(string.Format("||{0}||0; ", dateTime)),
            //    Z.GetMD5LowerString(this.guidStr)
            //    );
            //string cookie = string.Format("fvlid={1}; " +
            //  "sessionid=C0B882EA-FEDC-43CA-8E9F-B322F35528C8%7C%7C2020-07-25+12%3A38%3A50.381%7C%7C0; " +
            //  "autoid=c933fac8868713f3f0e2d3d4b83f16b0; " +
            //  "ahpau=1; __ah_uuid_ng=c_C0B882EA-FEDC-43CA-8E9F-B322F35528C8; " +
            //  "sessionip={0}; " +
            //  "sessionvid=681B9A1B-2EC5-4194-8437-33DEFB091DEA; " +
            //  "area=119999; v_no=0; visit_info_ad=C0B882EA-FEDC-43CA-8E9F-B322F35528C8||681B9A1B-2EC5-4194-8437-33DEFB091DEA||-1||-1||4; ref=0%7C0%7C0%7C0%7C2020-08-02+08%3A00%3A14.918%7C2020-07-25+12%3A38%3A50.381"
            //  , mdi.outip, mdi.fvlid);

            //this.guidStr = Guid.NewGuid().ToString().ToUpper();
            //string guidStr1 = Guid.NewGuid().ToString().ToUpper();
            string dateTime = DateTime.Now.AddDays(-random.Next(10)).ToString();
            string cookie = string.Format("fvlid={1}; " +
                "sessionid={2}{6} " +
                "autoid={7}; " +
                "ahpau=1; __ah_uuid_ng=c_{3}; " +
                "sessionip={0}; " +
                "sessionvid={8}; " +
                "area={10}; v_no=1; visit_info_ad={4}||{9}||-1||-1||1; ref={5}"
                , mdi.outip, mdi.fvlid,
                this.guidStr, this.guidStr, this.guidStr,
                HttpUtility.UrlEncode(string.Format("0|0|0|0|{0}|{1}", DateTime.Now.ToString(), dateTime))
                , HttpUtility.UrlEncode(string.Format("||{0}||0; ", dateTime)),
                Z.GetMD5LowerString(this.guidStr),this.guidStr1,this.guidStr1,
                cityareas[random2.Next(3293)]
                );
            #region 出流量策略
            //如有问题更换  guidStr 和 guidStr1  为最新未登录的值；调整proxy_rate:30,time_space_multi:3
            #endregion
            //string cookie = string.Format("fvlid={1}; " +
            //   "sessionid=EC51C026-F4BB-4AB0-9872-993F6CFF34A8%7C%7C2020-10-14+18%3A20%3A47.654%7C%7C0; " +
            //   "autoid=31bf984e655c9fe48f14c7176521ee08; " +
            //   "ahpau=1; __ah_uuid_ng=c_EC51C026-F4BB-4AB0-9872-993F6CFF34A8; " +
            //   "sessionip={0}; " +
            //   "sessionvid=52CD437F-1054-4AD5-A7F4-345B803C41AA; " +
            //   "area=330199; v_no=1; visit_info_ad=EC51C026-F4BB-4AB0-9872-993F6CFF34A8||52CD437F-1054-4AD5-A7F4-345B803C41AA||-1||-1||1; ref=0%7C0%7C0%7C0%7C2020-12-13+15%3A16%3A28.794%7C2020-10-14+18%3A20%3A47.654"
            //   , mdi.outip, mdi.fvlid);
            httpCient.DefaultRequestHeaders.Add("Cookie", cookie);
            httpCient.DefaultRequestHeaders.Add("Host", "al.autohome.com.cn");
            httpCient.DefaultRequestHeaders.Add("Referer", ConfigUtls.mda_pv_init_referer);
            httpCient.DefaultRequestHeaders.Add("User-Agent",string.Format("Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.{0}.77 Safari/537.36", 3000 + random1.Next()));
            return httpCient;
        }

        /// <summary>
        /// 获取动态代理
        /// </summary>
        /// <param name="proxyIPport"></param>
        /// <returns></returns>
        private HttpClient GetProxyHttpClientPegasus(IPMetaDataItem mdi)
        {
            var proxy = new WebProxy(string.Format("http://{0}", mdi.ipwithport));
            HttpClientHandler httpClientHandler = new HttpClientHandler()
            {
                Proxy = proxy
            };
            var httpCient = new HttpClient(httpClientHandler);
            // 增加头部
            httpCient.DefaultRequestHeaders.Add("Accept", "*/*");
            httpCient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            httpCient.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");

            ////string guidStr = Guid.NewGuid().ToString().ToUpper();
            ////string guidStr1 = Guid.NewGuid().ToString().ToUpper();
            //string dateTime = DateTime.Now.AddDays(-random.Next(10)).ToString();
            //string cookie = string.Format("fvlid={1}; " +
            //    "sessionid={2}{6} " +
            //    "autoid={7}; " +
            //    "ahpau=1; __ah_uuid_ng=c_{3}; " +
            //    "sessionip={0}; " +
            //    "sessionvid=681B9A1B-2EC5-4194-8437-33DEFB091DEA; " +
            //    "area=119999; v_no=0; visit_info_ad={4}||681B9A1B-2EC5-4194-8437-33DEFB091DEA||-1||-1||4; ref={5}"
            //    , mdi.outip, mdi.fvlid,
            //    this.guidStr, this.guidStr, this.guidStr,
            //    HttpUtility.UrlEncode(string.Format("0|0|0|0|{0}|{1}", DateTime.Now.ToString(), dateTime))
            //    , HttpUtility.UrlEncode(string.Format("||{0}||0; ", dateTime)),
            //    Z.GetMD5LowerString(this.guidStr)
            //    );
            //string cookie = string.Format("fvlid={1}; " +
            //  "sessionid=C0B882EA-FEDC-43CA-8E9F-B322F35528C8%7C%7C2020-07-25+12%3A38%3A50.381%7C%7C0; " +
            //  "autoid=c933fac8868713f3f0e2d3d4b83f16b0; " +
            //  "ahpau=1; __ah_uuid_ng=c_C0B882EA-FEDC-43CA-8E9F-B322F35528C8; " +
            //  "sessionip={0}; " +
            //  "sessionvid=681B9A1B-2EC5-4194-8437-33DEFB091DEA; " +
            //  "area=119999; v_no=0; visit_info_ad=C0B882EA-FEDC-43CA-8E9F-B322F35528C8||681B9A1B-2EC5-4194-8437-33DEFB091DEA||-1||-1||4; ref=0%7C0%7C0%7C0%7C2020-08-02+08%3A00%3A14.918%7C2020-07-25+12%3A38%3A50.381"
            //  , mdi.outip, mdi.fvlid);

            //this.guidStr = Guid.NewGuid().ToString().ToUpper();
            //string guidStr1 = Guid.NewGuid().ToString().ToUpper();
            string dateTime = DateTime.Now.AddDays(-random.Next(10)).ToString();
            //string cookie = string.Format("fvlid={1}; " +
            //    "sessionid={2}{6} " +
            //    "autoid={7}; " +
            //    "ahpau=1; __ah_uuid_ng=c_{3}; " +
            //    "sessionip={0}; " +
            //    "sessionvid={8}; " +
            //    "area={10}; v_no=1; visit_info_ad={4}||{9}||-1||-1||1; ref={5}"
            //    , mdi.outip, mdi.fvlid,
            //    this.guidStr, this.guidStr, this.guidStr,
            //    HttpUtility.UrlEncode(string.Format("0|0|0|0|{0}|{1}", DateTime.Now.ToString(), dateTime))
            //    , HttpUtility.UrlEncode(string.Format("||{0}||0; ", dateTime)),
            //    Z.GetMD5LowerString(this.guidStr), this.guidStr1, this.guidStr1,
            //    cityareas[random2.Next(3293)]
            //    );
            string cookie = "sm_diu=d511d92ecd2fcc63edaa1c7cd4066f3d%7C%7C11eef1ee4efd303772%7C1609310502";
            #region 出流量策略
            //如有问题更换  guidStr 和 guidStr1  为最新未登录的值；调整proxy_rate:30,time_space_multi:3
            #endregion
            //string cookie = string.Format("fvlid={1}; " +
            //   "sessionid=EC51C026-F4BB-4AB0-9872-993F6CFF34A8%7C%7C2020-10-14+18%3A20%3A47.654%7C%7C0; " +
            //   "autoid=31bf984e655c9fe48f14c7176521ee08; " +
            //   "ahpau=1; __ah_uuid_ng=c_EC51C026-F4BB-4AB0-9872-993F6CFF34A8; " +
            //   "sessionip={0}; " +
            //   "sessionvid=52CD437F-1054-4AD5-A7F4-345B803C41AA; " +
            //   "area=330199; v_no=1; visit_info_ad=EC51C026-F4BB-4AB0-9872-993F6CFF34A8||52CD437F-1054-4AD5-A7F4-345B803C41AA||-1||-1||1; ref=0%7C0%7C0%7C0%7C2020-12-13+15%3A16%3A28.794%7C2020-10-14+18%3A20%3A47.654"
            //   , mdi.outip, mdi.fvlid);
            httpCient.DefaultRequestHeaders.Add("Cookie", cookie);
            httpCient.DefaultRequestHeaders.Add("Host", "log.m.sm.cn");
            httpCient.DefaultRequestHeaders.Add("Referer", "https://quark.sm.cn/");
            httpCient.DefaultRequestHeaders.Add("User-Agent", string.Format("Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.{0}.77 Safari/537.36", 3000 + random1.Next()));
            return httpCient;
        }

        private void SetCookie(HttpClient httpClient, RequestUrlAndReferer requestUrlAndReferer, IPMetaDataItem mdi)
        {
            httpClient.DefaultRequestHeaders.Remove("Cookie");

            //this.guidStr = Guid.NewGuid().ToString().ToUpper();
            ////string guidStr1 = Guid.NewGuid().ToString().ToUpper();
            //string dateTime = DateTime.Now.AddDays(-random.Next(10)).ToString();
            //string cookie = string.Format("fvlid={1}; " +
            //    "sessionid={2}{6} " +
            //    "autoid={7}; " +
            //    "ahpau=1; __ah_uuid_ng=c_{3}; " +
            //    "sessionip={0}; " +
            //    "sessionvid=681B9A1B-2EC5-4194-8437-33DEFB091DEA; " +
            //    "area=119999; v_no=0; visit_info_ad={4}||681B9A1B-2EC5-4194-8437-33DEFB091DEA||-1||-1||4; ref={5}"
            //    , mdi.outip, mdi.fvlid,
            //    this.guidStr, this.guidStr, this.guidStr,
            //    HttpUtility.UrlEncode(string.Format("0|0|0|0|{0}|{1}", DateTime.Now.ToString(), dateTime))
            //    , HttpUtility.UrlEncode(string.Format("||{0}||0; ", dateTime)),
            //    Z.GetMD5LowerString(this.guidStr)
            //    );

            //string cookie = string.Format("fvlid={1}; " +
            //    "sessionid=C0B882EA-FEDC-43CA-8E9F-B322F35528C8%7C%7C2020-07-25+12%3A38%3A50.381%7C%7C0; " +
            //    "autoid=c933fac8868713f3f0e2d3d4b83f16b0; " +
            //    "ahpau=1; __ah_uuid_ng=c_C0B882EA-FEDC-43CA-8E9F-B322F35528C8; " +
            //    "sessionip={0}; " +
            //    "sessionvid=681B9A1B-2EC5-4194-8437-33DEFB091DEA; " +
            //    "area=119999; v_no=0; visit_info_ad=C0B882EA-FEDC-43CA-8E9F-B322F35528C8||681B9A1B-2EC5-4194-8437-33DEFB091DEA||-1||-1||4; ref=0%7C0%7C0%7C0%7C2020-08-02+08%3A00%3A14.918%7C2020-07-25+12%3A38%3A50.381"
            //    , mdi.outip, requestUrlAndReferer.fvlid);

            //this.guidStr = Guid.NewGuid().ToString().ToUpper();
            //string guidStr1 = Guid.NewGuid().ToString().ToUpper();
            string dateTime = DateTime.Now.AddDays(-random.Next(10)).ToString();
            string cookie = string.Format("fvlid={1}; " +
                "sessionid={2}{6} " +
                "autoid={7}; " +
                "ahpau=1; __ah_uuid_ng=c_{3}; " +
                "sessionip={0}; " +
                "sessionvid={8}; " +
                "area={10}; v_no=1; visit_info_ad={4}||{9}||-1||-1||1; ref={5}"
                , mdi.outip, mdi.fvlid,
                this.guidStr, this.guidStr, this.guidStr,
                HttpUtility.UrlEncode(string.Format("0|0|0|0|{0}|{1}", DateTime.Now.ToString(), dateTime))
                , HttpUtility.UrlEncode(string.Format("||{0}||0; ", dateTime)),
                Z.GetMD5LowerString(this.guidStr),this.guidStr1,this.guidStr1,
                cityareas[random2.Next(3293)]
                );

            //string cookie = string.Format("fvlid={1}; " +
            //   "sessionid=EC51C026-F4BB-4AB0-9872-993F6CFF34A8%7C%7C2020-10-14+18%3A20%3A47.654%7C%7C0; " +
            //   "autoid=31bf984e655c9fe48f14c7176521ee08; " +
            //   "ahpau=1; __ah_uuid_ng=c_EC51C026-F4BB-4AB0-9872-993F6CFF34A8; " +
            //   "sessionip={0}; " +
            //   "sessionvid=52CD437F-1054-4AD5-A7F4-345B803C41AA; " +
            //   "area=330199; v_no=1; visit_info_ad=EC51C026-F4BB-4AB0-9872-993F6CFF34A8||52CD437F-1054-4AD5-A7F4-345B803C41AA||-1||-1||1; ref=0%7C0%7C0%7C0%7C2020-12-13+15%3A16%3A28.794%7C2020-10-14+18%3A20%3A47.654"
            //   , mdi.outip, mdi.fvlid);
            httpClient.DefaultRequestHeaders.Add("Cookie", cookie);
            httpClient.DefaultRequestHeaders.Remove("Referer");
            httpClient.DefaultRequestHeaders.Add("Referer", requestUrlAndReferer.referer);
        }
        #endregion


        static int[] cityareas = new int[] 
        {
            110101,
110102,
110105,
110106,
110107,
110108,
110109,
110111,
110112,
110113,
110114,
110115,
110116,
110117,
110118,
110119,
120101,
120102,
120103,
120104,
120105,
120106,
120110,
120111,
120112,
120113,
120114,
120115,
120116,
120117,
120118,
120225,
130102,
130104,
130105,
130107,
130108,
130109,
130110,
130111,
130121,
130123,
130125,
130126,
130127,
130128,
130129,
130130,
130131,
130132,
130133,
130181,
130183,
130184,
130202,
130203,
130204,
130205,
130207,
130208,
130209,
130223,
130224,
130225,
130227,
130229,
130281,
130283,
130302,
130303,
130304,
130306,
130321,
130322,
130324,
130402,
130403,
130404,
130406,
130423,
130424,
130425,
130426,
130427,
130428,
130429,
130430,
130431,
130432,
130433,
130434,
130435,
130481,
130502,
130503,
130521,
130522,
130523,
130524,
130525,
130526,
130527,
130528,
130529,
130530,
130531,
130532,
130533,
130534,
130535,
130581,
130582,
130602,
130606,
130607,
130608,
130609,
130623,
130624,
130626,
130627,
130628,
130629,
130630,
130631,
130632,
130633,
130634,
130635,
130636,
130637,
130638,
130681,
130682,
130683,
130684,
130702,
130703,
130705,
130706,
130708,
130709,
130722,
130723,
130724,
130725,
130726,
130727,
130728,
130730,
130731,
130732,
130802,
130803,
130804,
130821,
130822,
130823,
130824,
130825,
130826,
130827,
130828,
130902,
130903,
130921,
130922,
130923,
130924,
130925,
130926,
130927,
130928,
130929,
130930,
130981,
130982,
130983,
130984,
131002,
131003,
131022,
131023,
131024,
131025,
131026,
131028,
131081,
131082,
131102,
131103,
131121,
131122,
131123,
131124,
131125,
131126,
131127,
131128,
131182,
140105,
140106,
140107,
140108,
140109,
140110,
140121,
140122,
140123,
140181,
140202,
140203,
140211,
140212,
140221,
140222,
140223,
140224,
140225,
140226,
140227,
140302,
140303,
140311,
140321,
140322,
140402,
140411,
140421,
140423,
140424,
140425,
140426,
140427,
140428,
140429,
140430,
140431,
140481,
140502,
140521,
140522,
140524,
140525,
140581,
140602,
140603,
140621,
140622,
140623,
140624,
140702,
140721,
140722,
140723,
140724,
140725,
140726,
140727,
140728,
140729,
140781,
140802,
140821,
140822,
140823,
140824,
140825,
140826,
140827,
140828,
140829,
140830,
140881,
140882,
140902,
140921,
140922,
140923,
140924,
140925,
140926,
140927,
140928,
140929,
140930,
140931,
140932,
140981,
141002,
141021,
141022,
141023,
141024,
141025,
141026,
141027,
141028,
141029,
141030,
141031,
141032,
141033,
141034,
141081,
141082,
141102,
141121,
141122,
141123,
141124,
141125,
141126,
141127,
141128,
141129,
141130,
141181,
141182,
150102,
150103,
150104,
150105,
150121,
150122,
150123,
150124,
150125,
150202,
150203,
150204,
150205,
150206,
150207,
150221,
150222,
150223,
150302,
150303,
150304,
150402,
150403,
150404,
150421,
150422,
150423,
150424,
150425,
150426,
150428,
150429,
150430,
150502,
150521,
150522,
150523,
150524,
150525,
150526,
150581,
150602,
150621,
150622,
150623,
150624,
150625,
150626,
150627,
150628,
150702,
150703,
150721,
150722,
150723,
150724,
150725,
150726,
150727,
150781,
150782,
150783,
150784,
150785,
150802,
150821,
150822,
150823,
150824,
150825,
150826,
150902,
150921,
150922,
150923,
150924,
150925,
150926,
150927,
150928,
150929,
150981,
152201,
152202,
152221,
152222,
152223,
152224,
152501,
152502,
152522,
152523,
152524,
152525,
152526,
152527,
152528,
152529,
152530,
152531,
152921,
152922,
152923,
210102,
210103,
210104,
210105,
210106,
210111,
210112,
210113,
210114,
210115,
210123,
210124,
210181,
210202,
210203,
210204,
210211,
210212,
210213,
210214,
210224,
210281,
210283,
210302,
210303,
210304,
210311,
210321,
210323,
210381,
210402,
210403,
210404,
210411,
210421,
210422,
210423,
210502,
210503,
210504,
210505,
210521,
210522,
210602,
210603,
210604,
210624,
210681,
210682,
210702,
210703,
210711,
210726,
210727,
210781,
210782,
210802,
210803,
210804,
210811,
210881,
210882,
210902,
210903,
210904,
210905,
210911,
210921,
210922,
211002,
211003,
211004,
211005,
211011,
211021,
211081,
211102,
211103,
211104,
211122,
211202,
211204,
211221,
211223,
211224,
211281,
211282,
211302,
211303,
211321,
211322,
211324,
211381,
211382,
211402,
211403,
211404,
211421,
211422,
211481,
220102,
220103,
220104,
220105,
220106,
220112,
220113,
220122,
220182,
220183,
220202,
220203,
220204,
220211,
220221,
220281,
220282,
220283,
220284,
220302,
220303,
220322,
220323,
220381,
220382,
220402,
220403,
220421,
220422,
220502,
220503,
220521,
220523,
220524,
220581,
220582,
220602,
220605,
220621,
220622,
220623,
220681,
220702,
220721,
220722,
220723,
220781,
220802,
220821,
220822,
220881,
220882,
222401,
222402,
222403,
222404,
222405,
222406,
222424,
222426,
230102,
230103,
230104,
230108,
230109,
230110,
230111,
230112,
230113,
230123,
230124,
230125,
230126,
230127,
230128,
230129,
230183,
230184,
230202,
230203,
230204,
230205,
230206,
230207,
230208,
230221,
230223,
230224,
230225,
230227,
230229,
230230,
230231,
230281,
230302,
230303,
230304,
230305,
230306,
230307,
230321,
230381,
230382,
230402,
230403,
230404,
230405,
230406,
230407,
230421,
230422,
230502,
230503,
230505,
230506,
230521,
230522,
230523,
230524,
230602,
230603,
230604,
230605,
230606,
230621,
230622,
230623,
230624,
230702,
230703,
230704,
230705,
230706,
230707,
230708,
230709,
230710,
230711,
230712,
230713,
230714,
230715,
230716,
230722,
230781,
230803,
230804,
230805,
230811,
230822,
230826,
230828,
230881,
230882,
230883,
230902,
230903,
230904,
230921,
231002,
231003,
231004,
231005,
231025,
231081,
231083,
231084,
231085,
231086,
231102,
231121,
231123,
231124,
231181,
231182,
231202,
231221,
231222,
231223,
231224,
231225,
231226,
231281,
231282,
231283,
232701,
232703,
232704,
232721,
232722,
232723,
310101,
310104,
310105,
310106,
310107,
310109,
310110,
310112,
310113,
310114,
310115,
310116,
310117,
310118,
310120,
310151,
320102,
320104,
320105,
320106,
320111,
320113,
320114,
320115,
320116,
320117,
320118,
320205,
320206,
320211,
320213,
320214,
320281,
320282,
320302,
320303,
320305,
320311,
320312,
320321,
320322,
320324,
320381,
320382,
320402,
320404,
320411,
320412,
320413,
320481,
320505,
320506,
320507,
320508,
320509,
320581,
320582,
320583,
320585,
320602,
320611,
320612,
320621,
320623,
320681,
320682,
320684,
320703,
320706,
320707,
320722,
320723,
320724,
320803,
320804,
320811,
320813,
320826,
320830,
320831,
320902,
320903,
320904,
320921,
320922,
320923,
320924,
320925,
320981,
321002,
321003,
321012,
321023,
321081,
321084,
321102,
321111,
321112,
321181,
321182,
321183,
321202,
321203,
321204,
321281,
321282,
321283,
321302,
321311,
321322,
321323,
321324,
330102,
330103,
330104,
330105,
330106,
330108,
330109,
330110,
330111,
330122,
330127,
330182,
330185,
330203,
330205,
330206,
330211,
330212,
330225,
330226,
330281,
330282,
330283,
330302,
330303,
330304,
330305,
330324,
330326,
330327,
330328,
330329,
330381,
330382,
330402,
330411,
330421,
330424,
330481,
330482,
330483,
330502,
330503,
330521,
330522,
330523,
330602,
330603,
330604,
330624,
330681,
330683,
330702,
330703,
330723,
330726,
330727,
330781,
330782,
330783,
330784,
330802,
330803,
330822,
330824,
330825,
330881,
330902,
330903,
330921,
330922,
331002,
331003,
331004,
331021,
331022,
331023,
331024,
331081,
331082,
331102,
331121,
331122,
331123,
331124,
331125,
331126,
331127,
331181,
340102,
340103,
340104,
340111,
340121,
340122,
340123,
340124,
340181,
340202,
340203,
340207,
340208,
340221,
340222,
340223,
340225,
340302,
340303,
340304,
340311,
340321,
340322,
340323,
340402,
340403,
340404,
340405,
340406,
340421,
340422,
340503,
340504,
340506,
340521,
340522,
340523,
340602,
340603,
340604,
340621,
340705,
340706,
340711,
340722,
340802,
340803,
340811,
340822,
340824,
340825,
340826,
340827,
340828,
340881,
341002,
341003,
341004,
341021,
341022,
341023,
341024,
341102,
341103,
341122,
341124,
341125,
341126,
341181,
341182,
341202,
341203,
341204,
341221,
341222,
341225,
341226,
341282,
341302,
341321,
341322,
341323,
341324,
341502,
341503,
341504,
341522,
341523,
341524,
341525,
341602,
341621,
341622,
341623,
341702,
341721,
341722,
341723,
341802,
341821,
341822,
341823,
341824,
341825,
341881,
350102,
350103,
350104,
350105,
350111,
350121,
350122,
350123,
350124,
350125,
350128,
350181,
350182,
350203,
350205,
350206,
350211,
350212,
350213,
350302,
350303,
350304,
350305,
350322,
350402,
350403,
350421,
350423,
350424,
350425,
350426,
350427,
350428,
350429,
350430,
350481,
350502,
350503,
350504,
350505,
350521,
350524,
350525,
350526,
350527,
350581,
350582,
350583,
350602,
350603,
350622,
350623,
350624,
350625,
350626,
350627,
350628,
350629,
350681,
350702,
350703,
350721,
350722,
350723,
350724,
350725,
350781,
350782,
350783,
350802,
350803,
350821,
350823,
350824,
350825,
350881,
350902,
350921,
350922,
350923,
350924,
350925,
350926,
350981,
350982,
360102,
360103,
360104,
360105,
360111,
360112,
360121,
360123,
360124,
360202,
360203,
360222,
360281,
360302,
360313,
360321,
360322,
360323,
360402,
360403,
360421,
360423,
360424,
360425,
360426,
360428,
360429,
360430,
360481,
360482,
360483,
360502,
360521,
360602,
360622,
360681,
360702,
360703,
360721,
360722,
360723,
360724,
360725,
360726,
360727,
360728,
360729,
360730,
360731,
360732,
360733,
360734,
360735,
360781,
360802,
360803,
360821,
360822,
360823,
360824,
360825,
360826,
360827,
360828,
360829,
360830,
360881,
360902,
360921,
360922,
360923,
360924,
360925,
360926,
360981,
360982,
360983,
361002,
361021,
361022,
361023,
361024,
361025,
361026,
361027,
361028,
361029,
361030,
361102,
361103,
361121,
361123,
361124,
361125,
361126,
361127,
361128,
361129,
361130,
361181,
370102,
370103,
370104,
370105,
370112,
370113,
370124,
370125,
370126,
370181,
370202,
370203,
370211,
370212,
370213,
370214,
370281,
370282,
370283,
370285,
370302,
370303,
370304,
370305,
370306,
370321,
370322,
370323,
370402,
370403,
370404,
370405,
370406,
370481,
370502,
370503,
370505,
370522,
370523,
370602,
370611,
370612,
370613,
370634,
370681,
370682,
370683,
370684,
370685,
370686,
370687,
370702,
370703,
370704,
370705,
370724,
370725,
370781,
370782,
370783,
370784,
370785,
370786,
370811,
370812,
370826,
370827,
370828,
370829,
370830,
370831,
370832,
370881,
370883,
370902,
370911,
370921,
370923,
370982,
370983,
371002,
371003,
371082,
371083,
371102,
371103,
371121,
371122,
371202,
371203,
371302,
371311,
371312,
371321,
371322,
371323,
371324,
371325,
371326,
371327,
371328,
371329,
371402,
371403,
371422,
371423,
371424,
371425,
371426,
371427,
371428,
371481,
371482,
371502,
371521,
371522,
371523,
371524,
371525,
371526,
371581,
371602,
371603,
371621,
371622,
371623,
371625,
371626,
371702,
371703,
371721,
371722,
371723,
371724,
371725,
371726,
371728,
410102,
410103,
410104,
410105,
410106,
410108,
410122,
410181,
410182,
410183,
410184,
410185,
410202,
410203,
410204,
410205,
410212,
410221,
410222,
410223,
410225,
410302,
410303,
410304,
410305,
410306,
410311,
410322,
410323,
410324,
410325,
410326,
410327,
410328,
410329,
410381,
410402,
410403,
410404,
410411,
410421,
410422,
410423,
410425,
410481,
410482,
410502,
410503,
410505,
410506,
410522,
410523,
410526,
410527,
410581,
410602,
410603,
410611,
410621,
410622,
410702,
410703,
410704,
410711,
410721,
410724,
410725,
410726,
410727,
410728,
410781,
410782,
410802,
410803,
410804,
410811,
410821,
410822,
410823,
410825,
410882,
410883,
410902,
410922,
410923,
410926,
410927,
410928,
411002,
411023,
411024,
411025,
411081,
411082,
411102,
411103,
411104,
411121,
411122,
411202,
411203,
411221,
411224,
411281,
411282,
411302,
411303,
411321,
411322,
411323,
411324,
411325,
411326,
411327,
411328,
411329,
411330,
411381,
411402,
411403,
411421,
411422,
411423,
411424,
411425,
411426,
411481,
411502,
411503,
411521,
411522,
411523,
411524,
411525,
411526,
411527,
411528,
411602,
411621,
411622,
411623,
411624,
411625,
411626,
411627,
411628,
411681,
411702,
411721,
411722,
411723,
411724,
411725,
411726,
411727,
411728,
411729,
419001,
420102,
420103,
420104,
420105,
420106,
420107,
420111,
420112,
420113,
420114,
420115,
420116,
420117,
420202,
420203,
420204,
420205,
420222,
420281,
420302,
420303,
420304,
420322,
420323,
420324,
420325,
420381,
420502,
420503,
420504,
420505,
420506,
420525,
420526,
420527,
420528,
420529,
420581,
420582,
420583,
420602,
420606,
420607,
420624,
420625,
420626,
420682,
420683,
420684,
420702,
420703,
420704,
420802,
420804,
420821,
420822,
420881,
420902,
420921,
420922,
420923,
420981,
420982,
420984,
421002,
421003,
421022,
421023,
421024,
421081,
421083,
421087,
421102,
421121,
421122,
421123,
421124,
421125,
421126,
421127,
421181,
421182,
421202,
421221,
421222,
421223,
421224,
421281,
421303,
421321,
421381,
422801,
422802,
422822,
422823,
422825,
422826,
422827,
422828,
429004,
429005,
429006,
429021,
430102,
430103,
430104,
430105,
430111,
430112,
430121,
430124,
430181,
430202,
430203,
430204,
430211,
430221,
430223,
430224,
430225,
430281,
430302,
430304,
430321,
430381,
430382,
430405,
430406,
430407,
430408,
430412,
430421,
430422,
430423,
430424,
430426,
430481,
430482,
430502,
430503,
430511,
430521,
430522,
430523,
430524,
430525,
430527,
430528,
430529,
430581,
430602,
430603,
430611,
430621,
430623,
430624,
430626,
430681,
430682,
430702,
430703,
430721,
430722,
430723,
430724,
430725,
430726,
430781,
430802,
430811,
430821,
430822,
430902,
430903,
430921,
430922,
430923,
430981,
431002,
431003,
431021,
431022,
431023,
431024,
431025,
431026,
431027,
431028,
431081,
431102,
431103,
431121,
431122,
431123,
431124,
431125,
431126,
431127,
431128,
431129,
431202,
431221,
431222,
431223,
431224,
431225,
431226,
431227,
431228,
431229,
431230,
431281,
431302,
431321,
431322,
431381,
431382,
433101,
433122,
433123,
433124,
433125,
433126,
433127,
433130,
440103,
440104,
440105,
440106,
440111,
440112,
440113,
440114,
440115,
440117,
440118,
440203,
440204,
440205,
440222,
440224,
440229,
440232,
440233,
440281,
440282,
440303,
440304,
440305,
440306,
440307,
440308,
440309,
440310,
440402,
440403,
440404,
440507,
440511,
440512,
440513,
440514,
440515,
440523,
440604,
440605,
440606,
440607,
440608,
440703,
440704,
440705,
440781,
440783,
440784,
440785,
440802,
440803,
440804,
440811,
440823,
440825,
440881,
440882,
440883,
440902,
440904,
440981,
440982,
440983,
441202,
441203,
441204,
441223,
441224,
441225,
441226,
441284,
441302,
441303,
441322,
441323,
441324,
441402,
441403,
441422,
441423,
441424,
441426,
441427,
441481,
441502,
441521,
441523,
441581,
441602,
441621,
441622,
441623,
441624,
441625,
441702,
441704,
441721,
441781,
441802,
441803,
441821,
441823,
441825,
441826,
441881,
441882,
441901,
441902,
441903,
441904,
441905,
441906,
441907,
441908,
441909,
441910,
441911,
441912,
441913,
441914,
441915,
441916,
441917,
441918,
441919,
441920,
441921,
441922,
441923,
441924,
441925,
441926,
441927,
441928,
441929,
441930,
441931,
441932,
442001,
445102,
445103,
445122,
445202,
445203,
445222,
445224,
445281,
445302,
445303,
445321,
445322,
445381,
450102,
450103,
450105,
450107,
450108,
450109,
450110,
450123,
450124,
450125,
450126,
450127,
450202,
450203,
450204,
450205,
450221,
450222,
450223,
450224,
450225,
450226,
450302,
450303,
450304,
450305,
450311,
450312,
450321,
450323,
450324,
450325,
450326,
450327,
450328,
450329,
450330,
450331,
450332,
450403,
450405,
450406,
450421,
450422,
450423,
450481,
450502,
450503,
450512,
450521,
450602,
450603,
450621,
450681,
450702,
450703,
450721,
450722,
450802,
450803,
450804,
450821,
450881,
450902,
450903,
450921,
450922,
450923,
450924,
450981,
451002,
451021,
451022,
451023,
451024,
451026,
451027,
451028,
451029,
451030,
451031,
451081,
451102,
451103,
451121,
451122,
451123,
451202,
451221,
451222,
451223,
451224,
451225,
451226,
451227,
451228,
451229,
451281,
451302,
451321,
451322,
451323,
451324,
451381,
451402,
451421,
451422,
451423,
451424,
451425,
451481,
460105,
460106,
460107,
460108,
460202,
460203,
460204,
460205,
460321,
460322,
460323,
460400,
469001,
469002,
469005,
469006,
469007,
469021,
469022,
469023,
469024,
469025,
469026,
469027,
469028,
469029,
469030,
500101,
500102,
500103,
500104,
500105,
500106,
500107,
500108,
500109,
500110,
500111,
500112,
500113,
500114,
500115,
500116,
500117,
500118,
500119,
500120,
500151,
500152,
500153,
500154,
500228,
500229,
500230,
500231,
500232,
500233,
500235,
500236,
500237,
500238,
500240,
500241,
500242,
500243,
510104,
510105,
510106,
510107,
510108,
510112,
510113,
510114,
510115,
510116,
510121,
510124,
510129,
510131,
510132,
510181,
510182,
510183,
510184,
510185,
510302,
510303,
510304,
510311,
510321,
510322,
510402,
510403,
510411,
510421,
510422,
510502,
510503,
510504,
510521,
510522,
510524,
510525,
510603,
510623,
510626,
510681,
510682,
510683,
510703,
510704,
510705,
510722,
510723,
510725,
510726,
510727,
510781,
510802,
510811,
510812,
510821,
510822,
510823,
510824,
510903,
510904,
510921,
510922,
510923,
511002,
511011,
511024,
511025,
511028,
511102,
511111,
511112,
511113,
511123,
511124,
511126,
511129,
511132,
511133,
511181,
511302,
511303,
511304,
511321,
511322,
511323,
511324,
511325,
511381,
511402,
511403,
511421,
511423,
511424,
511425,
511502,
511503,
511521,
511523,
511524,
511525,
511526,
511527,
511528,
511529,
511602,
511603,
511621,
511622,
511623,
511681,
511702,
511703,
511722,
511723,
511724,
511725,
511781,
511802,
511803,
511822,
511823,
511824,
511825,
511826,
511827,
511902,
511903,
511921,
511922,
511923,
512002,
512021,
512022,
513201,
513221,
513222,
513223,
513224,
513225,
513226,
513227,
513228,
513230,
513231,
513232,
513233,
513301,
513322,
513323,
513324,
513325,
513326,
513327,
513328,
513329,
513330,
513331,
513332,
513333,
513334,
513335,
513336,
513337,
513338,
513401,
513422,
513423,
513424,
513425,
513426,
513427,
513428,
513429,
513430,
513431,
513432,
513433,
513434,
513435,
513436,
513437,
520102,
520103,
520111,
520112,
520113,
520115,
520121,
520122,
520123,
520181,
520201,
520203,
520221,
520222,
520302,
520303,
520304,
520322,
520323,
520324,
520325,
520326,
520327,
520328,
520329,
520330,
520381,
520382,
520402,
520403,
520422,
520423,
520424,
520425,
520502,
520521,
520522,
520523,
520524,
520525,
520526,
520527,
520602,
520603,
520621,
520622,
520623,
520624,
520625,
520626,
520627,
520628,
522301,
522322,
522323,
522324,
522325,
522326,
522327,
522328,
522601,
522622,
522623,
522624,
522625,
522626,
522627,
522628,
522629,
522630,
522631,
522632,
522633,
522634,
522635,
522636,
522701,
522702,
522722,
522723,
522725,
522726,
522727,
522728,
522729,
522730,
522731,
522732,
530102,
530103,
530111,
530112,
530113,
530114,
530122,
530124,
530125,
530126,
530127,
530128,
530129,
530181,
530302,
530303,
530321,
530322,
530323,
530324,
530325,
530326,
530381,
530402,
530403,
530422,
530423,
530424,
530425,
530426,
530427,
530428,
530502,
530521,
530523,
530524,
530581,
530602,
530621,
530622,
530623,
530624,
530625,
530626,
530627,
530628,
530629,
530630,
530702,
530721,
530722,
530723,
530724,
530802,
530821,
530822,
530823,
530824,
530825,
530826,
530827,
530828,
530829,
530902,
530921,
530922,
530923,
530924,
530925,
530926,
530927,
532301,
532322,
532323,
532324,
532325,
532326,
532327,
532328,
532329,
532331,
532501,
532502,
532503,
532504,
532523,
532524,
532525,
532527,
532528,
532529,
532530,
532531,
532532,
532601,
532622,
532623,
532624,
532625,
532626,
532627,
532628,
532801,
532822,
532823,
532901,
532922,
532923,
532924,
532925,
532926,
532927,
532928,
532929,
532930,
532931,
532932,
533102,
533103,
533122,
533123,
533124,
533301,
533323,
533324,
533325,
533401,
533422,
533423,
540102,
540103,
540121,
540122,
540123,
540124,
540126,
540127,
540202,
540221,
540222,
540223,
540224,
540225,
540226,
540227,
540228,
540229,
540230,
540231,
540232,
540233,
540234,
540235,
540236,
540237,
540302,
540321,
540322,
540323,
540324,
540325,
540326,
540327,
540328,
540329,
540330,
540402,
540421,
540422,
540423,
540424,
540425,
540426,
540502,
540521,
540522,
540523,
540524,
540525,
540526,
540527,
540528,
540529,
540530,
540531,
542421,
542422,
542423,
542424,
542425,
542426,
542427,
542428,
542429,
542430,
542431,
542521,
542522,
542523,
542524,
542525,
542526,
542527,
610102,
610103,
610104,
610111,
610112,
610113,
610114,
610115,
610116,
610117,
610122,
610124,
610125,
610202,
610203,
610204,
610222,
610302,
610303,
610304,
610322,
610323,
610324,
610326,
610327,
610328,
610329,
610330,
610331,
610402,
610403,
610404,
610422,
610423,
610424,
610425,
610426,
610427,
610428,
610429,
610430,
610431,
610481,
610502,
610503,
610522,
610523,
610524,
610525,
610526,
610527,
610528,
610581,
610582,
610602,
610621,
610622,
610623,
610624,
610625,
610626,
610627,
610628,
610629,
610630,
610631,
610632,
610702,
610721,
610722,
610723,
610724,
610725,
610726,
610727,
610728,
610729,
610730,
610802,
610803,
610821,
610822,
610824,
610825,
610826,
610827,
610828,
610829,
610830,
610831,
610902,
610921,
610922,
610923,
610924,
610925,
610926,
610927,
610928,
610929,
611002,
611021,
611022,
611023,
611024,
611025,
611026,
620102,
620103,
620104,
620105,
620111,
620121,
620122,
620123,
620201,
620302,
620321,
620402,
620403,
620421,
620422,
620423,
620502,
620503,
620521,
620522,
620523,
620524,
620525,
620602,
620621,
620622,
620623,
620702,
620721,
620722,
620723,
620724,
620725,
620802,
620821,
620822,
620823,
620824,
620825,
620826,
620902,
620921,
620922,
620923,
620924,
620981,
620982,
621002,
621021,
621022,
621023,
621024,
621025,
621026,
621027,
621102,
621121,
621122,
621123,
621124,
621125,
621126,
621202,
621221,
621222,
621223,
621224,
621225,
621226,
621227,
621228,
622901,
622921,
622922,
622923,
622924,
622925,
622926,
622927,
623001,
623021,
623022,
623023,
623024,
623025,
623026,
623027,
630102,
630103,
630104,
630105,
630121,
630122,
630123,
630202,
630203,
630222,
630223,
630224,
630225,
632221,
632222,
632223,
632224,
632321,
632322,
632323,
632324,
632521,
632522,
632523,
632524,
632525,
632621,
632622,
632623,
632624,
632625,
632626,
632701,
632722,
632723,
632724,
632725,
632726,
632801,
632802,
632821,
632822,
632823,
640104,
640105,
640106,
640121,
640122,
640181,
640202,
640205,
640221,
640302,
640303,
640323,
640324,
640381,
640402,
640422,
640423,
640424,
640425,
640502,
640521,
640522,
650102,
650103,
650104,
650105,
650106,
650107,
650109,
650121,
650202,
650203,
650204,
650205,
650402,
650421,
650422,
650502,
650521,
650522,
652301,
652302,
652323,
652324,
652325,
652327,
652328,
652701,
652702,
652722,
652723,
652801,
652822,
652823,
652824,
652825,
652826,
652827,
652828,
652829,
652901,
652922,
652923,
652924,
652925,
652926,
652927,
652928,
652929,
653001,
653022,
653023,
653024,
653101,
653121,
653122,
653123,
653124,
653125,
653126,
653127,
653128,
653129,
653130,
653131,
653201,
653221,
653222,
653223,
653224,
653225,
653226,
653227,
654002,
654003,
654004,
654021,
654022,
654023,
654024,
654025,
654026,
654027,
654028,
654201,
654202,
654221,
654223,
654224,
654225,
654226,
654301,
654321,
654322,
654323,
654324,
654325,
654326,
659001,
659002,
659005,
659006,
659007,
659008,
659009,
659301,
659401,
710101,
810101,
820101,
110100,
120100,
130100,
130200,
130300,
130400,
130500,
130600,
130700,
130800,
130900,
131000,
131100,
139900,
140100,
140200,
140300,
140400,
140500,
140600,
140700,
140800,
140900,
141000,
141100,
149900,
150100,
150200,
150300,
150400,
150500,
150600,
150700,
150800,
150900,
152200,
152500,
152900,
159900,
210100,
210200,
210300,
210400,
210500,
210600,
210700,
210800,
210900,
211000,
211100,
211200,
211300,
211400,
219900,
220100,
220200,
220300,
220400,
220500,
220600,
220700,
220800,
222400,
229900,
230100,
230200,
230300,
230400,
230500,
230600,
230700,
230800,
230900,
231000,
231100,
231200,
232700,
239900,
310100,
320100,
320200,
320300,
320400,
320500,
320600,
320700,
320800,
320900,
321000,
321100,
321200,
321300,
329900,
330100,
330200,
330300,
330400,
330500,
330600,
330700,
330800,
330900,
331000,
331100,
339900,
340100,
340200,
340300,
340400,
340500,
340600,
340700,
340800,
341000,
341100,
341200,
341300,
341500,
341600,
341700,
341800,
349900,
350100,
350200,
350300,
350400,
350500,
350600,
350700,
350800,
350900,
359900,
360100,
360200,
360300,
360400,
360500,
360600,
360700,
360800,
360900,
361000,
361100,
369900,
370100,
370200,
370300,
370400,
370500,
370600,
370700,
370800,
370900,
371000,
371100,
371200,
371300,
371400,
371500,
371600,
371700,
379900,
410100,
410200,
410300,
410400,
410500,
410600,
410700,
410800,
410900,
411000,
411100,
411200,
411300,
411400,
411500,
411600,
411700,
419001,
419900,
420100,
420200,
420300,
420500,
420600,
420700,
420800,
420900,
421000,
421100,
421200,
421300,
422800,
429004,
429005,
429006,
429021,
429900,
430100,
430200,
430300,
430400,
430500,
430600,
430700,
430800,
430900,
431000,
431100,
431200,
431300,
433100,
439900,
440100,
440200,
440300,
440400,
440500,
440600,
440700,
440800,
440900,
441200,
441300,
441400,
441500,
441600,
441700,
441800,
441900,
442000,
445100,
445200,
445300,
449900,
450100,
450200,
450300,
450400,
450500,
450600,
450700,
450800,
450900,
451000,
451100,
451200,
451300,
451400,
459900,
460100,
460200,
460300,
460400,
469001,
469002,
469005,
469006,
469007,
469021,
469022,
469023,
469024,
469025,
469026,
469027,
469028,
469029,
469030,
469900,
500100,
510100,
510300,
510400,
510500,
510600,
510700,
510800,
510900,
511000,
511100,
511300,
511400,
511500,
511600,
511700,
511800,
511900,
512000,
513200,
513300,
513400,
519900,
520100,
520200,
520300,
520400,
520500,
520600,
522300,
522600,
522700,
529900,
530100,
530300,
530400,
530500,
530600,
530700,
530800,
530900,
532300,
532500,
532600,
532800,
532900,
533100,
533300,
533400,
539900,
540100,
540200,
540300,
540400,
540500,
542400,
542500,
549900,
610100,
610200,
610300,
610400,
610500,
610600,
610700,
610800,
610900,
611000,
619900,
620100,
620200,
620300,
620400,
620500,
620600,
620700,
620800,
620900,
621000,
621100,
621200,
622900,
623000,
629900,
630100,
630200,
632200,
632300,
632500,
632600,
632700,
632800,
639900,
640100,
640200,
640300,
640400,
640500,
649900,
650100,
650200,
650400,
650500,
652300,
652700,
652800,
652900,
653000,
653100,
653200,
654000,
654200,
654300,
659001,
659002,
659003,
659004,
659005,
659006,
659007,
659008,
659009,
659900,
710100,
810100,
820100,
910100,
999900,

        };
    }

    public class GuidMapper
    {
        public string guid { get; set; }

        public string guid1 { get; set; }
    }
}