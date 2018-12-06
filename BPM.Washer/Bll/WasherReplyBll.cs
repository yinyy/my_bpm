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
    public class WasherReplyBll
    {
        public static WasherReplyBll Instance
        {
            get { return SingletonProvider<WasherReplyBll>.Instance; }
        }

        public WasherReplyModel Get(int keyId)
        {
            return WasherReplyDal.Instance.Get(keyId);
        }

        public WasherReplyModel Get(int departmentId, string kind)
        {
            return WasherReplyDal.Instance.GetWhere(new { DepartmentId = departmentId, Kind = kind }).FirstOrDefault();
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "Keyid", string order = "asc")
        {
            return WasherReplyDal.Instance.GetJson(pageindex, pagesize, filterJson, sort, order);
        }

        public int Add(WasherReplyModel model)
        {
            return WasherReplyDal.Instance.Insert(model);
        }

        public int Update(WasherReplyModel news)
        {
            return WasherReplyDal.Instance.Update(news);
        }

        public int Delete(int kid)
        {
            return WasherReplyDal.Instance.Delete(kid);
        }
    }
}
