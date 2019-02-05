using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Exam.Core.Model
{
    [TableName("Exam_Exams")]
    [Description("考试信息")]
    public class ExamExamModel
    {
        public int KeyId { get; set; }
        public string Title { get; set; }
        public DateTime Started { get; set; }
        public DateTime Ended { get; set; }
        public string Memo { get; set; }
        public DateTime SecKillStarted { get; set; }
        public DateTime SecKillEnded { get; set; }
        public int Freezed { get; set; }
    }
}
