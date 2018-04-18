using BPM.Core.Bll;
using BPM.Core.Dal;
using BPM.Core.Model;
using System.Linq;

namespace BPM.FivePower.Extension
{
    public static class DepartmentBllExtension
    {
        public static Department Get(this DepartmentBll departmentBll, int keyId)
        {
            return DepartmentDal.Instance.GetWhere(new { KeyId = keyId }).FirstOrDefault();
        }

        public static int Update(this DepartmentBll departmentBll, Department department)
        {
            return DepartmentDal.Instance.Update(department);
        }
    }
}
