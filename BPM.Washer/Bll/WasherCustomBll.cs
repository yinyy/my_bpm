using BPM.Common;
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
    public class WasherCustomBll
    {
        public static WasherCustomBll Instance
        {
            get { return SingletonProvider<WasherCustomBll>.Instance; }
        }

        public long Add(WasherCustomModel model)
        {
            return WasherCustomDal.Instance.Insert(model);
        }

        public int Update(WasherCustomModel model)
        {
            return WasherCustomDal.Instance.Update(model);
        }

        public int Delete(int keyid)
        {
            return WasherCustomDal.Instance.Delete(keyid);
        }

        public IEnumerable<WasherCustomModel> GetAll()
        {
            return WasherCustomDal.Instance.GetAll();
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "Keyid", string order = "asc")
        {
            return WasherCustomDal.Instance.GetJson(pageindex, pagesize, filterJson, sort, order);
        }

        public WasherCustomModel GetById(int keyId)
        {
            return WasherCustomDal.Instance.GetWhere(new { KeyId = keyId }).FirstOrDefault();
        }

        public string GetJsonByUserId(int userId)
        {
            var q = WasherCustomDal.Instance.GetWhere(new { UserId = userId }).OrderBy(a => a.Name).Select(a => new { Name=a.Name+"["+a.Card+"]", KeyId=a.KeyId});
            return JSONhelper.ToJson(q);
        }
    }
}
