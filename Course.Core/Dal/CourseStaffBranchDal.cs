using BPM.Common;
using BPM.Common.Data;
using BPM.Common.Data.Filter;
using BPM.Common.Provider;
using Course.Core.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Course.Core.Dal
{
    public class CourseStaffBranchDal : BaseRepository<CourseStaffBranchModel>
    {

        public static CourseStaffBranchDal Instance
        {
            get { return SingletonProvider<CourseStaffBranchDal>.Instance; }
        }

        public string JsonDataForEasyUIdataGrid(int pageindex, int pagesize, string filterJson, string sort = "keyid", string order = "asc")
        {
            string sortorder = sort + " " + order;

            var pcp = new ProcCustomPage("Course_Branchs")
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

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "StaffSerial", string order = "asc")
        {
            return base.JsonDataForEasyUIdataGrid("V_Course_Staff_Branch", pageindex, pagesize, filterJson, sort, order);
        }
    }
}