using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgicultureServer
{
    public class ThresholdValue
    {
        public string Value { get; set; }
        public bool Enabled { get; set; }
    }

    public class ThresholdModel
    {
        public ThresholdValue Low { get; set; }
        public ThresholdValue High { get; set; }
    }
}
