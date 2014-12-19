using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BPM.Common.Data;
using BPM.Common.Provider;

using Sanitation.Model;

namespace Sanitation.Dal
{
    public class SanitationTrunkDal : BaseRepository<SanitationTrunkModel>
    {
        public static SanitationTrunkDal Instance
        {
            get { return SingletonProvider<SanitationTrunkDal>.Instance; }
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "keyid",
                              string order = "asc")
        {
            return base.JsonDataForEasyUIdataGrid(TableConvention.Resolve(typeof(SanitationTrunkModel)), pageindex, pagesize, filterJson,
                                                  sort, order);
        }
    }
}