using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Washer.Dal;
using Washer.Model;

namespace Washer.Bll
{
    public class WasherSettingBll
    {
        public static WasherSettingBll Instance
        {
            get { return SingletonProvider<WasherSettingBll>.Instance; }
        }

        public WasherSettingModel GetByDepartmentId(int departmentId)
        {
            return WasherSettingDal.Instance.GetWhere(new { DepartmentId = departmentId }).FirstOrDefault();
        }

        public int Update(WasherSettingModel model)
        {
            return WasherSettingDal.Instance.Update(model);
        }

        public int Add(WasherSettingModel model)
        {
            return WasherSettingDal.Instance.Insert(model);
        }
    }
}
