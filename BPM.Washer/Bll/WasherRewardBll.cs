using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Washer.Dal;
using Washer.Model;

namespace Washer.Bll
{
    public class WasherRewardBll
    {
        public static class Kind
        {
            public const string Subscribe = "首次关注";
            public const string WashCar = "洗车";
        }

        public static WasherRewardBll Instance
        {
            get { return SingletonProvider<WasherRewardBll>.Instance; }
        }

        public int Add(WasherRewardModel model)
        {
            return WasherRewardDal.Instance.Insert(model);
        }
    }
}
