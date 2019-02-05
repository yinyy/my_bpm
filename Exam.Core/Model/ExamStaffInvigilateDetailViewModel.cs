using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Exam.Core.Model
{
    [TableName("V_Exam_Staff_Invigilate_Detail")]
    [Description("监考员监考明细")]
    public class ExamStaffInvigilateDetailViewModel
    {
        public int ExamSectionItemStaffId { get; set; }
        public int StaffId { get; set; }
        public string StaffName{ get; set; }
        public DateTime Started { get; set; }
        public DateTime Ended { get; set; }
        public int ExamSectionItemId { get; set; }
        public string Address { get; set; }
        public string Subject { get; set; }
        public int ExamId { get; set; }
        public string ExamTitle { get; set; }
        public int ExamSectionId { get; set; }
        public string ExamSectionTitle { get; set; }
        public int Confirmed { get; set; }
    }
}
