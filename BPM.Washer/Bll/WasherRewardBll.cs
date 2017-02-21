using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Washer.Dal;
using Washer.Model;
using System.Web.UI.WebControls;
using System.Data;

namespace Washer.Bll
{
    public class WasherRewardBll
    {

        public static WasherRewardBll Instance
        {
            get { return SingletonProvider<WasherRewardBll>.Instance; }
        }

        public int Add(WasherRewardModel model)
        {
            return WasherRewardDal.Instance.Insert(model);
        }

        public IEnumerable<WasherRewardModel> GetValidRewards(int consumeId)
        {
            return WasherRewardDal.Instance.GetWhere(new { ConsumeId = consumeId, Expired = false });
        }

        public int GetRemainReward(int consumeId)
        {
            int points = 0;
            foreach(var r in GetValidRewards(consumeId))
            {
                points += r.Points - r.Used;
            }

            return points;
        }

        public int Delete(int keyId)
        {
            return WasherRewardDal.Instance.Delete(keyId);
        }

        public string GetJson(int pageindex, int pagesize, string filter, string sort = "Time", string order = "desc")
        {
            return WasherRewardDal.Instance.GetJson(pageindex, pagesize, filter, sort, order);
        }

        public DataTable Export(string filter, string sort, string order)
        {
            return WasherRewardDal.Instance.Export(filter, sort??"KeyId", order??"desc");
        }
    }
}
