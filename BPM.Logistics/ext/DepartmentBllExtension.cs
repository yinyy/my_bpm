using BPM.Common;
using BPM.Core.Bll;
using BPM.Core.Dal;
using BPM.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPM.Logistics.ext
{
    public static class DepartmentBllExtension
    {
        public static List<Department> GetFreight(this DepartmentBll bll, int[] ids)
        {
            return DepartmentDal.Instance.GetAll().Where(d => ids.Contains(d.KeyId)).ToList();
        }

        public static string GetFreightForDatagrid(this DepartmentBll bll, int parentId)
        {
            var q = DepartmentDal.Instance.GetChildren(parentId);
            var o = new { count = q.Count(), rows = q.ToArray() };

            return JSONhelper.ToJson(o);
        }
    }
}
