using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BPM.Common.Data;
using BPM.Common.Provider;

using Sanitation.Model;

namespace Sanitation.Dal
{
    public class SanitationDriverDal : BaseRepository<SanitationDriverModel>
    {
        public static SanitationDriverDal Instance
        {
            get { return SingletonProvider<SanitationDriverDal>.Instance; }
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "keyid",
                              string order = "asc")
        {
            return base.JsonDataForEasyUIdataGrid(TableConvention.Resolve(typeof(SanitationDriverModel)), pageindex, pagesize, filterJson,
                                                  sort, order);
        }
    }
}