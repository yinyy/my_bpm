using BPM.Agriculture.Model;
using BPM.Common;
using BPM.Common.Data;
using BPM.Common.Data.Filter;
using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BPM.Agriculture.Dal
{
    public class AgricultureSubscriberDal : BaseRepository<AgricultureSubscriberModel>
    {

        public static AgricultureSubscriberDal Instance
        {
            get { return SingletonProvider<AgricultureSubscriberDal>.Instance; }
        }

        public string JsonDataForEasyUIdataGrid(int pageindex, int pagesize, string filterJson, string sort = "keyid", string order = "asc")
        {
            string sortorder = sort + " " + order;

            var pcp = new ProcCustomPage("V_Subscribers")
            {
                PageIndex = pageindex,
                PageSize = pagesize,
                OrderFields = sortorder,
                WhereString = FilterTranslator.ToSql(filterJson)
            };
            int recordCount;
            DataTable dt = base.GetPageWithSp(pcp, out recordCount);
            return JSONhelper.FormatJSONForEasyuiDataGrid(recordCount, dt);
        }

        public string JsonDataForSubscriber(string openid)
        {
            var pcp = new ProcCustomPage("V_Subscribers")
            {
                PageIndex = 0,
                PageSize = Int32.MaxValue,
                OrderFields = "Title",
                WhereString = ""
            };

            int recordCount;
            DataTable dt = base.GetPageWithSp(pcp, out recordCount);

            List<object> objs = new List<object>();
            foreach (DataRowView drv in dt.DefaultView)
            {
                var o = new {KeyId=drv["KeyId"], Title=drv["Title"], Mac=drv["Mac"], Kind=drv["Kind"]};
                objs.Add(o);
            }

            return JSONhelper.ToJson(objs);
        }
    }
}