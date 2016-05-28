using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Washer.Dal;
using Washer.Model;

namespace Washer.Bll
{
    public class WasherCardLogBll
    {
        public static WasherCardLogBll Instance
        {
            get { return SingletonProvider<WasherCardLogBll>.Instance; }
        }

        public string GetJson(int pageindex, int pagesize, string filter, string sort = "Time", string order = "desc")
        {
            return WasherCardLogDal.Instance.GetJson(pageindex, pagesize, filter, sort, order);
        }
    }
}
