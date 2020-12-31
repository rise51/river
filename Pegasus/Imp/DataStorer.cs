
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pegasus
{
    public class DataStorer : IStorer
    {
        #region IStorer 成员
        //private readonly ILogger _logger = LoggerFactory.GetChannelLog(typeof(DataStorer));
        //YouxinpaiBLL yxpBLL = BusinessContainer.GetBusiness<YouxinpaiBLL>();

        public void Save(List<YouxinpaiStoreModel> mdiList)
        {
            //yxpBLL.StoreYxpSpiderData(mdiList);
        }

        #endregion
    }
}