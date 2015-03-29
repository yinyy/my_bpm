using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sanitation.Dal;
using Sanitation.Model;
using BPM.Common.Provider;
using BPM.Common.Data;

namespace Sanitation.Bll
{
    public class SanitationDriverBll
    {
        public static SanitationDriverBll Instance
        {
            get { return SingletonProvider<SanitationDriverBll>.Instance; }
        }

        public int Add(SanitationDriverModel model)
        {
            return SanitationDriverDal.Instance.Insert(model);
        }

        public int Update(SanitationDriverModel model)
        {
            return SanitationDriverDal.Instance.Update(model);
        }

        public int Delete(int keyid)
        {
            return SanitationDriverDal.Instance.Delete(keyid);
        }

        public IEnumerable<SanitationDriverModel> GetAll()
        {
            return SanitationDriverDal.Instance.GetAll();
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "Keyid", string order = "asc")
        {
            return SanitationDriverDal.Instance.GetJson(pageindex, pagesize, filterJson, sort, order);
        }

        public SanitationDriverModel GetById(int driverId)
        {
            return SanitationDriverDal.Instance.Get(driverId);
        }

        public SanitationDriverModel GetByCode(string code)
        {
            return SanitationDriverDal.Instance.GetWhere(new { Code = code }).FirstOrDefault();
        }
    }
}
