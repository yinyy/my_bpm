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
    public class SanitationDetailDal : BaseRepository<SanitationDetailModel>
    {
        public static SanitationDetailDal Instance
        {
            get { return SingletonProvider<SanitationDetailDal>.Instance; }
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "keyid",
                              string order = "asc")
        {
            string where = FilterTranslator.ToSql(filterJson);

            var pcp = new ProcCustomPage("V_Detail")
            {
                PageIndex = pageindex,
                PageSize = pagesize,
                WhereString = where,
                OrderFields = "Time desc, Name asc, Plate asc"
            };

            int count = 0;
            DataTable table = DbUtils.GetPageWithSp(pcp, out count);

            return JSONhelper.FormatJSONForEasyuiDataGrid(count, table);
            //return base.JsonDataForEasyUIdataGrid(TableConvention.Resolve(typeof(SanitationDetailModel)), pageindex, pagesize, filterJson,
            //                                      sort, order);
        }
    }
}