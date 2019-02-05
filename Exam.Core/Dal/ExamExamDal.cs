using BPM.Common.Data;
using BPM.Common.Provider;
using Exam.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exam.Core.Dal
{
    public class ExamExamDal : BaseRepository<ExamExamModel>
    {

        public static ExamExamDal Instance
        {
            get { return SingletonProvider<ExamExamDal>.Instance; }
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort, string order)
        {
            return base.JsonDataForEasyUIdataGrid("Exam_Exams", pageindex, pagesize, filterJson, sort, order);
        }
    }
}
