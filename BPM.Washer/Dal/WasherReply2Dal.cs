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
    public class WasherReply2Dal : BaseRepository<WasherReply2Model>
    {
        public static WasherReply2Dal Instance
        {
            get { return SingletonProvider<WasherReply2Dal>.Instance; }
        }

        internal string GetJson(int pageindex, int pagesize, string filterJson, string sort, string order)
        {
            return base.JsonDataForEasyUIdataGrid("Washer_Replies2", pageindex, pagesize, filterJson,
                                                  sort, order);
        }
    }
}
