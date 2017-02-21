using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Washer.Model
{
    [TableName("Washer_Outsiders")]
    public class WasherOutsiderModel
    {
        public int KeyId { get; set; }
        public int DepartmentId { get; set; }
        public string OutTag { get; set; }
        public string Token { get; set; }
        public string Url { get; set; }
        public string Memo { get; set; }
    }
}
