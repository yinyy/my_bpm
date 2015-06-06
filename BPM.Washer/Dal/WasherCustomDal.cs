using BPM.Common.Data;
using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Washer.Model;

namespace Washer.Dal
{
    public class WasherCustomDal:BaseRepository<WasherCustomModel>
    {
        public static WasherCustomDal Instance
        {
            get { return SingletonProvider<WasherCustomDal>.Instance; }
        }

        internal string GetJson(int pageindex, int pagesize, string filterJson, string sort, string order)
        {
            return base.JsonDataForEasyUIdataGrid(TableConvention.Resolve(typeof(WasherCustomModel)), pageindex, pagesize, filterJson,
                                                  sort, order);
        }
    }
}
