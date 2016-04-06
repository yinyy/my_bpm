using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Washer.Model
{
    [TableName("Washer_Orders")]
    public class WasherOrderModel
    {
        public int KeyId { get; set; }
        public string UnionId { get; set; }
        public string OpenId { get; set; }
        public DateTime Time { get; set; }
        public string Payment { get; set; }
        public float Money { get; set; }
        public string Memo { get; set; }
    }
}
