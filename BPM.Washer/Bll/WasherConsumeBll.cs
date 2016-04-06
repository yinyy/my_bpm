using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Washer.Dal;
using Washer.Model;

namespace Washer.Bll
{
    public class WasherConsumeBll
    {
        public static WasherConsumeBll Instance
        {
            get { return SingletonProvider<WasherConsumeBll>.Instance; }
        }

        public int Add(WasherConsumeModel model)
        {
            return WasherConsumeDal.Instance.Insert(model);
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "Keyid", string order = "asc")
        {
            return WasherConsumeDal.Instance.GetJson(pageindex, pagesize, filterJson, sort, order);
        }

        public WasherConsumeModel Get(int keyId)
        {
            return WasherConsumeDal.Instance.Get(keyId);
        }

        public WasherConsumeModel Get(int deptId, string openId)
        {
            return WasherConsumeDal.Instance.GetWhere(new { OpenId = openId, DepartmentId = deptId }).FirstOrDefault();
        }

        public WasherConsumeModel Get(string unionId, string openId)
        {
            return WasherConsumeDal.Instance.GetWhere(new { UnionId = unionId, OpenId = openId }).FirstOrDefault();
        }

        public int Update(WasherConsumeModel consume)
        {
            return WasherConsumeDal.Instance.Update(consume);
        }
    }
}
