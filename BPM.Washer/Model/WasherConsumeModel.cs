using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Washer.Model
{
    [TableName("Washer_Consumes")]
    [Description("消费者")]
    public class WasherConsumeModel
    {
        public int KeyId { get; set; }

        public string UnionId { get; set; }

        public string OpenId { get; set; }

        public string NickName { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string Province { get; set; }

        public string Gender { get; set; }

        public int DepartmentId{ get; set; }

        public int RefererId { get; set; }

        public string Card { get; set; }

        public float Coins { get; set; }

        public int Points { get; set; }

        public string Memo { get; set; }
    }
}
