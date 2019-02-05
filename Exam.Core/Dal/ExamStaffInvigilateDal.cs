using BPM.Common.Data;
using BPM.Common.Provider;
using Exam.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exam.Core.Dal
{
    public class ExamStaffInvigilateDal : BaseRepository<ExamStaffInvigilateModel>
    {

        public static ExamStaffInvigilateDal Instance
        {
            get { return SingletonProvider<ExamStaffInvigilateDal>.Instance; }
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort, string order)
        {
            return base.JsonDataForEasyUIdataGrid("V_Exam_Staff_Invigilate", pageindex, pagesize, filterJson, sort, order);
        }
    }
}
