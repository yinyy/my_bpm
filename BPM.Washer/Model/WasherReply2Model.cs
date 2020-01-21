using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Washer.Model
{
    [TableName("Washer_Replies2")]
    public class WasherReply2Model
    {
        public int KeyId { get; set; }
        public int DepartmentId { get; set; }
        public string MenuKey { get; set; }
        public string Message { get; set; }
    }
}
