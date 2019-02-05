using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Exam.Core.Model
{
    [TableName("Exam_ExamSectionItems")]
    [Description("场次信息")]
    public class ExamExamSectionItemModel
    {
        public int KeyId { get; set; }
        public int ExamSectionId { get; set; }
        public string Address { get; set; }
        public string Subject { get; set; }
        public int StudentCount { get; set; }
        public int TeacherCount { get; set; }
        public string Memo { get; set; }
    }
}
