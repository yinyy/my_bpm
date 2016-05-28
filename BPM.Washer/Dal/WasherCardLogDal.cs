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
    public class WasherCardLogDal : BaseRepository<WasherCardLogModel>
    {
        public static WasherCardLogDal Instance
        {
            get { return SingletonProvider<WasherCardLogDal>.Instance; }
        }

        internal string GetJson(int pageindex, int pagesize, string filter, string sort, string order)
        {
            return base.JsonDataForEasyUIdataGrid("V_CardLogs", pageindex, pagesize, filter,
                                                  sort, order);
        }
    }
}
