using BPM.Common;
using BPM.Common.Data;
using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Washer.Model;

namespace Washer.Dal
{
    public class WasherConsumeDal:BaseRepository<WasherConsumeModel>
    {
        class T1
        {
            public int KeyId { get; set; }
            public int CardId { get; set; }
            public int DeviceId { get; set; }
            public DateTime Time { get; set; }
            public decimal Money { get; set; }
            public string CardSerial { get; set; }
            public string DeviceSerial { get; set; }
            public string CustomName { get; set; }
            public int CustomId { get; set; }
            public string DeviceTitle { get; set; }
            public string DeviceAddress { get; set; }
        }


        public static WasherConsumeDal Instance
        {
            get { return SingletonProvider<WasherConsumeDal>.Instance; }
        }

        internal string GetJson(int pageindex, int pagesize, string filterJson, string sort, string order)
        {
            return base.JsonDataForEasyUIdataGrid("V_Consumes", pageindex, pagesize, filterJson,
                                                  sort, order);
        }

        internal string GetConsumeJsonByCustomId(int customId)
        {
            var q = DbUtils.GetList<T1>(string.Format("select * from V_Consumes where CustomId = {0}", customId), null).GroupBy(a => new { Serial = a.CardSerial, ID = a.CardId }).OrderBy(a => a.Key.Serial);
            List<object> list = new List<object>();
            foreach (var g in q)
            {
                var p = g.OrderByDescending(a => a.Time);
                var f = p.FirstOrDefault();
                var o = new
                {
                    KeyId = f.KeyId,
                    CardId = f.CardId,
                    DeviceId = f.DeviceId,
                    Time = f.Time,
                    Money = f.Money,
                    CardSerial = f.CardSerial,
                    DeviceSerial = f.DeviceSerial,
                    CustomName = f.CustomName,
                    CustomId = f.CustomId,
                    DeviceTitle = f.DeviceTitle,
                    DeviceAddress = f.DeviceAddress,
                    children = p.Skip(1).OrderByDescending(a=>a.Time)
                };

                list.Add(o);
            }

            return JSONhelper.ToJson(list);
        }
    }
}
