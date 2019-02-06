using BPM.Common.Provider;
using Course.Common.Dal;
using Course.Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Course.Common.Bll
{
    public class CommonStaffBll
    {
        public static CommonStaffBll Instance
        {
            get { return SingletonProvider<CommonStaffBll>.Instance; }
        }

        public CommonStaffModel Get(int keyId)
        {
            return CommonStaffDal.Instance.Get(keyId);
        }

        public CommonStaffModel Get(string openid, string type)
        {
            return CommonStaffDal.Instance.GetWhere(new { OpenId = openid, Type = type }).FirstOrDefault();
        }

        public int Insert(CommonStaffModel model)
        {
            return CommonStaffDal.Instance.Insert(model);
        }

        public CommonStaffModel Get(string openId)
        {
            return CommonStaffDal.Instance.GetWhere(new { OpenId = openId }).FirstOrDefault();
        }

        public int Update(CommonStaffModel model)
        {
            return CommonStaffDal.Instance.Update(model);
        }

        public CommonStaffModel[] GetTeachers()
        {
            return CommonStaffDal.Instance.GetWhere(new { Type = "teacher" }).ToArray();
        }

        public CommonStaffModel Get(string serial, string name, string type)
        {
            return CommonStaffDal.Instance.GetWhere(new { Serial = serial, Name = name, Type = type }).FirstOrDefault();
        }

        public int Delete(int keyId)
        {
            return CommonStaffDal.Instance.Delete(keyId);
        }

        public CommonStaffModel GetBySerial(string serial)
        {
            return CommonStaffDal.Instance.GetWhere(new { Serial = serial }).FirstOrDefault();
        }
    }
}
