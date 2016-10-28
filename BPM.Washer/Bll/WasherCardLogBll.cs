using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Data;
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

        public int Insert(WasherCardLogModel model)
        {
            return WasherCardLogDal.Instance.Insert(model);
        }

        public DataTable Export(string filter, string sort = "KeyId", string order = "desc")
        {
            return WasherCardLogDal.Instance.Export(filter, sort, order);
        }
    }
}
