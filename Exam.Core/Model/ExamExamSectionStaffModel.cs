using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Exam.Core.Model
{
    [TableName("Exam_ExamSectionStaff")]
    [Description("考试信息")]
    public class ExamExamSectionStaffModel
    {
        public int KeyId { get; set; }
        public int ExamSectionId { get; set; }
        public int StaffId { get; set; }
        public DateTime Created { get; set; }
    }
}
