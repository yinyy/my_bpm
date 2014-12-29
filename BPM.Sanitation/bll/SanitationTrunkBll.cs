using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sanitation.Dal;
using Sanitation.Model;
using BPM.Common.Provider;

namespace Sanitation.Bll
{
    public class SanitationTrunkBll
    {
        public static SanitationTrunkBll Instance
        {
            get { return SingletonProvider<SanitationTrunkBll>.Instance; }
        }

        public int Add(SanitationTrunkModel model)
        {
            return SanitationTrunkDal.Instance.Insert(model);
        }

        public int Update(SanitationTrunkModel model)
        {
            return SanitationTrunkDal.Instance.Update(model);
        }

        public int Delete(int keyid)
        {
            return SanitationTrunkDal.Instance.Delete(keyid);
        }

        public IEnumerable<SanitationTrunkModel> GetAll()
        {
            return SanitationTrunkDal.Instance.GetAll();
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "Keyid", string order = "asc")
        {
            return SanitationTrunkDal.Instance.GetJson(pageindex, pagesize, filterJson, sort, order);
        }

        public SanitationTrunkModel GetById(int trunkId)
        {
            return SanitationTrunkDal.Instance.Get(trunkId);
        }
    }
}
