using Autohome.Club.Framework;
using River.utils;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace River
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
        static Random random2 = new Random(3);
        string guidStr = "C0B882EA-FEDC-43CA-8E9F-B322F35528C8";
        string guidStr1 = "";

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
                ProcessTradeDataMulti();
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

        static string[] pageurls = new string[] {
            "https://club.m.autohome.com.cn/partner/yidian/thread/91341569",
            "https://club.m.autohome.com.cn/partner/qutoutiao/thread/91341569",
            "https://club.m.autohome.com.cn/partner/uc/thread/91341569" };

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
                                #region pv方式
                                //foreach (var item in mdi.requestUrlAndReferers)
                                //{
                                //    if (item != null && !string.IsNullOrWhiteSpace(item.requesturl))
                                //    {
                                //        string httpResult = "";
                                //        string tempRequest = HttpUtility.UrlDecode(item.requesturl, System.Text.Encoding.UTF8);
                                //        try
                                //        {
                                //            SetCookie(httpClient, item, mdi);
                                //            httpClient.GetAsync(tempRequest);
                                //            httpResult = "dddd" + httpResult;
                                //            this.internalSingleton.RequestCount++;
                                //            //Console.WriteLine(string.Format("*执行总数totalCount:{0}/{1}当前执行数量 /获取资源数量{2}/消耗资源数量{5}/开始时间{3} 结束时间{4}",
                                //            //    this.internalSingleton.RequestTotal,
                                //            //    internalSingleton.RequestCount,
                                //            //    internalSingleton.RequestIpCount,
                                //            //    internalSingleton.BeginTime.ToString("yyyy-MM-dd hh:mm:ss fff"),
                                //            //    DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff"),
                                //            //    this.internalSingleton.ConsumerIpCount));
                                //        }
                                //        catch (Exception e1)
                                //        {
                                //            //Console.WriteLine(string.Format("{0}\n{1}", mdi.ipwithport, e1.StackTrace.ToString()));
                                //        }
                                //        //参照间隔时间Thread.Sleep(20);
                                //        //Thread.Sleep(6);
                                //        Thread.Sleep(ConfigUtls.time_space_multi);
                                //    }
                                //}
                                #endregion
                                //string item = pageurls[random2.Next(3)];
                                foreach (var item in pageurls)
                                {
                                    //if (item != null && !string.IsNullOrWhiteSpace(item.requesturl))
                                    //{
                                    string httpResult = "";
                                    string tempRequest = HttpUtility.UrlDecode(item, System.Text.Encoding.UTF8);
                                    try
                                    {
                                        //SetCookie(httpClient, item, mdi);
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
                                        Console.WriteLine(string.Format("{0}\n{1}", mdi.ipwithport, e1.StackTrace.ToString()));
                                    }
                                    //参照间隔时间Thread.Sleep(20);
                                    //Thread.Sleep(6);
                                    //Thread.Sleep(ConfigUtls.time_space_multi);
                                    //}
                                }
                                //if (ConfigUtls.probe_switch > 0 && DateTime.Now.Minute == 29 && 28 < DateTime.Now.Second && DateTime.Now.Second < 31)
                                //{
                                    //string tempRequest1 = HttpUtility.UrlDecode("https://club.m.autohome.com.cn/partner/oppo/thread/91341569", System.Text.Encoding.UTF8);
                                    //var response = httpClient.GetAsync(tempRequest1).Result;
                                    //mdi.result = string.Format("Ipport {3} -{4} Datetime {2} IsSuccessStatusCode {0} StatusCode {1} Cookie {5} httpContent{6}"
                                    //    , response.IsSuccessStatusCode, response.StatusCode, DateTime.Now, mdi.ipwithport, mdi.outip,
                                    //   JsonConvert.SerializeObject(response), JsonConvert.SerializeObject(response.Content));
                                    //this._storeQueue.Enqueue(mdi);


                                string tempRequest1 = HttpUtility.UrlDecode("https://club.m.autohome.com.cn/partner/oppo/thread/91341569", System.Text.Encoding.UTF8);
                                var response = httpClient.GetStringAsync(tempRequest1).Result;
                                mdi.result = string.Format("Ipport {2} -{3} Datetime {1} response {0}  "
                                    , response, DateTime.Now, mdi.ipwithport, mdi.outip);
                                this._storeQueue.Enqueue(mdi);

                                //}
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
                                Console.WriteLine(string.Format("{0}\n{1}", mdi.ipwithport, e2.StackTrace.ToString()));
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

        static DateTime dt = Convert.ToDateTime("2020-12-15 20:42:05");
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
            string areaid = "119999";
            if (DateTime.Now < dt)//如果相加后的时间大于现在的时间
            {
                string url = string.Format("http://heycar.api.autohome.com.cn/OpenFlipboard/GetIP2Area?ip={0}", mdi.outip);
                areaid = Z.WebRequest(url);
            }

            this.guidStr = Guid.NewGuid().ToString().ToUpper();
            this.guidStr1 = Guid.NewGuid().ToString().ToUpper();
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
                Z.GetMD5LowerString(this.guidStr),
                this.guidStr1, this.guidStr1, areaid
                );

            //string cookie = string.Format("fvlid={1}; " +
            //    "sessionid=C0B882EA-FEDC-43CA-8E9F-B322F35528C8%7C%7C2020-07-25+12%3A38%3A50.381%7C%7C0; " +
            //    "autoid=c933fac8868713f3f0e2d3d4b83f16b0; " +
            //    "ahpau=1; __ah_uuid_ng=c_C0B882EA-FEDC-43CA-8E9F-B322F35528C8; " +
            //    "sessionip={0}; " +
            //    "sessionvid=681B9A1B-2EC5-4194-8437-33DEFB091DEA; " +
            //    "area={2}; v_no=0; visit_info_ad=C0B882EA-FEDC-43CA-8E9F-B322F35528C8||681B9A1B-2EC5-4194-8437-33DEFB091DEA||-1||-1||4; ref=0%7C0%7C0%7C0%7C2020-08-02+08%3A00%3A14.918%7C2020-07-25+12%3A38%3A50.381"
            //    , mdi.outip, mdi.fvlid, areaid);
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
            httpCient.DefaultRequestHeaders.Add("User-Agent", string.Format("Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.{0}.77 Safari/537.36", 3000 + random1.Next()));
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
            //this.guidStr1 = Guid.NewGuid().ToString().ToUpper();
            //string dateTime = DateTime.Now.AddDays(-random.Next(10)).ToString();
            ////string cookie = string.Format("fvlid={1}; " +
            //    "sessionid={2}{6} " +
            //    "autoid={7}; " +
            //    "ahpau=1; __ah_uuid_ng=c_{3}; " +
            //    "sessionip={0}; " +
            //    "sessionvid={8}; " +
            //    "area=330199; v_no=1; visit_info_ad={4}||{9}||-1||-1||1; ref={5}"
            //    , mdi.outip, mdi.fvlid,
            //    this.guidStr, this.guidStr, this.guidStr,
            //    HttpUtility.UrlEncode(string.Format("0|0|0|0|{0}|{1}", DateTime.Now.ToString(), dateTime))
            //    , HttpUtility.UrlEncode(string.Format("||{0}||0; ", dateTime)),
            //    Z.GetMD5LowerString(this.guidStr),
            //    this.guidStr1, this.guidStr1
            //    );

            //string cookie = string.Format("fvlid={1}; " +
            //   "sessionid=EC51C026-F4BB-4AB0-9872-993F6CFF34A8%7C%7C2020-10-14+18%3A20%3A47.654%7C%7C0; " +
            //   "autoid=31bf984e655c9fe48f14c7176521ee08; " +
            //   "ahpau=1; __ah_uuid_ng=c_EC51C026-F4BB-4AB0-9872-993F6CFF34A8; " +
            //   "sessionip={0}; " +
            //   "sessionvid=52CD437F-1054-4AD5-A7F4-345B803C41AA; " +
            //   "area=330199; v_no=1; visit_info_ad=EC51C026-F4BB-4AB0-9872-993F6CFF34A8||52CD437F-1054-4AD5-A7F4-345B803C41AA||-1||-1||1; ref=0%7C0%7C0%7C0%7C2020-12-13+15%3A16%3A28.794%7C2020-10-14+18%3A20%3A47.654"
            //   , mdi.outip, mdi.fvlid);
            //httpCient.DefaultRequestHeaders.Add("Cookie", cookie);
            //httpCient.DefaultRequestHeaders.Add("Host", "al.autohome.com.cn");
            //httpCient.DefaultRequestHeaders.Add("Referer", ConfigUtls.mda_pv_init_referer);
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
            //this.guidStr1 = Guid.NewGuid().ToString().ToUpper();
            //string dateTime = DateTime.Now.AddDays(-random.Next(10)).ToString();
            //string cookie = string.Format("fvlid={1}; " +
            //    "sessionid={2}{6} " +
            //    "autoid={7}; " +
            //    "ahpau=1; __ah_uuid_ng=c_{3}; " +
            //    "sessionip={0}; " +
            //    "sessionvid={8}; " +
            //    "area=330199; v_no=1; visit_info_ad={4}||{9}||-1||-1||1; ref={5}"
            //    , mdi.outip, mdi.fvlid,
            //    this.guidStr, this.guidStr, this.guidStr,
            //    HttpUtility.UrlEncode(string.Format("0|0|0|0|{0}|{1}", DateTime.Now.ToString(), dateTime))
            //    , HttpUtility.UrlEncode(string.Format("||{0}||0; ", dateTime)),
            //    Z.GetMD5LowerString(this.guidStr),
            //     this.guidStr1, this.guidStr1
            //    );

            //string cookie = string.Format("fvlid={1}; " +
            //   "sessionid=EC51C026-F4BB-4AB0-9872-993F6CFF34A8%7C%7C2020-10-14+18%3A20%3A47.654%7C%7C0; " +
            //   "autoid=31bf984e655c9fe48f14c7176521ee08; " +
            //   "ahpau=1; __ah_uuid_ng=c_EC51C026-F4BB-4AB0-9872-993F6CFF34A8; " +
            //   "sessionip={0}; " +
            //   "sessionvid=52CD437F-1054-4AD5-A7F4-345B803C41AA; " +
            //   "area=330199; v_no=1; visit_info_ad=EC51C026-F4BB-4AB0-9872-993F6CFF34A8||52CD437F-1054-4AD5-A7F4-345B803C41AA||-1||-1||1; ref=0%7C0%7C0%7C0%7C2020-12-13+15%3A16%3A28.794%7C2020-10-14+18%3A20%3A47.654"
            //   , mdi.outip, mdi.fvlid);
            //httpClient.DefaultRequestHeaders.Add("Cookie", cookie);
            httpClient.DefaultRequestHeaders.Remove("Referer");
            //httpClient.DefaultRequestHeaders.Add("Referer", requestUrlAndReferer.referer);
        }
        #endregion

    }
}