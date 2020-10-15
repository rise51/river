using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using River.utils;

namespace River
{
    public static class RiverExtensions
    {


        static string[] alRequestUrls = new[] {
            "https://al.autohome.com.cn/mda_pv_init?ahpvers=20200114&ahpplid=1596326603158vXrEyx9cl_&ahpprlid=undefined&ahpsign=12040825745&ref=&cur=https%3A%2F%2Fheycar.m.autohome.com.cn%2FAnnualMeetingAnswer%2FAnswerIndex&scene_type=0&show_id=0&site=1211110&category=1303&subcategory=13441&object=0&series=0&spec=0&level=0&dealer=0&pgvar=%7B%22type%22%3A0%2C%22typeid%22%3A0%2C%22ah_uuid%22%3A%22c_C0B882EA-FEDC-43CA-8E9F-B322F35528C8%22%7D&ahpcs=UTF-8&ahpsr=1536x864&ahpsc=24-bit&ahpul=zh-cn&ahpce=1&ahpdtl=%E6%8C%91%E6%88%982%E5%88%86%E9%92%9F%20%E8%AE%A9%E4%BD%A0%E4%BA%86%E8%A7%A3%E5%98%BFcar&fvlid=1595652059703PsQCj4PyQ4",

        };

        public static IPMetaDataItem Convert2IPMetaDataItem(this IPItem di, string requesturl = @"https://club.m.autohome.com.cn/young")
        {
            string ahpplid = "";
            string requestUrl = ConfigUtls.mda_pv_init;
            if (string.IsNullOrWhiteSpace(requesturl))
            {
                requestUrl = alRequestUrls.FirstOrDefault();
            }
            List<string> tempAiRequestUrls = new List<string>();
            if (ConfigUtls.proxy_rate_open > 0)
            {
                for (int i = 0; i < ConfigUtls.proxy_rate; i++)
                {
                    ahpplid = GetAhpplid(10);
                    //requesturl = requesturl.Replace("&ahpplid=1596326603158vXrEyx9cl_&", string.Format("&ahpplid={0}&", ahpplid));
                    requestUrl = requestUrl.Replace(ConfigUtls.mda_pv_init_ahpplid, string.Format("%26ahpplid%3d{0}%26", ahpplid));
                    string outStr = (long.Parse(ahpplid.Substring(2, 10)) >> 3).ToString() + new Random().Next().ToString().Substring(2, 5);
                    outStr = outStr.Substring(0, 11);
                    //requesturl = requesturl.Replace("&ahpsign=1204082574&", string.Format("&ahpsign={0}&", outStr));
                    requestUrl = requestUrl.Replace(ConfigUtls.mda_pv_init_ahpsign, string.Format("%26ahpsign%3d{0}%26", outStr));
                    //}
                    tempAiRequestUrls.Add(requestUrl);
                }
            }
            else
            {
                ahpplid = GetAhpplid(10);
                //requesturl = requesturl.Replace("&ahpplid=1596326603158vXrEyx9cl_&", string.Format("&ahpplid={0}&", ahpplid));
                requestUrl = requestUrl.Replace(ConfigUtls.mda_pv_init_ahpplid, string.Format("%26ahpplid%3D{0}%26", ahpplid));
                string outStr = (long.Parse(ahpplid.Substring(2, 10)) >> 3).ToString() + new Random().Next().ToString().Substring(2, 5);
                outStr = outStr.Substring(0, 11);
                //requesturl = requesturl.Replace("&ahpsign=1204082574&", string.Format("&ahpsign={0}&", outStr));
                requestUrl = requestUrl.Replace(ConfigUtls.mda_pv_init_ahpsign, string.Format("%26ahpsign%3d{0}%26", outStr));
                //}
                tempAiRequestUrls.Add(requestUrl);
            }
            return new IPMetaDataItem() { ipwithport = di.ipwithport, requesturl = requestUrl, requesturls = tempAiRequestUrls.ToArray(), fvlid = ahpplid, outip = di.outip };
        }

        public static IPMetaDataItem Convert2IPMetaDataItemMulti(this IPItem di, string requesturl = @"https://club.m.autohome.com.cn/young")
        {
            string ahpplid = "";
            string requestUrl = ConfigUtls.mda_pv_init;
            if (string.IsNullOrWhiteSpace(requesturl))
            {
                requestUrl = alRequestUrls.FirstOrDefault();
            }
            
            List<RequestUrlAndReferer> tempAiRequestUrls = new List<RequestUrlAndReferer>();
            if (ConfigUtls.proxy_rate_open > 0)
            {
                foreach (var item in ConfigUtls.mda_pv_initInfos)
                {
                    for (int i = 0; i < ConfigUtls.proxy_rate; i++)
                    {
                        ahpplid = GetAhpplid(10);
                        //requesturl = requesturl.Replace("&ahpplid=1596326603158vXrEyx9cl_&", string.Format("&ahpplid={0}&", ahpplid));
                        requestUrl = requestUrl.Replace(item.mda_pv_init_ahpplid, string.Format("%26ahpplid%3d{0}%26", ahpplid));
                        string outStr = (long.Parse(ahpplid.Substring(2, 10)) >> 3).ToString() + new Random().Next().ToString().Substring(2, 5);
                        outStr = outStr.Substring(0, 11);
                        //requesturl = requesturl.Replace("&ahpsign=1204082574&", string.Format("&ahpsign={0}&", outStr));
                        requestUrl = requestUrl.Replace(item.mda_pv_init_ahpsign, string.Format("%26ahpsign%3d{0}%26", outStr));
                        //}
                        tempAiRequestUrls.Add(new RequestUrlAndReferer() { requesturl = requestUrl, referer=item.mda_pv_init_referer, fvlid = ahpplid });
                    }
                }
            }
            else
            {
                foreach (var item in ConfigUtls.mda_pv_initInfos)
                {
                    ahpplid = GetAhpplid(10);
                    //requesturl = requesturl.Replace("&ahpplid=1596326603158vXrEyx9cl_&", string.Format("&ahpplid={0}&", ahpplid));
                    requestUrl = requestUrl.Replace(item.mda_pv_init_ahpplid, string.Format("%26ahpplid%3D{0}%26", ahpplid));
                    string outStr = (long.Parse(ahpplid.Substring(2, 10)) >> 3).ToString() + new Random().Next().ToString().Substring(2, 5);
                    outStr = outStr.Substring(0, 11);
                    //requesturl = requesturl.Replace("&ahpsign=1204082574&", string.Format("&ahpsign={0}&", outStr));
                    requestUrl = requestUrl.Replace(item.mda_pv_init_ahpsign, string.Format("%26ahpsign%3d{0}%26", outStr));
                    tempAiRequestUrls.Add(new RequestUrlAndReferer() { requesturl = requestUrl, referer = item.mda_pv_init_referer, fvlid= ahpplid });
                }
            }
            return new IPMetaDataItem() { ipwithport = di.ipwithport, requesturl = requestUrl, requestUrlAndReferers= tempAiRequestUrls, fvlid = ahpplid, outip = di.outip };
        }

        public static string GetAhpplid(int len)
        {
            string[] randchar = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            int i = 0;
            string r = "";
            Random random = new Random();
            for (i = 0; i < len; i++)
            {
                int index = (int)(random.NextDouble() * Math.Pow(10, 6)) % randchar.Length;
                r += randchar[index];
            }

            DateTime d = new DateTime();
            //return d.GetTimeStamp(false) + r + "_";
            return d.GetTimeStamp(false) + r ;
        }

        /// <summary>  
        /// 获取当前时间戳  
        /// </summary>  
        /// <param name="bflag">为真时获取10位时间戳,为假时获取13位时间戳.bool bflag = true</param>  
        /// <returns></returns>  
        public static string GetTimeStamp(this DateTime date, bool bflag)
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string ret = string.Empty;
            if (bflag)
                ret = Convert.ToInt64(ts.TotalSeconds).ToString();
            else
                ret = Convert.ToInt64(ts.TotalMilliseconds).ToString();

            return ret;
        }
    }
}