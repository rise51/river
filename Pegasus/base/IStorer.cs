
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pegasus
{
    public interface IStorer
    {
        void Save(List<YouxinpaiStoreModel> mdi);
    }
}