using BPM.Common.Provider;
using Exam.Core.Dal;
using Exam.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exam.Core.Bll
{
    public class ExamExamSectionItemStaffBll
    {
        public static ExamExamSectionItemStaffBll Instance
        {
            get { return SingletonProvider<ExamExamSectionItemStaffBll>.Instance; }
        }

        public string GetJson(int pageindex, int pagesize, string filter, string sort, string order)
        {
            return ExamExamSectionItemStaffDal.Instance.GetJson(pageindex, pagesize, filter, sort, order);
        }

        public ExamExamSectionItemStaffModel[] GetList(ExamExamSectionItemModel examExamSectionItemModel)
        {
            return ExamExamSectionItemStaffDal.Instance.GetWhere(new { ExamSectionItemId = examExamSectionItemModel.KeyId}).ToArray();
        }

        public int Delete(ExamExamSectionItemModel examExamSectionItemModel)
        {
            string ids = string.Join(",", ExamExamSectionItemStaffDal.Instance.GetWhere(new { ExamSectionItemId = examExamSectionItemModel.KeyId }).Select(m=>m.KeyId));
            return ExamExamSectionItemStaffDal.Instance.Delete(ids);
        }

        public int Insert(ExamExamSectionItemStaffModel eesism)
        {
            return ExamExamSectionItemStaffDal.Instance.Insert(eesism);
        }

        public ExamExamSectionItemStaffModel Get(int keyId)
        {
            return ExamExamSectionItemStaffDal.Instance.Get(keyId);
        }

        public int Update(ExamExamSectionItemStaffModel eesis)
        {
            return ExamExamSectionItemStaffDal.Instance.Update(eesis);
        }
    }
}
