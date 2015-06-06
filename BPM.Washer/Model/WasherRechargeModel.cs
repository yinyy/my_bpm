using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Washer.Model
{
    [TableName("Washer_Recharges")]
    [Description("充值表")]
    public class WasherRechargeModel
    {
        public int KeyId { get; set; }
        public int CardId { get; set; }
        public DateTime Time { get; set; }
        public string Way { get; set; }
        public string Serial { get; set; }
        public decimal Money { get; set; }
        public string Memo { get; set; }
    }
}
