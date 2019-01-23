using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Course.Common.Model
{
    [TableName("Common_Settings")]
    [Description("子系统设置表")]

    public class CommonSettingModel
    {
        public int KeyId { get; set; }
        public int DepartmentId { get; set; }
        
    }
}
