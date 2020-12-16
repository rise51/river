using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Broswer
{
    public partial class Form1 : Form
    {
        static ConcurrentQueue<IPItem> listConQueue = new ConcurrentQueue<IPItem>();
        static Producer yxpProducer = new Producer("优信拍列表页", listConQueue, null);
        public Form1()
        {
            InitializeComponent();
            
            
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

        private void button1_Click(object sender, EventArgs e)
        {
            //RefreshIESettings(string.Empty);
            //webBrowser1.Navigate("https://https://www.baidu.com/", null, null, null);
            //webBrowser1.Navigate("https://heycar.m.autohome.com.cn/NewYearH5/Index?ssor=1851197957", null, null, null);
            //webBrowser2.Navigate("https://heycar.m.autohome.com.cn/AnnualMeetingAnswer/AnswerIndex");
            //webBrowser3.Navigate("https://heycar.m.autohome.com.cn/young/SchoolZmActivity");
            //webBrowser4.Navigate("https://club.m.autohome.com.cn/bbs/thread/922a3efdd3f63a4c/91344584-1.html#pvareaid=3460109");
            //webBrowser5.Navigate("https://club.m.autohome.com.cn/partner/uc/thread/91341569");
            //webBrowser6.Navigate("https://club.m.autohome.com.cn/partner/yidian/thread/91341569");
            //webBrowser7.Navigate("https://club.m.autohome.com.cn/partner/qutoutiao/thread/91341569");
            //webBrowser8.Navigate("https://club.m.autohome.com.cn/partner/oppo/thread/91341569");
            //webBrowser9.Navigate("https://club.autohome.com.cn/hongrenzhuanti#pvareaid=3454633");
            //webBrowser10.Navigate("https://heycar.m.autohome.com.cn/Act/haowu/index.htm");
            //webBrowser11.Navigate("https://heycar.m.autohome.com.cn/Act/haowu/liebian.htm");
            //webBrowser12.Navigate("https://heycar.m.autohome.com.cn/Act/oneyear/index.htm");
            ProcessWork();
        }

        object objec = new object();
        private void ProcessWork()
        {
            while (true)
            {
               
                if (listConQueue.Count > 0 )
                {
                    try
                    {
                        IPMetaDataItem mdi = null;
                        IPItem di = null;
                        lock (objec)
                        {
                            if (listConQueue.TryDequeue(out di))
                            {
                                RefreshIESettings(di.ipwithport);
                                //webBrowser1.Navigate("https://https://www.baidu.com/", null, null, null);
                                webBrowser1.Navigate("https://heycar.m.autohome.com.cn/NewYearH5/Index?ssor=1851197957", null, null, null);
                                //webBrowser2.Navigate("https://heycar.m.autohome.com.cn/AnnualMeetingAnswer/AnswerIndex");
                                //webBrowser3.Navigate("https://heycar.m.autohome.com.cn/young/SchoolZmActivity");
                                //webBrowser4.Navigate("https://club.m.autohome.com.cn/bbs/thread/922a3efdd3f63a4c/91344584-1.html#pvareaid=3460109");
                                //webBrowser5.Navigate("https://club.m.autohome.com.cn/partner/uc/thread/91341569");
                                //webBrowser6.Navigate("https://club.m.autohome.com.cn/partner/yidian/thread/91341569");
                                //webBrowser7.Navigate("https://club.m.autohome.com.cn/partner/qutoutiao/thread/91341569");
                                //webBrowser8.Navigate("https://club.m.autohome.com.cn/partner/oppo/thread/91341569");
                                //webBrowser9.Navigate("https://club.autohome.com.cn/hongrenzhuanti#pvareaid=3454633");
                                //webBrowser10.Navigate("https://heycar.m.autohome.com.cn/Act/haowu/index.htm");
                                //webBrowser11.Navigate("https://heycar.m.autohome.com.cn/Act/haowu/liebian.htm");
                                //webBrowser12.Navigate("https://heycar.m.autohome.com.cn/Act/oneyear/index.htm");
                                Thread.Sleep(8000);
                                RefreshIESettings(string.Empty);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        //_logger.Error("优信拍 抓取主过程 异常：{0}", ex);
                    }
                   
                }
                else
                {
                    yxpProducer.Process(null);
                }
            }
        }
    }
}
