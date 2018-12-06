using BPM.Core.Bll;
using BPM.Core.Dal;
using BPM.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Washer.Extension
{
    public static class DepartmentBllExtension
    {
        public static string GetMerchantKey(this DepartmentBll bll, string merchantId)
        {
            return DepartmentDal.Instance.GetWhere(new { MerchantId = merchantId }).Select(d => d.MerchantKey).FirstOrDefault();
        }
    }
}
