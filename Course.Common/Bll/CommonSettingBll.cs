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

        /// <summary>
        /// 根据单位编号获得单位的参数设置
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public CommonSettingModel Get(int departmentId)
        {
            return CommonSettingDal.Instance.GetWhere(new { DepartmentId = departmentId }).FirstOrDefault();
        }

        public int Insert(CommonSettingModel settings)
        {
            return CommonSettingDal.Instance.Insert(settings);
        }

        public int Update(CommonSettingModel settings)
        {
            return CommonSettingDal.Instance.Update(settings);
        }
    }
}
