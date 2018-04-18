using BPM.Common.Data;
using BPM.Common.Provider;
using BPM.FivePower.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPM.FivePower.Dal
{
    public class FivePowerProductDal : BaseRepository<FivePowerProductModel>
    {
        public static FivePowerProductDal Instance
        {
            get { return SingletonProvider<FivePowerProductDal>.Instance; }
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "InstallTime",
                              string order = "asc")
        {
            return base.JsonDataForEasyUIdataGrid("V_Product", pageindex, pagesize, filterJson,
                                                  sort, order);
        }
    }
}
