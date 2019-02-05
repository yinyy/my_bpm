using BPM.Common.Provider;
using Exam.Core.Dal;
using Exam.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exam.Core.Bll
{
    public class ExamExamSectionStaffBll
    {
        public static ExamExamSectionStaffBll Instance
        {
            get { return SingletonProvider<ExamExamSectionStaffBll>.Instance; }
        }

        public int Insert(ExamExamSectionStaffModel model)
        {
            return ExamExamSectionStaffDal.Instance.Insert(model);
        }

        public int Delete(int examSectionId, int staffId)
        {
            int res = 0;
            foreach(var m in ExamExamSectionStaffDal.Instance.GetWhere(new { ExamSectionId = examSectionId, StaffId = staffId }))
            {
                res += ExamExamSectionStaffDal.Instance.Delete(m.KeyId);
            }

            return res;
        }

        public int Update(ExamExamSectionStaffModel model)
        {
            return ExamExamSectionStaffDal.Instance.Update(model);
        }

        public int CountSelected(int examSectionId)
        {
            return ExamExamSectionStaffDal.Instance.CountWhere(new { ExamSectionId = examSectionId });
        }

        public bool HasSelected(int examSectionId, int staffId)
        {
            return ExamExamSectionStaffDal.Instance.CountWhere(new { ExamSectionId = examSectionId, StaffId = staffId }) > 0;
        }

        public ExamExamSectionStaffModel[] GetList(ExamExamSectionModel examExamSectionModel)
        {
            return ExamExamSectionStaffDal.Instance.GetWhere(new { ExamSectionId = examExamSectionModel.KeyId }).ToArray();
        }
    }
}
