using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Washer.Model
{
    [TableName("Washer_Vcodes")]
    public class WasherVcodeModel
    {
        public int KeyId { get; set; }
        public string Telphone { get; set; }
        public string Vcode { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Validated { get; set; }
        public string Memo { get; set; }
    }
}
