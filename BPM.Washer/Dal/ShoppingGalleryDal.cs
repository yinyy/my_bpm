using BPM.Common.Data;
using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Washer.Model;

namespace Washer.Dal
{
    public class ShoppingGalleryDal : BaseRepository<ShoppingGalleryModel>
    {
        public static ShoppingGalleryDal Instance
        {
            get { return SingletonProvider<ShoppingGalleryDal>.Instance; }
        }

        internal string GetJson(int pageindex, int pagesize, string filterJson, string sort, string order)
        {
            return base.JsonDataForEasyUIdataGrid("V_Commodities", pageindex, pagesize, filterJson, sort, order);
        }
    }
}
