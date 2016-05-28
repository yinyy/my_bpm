using BPM.Common.Data;
using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Washer.Model;

namespace Washer.Dal
{
    public class WasherDeviceLogDal : BaseRepository<WasherDeviceLogModel>
    {
        public static WasherDeviceLogDal Instance
        {
            get { return SingletonProvider<WasherDeviceLogDal>.Instance; }
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "Started",
                              string order = "desc")
        {
            return base.JsonDataForEasyUIdataGrid("V_DeviceLogs", pageindex, pagesize, filterJson,
                                                  sort, order);
        }
    }
}