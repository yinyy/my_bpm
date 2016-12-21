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
    public class WasherReplyDal : BaseRepository<WasherReplyModel>
    {
        public static WasherReplyDal Instance
        {
            get { return SingletonProvider<WasherReplyDal>.Instance; }
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort, string order)
        {
            return base.JsonDataForEasyUIdataGrid("V_News", pageindex, pagesize, filterJson,
                                                  sort, order);
        }
    }
}
