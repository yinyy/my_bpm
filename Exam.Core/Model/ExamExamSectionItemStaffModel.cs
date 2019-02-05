using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Exam.Core.Model
{
    [TableName("Exam_ExamSectionItemStaff")]
    [Description("某场次的已安排的监考信息")]
    public class ExamExamSectionItemStaffModel
    {
        public int KeyId { get; set; }
        public int ExamSectionItemId { get; set; }
        public int StaffId { get; set; }
        public int Confirmed { get; set; }
    }
}
