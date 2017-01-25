using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Washer.Model
{
    [TableName("Washer_Cards")]
    public class WasherCardModel
    {
        public int KeyId { get; set; }

        public string CardNo { get; set; }

        public string Password{ get; set; }

        public int DepartmentId { get; set; }

        public string Kind { get; set; }

        public int? BinderId { get; set; }

        public DateTime? Binded { get; set; }

        public DateTime ValidateFrom { get; set; }

        public DateTime ValidateEnd { get; set; }
        public int Coins { get; set; }
        public string Memo { get; set; }
        public DateTime? Locked { get; set; }
    }
}
