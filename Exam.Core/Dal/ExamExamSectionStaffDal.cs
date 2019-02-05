using BPM.Common.Data;
using BPM.Common.Provider;
using Exam.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exam.Core.Dal
{
    public class ExamExamSectionStaffDal : BaseRepository<ExamExamSectionStaffModel>
    {
        public static ExamExamSectionStaffDal Instance
        {
            get { return SingletonProvider<ExamExamSectionStaffDal>.Instance; }
        }
        
    }
}
