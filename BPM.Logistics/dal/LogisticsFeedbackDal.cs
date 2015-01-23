using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BPM.Common.Data;
using BPM.Common.Provider;

using BPM.Logistics.Model;

namespace BPM.Logistics.Dal
{
    public class LogisticsFeedbackDal : BaseRepository<LogisticsFeedbackModel>
    {
        public static LogisticsFeedbackDal Instance
        {
            get { return SingletonProvider<LogisticsFeedbackDal>.Instance; }
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "keyid",
                              string order = "asc")
        {
            return base.JsonDataForEasyUIdataGrid(TableConvention.Resolve(typeof(LogisticsFeedbackModel)), pageindex, pagesize, filterJson,
                                                  sort, order);
        }
    }
}