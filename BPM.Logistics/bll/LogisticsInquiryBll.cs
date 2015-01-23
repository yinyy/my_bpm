using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPM.Logistics.Dal;
using BPM.Logistics.Model;
using BPM.Common.Provider;
using BPM.Common;

namespace BPM.Logistics.Bll
{
    public class LogisticsInquiryBll
    {
        public static LogisticsInquiryBll Instance
        {
            get { return SingletonProvider<LogisticsInquiryBll>.Instance; }
        }

        public int Add(LogisticsInquiryModel model)
        {
            return LogisticsInquiryDal.Instance.Insert(model);
        }

        public int Update(LogisticsInquiryModel model)
        {
            return LogisticsInquiryDal.Instance.Update(model);
        }

        public int Delete(int keyid)
        {
            return LogisticsInquiryDal.Instance.Delete(keyid);
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "Keyid", string order = "asc")
        {
            return LogisticsInquiryDal.Instance.GetJson(pageindex, pagesize, filterJson, sort, order);
        }

        public int AdminDelete(int id)
        {
            int count = 0;
            foreach (int fid in LogisticsFeedbackDal.Instance.GetAll().Where(f => f.InquiryId == id).Select(f => f.KeyId).ToArray())
            {
                count += LogisticsFeedbackDal.Instance.Delete(fid);
            }

            count += LogisticsInquiryDal.Instance.Delete(id);

            return count;
        }
    }
}
