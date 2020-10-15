using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace River
{
    public class YouxinpaiStoreModel
    {
        /// <summary>
        /// 数据唯一标识
        /// </summary>
        public string PublishID { get; set; }

        /// <summary>
        /// 城市名称
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// 注册日期
        /// </summary>
        public string RegisterDate { get; set; }

        /// <summary>
        /// 交强险日期
        /// </summary>
        public string InsuranceDate { get; set; }

        /// <summary>
        /// 里程
        /// </summary>
        public string Mileage { get; set; }

        /// <summary>
        /// 车牌前两位
        /// </summary>
        public string Li { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 车系
        /// </summary>
        public string Series { get; set; }

        /// <summary>
        /// 车型
        /// </summary>
        public string CarModel { get; set; }

        /// <summary>
        /// 颜色
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// 排量
        /// </summary>
        public string Emissions { get; set; }

        /// <summary>
        /// 变速箱
        /// </summary>
        public string Gear { get; set; }

        /// <summary>
        /// 车况评级
        /// </summary>
        public string Rank { get; set; }

        /// <summary>
        /// 起拍价
        /// </summary>
        public string StartAuctionPrice { get; set; }

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
        public string BidCount { get; set; }

        /// <summary>
        /// 成交状态
        /// </summary>
        public string DealState { get; set; }

        /// <summary>
        /// 保留价
        /// </summary>
        public string ReservePrice { get; set; }

        /// <summary>
        /// 过户次数
        /// </summary>
        public string TransferTime { get; set; }

        /// <summary>
        /// 出厂日期
        /// </summary>
        public string LeaveFactoryDate { get; set; }

        /// <summary>
        /// 车牌号完整
        /// </summary>
        public string License { get; set; }

        /// <summary>
        /// VIN码（完整）
        /// </summary>
        public string VinCode { get; set; }

        /// <summary>
        /// 发动机号（完整）
        /// </summary>
        public string EngineNum { get; set; }

        /// <summary>
        /// 手续1(出厂日期、是否一手车)
        /// </summary>
        public string ProcedureImgUrl1 { get; set; }

        /// <summary>
        /// 手续2(出厂日期、是否一手车)   driving license
        /// </summary>
        public string ProcedureImgUrl2 { get; set; }

        /// <summary>
        /// 行驶证正面
        /// </summary>
        public string DrivingLicenseUrl1 { get; set; }

        /// <summary>
        /// 行驶证反面
        /// </summary>
        public string DrivingLicenseUrl2 { get; set; }

        /// <summary>
        /// 详情页地址
        /// </summary>
        public string DetailUrl { get; set; }

        /// <summary>
        /// 汽车资源Id
        /// </summary>
        public string CarSourceId { get; set; }

        /// <summary>
        /// 保留字段1
        /// </summary>
        public string Reserve1 { get; set; }

        /// <summary>
        /// 保留字段2
        /// </summary>
        public string Reserve2 { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public string CreateDate { get { return DateTime.Now.ToString(); } }

    }
}
