using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Washer.Model
{
    [TableName("Washer_Devices")]
    [Description("自助洗车机设备")]
    public class WasherDeviceModel
    {
        [Description("编号")]
        public int KeyId { get; set; }

        [Description("设备名称")]
        public string Title { get; set; }

        [Description("设备序列号")]
        public string Serial { get; set; }

        [Description("设备所属大客户")]
        public int DepartmentId { get; set; }

        [Description("设备当前的状态")]
        public int Status { get; set; }

        [Description("设备当前状态最后更新时间")]
        public DateTime Updated { get; set; }

        [Description("设备地址")]
        public string Address { get; set; }

        [Description("设备位置坐标")]
        public string Position { get; set; }

        [Description("备注")]
        public string Memo { get; set; }

        [Description("大客户的备注")]
        public string Memo2 { get; set; }

        [Description("是否被大客户删除")]
        public Boolean Deleted { get; set; }
    }
}
