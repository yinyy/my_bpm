using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Washer.Model
{
    [TableName("Washer_Consumes")]
    [Description("洗车卡消费记录表")]
    public class WasherConsumeModel
    {
        public int KeyId { get; set; }
        public int CardId { get; set; }
        public int DeviceId { get; set; }
        public DateTime Time { get; set; }
        public decimal Money { get; set; }
    }
}
