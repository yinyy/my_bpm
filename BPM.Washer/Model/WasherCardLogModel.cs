using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Washer.Model
{
    [TableName("Washer_CardLogs")]
    public class WasherCardLogModel
    {
        public int KeyId { get; set; }
        public DateTime Time { get; set; }
        public int CardId { get; set; }
        public float Coins { get; set; }
        public string Memo { get; set; }
    }
}
