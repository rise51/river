using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace River
{
    public class DataItem
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
        public string StartPrice { get; set; }

        /// <summary>
        ///起拍时间
        /// </summary>
        public string PriceStartTime { get; set; }

        /// <summary>
        /// 详情页跳转url
        /// </summary>
        public string jump_url { get; set; }

        /// <summary>
        /// 对应数据
        /// </summary>
        public dynamic Content { get; set; }

    }

    public class IPItem
    {
        /// <summary>
        /// IP和端口
        /// 58.218.200.227:4010
        /// </summary>
        public string ipwithport { get; set; }

        /// <summary>
        /// IP
        /// </summary>
        public string ip { get; set; }
        /// <summary>
        /// 端口号
        /// </summary>
        public string port { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime expire_time { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        public String city { get; set; }
        /// <summary>
        /// 运营商（电信、联通）
        /// </summary>
        public String isp { get; set; }
        /// <summary>
        /// 隧道ip的出口ip
        /// </summary>

        public String outip { get; set; }



    }
}
