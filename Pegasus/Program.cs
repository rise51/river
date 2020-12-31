using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pegasus
{
    class Program
    {
        static void Main(string[] args)
        {
            #region UC的神马搜索:https://quark.sm.cn/api/rest?uc_param_str=dsdnfrpfbivesscpgimibtbmnijblauputogpintnwkt&method=Ucnetdisc.index&format=html&type=star&ch=kk@quark_mingzhentopic_11#/
            #endregion

            int total = 8000000;
            //Console.WriteLine(total);
            //Console.ReadKey();
            Singleton.Instance.RequestTotal = total;
            Singleton.Instance.DoWork();
        }
    }
}
