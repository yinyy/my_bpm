using BPM.Common.Data;
using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Washer.Model;

namespace Washer.Dal
{
    public class WasherDeviceDal : BaseRepository<WasherDeviceModel>
    {
        public static WasherDeviceDal Instance
        {
            get { return SingletonProvider<WasherDeviceDal>.Instance; }
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "keyid",
                              string order = "asc")
        {
            return base.JsonDataForEasyUIdataGrid("V_Devices", pageindex, pagesize, filterJson,
                                                  sort, order);
        }
    }
}