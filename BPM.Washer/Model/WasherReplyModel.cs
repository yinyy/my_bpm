using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Washer.Model
{
    [TableName("Washer_Replies")]
    public class WasherReplyModel
    {
        public int KeyId { get; set; }
        public int DepartmentId { get; set; }
        public string Kind { get; set; }
        public string Body { get; set; }
    }
}
