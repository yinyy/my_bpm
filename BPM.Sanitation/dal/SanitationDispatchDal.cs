using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BPM.Common.Data;
using BPM.Common.Provider;

using Sanitation.Model;
using BPM.Common.Data.Filter;
using System.Data;
using BPM.Common;

namespace Sanitation.Dal
{
    public class SanitationDispatchDal : BaseRepository<SanitationDispatchModel>
    {
        public static SanitationDispatchDal Instance
        {
            get { return SingletonProvider<SanitationDispatchDal>.Instance; }
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "keyid",
                              string order = "asc")
        {
            string where = FilterTranslator.ToSql(filterJson);

            var pcp = new ProcCustomPage("V_Dispatch")
            {
                PageIndex = pageindex,
                PageSize = pagesize,
                WhereString = where,
                OrderFields="Time desc, Name asc, Plate asc"
            };

            int count = 0;
            DataTable table = DbUtils.GetPageWithSp(pcp, out count);

            return JSONhelper.FormatJSONForEasyuiDataGrid(count, table);

            //return base.JsonDataForEasyUIdataGrid(TableConvention.Resolve(typeof(SanitationDispatchModel)), pageindex, pagesize, filterJson,
            //                                      sort, order);
        }
    }
}