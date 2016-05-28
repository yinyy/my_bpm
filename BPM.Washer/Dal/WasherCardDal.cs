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
    public class WasherCardDal : BaseRepository<WasherCardModel>
    {
        public static WasherCardDal Instance
        {
            get { return SingletonProvider<WasherCardDal>.Instance; }
        }

        internal string GetJson(int pageindex, int pagesize, string filterJson, string sort, string order)
        {
            return base.JsonDataForEasyUIdataGrid("V_Cards", pageindex, pagesize, filterJson,
                                                  sort, order);
        }
    }
}
