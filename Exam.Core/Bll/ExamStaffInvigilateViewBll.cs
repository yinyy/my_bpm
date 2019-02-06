using BPM.Common.Provider;
using Exam.Core.Dal;
using Exam.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exam.Core.Bll
{
    public class ExamStaffInvigilateViewBll
    {
        public static ExamStaffInvigilateViewBll Instance
        {
            get { return SingletonProvider<ExamStaffInvigilateViewBll>.Instance; }
        }

        public IEnumerable<ExamStaffInvigilateViewModel> GetList()
        {
            return ExamStaffInvigilateViewDal.Instance.GetAll();
        }
    }
}
