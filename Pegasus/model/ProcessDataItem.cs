using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pegasus
{
    public class MetaDataItem
    {
        /// <summary>
        /// 发布ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 车辆资源信息
        /// </summary>
        public string CarSourceId { get; set; }

        /// <summary>
        /// 起拍价
        /// </summary>
        public string StartAuctionPrice { get; set; }

        /// <summary>
        /// 保留价
        /// </summary>
        public string ReservePrice { get; set; }

        /// <summary>
        /// 最高价
        /// </summary>
        public string HighTradePrice { get; set; }

        /// <summary>
        /// 交付佣金
        /// </summary>
        public string BuyerAgentFee { get; set; }

        /// <summary>
        /// 交易佣金
        /// </summary>
        public string BuyerTradeFee { get; set; }

        /// <summary>
        /// 最高合手价
        /// </summary>
        public string TotalPrcie { get; set; }

        /// <summary>
        /// 加价手次
        /// </summary>
        public int BidCount { get; set; }

        /// <summary>
        /// 交易类型： -3:未成交  -15:成交
        /// </summary>
        public int BidSourceType { get; set; }

        /// <summary>
        /// 车辆数据
        /// </summary>
        public string Content { get; set; }

    }

    public class IPMetaDataItem : IDisposable
    {
        /// <summary>
        /// 发布ID
        /// </summary>
        public string ipwithport { get; set; }

        /// <summary>
        /// 车辆资源信息
        /// </summary>
        public string requesturl { get; set; }

        /// <summary>
        /// ai
        /// </summary>
        public string[] requesturls { get; set; }

        public string fvlid { get; set; }


        public string outip { get; set; }

        public string result { get; set; }

        public List<RequestUrlAndReferer> requestUrlAndReferers = new List<RequestUrlAndReferer>();

        #region Dispose

        //是否回收完毕
        bool _disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); //标记gc不在调用析构函数
        }
        ~IPMetaDataItem()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return; //如果已经被回收，就中断执行
            if (disposing)
            {
                //TODO:释放本对象中管理的托管资源
                this.fvlid = "";
                this.ipwithport = "";
                this.outip = "";
                this.result = "";
                this.requesturl = "";
                this.requestUrlAndReferers.Clear();
                this.requestUrlAndReferers = null;
                this.requesturls = null;
            }
            //TODO:释放非托管资源
            _disposed = true;
        }
        #endregion

    }

    public class RequestUrlAndReferer
    {
        public string requesturl { get; set; }

        public string referer { get; set; }

        public string fvlid { get; set; }
    }
}
