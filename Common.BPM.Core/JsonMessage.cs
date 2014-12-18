using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPM.Core
{
    public class JsonMessage
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }

        public override string ToString()
        {
            return BPM.Common.JSONhelper.ToJson(this);
        }
    }
}
