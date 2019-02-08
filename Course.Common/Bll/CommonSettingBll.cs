using BPM.Common.Provider;
using Course.Common.Dal;
using Course.Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Course.Common.Bll
{
    public class CommonSettingBll
    {
        public static CommonSettingBll Instance
        {
            get { return SingletonProvider<CommonSettingBll>.Instance; }
        }

        public CommonSettingModel[] GetAll()
        {
            return CommonSettingDal.Instance.GetAll().ToArray();
        }

        public int Insert(CommonSettingModel settings)
        {
            return CommonSettingDal.Instance.Insert(settings);
        }

        public int Update(CommonSettingModel settings)
        {
            return CommonSettingDal.Instance.Update(settings);
        }

        public CommonSettingModel Get(string keyword)
        {
            return CommonSettingDal.Instance.GetWhere(new { Keyword = keyword }).FirstOrDefault();
        }
    }
}
