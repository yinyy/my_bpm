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
    public class ShoppingGalleryBll
    {
        public static ShoppingGalleryBll Instance
        {
            get { return SingletonProvider<ShoppingGalleryBll>.Instance; }
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "Keyid", string order = "desc")
        {
            return ShoppingGalleryDal.Instance.GetJson(pageindex, pagesize, filterJson, sort, order);
        }

        public int Add(ShoppingGalleryModel model)
        {
            return ShoppingGalleryDal.Instance.Insert(model);
        }
    }
}
