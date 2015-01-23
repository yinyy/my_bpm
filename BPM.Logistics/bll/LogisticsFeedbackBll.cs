using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPM.Logistics.Dal;
using BPM.Logistics.Model;
using BPM.Common.Provider;
using BPM.Common.Data;
using System.Data;
using BPM.Common;

namespace BPM.Logistics.Bll
{
    public class LogisticsFeedbackBll
    {
        public static LogisticsFeedbackBll Instance
        {
            get { return SingletonProvider<LogisticsFeedbackBll>.Instance; }
        }

        public int Add(LogisticsFeedbackModel model)
        {
            return LogisticsFeedbackDal.Instance.Insert(model);
        }

        public int Update(LogisticsFeedbackModel model)
        {
            return LogisticsFeedbackDal.Instance.Update(model);
        }

        public int Delete(int keyid)
        {
            return LogisticsFeedbackDal.Instance.Delete(keyid);
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "Keyid", string order = "asc")
        {
            return LogisticsFeedbackDal.Instance.GetJson(pageindex, pagesize, filterJson, sort, order);
        }

        public String Detail(int id)
        {
            var pcp = new ProcCustomPage("V_Inquiry_Quoted")
            {
                PageIndex = 1,
                PageSize = 9999,
                WhereString = " InquiryId = "+id,
                OrderFields = " NewIndex "
            };

            int count = 0;
            DataTable table = DbUtils.GetPageWithSp(pcp, out count);

            return JSONhelper.FormatJSONForEasyuiDataGrid(count, table);
        }
    }
}
