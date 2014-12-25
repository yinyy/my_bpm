using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sanitation.Dal;
using Sanitation.Model;
using BPM.Common.Provider;

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

        public bool Validate(SanitationDispatchModel d)
        {
            //验证司机和车辆是否被安排过
            if (SanitationDispatchDal.Instance.GetWhere(new
            {
                Time = d.Time.Date,
                DriverId = d.DriverId,
                Enabled = "是"
            }).Count() > 0)
            {
                return false;
            }

            if (SanitationDispatchDal.Instance.GetWhere(new
                {
                    Time = d.Time.Date,
                    TrunkId = d.TrunkId,
                    Enabled = "是"
                }).Count() > 0)
            {
                return false;
            }

            return true;
        }

        public SanitationDispatchModel Get(DateTime now, string plate)
        {
            return SanitationDispatchDal.Instance.GetWhere(new { Time = now.Date, Plate = plate, Enabled="是" }).FirstOrDefault();
        }
    }
}
