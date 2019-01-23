using BPM.Common.Provider;
using Course.Common.Model;
using Course.Core.Dal;
using Course.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Course.Core.Bll
{
    public class CourseStaffBranchBll
    {
        public static CourseStaffBranchBll Instance
        {
            get { return SingletonProvider<CourseStaffBranchBll>.Instance; }
        }

        public int Insert(CourseStaffBranchModel model)
        {
            return CourseStaffBranchDal.Instance.Insert(model);
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "StaffSerial", string order = "asc")
        {
            return CourseStaffBranchDal.Instance.GetJson(pageindex, pagesize, filterJson, sort, order);
        }

        public int Update(CourseStaffBranchModel entity)
        {
            return CourseStaffBranchDal.Instance.Update(entity);
        }

        public int Delete(int keyId)
        {
            return CourseStaffBranchDal.Instance.Delete(keyId);
        }

        public CourseStaffBranchModel[] FindAll()
        {
            return CourseStaffBranchDal.Instance.GetAll().ToArray();
        }
        
        public CourseStaffBranchModel Get(int staffId, int sorted)
        {
            return CourseStaffBranchDal.Instance.GetWhere(new { StaffId = staffId, Sorted = sorted }).FirstOrDefault();
        }

        public CourseStaffBranchModel[] Get(CommonStaffModel staff)
        {
            return CourseStaffBranchDal.Instance.GetWhere(new { StaffId = staff.KeyId }).OrderBy(m=>m.Sorted).ToArray();
        }
    }
}
