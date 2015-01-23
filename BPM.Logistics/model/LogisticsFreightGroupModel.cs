using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using BPM.Common.Data;

namespace BPM.Logistics.model
{
    [TableName("Logistics_FreightGroup")]
    [Description("货代公司分组")]
    public class LogisticsFreightGroupModel
    {
        public int KeyId { get; set; }
        public int DicId { get; set; }
        public int DepartmentId { get; set; }
    }
}
