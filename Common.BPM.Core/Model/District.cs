using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.Core.Model
{
    [TableName("Sys_District")]
    public class District
    {
        public int KeyId { get; set; }
        public string Name { get; set; }
        public int Sort { get; set; }
        public int ParentId { get; set; }
        public string Memo { get; set; }
    }
}
