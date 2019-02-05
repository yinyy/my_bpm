using BPM.Common.Data;
using BPM.Common.Provider;
using Exam.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exam.Core.Dal
{
    public class ExamStaffInvigilateDetailViewDal : BaseRepository<ExamStaffInvigilateDetailViewModel>
    {

        public static ExamStaffInvigilateDetailViewDal Instance
        {
            get { return SingletonProvider<ExamStaffInvigilateDetailViewDal>.Instance; }
        }
    }
}
