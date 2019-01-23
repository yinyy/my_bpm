using BPM.Core.Bll;
using BPM.Core.Dal;
using BPM.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Course.Common.Bll
{
    public static class DepartmentBllExt
    {
        public static Department Get(this DepartmentBll bll, int keyId)
        {
            return DepartmentDal.Instance.Get(keyId);
        }
    }
}
