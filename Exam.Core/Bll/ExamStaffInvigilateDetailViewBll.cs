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
    public class ExamStaffInvigilateDetailViewBll
    {
        public static ExamStaffInvigilateDetailViewBll Instance
        {
            get { return SingletonProvider<ExamStaffInvigilateDetailViewBll>.Instance; }
        }

        public ExamStaffInvigilateDetailViewModel[] GetList(ExamExamModel exam)
        {
            return ExamStaffInvigilateDetailViewDal.Instance.GetWhere(new { ExamId = exam.KeyId}).ToArray();
        }

        public ExamStaffInvigilateDetailViewModel[] GetList(ExamExamSectionModel examExamSectionModel)
        {
            return ExamStaffInvigilateDetailViewDal.Instance.GetWhere(new { ExamSectionId = examExamSectionModel.KeyId}).ToArray();
        }

        public IEnumerable<ExamStaffInvigilateDetailViewModel> GetList(CommonStaffModel staff)
        {
            return ExamStaffInvigilateDetailViewDal.Instance.GetWhere(new { StaffId = staff.KeyId });
        }
    }
}
