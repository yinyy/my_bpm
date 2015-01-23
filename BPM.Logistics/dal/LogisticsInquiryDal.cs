using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPM.Common.Data;
using BPM.Common.Data.Filter;
using BPM.Common.Provider;
using BPM.Logistics.Model;

namespace BPM.Logistics.Dal
{
    public class LogisticsInquiryDal : BaseRepository<LogisticsInquiryModel>
    {
        public static LogisticsInquiryDal Instance
        {
            get { return SingletonProvider<LogisticsInquiryDal>.Instance; }
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "keyid",
                              string order = "asc")
        {
            return base.JsonDataForEasyUIdataGrid(TableConvention.Resolve(typeof(LogisticsInquiryModel)), pageindex, pagesize, filterJson,
                                                  sort, order);
        }
    }
}