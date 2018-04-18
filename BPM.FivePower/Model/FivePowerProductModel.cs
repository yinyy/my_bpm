using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPM.FivePower.Model
{
    [TableName("FivePower_Product")]
    [Description("产品安装信息")]
    public class FivePowerProductModel
    {
        public int KeyId { get; set; }
        public int DepartmentId { get; set; }
        public string Type { get; set; }
        public string Plate { get; set; }
        public string Owner { get; set; }
        public string Phone { get; set; }
        public int? Driving { get; set; }
        public bool? Trouble { get; set; }
        public string Detail { get; set; }
        public string Model { get; set; }
        public string Serial { get; set; }
        public DateTime InstallTime { get; set; }
        public int FinishedDriving { get; set; }
        public DateTime FinishedTime { get; set; }
        public string Address { get; set; }
        public string Memo { get; set; }
    }
}
