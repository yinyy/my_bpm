using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPM.Common.Data;
using BPM.Common.Provider;
using BPM.Logistics.model;

namespace BPM.Logistics.Dal
{
    public class LogisticsFreightGroupDal : BaseRepository<LogisticsFreightGroupModel>
    {
        public static LogisticsFreightGroupDal Instance
        {
            get { return SingletonProvider<LogisticsFreightGroupDal>.Instance; }
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "keyid",
                              string order = "asc")
        {
            return base.JsonDataForEasyUIdataGrid(TableConvention.Resolve(typeof(LogisticsFreightGroupModel)), pageindex, pagesize, filterJson,
                                                  sort, order);
        }
    }
}
