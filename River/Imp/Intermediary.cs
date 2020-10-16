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
            //ProcessTradeData();
            ProcessTradeDataMulti();
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
                                            Console.WriteLine(string.Format("*执行总数totalCount:{0}/{1}当前执行数量 /获取资源数量{2}/开始时间{3} 结束时间{4}",
                                                this.internalSingleton.RequestTotal,
                                                internalSingleton.RequestCount,
                                                internalSingleton.RequestIpCount,
                                                internalSingleton.BeginTime.ToString("yyyy-MM-dd hh:mm:ss fff"),
                                                DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff")));
                                        }
                                        catch (Exception e1)
                                        {
                                            //Console.WriteLine(string.Format("{0}\n{1}", mdi.ipwithport, e1.StackTrace.ToString()));
                                        }
                                        
                                    }
                                    Thread.Sleep(20);
                                }
                            }
                            catch (Exception)
                            {
                            }
                            finally
                            {
                                httpClient.Dispose();
                                httpClient = null;
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
                                Console.WriteLine(string.Format("执行总数totalCount:{0}/{1}当前执行数量 /获取资源数量{2}/开始时间{3} 结束时间{4}",
                                    this.internalSingleton.RequestTotal,
                                    internalSingleton.RequestCount,
                                    internalSingleton.RequestIpCount,
                                    internalSingleton.BeginTime.ToString("yyyy-MM-dd hh:mm:ss fff"),
                                    DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff")));
                            }
                            catch (Exception e2)
                            {
                            }
                            finally
                            {
                                httpClient.Dispose();
                                httpClient = null;
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
                                            Console.WriteLine(string.Format("*执行总数totalCount:{0}/{1}当前执行数量 /获取资源数量{2}/消耗资源数量{5}/开始时间{3} 结束时间{4}",
                                                this.internalSingleton.RequestTotal,
                                                internalSingleton.RequestCount,
                                                internalSingleton.RequestIpCount,
                                                internalSingleton.BeginTime.ToString("yyyy-MM-dd hh:mm:ss fff"),
                                                DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff"),this.internalSingleton.ConsumerIpCount));
                                        }
                                        catch (Exception e1)
                                        {
                                            //Console.WriteLine(string.Format("{0}\n{1}", mdi.ipwithport, e1.StackTrace.ToString()));
                                        }
                                        //参照间隔时间Thread.Sleep(20);
                                        Thread.Sleep(6);
                                    }
                                }
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
                                            Console.WriteLine(string.Format("*执行总数totalCount:{0}/{1}当前执行数量 /获取资源数量{2}/消耗资源数量{5}/开始时间{3} 结束时间{4}",
                                                 this.internalSingleton.RequestTotal,
                                                 internalSingleton.RequestCount,
                                                 internalSingleton.RequestIpCount,
                                                 internalSingleton.BeginTime.ToString("yyyy-MM-dd hh:mm:ss fff"),
                                                 DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff"), this.internalSingleton.ConsumerIpCount));
                                        }
                                        catch (Exception e1)
                                        {
                                            //Console.WriteLine(string.Format("{0}\n{1}", mdi.ipwithport, e1.StackTrace.ToString()));
                                        }

                                    }

                                }
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
                , mdi.ipwithport.Split(':').FirstOrDefault(), mdi.fvlid);
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
            httpCient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36");
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
            string cookie = string.Format("fvlid={1}; " +
                "sessionid=C0B882EA-FEDC-43CA-8E9F-B322F35528C8%7C%7C2020-07-25+12%3A38%3A50.381%7C%7C0; " +
                "autoid=c933fac8868713f3f0e2d3d4b83f16b0; " +
                "ahpau=1; __ah_uuid_ng=c_C0B882EA-FEDC-43CA-8E9F-B322F35528C8; " +
                "sessionip={0}; " +
                "sessionvid=681B9A1B-2EC5-4194-8437-33DEFB091DEA; " +
                "area=119999; v_no=0; visit_info_ad=C0B882EA-FEDC-43CA-8E9F-B322F35528C8||681B9A1B-2EC5-4194-8437-33DEFB091DEA||-1||-1||4; ref=0%7C0%7C0%7C0%7C2020-08-02+08%3A00%3A14.918%7C2020-07-25+12%3A38%3A50.381"
                , mdi.ipwithport.Split(':').FirstOrDefault(), mdi.fvlid);
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
            httpCient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36");
            return httpCient;
        }

        private void SetCookie(HttpClient httpClient, RequestUrlAndReferer requestUrlAndReferer, IPMetaDataItem mdi)
        {
            httpClient.DefaultRequestHeaders.Remove("Cookie");
            string cookie = string.Format("fvlid={1}; " +
                "sessionid=C0B882EA-FEDC-43CA-8E9F-B322F35528C8%7C%7C2020-07-25+12%3A38%3A50.381%7C%7C0; " +
                "autoid=c933fac8868713f3f0e2d3d4b83f16b0; " +
                "ahpau=1; __ah_uuid_ng=c_C0B882EA-FEDC-43CA-8E9F-B322F35528C8; " +
                "sessionip={0}; " +
                "sessionvid=681B9A1B-2EC5-4194-8437-33DEFB091DEA; " +
                "area=119999; v_no=0; visit_info_ad=C0B882EA-FEDC-43CA-8E9F-B322F35528C8||681B9A1B-2EC5-4194-8437-33DEFB091DEA||-1||-1||4; ref=0%7C0%7C0%7C0%7C2020-08-02+08%3A00%3A14.918%7C2020-07-25+12%3A38%3A50.381"
                , mdi.ipwithport.Split(':').FirstOrDefault(), requestUrlAndReferer.fvlid);
            httpClient.DefaultRequestHeaders.Add("Cookie", cookie);
            httpClient.DefaultRequestHeaders.Remove("Referer");
            httpClient.DefaultRequestHeaders.Add("Referer", requestUrlAndReferer.referer);
        }
        #endregion

    }
}