using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.Core.Model
{
    public partial class Department
    {
        public string Appid { get; set; }
        public string Secret { get; set; }
        public string Aeskey { get; set; }
        public string Token { get; set; }
        public string Address { get; set; }
    }
}
