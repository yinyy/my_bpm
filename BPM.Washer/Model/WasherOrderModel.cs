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
        public string Serial { get; set; }
        public int DepartmentId { get; set; }

        public string OpenId { get; set; }
        public DateTime Time { get; set; }
        public string Kind { get; set; }
        public int Money { get; set; }
        public string Status { get; set; }
        public string TransactionId { get; set; }
        public string Memo { get; set; }
        public int? ConsumeId { get; set; }
    }
}
