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
    public class ExamExamSectionViewBll
    {
        public static ExamExamSectionViewBll Instance
        {
            get { return SingletonProvider<ExamExamSectionViewBll>.Instance; }
        }

        public ExamExamSectionViewModel[] GetList(ExamExamModel exam)
        {
            return ExamExamSectionViewDal.Instance.GetWhere(new { ExamId = exam.KeyId }).OrderBy(m=>m.Started).ToArray();
        }

        public IEnumerable<ExamExamSectionViewModel> GetList(CommonStaffModel staff)
        {
            return ExamExamSectionViewDal.Instance.GetAll().Where(m => m.TeacherNames!=null && m.TeacherNames.Contains(string.Format("{0},{1}", staff.KeyId, staff.Name)));
        }
    }
}
