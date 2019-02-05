using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Exam.Core.Model
{
    [TableName("Exam_ExamSections")]
    [Description("场次信息")]
    public class ExamExamSectionModel
    {
        public int KeyId { get; set; }
        public int ExamId { get; set; }
        public string Title { get; set; }
        public DateTime Started { get; set; }
        public DateTime Ended { get; set; }
        public string Memo { get; set; }
    }
}
