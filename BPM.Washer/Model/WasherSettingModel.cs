using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Washer.Model
{
    [TableName("Washer_Settings")]
    [Description("大客户的参数设置")]
    public class WasherSettingModel
    {
        [Description("主键")]
        public int KeyId { get; set; }
        [Description("大客户编号")]
        public int DepartmentId { get; set; }
        [Description("参数设置字符串")]
        public string Value { get; set; }
    }
}
