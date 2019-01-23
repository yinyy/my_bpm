using BPM.Common.Provider;
using Course.Core.Dal;
using Course.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Course.Core.Bll
{
    public class CourseBranchBll
    {
        public static CourseBranchBll Instance
        {
            get { return SingletonProvider<CourseBranchBll>.Instance; }
        }

        public int Insert(CourseBranchModel model)
        {
            return CourseBranchDal.Instance.Insert(model);
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "KeyId", string order = "asc")
        {
            return CourseBranchDal.Instance.GetJson(pageindex, pagesize, filterJson, sort, order);
        }

        public int Update(CourseBranchModel entity)
        {
            return CourseBranchDal.Instance.Update(entity);
        }

        public int Delete(int keyId)
        {
            return CourseBranchDal.Instance.Delete(keyId);
        }

        public CourseBranchModel[] FindAll()
        {
            return CourseBranchDal.Instance.GetAll().ToArray();
        }

        public CourseBranchModel Get(int keyId)
        {
            return CourseBranchDal.Instance.Get(keyId);
        }
    }
}
