using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Washer.Model
{
    [TableName("Washer_DeviceLogs")]
    public  class WasherDeviceLogModel
    {
        public int KeyId { get; set; }
        public int DeviceId { get; set; }
        public int? ConsumeId { get; set; }
        public int CardId { get; set; }
        public DateTime Started { get; set; }
        public DateTime Ended { get; set; }
        public string Kind { get; set; }
        public string Memo { get; set; }
        public int RemainCoins { get; set; }
        public int PayCoins { get; set; }
        public int? Ticks { get; set; }
    }
}
