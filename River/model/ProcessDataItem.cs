using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace River
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

    public class IPMetaDataItem
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

        public List<RequestUrlAndReferer> requestUrlAndReferers = new List<RequestUrlAndReferer>();

    }

    public class RequestUrlAndReferer
    {
        public string requesturl { get; set; }

        public string referer { get; set; }

        public string fvlid { get; set; }
    }
}
