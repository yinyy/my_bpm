using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sanitation.Dal;
using Sanitation.Model;
using BPM.Common.Provider;

namespace Sanitation.Bll
{
    public class SanitationDetailBll
    {
        public static SanitationDetailBll Instance
        {
            get { return SingletonProvider<SanitationDetailBll>.Instance; }
        }

        public int Add(SanitationDetailModel model)
        {
            return SanitationDetailDal.Instance.Insert(model);
        }

        public int Update(SanitationDetailModel model)
        {
            return SanitationDetailDal.Instance.Update(model);
        }

        public int Delete(int keyid)
        {
            return SanitationDetailDal.Instance.Delete(keyid);
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "Keyid", string order = "asc")
        {
            return SanitationDetailDal.Instance.GetJson(pageindex, pagesize, filterJson, sort, order);
        }

        public IEnumerable<SanitationDetailModel>  Get(DateTime time, int dispatchId)
        {
            return SanitationDetailDal.Instance.GetWhere(new { ReferDispatchId = dispatchId }).Where(a => a.Time.Date == time.Date);
        }
    }
}
