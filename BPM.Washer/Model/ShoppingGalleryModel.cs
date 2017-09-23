using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Washer.Model
{
    [TableName("Shopping_Galleries")]
    [Description("商品图库")]
    public class ShoppingGalleryModel
    {
        public int KeyId { get; set; }
        public int CommodityId { get; set; }
        public string Picture { get; set; }
        public int Sorting { get; set; }

    }
}
