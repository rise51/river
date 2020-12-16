
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Broswer
{
    public interface IStorer
    {
        void Save(List<YouxinpaiStoreModel> mdi);
    }
}