using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Washer.Dal;
using Washer.Model;

namespace Washer.Bll
{
   public  class WasherOutsiderBll
    {
        public static WasherOutsiderBll Instance
        {
            get { return SingletonProvider<WasherOutsiderBll>.Instance; }
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "Keyid", string order = "asc")
        {
            return WasherOutsiderDal.Instance.GetJson(pageindex, pagesize, filterJson, sort, order);
        }

        public WasherOutsiderModel Get(int departmentId, string outTag)
        {
            return WasherOutsiderDal.Instance.GetWhere(new { DepartmentId = departmentId, OutTag = outTag }).FirstOrDefault();
        }

        public int Add(WasherOutsiderModel model)
        {
            return WasherOutsiderDal.Instance.Insert(model);
        }

        public WasherOutsiderModel Get(int keyId)
        {
            return WasherOutsiderDal.Instance.Get(keyId);
        }

        public int Update(WasherOutsiderModel model)
        {
            return WasherOutsiderDal.Instance.Update(model);
        }

        public int Delete(int keyId)
        {
            return WasherOutsiderDal.Instance.Delete(keyId);
        }
    }
}
