using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Washer.Model
{
    [TableName("Washer_ReceivedMessages")]
    [Description("微信公众平台接收到的消息记录")]
    public class WasherReceivedMessageModel
    {
        [Description("主键")]
        public int KeyId { get; set; }
        [Description("消息创建时间")]
        public DateTime Time { get; set; }
        [Description("发送者微信号")]
        public string OpenId { get; set; }
        [Description("Unix系统时间")]
        public int UnixTime { get; set; }
        [Description("消息类型")]
        public string Type { get; set; }
        [Description("原始消息")]
        public string Primitive{get;set;}
    }
}
