using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPM.Core.Model;
using BPM.Core.Dal;

namespace BPM.Core.Bll
{
    public class DistrictBll
    {
        public static DistrictBll Instance
        {
            get { return SingletonProvider<DistrictBll>.Instance; }
        }

        public int Add(District d)
        {
            return DistrictDal.Instance.Insert(d);
        }

        public List<District> GetDistricts(int kid=0)
        {
            return DistrictDal.Instance.GetWhere(new {ParentId=kid }).ToList();
        }
       
        public District GetDistrict(string name, District parent=null)
        {
            return DistrictDal.Instance.GetWhere(new { ParentId = parent==null?0:parent.KeyId, Name = name}).FirstOrDefault();
        }
    }
}
