using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Broswer.utils
{
    public static class ConfigUtls
    {
        static List<mda_pv_initInfo> cache = null;
        public static string mda_pv_init
        {
            get
            {
                return ConfigurationManager.AppSettings["mda_pv_init"].ToString();
            }
        }
        /// <summary>
        /// 配置格式【&ahpplid=1596326603158vXrEyx9cl_&】
        /// </summary>
        public static string mda_pv_init_ahpplid
        {
            get
            {
                return ConfigurationManager.AppSettings["mda_pv_init_ahpplid"].ToString();
            }
        }
        /// <summary>
        /// 配置格式：&ahpsign=1204082574&
        /// </summary>
        public static string mda_pv_init_ahpsign
        {
            get
            {
                return ConfigurationManager.AppSettings["mda_pv_init_ahpsign"].ToString();
            }
        }

        /// <summary>
        /// Referer
        /// </summary>
        public static string mda_pv_init_referer
        {
            get
            {
                return ConfigurationManager.AppSettings["mda_pv_init_referer"].ToString();
            }
        }

        public static string mda_pv_init1
        {
            get
            {
                return ConfigurationManager.AppSettings["mda_pv_init1"].ToString();
            }
        }
        /// <summary>
        /// 配置格式【&ahpplid=1596326603158vXrEyx9cl_&】
        /// </summary>
        public static string mda_pv_init_ahpplid1
        {
            get
            {
                return ConfigurationManager.AppSettings["mda_pv_init_ahpplid1"].ToString();
            }
        }
        /// <summary>
        /// 配置格式：&ahpsign=1204082574&
        /// </summary>
        public static string mda_pv_init_ahpsign1
        {
            get
            {
                return ConfigurationManager.AppSettings["mda_pv_init_ahpsign1"].ToString();
            }
        }

        /// <summary>
        /// Referer
        /// </summary>
        public static string mda_pv_init_referer1
        {
            get
            {
                return ConfigurationManager.AppSettings["mda_pv_init_referer1"].ToString();
            }
        }

        public static string mda_pv_init2
        {
            get
            {
                return ConfigurationManager.AppSettings["mda_pv_init2"].ToString();
            }
        }
        /// <summary>
        /// 配置格式【&ahpplid=1596326603158vXrEyx9cl_&】
        /// </summary>
        public static string mda_pv_init_ahpplid2
        {
            get
            {
                return ConfigurationManager.AppSettings["mda_pv_init_ahpplid2"].ToString();
            }
        }
        /// <summary>
        /// 配置格式：&ahpsign=1204082574&
        /// </summary>
        public static string mda_pv_init_ahpsign2
        {
            get
            {
                return ConfigurationManager.AppSettings["mda_pv_init_ahpsign2"].ToString();
            }
        }

        /// <summary>
        /// Referer
        /// </summary>
        public static string mda_pv_init_referer2
        {
            get
            {
                return ConfigurationManager.AppSettings["mda_pv_init_referer2"].ToString();
            }
        }


        public static string mda_pv_init3
        {
            get
            {
                return ConfigurationManager.AppSettings["mda_pv_init3"].ToString();
            }
        }
        /// <summary>
        /// 配置格式【&ahpplid=1596326603158vXrEyx9cl_&】
        /// </summary>
        public static string mda_pv_init_ahpplid3
        {
            get
            {
                return ConfigurationManager.AppSettings["mda_pv_init_ahpplid3"].ToString();
            }
        }
        /// <summary>
        /// 配置格式：&ahpsign=1204082574&
        /// </summary>
        public static string mda_pv_init_ahpsign3
        {
            get
            {
                return ConfigurationManager.AppSettings["mda_pv_init_ahpsign3"].ToString();
            }
        }

        /// <summary>
        /// Referer
        /// </summary>
        public static string mda_pv_init_referer3
        {
            get
            {
                return ConfigurationManager.AppSettings["mda_pv_init_referer3"].ToString();
            }
        }


        public static string mda_pv_init4
        {
            get
            {
                return ConfigurationManager.AppSettings["mda_pv_init4"].ToString();
            }
        }
        /// <summary>
        /// 配置格式【&ahpplid=1596326603158vXrEyx9cl_&】
        /// </summary>
        public static string mda_pv_init_ahpplid4
        {
            get
            {
                return ConfigurationManager.AppSettings["mda_pv_init_ahpplid4"].ToString();
            }
        }
        /// <summary>
        /// 配置格式：&ahpsign=1204082574&
        /// </summary>
        public static string mda_pv_init_ahpsign4
        {
            get
            {
                return ConfigurationManager.AppSettings["mda_pv_init_ahpsign4"].ToString();
            }
        }

        /// <summary>
        /// Referer
        /// </summary>
        public static string mda_pv_init_referer4
        {
            get
            {
                return ConfigurationManager.AppSettings["mda_pv_init_referer4"].ToString();
            }
        }

        public static string mda_pv_init5
        {
            get
            {
                return ConfigurationManager.AppSettings["mda_pv_init5"].ToString();
            }
        }
        /// <summary>
        /// 配置格式【&ahpplid=1596326603158vXrEyx9cl_&】
        /// </summary>
        public static string mda_pv_init_ahpplid5
        {
            get
            {
                return ConfigurationManager.AppSettings["mda_pv_init_ahpplid5"].ToString();
            }
        }
        /// <summary>
        /// 配置格式：&ahpsign=1204082574&
        /// </summary>
        public static string mda_pv_init_ahpsign5
        {
            get
            {
                return ConfigurationManager.AppSettings["mda_pv_init_ahpsign5"].ToString();
            }
        }

        /// <summary>
        /// Referer
        /// </summary>
        public static string mda_pv_init_referer5
        {
            get
            {
                return ConfigurationManager.AppSettings["mda_pv_init_referer5"].ToString();
            }
        }

        public static string mda_pv_init6
        {
            get
            {
                return ConfigurationManager.AppSettings["mda_pv_init6"].ToString();
            }
        }
        /// <summary>
        /// 配置格式【&ahpplid=1596326603158vXrEyx9cl_&】
        /// </summary>
        public static string mda_pv_init_ahpplid6
        {
            get
            {
                return ConfigurationManager.AppSettings["mda_pv_init_ahpplid6"].ToString();
            }
        }
        /// <summary>
        /// 配置格式：&ahpsign=1204082574&
        /// </summary>
        public static string mda_pv_init_ahpsign6
        {
            get
            {
                return ConfigurationManager.AppSettings["mda_pv_init_ahpsign6"].ToString();
            }
        }

        /// <summary>
        /// Referer
        /// </summary>
        public static string mda_pv_init_referer6
        {
            get
            {
                return ConfigurationManager.AppSettings["mda_pv_init_referer6"].ToString();
            }
        }

        /// <summary>
        /// 消费线程数，默认20
        /// </summary>
        public static int intermediary_taskscale
        {
            get
            {
                int result = 0;
                if (int.TryParse(ConfigurationManager.AppSettings["intermediary_taskscale"].ToString(), out result))
                {
                }
                return result;
            }
        }
        /// <summary>
        /// 是否开放大招模式
        /// </summary>
        public static int proxy_rate_open
        {
            get
            {
                int result = 0;
                if (int.TryParse(ConfigurationManager.AppSettings["proxy_rate_open"].ToString(), out result))
                {
                }
                return result;
            }
        }
        /// <summary>
        /// 释放大招倍数
        /// </summary>
        public static int proxy_rate
        {
            get
            {
                int result = 0;
                if (int.TryParse(ConfigurationManager.AppSettings["proxy_rate"].ToString(), out result))
                {
                }
                return result;
            }
        }

        /// <summary>
        /// 多刷 >0
        /// 单刷=0
        /// </summary>
        public static int process_multi
        {
            get
            {
                int result = 0;
                if (int.TryParse(ConfigurationManager.AppSettings["process_multi"].ToString(), out result))
                {
                }
                return result;
            }
        }

        /// <summary>
        /// 时间间隔
        /// before: 单刷 20,多刷 6
        /// now:单刷 40，多刷 12
        /// </summary>
        public static int time_space
        {
            get
            {
                int result = 20;
                if (int.TryParse(ConfigurationManager.AppSettings["time_space"].ToString(), out result))
                {
                }
                return result;
            }
        }

        /// <summary>
        /// 时间间隔
        /// before: 单刷 20,多刷 6
        /// now:单刷 40，多刷 12
        /// </summary>
        public static int time_space_multi
        {
            get
            {
                int result = 12;
                if (int.TryParse(ConfigurationManager.AppSettings["time_space_multi"].ToString(), out result))
                {
                }
                return result;
            }
        }

        /// <summary>
        /// 时间间隔
        /// 生产者生产数据间隔
        /// 4000ms
        /// </summary>
        public static int producer_time_space
        {
            get
            {
                int result = 4000;
                if (int.TryParse(ConfigurationManager.AppSettings["producer_time_space"].ToString(), out result))
                {
                }
                return result;
            }
        }

        /// <summary>
        /// 时间间隔
        /// 生产队列阀值150，大于就将多出的数据清空
        /// now:单刷 40，多刷 12
        /// </summary>
        public static int producer_queue_count
        {
            get
            {
                int result = 150;
                if (int.TryParse(ConfigurationManager.AppSettings["producer_queue_count"].ToString(), out result))
                {
                }
                return result;
            }
        }

        /// <summary>
        /// 时间间隔
        /// 是否开启清除队列开关
        /// 默认不开启
        /// </summary>
        public static int producer_queue_take
        {
            get
            {
                int result = 0;
                if (int.TryParse(ConfigurationManager.AppSettings["producer_queuetake_switch"].ToString(), out result))
                {
                }
                return result;
            }
        }

        /// <summary>
        /// 时间间隔
        /// 是否开启探针开关
        /// 默认不开启
        /// </summary>
        public static int probe_switch
        {
            get
            {
                int result = 0;
                if (int.TryParse(ConfigurationManager.AppSettings["probe_switch"].ToString(), out result))
                {
                }
                return result;
            }
        }

        public static List<mda_pv_initInfo> mda_pv_initInfos
        {
            get
            {
                if (cache == null)
                {
                    cache = new List<mda_pv_initInfo>() {
                     new mda_pv_initInfo(){mda_pv_init =mda_pv_init,  mda_pv_init_ahpplid=mda_pv_init_ahpplid, mda_pv_init_ahpsign= mda_pv_init_ahpsign, mda_pv_init_referer= mda_pv_init_referer},
                     new mda_pv_initInfo(){mda_pv_init =mda_pv_init1,  mda_pv_init_ahpplid=mda_pv_init_ahpplid1, mda_pv_init_ahpsign= mda_pv_init_ahpsign1, mda_pv_init_referer= mda_pv_init_referer1},
                    //new mda_pv_initInfo(){mda_pv_init =mda_pv_init2,  mda_pv_init_ahpplid=mda_pv_init_ahpplid2, mda_pv_init_ahpsign= mda_pv_init_ahpsign2, mda_pv_init_referer= mda_pv_init_referer2},
                    //new mda_pv_initInfo(){mda_pv_init =mda_pv_init3,  mda_pv_init_ahpplid=mda_pv_init_ahpplid3, mda_pv_init_ahpsign= mda_pv_init_ahpsign3, mda_pv_init_referer= mda_pv_init_referer3},
                    //new mda_pv_initInfo(){mda_pv_init =mda_pv_init4,  mda_pv_init_ahpplid=mda_pv_init_ahpplid4, mda_pv_init_ahpsign= mda_pv_init_ahpsign4, mda_pv_init_referer= mda_pv_init_referer4},
                    //new mda_pv_initInfo(){mda_pv_init =mda_pv_init5,  mda_pv_init_ahpplid=mda_pv_init_ahpplid5, mda_pv_init_ahpsign= mda_pv_init_ahpsign5, mda_pv_init_referer= mda_pv_init_referer5},
                    //new mda_pv_initInfo(){mda_pv_init =mda_pv_init6,  mda_pv_init_ahpplid=mda_pv_init_ahpplid6, mda_pv_init_ahpsign= mda_pv_init_ahpsign6, mda_pv_init_referer= mda_pv_init_referer6},
                };
                }
                return cache;
            }
        }
    }

    public class mda_pv_initInfo
    {
        public string mda_pv_init { get; set; }

        public string mda_pv_init_ahpplid { get; set; }

        public string mda_pv_init_ahpsign { get; set; }

        public string mda_pv_init_referer { get; set; }
    }

    public static class DidUtils
    {
        internal static int YouMayDid()
        {

            //if(DateTime.Now.)
            return 1;
        }

        private static bool IsEffectDate()
        {
            //string effectDateTime = "";
            //DateTime dateTime =DateTime.Now
            return true;
        }
    }
}
