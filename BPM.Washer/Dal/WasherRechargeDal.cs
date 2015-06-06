using BPM.Common;
using BPM.Common.Data;
using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Washer.Model;

namespace Washer.Dal
{
    public class WasherRechargeDal:BaseRepository<WasherRechargeModel>
    {
        class T1
        {
            public int KeyId { get; set; }
            public int CardId { get; set; }
            public DateTime Time { get; set; }
            public decimal Money { get; set; }
            public string CardSerial { get; set; }
            public string CustomName { get; set; }
            public int CustomId { get; set; }
            public string Way { get; set; }
            public string Memo { get; set; }
            public string Serial { get; set; }
        }


        public static WasherRechargeDal Instance
        {
            get { return SingletonProvider<WasherRechargeDal>.Instance; }
        }

        internal string GetRechargeJsonByCustomId(int customId)
        {
            var q = DbUtils.GetList<T1>(string.Format("select * from V_Recharges where CustomId = {0}", customId), null).GroupBy(a => new { Serial = a.CardSerial, ID = a.CardId }).OrderBy(a => a.Key.Serial);
            List<object> list = new List<object>();
            foreach (var g in q)
            {
                var p = g.OrderByDescending(a => a.Time);
                var f = p.FirstOrDefault();
                var o = new
                {
                    KeyId = f.KeyId,
                    CardId = f.CardId,
                    Time = f.Time,
                    Money = f.Money,
                    CardSerial = f.CardSerial,
                    CustomName = f.CustomName,
                    CustomId = f.CustomId,
                    Way = f.Way,
                    Memo=f.Memo,
                    Serial=f.Serial,
                    children = p.Skip(1).OrderByDescending(a=>a.Time)
                };

                list.Add(o);
            }

            return JSONhelper.ToJson(list);
        }
    }
}
