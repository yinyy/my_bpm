using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Washer.Model
{
    [TableName("Washer_WeChatConsumes")]
    public class WasherWeChatConsumeModel
    {
        public int KeyId { get; set; }
        public string OpenId { get; set; }
        public string UnionId { get; set; }
        public int DepartmentId { get; set;}
        public int RefererId { get; set; }
        public string NickName { get; set; }
        public string Gender { get; set; }
        public string Country { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Memo { get; set; }

        public string Coordinate { get; set; }
    }
}
