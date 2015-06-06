using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Washer.Dal;
using Washer.Model;

namespace Washer.Bll
{
    public class WasherConsumeBll
    {
        public static WasherConsumeBll Instance
        {
            get { return SingletonProvider<WasherConsumeBll>.Instance; }
        }

        public long Add(WasherConsumeModel model)
        {
            return WasherConsumeDal.Instance.Insert(model);
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "Keyid", string order = "asc")
        {
            return WasherConsumeDal.Instance.GetJson(pageindex, pagesize, filterJson, sort, order);
        }

        public string GetConsumeJsonByCustomId(int customId)
        {
            return WasherConsumeDal.Instance.GetConsumeJsonByCustomId(customId);
        }
    }
}
