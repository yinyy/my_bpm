using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Washer.Toolkit
{
    public class ResponseObject
    {
        public bool Success { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
    }
}
