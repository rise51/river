
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace River
{
    public interface IStorer
    {
        void Save(List<YouxinpaiStoreModel> mdi);
    }
}