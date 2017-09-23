using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Washer.Model
{
    [TableName("Shopping_Commodities")]
    [Description("商城商品表")]
    public class ShoppingCommodityModel
    {
        public int KeyId { get; set; }
        public int DepartmentId { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public int Price { get; set; }
        public int Point { get; set; }
        public string Picture { get; set; }      
        public string Details { get; set; }

        [DefaultValue(0)]
        public int Sorting { get; set; }

        [DbField(false)]
        public string[] Gallery { get; set; }
    }
}
