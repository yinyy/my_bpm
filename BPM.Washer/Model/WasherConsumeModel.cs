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
        
        public int DepartmentId { get; set; }

        public int? BinderId { get; set; }
        public DateTime? Bindered { get; set; }
        
        public string Name { get; set; }

        public string Gender { get; set; }

        public string Telphone { get; set; }

        public string Password { get; set; }

        public int Points { get; set; }

        public string Memo { get; set; }

        [DbField(false)]
        public string Vcode { get; set; }
        public string Setting { get; set; }
    }
}
