using BPM.Common;
using BPM.Core.Bll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPM.Logistics.ext
{
    public static class DicBllExtension
    {
        public static string DicJson(this DicBll bll, string categoryCode)
        {
            return JSONhelper.ToJson(DicBll.Instance.GetListBy(categoryCode).ToList().OrderBy(d => d.Sortnum).Select(n => new
            {
                id = n.KeyId,
                text = n.Title,
                iconCls = "icon-bullet_green",
                attributes = new { n.Sortnum, n.Remark, n.Code }
            }));
        }
    }
}
