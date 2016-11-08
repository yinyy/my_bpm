using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPM.Agriculture.Model
{
    [TableName("Agriculture_Subscribers")]
    [Description("用户关注表")]
    public class AgricultureSubscriberModel
    {
        public int KeyId { get; set; }
        public string OpenId { get; set; }
        public int DeviceId { get; set; }
    }
}
