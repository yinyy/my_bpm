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

        [Description("设备序列号")]
        public string SerialNumber { get; set; }

        [Description("主板编号")]
        public string BoardNumber { get; set; }

        [Description("设备所属大客户")]
        public int DepartmentId{ get; set; }

        [Description("设备安装地点")]
        public string Province { get; set; }

        [Description("设备安装地点")]
        public string City { get; set; }

        public string Region { get; set; }

        [Description("设备当前的状态")]
        public string Address { get; set; }

        [Description("设备当前的状态")]
        public string Status { get; set; }

        [Description("设备当前状态最后更新时间")]
        public DateTime UpdateTime { get; set; }

        [Description("设备的生产时间")]
        public DateTime ProductionTime { get; set; }

        [Description("设备的出厂时间")]
        public DateTime DeliveryTime { get; set; }

        [Description("设备位置坐标")]
        public string IpAddress { get; set; }

        [Description("备注")]
        public string Memo { get; set; }

        [Description("备注")]
        public string Memo2 { get; set; }

        [Description("设备的参数设置")]
        public string Setting { get; set; }

        [Description("管理员设置设备是否可以")]
        public bool Enabled { get; set; }
        public string ListenerIp { get; set; }

        public string Coordinate { get; set; }

        [Description("设备有效期")]
        public DateTime ValidateDate { get; set; }
    }
}
