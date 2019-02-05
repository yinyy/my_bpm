using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exam.Core.Model
{
    [TableName("V_Exam_Section")]
    public class ExamExamSectionViewModel: ExamExamSectionModel
    {
        public int ExamSectionId { get; set; }
        public int? ItemCount { get; set; }
        public int? TeacherRequired { get; set; }
        public int TeacherSelected { get; set; }
        public string TeacherNames { get; set; }
    }
}
