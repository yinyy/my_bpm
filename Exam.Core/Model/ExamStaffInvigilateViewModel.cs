using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exam.Core.Model
{
    [TableName("V_Exam_Staff_Invigilate")]
    public class ExamStaffInvigilateViewModel:ExamStaffInvigilateModel
    {
        public string Serial { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
    }
}
