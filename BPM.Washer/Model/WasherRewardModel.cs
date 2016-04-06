using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Washer.Model
{
    [TableName("Washer_Rewards")]
    public class WasherRewardModel
    {
        public int KeyId { get; set; }
        public int ConsumeId { get; set; }
        public DateTime Time { get; set; }
        public string Kind { get; set; }

        public int Points{ get; set; }
        public string Memo { get; set; }
    }
}
