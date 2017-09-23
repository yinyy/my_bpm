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
    public class ShoppingCommodityBll
    {
        public static ShoppingCommodityBll Instance
        {
            get { return SingletonProvider<ShoppingCommodityBll>.Instance; }
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "Keyid", string order = "desc")
        {
            return ShoppingCommodityDal.Instance.GetJson(pageindex, pagesize, filterJson, sort, order);
        }

        public int Add(ShoppingCommodityModel model)
        {
            return ShoppingCommodityDal.Instance.Insert(model);
        }
    }
}