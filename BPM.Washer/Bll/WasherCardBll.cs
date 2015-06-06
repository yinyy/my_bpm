using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Washer.Dal;
using Washer.Model;

namespace Washer.Bll
{
    public class WasherCardBll
    {
        public static WasherCardBll Instance
        {
            get { return SingletonProvider<WasherCardBll>.Instance; }
        }

        public long Add(WasherCardModel model)
        {
            return WasherCardDal.Instance.Insert(model);
        }

        public int Update(WasherCardModel model)
        {
            return WasherCardDal.Instance.Update(model);
        }

        public int Delete(int keyid)
        {
            return WasherCardDal.Instance.Delete(keyid);
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "Keyid", string order = "asc")
        {
            return WasherCardDal.Instance.GetJson(pageindex, pagesize, filterJson, sort, order);
        }

        public WasherCardModel GetById(int keyId)
        {
            return WasherCardDal.Instance.GetWhere(new { KeyId = keyId }).FirstOrDefault();
        }

        public WasherCardModel GetBySerial(string serial)
        {
            return WasherCardDal.Instance.GetWhere(new { Serial = serial }).FirstOrDefault();
        }
    }
}
