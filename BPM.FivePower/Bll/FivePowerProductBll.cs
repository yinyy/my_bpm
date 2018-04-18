using BPM.Common.Provider;
using BPM.FivePower.Dal;
using BPM.FivePower.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPM.FivePower.Bll
{
    public class FivePowerProductBll
    {
        public static FivePowerProductBll Instance
        {
            get { return SingletonProvider<FivePowerProductBll>.Instance; }
        }

        public string GetJson(int pageindex, int pagesize, string filter, string sort = "InstallTime", string order = "asc")
        {
            return FivePowerProductDal.Instance.GetJson(pageindex, pagesize, filter, sort, order);
        }

        public int Insert(FivePowerProductModel model)
        {
            return FivePowerProductDal.Instance.Insert(model);
        }

        public FivePowerProductModel Get(int deptId, string serial)
        {
            return FivePowerProductDal.Instance.GetWhere(new { DepartmentId = deptId, Serial = serial }).FirstOrDefault();
        }
    }
}
