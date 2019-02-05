using BPM.Common.Data;
using BPM.Common.Provider;
using Exam.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exam.Core.Dal
{
    public class ExamExamSectionDal : BaseRepository<ExamExamSectionModel>
    {

        public static ExamExamSectionDal Instance
        {
            get { return SingletonProvider<ExamExamSectionDal>.Instance; }
        }

        public string GetJson(int pageindex, int pagesize, string filter, string sort, string order)
        {
            return base.JsonDataForEasyUIdataGrid("V_Exam_Section", pageindex, pagesize, filter, sort, order);
        }
    }
}
