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
    public class WasherSpendingBll
    {
        public static class Kind
        {
            public const string WashCar = "洗车";
        }

        public static WasherSpendingBll Instance
        {
            get { return SingletonProvider<WasherSpendingBll>.Instance; }
        }

        public int Add(WasherSpendingModel model)
        {
            return WasherSpendingDal.Instance.Insert(model);
        }
    }
}
