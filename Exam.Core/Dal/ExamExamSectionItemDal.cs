using BPM.Common.Data;
using BPM.Common.Provider;
using Exam.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exam.Core.Dal
{
    public class ExamExamSectionItemDal : BaseRepository<ExamExamSectionItemModel>
    {

        public static ExamExamSectionItemDal Instance
        {
            get { return SingletonProvider<ExamExamSectionItemDal>.Instance; }
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort, string order)
        {
            return base.JsonDataForEasyUIdataGrid("Exam_ExamSectionItems", pageindex, pagesize, filterJson, sort, order);
        }
    }
}
