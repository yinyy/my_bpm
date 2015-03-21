using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sanitation.Dal;
using Sanitation.Model;
using BPM.Common.Provider;
using BPM.Common;

namespace Sanitation.Bll
{
    public class SanitationDispatchBll
    {
        public static SanitationDispatchBll Instance
        {
            get { return SingletonProvider<SanitationDispatchBll>.Instance; }
        }

        public int Add(SanitationDispatchModel model)
        {
            return SanitationDispatchDal.Instance.Insert(model);
        }

        public int Update(SanitationDispatchModel model)
        {
            return SanitationDispatchDal.Instance.Update(model);
        }

        public int Delete(int keyid)
        {
            return SanitationDispatchDal.Instance.Delete(keyid);
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "Keyid", string order = "asc")
        {
            return SanitationDispatchDal.Instance.GetJson(pageindex, pagesize, filterJson, sort, order);
        }

        public SanitationDispatchModel GetById(int id)
        {
            return SanitationDispatchDal.Instance.Get(id);
        }

        public SanitationDispatchModel Current(string code, string plate)
        {
            SanitationDriverModel driver = SanitationDriverDal.Instance.GetWhere(new { Code = code }).FirstOrDefault();
            SanitationTrunkModel trunk = SanitationTrunkDal.Instance.GetWhere(new { Plate = plate }).FirstOrDefault();

            return SanitationDispatchDal.Instance.GetWhere(new
            {
                DriverId = driver.KeyId,
                TrunkId = trunk.KeyId,
                Status = 0
            }).Where(r => r.Time.Date == DateTime.Now.Date).OrderByDescending(r => r.KeyId).FirstOrDefault();
        }
    }
}
