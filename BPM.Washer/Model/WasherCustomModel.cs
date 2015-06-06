using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Washer.Model
{
    [TableName("Washer_Customs")]
    [Description("客户表")]
    public class WasherCustomModel
    {
        public int KeyId { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Card { get; set; }
        public int DepartmentId { get; set; }
        public int UserId { get; set; }
        public string Memo { get; set; }
    }
}
