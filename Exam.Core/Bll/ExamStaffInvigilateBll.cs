using BPM.Common.Provider;
using Course.Common.Model;
using Exam.Core.Dal;
using Exam.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exam.Core.Bll
{
    public class ExamStaffInvigilateBll
    {
        public static ExamStaffInvigilateBll Instance
        {
            get { return SingletonProvider<ExamStaffInvigilateBll>.Instance; }
        }

        public ExamStaffInvigilateModel Get(CommonStaffModel staff)
        {
            return ExamStaffInvigilateDal.Instance.GetWhere(new { StaffId = staff.KeyId }).FirstOrDefault();
        }

        public ExamStaffInvigilateModel Get(int keyId)
        {
            return ExamStaffInvigilateDal.Instance.Get(keyId);
        }

        public string GetJson(int pageindex, int pagesize, string filter, string sort, string order)
        {
            return ExamStaffInvigilateDal.Instance.GetJson(pageindex, pagesize, filter, sort, order);
        }

        public int Update(ExamStaffInvigilateModel esim)
        {
            return ExamStaffInvigilateDal.Instance.Update(esim);
        }

        public int Insert(ExamStaffInvigilateModel esim)
        {
            return ExamStaffInvigilateDal.Instance.Insert(esim);
        }
        
    }
}