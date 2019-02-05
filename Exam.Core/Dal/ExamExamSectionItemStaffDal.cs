using BPM.Common.Data;
using BPM.Common.Provider;
using Exam.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exam.Core.Dal
{
    public class ExamExamSectionItemStaffDal : BaseRepository<ExamExamSectionItemStaffModel>
    {

        public static ExamExamSectionItemStaffDal Instance
        {
            get { return SingletonProvider<ExamExamSectionItemStaffDal>.Instance; }
        }

        public string GetJson(int pageindex, int pagesize, string filter, string sort, string order)
        {
            return base.JsonDataForEasyUIdataGrid("V_Exam_Section_Item_Staff", pageindex, pagesize, filter, sort, order);
        }
    }
}