using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Washer.Model
{
    [TableName("Washer_Spendings")]
    public class WasherSpendingModel
    {
        public int KeyId { get; set; }
        public int ConsumerId{get;set;}
        public DateTime Time { get; set; }
        public int DeviceId { get; set; }
        public string Kind { get; set; }
        public float Coins { get; set; }
        public string Memo { get; set; }
    }
}
