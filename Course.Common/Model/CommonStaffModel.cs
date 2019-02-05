using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Course.Common.Model
{
    [TableName("Common_Staff")]
    [Description("用户信息表，包含学生和教师")]
    public class CommonStaffModel
    {
        public int KeyId { get; set; }
        public string Serial { get; set; }
        public string Name { get; set; }
        public string OpenId { get; set; }
        public string Type { get; set; }
        public string Gender { get; set; }
    }
}