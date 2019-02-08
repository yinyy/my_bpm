using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Course.Common.Model
{
    [TableName("Common_Messages")]
    [Description("发送消息的表")]
    public class CommonMessageModel
    {
        public int KeyId { get; set; }
        public string Kind { get; set; }
        public string TaskId { get; set; }
        public int StaffId { get; set; }
        public string TemplateId { get; set; }
        public string Data { get; set; }
        public string Result { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}
