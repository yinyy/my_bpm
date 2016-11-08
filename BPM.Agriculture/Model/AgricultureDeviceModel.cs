using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.Agriculture.Model
{
    [TableName("Agriculture_Devices")]
    [Description("Zigbee设备表")]
    public class AgricultureDeviceModel
    {
        public int KeyId { get; set; }
        public string Mac { get; set; }
        public string Title { get; set; }
        public string Kind { get; set; }
        public int GroupId { get; set; }
        public string Coordinate { get; set; }
        public string Memo { get; set; }
        public string Threshold { get; set; }
    }
}
