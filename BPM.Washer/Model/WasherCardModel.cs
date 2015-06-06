using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Washer.Model
{
    [TableName("Washer_Cards")]
    [Description("洗车卡")]
    public class WasherCardModel
    {
        public int KeyId { get; set; }
        public string Serial { get; set; }
        public int DepartmentId { get; set; }
        public int CustomId { get; set; }
        public int UserId { get; set; }
        public decimal Money { get; set; }
        public int Status { get; set; }
        public string Memo { get; set; }
    }
}
