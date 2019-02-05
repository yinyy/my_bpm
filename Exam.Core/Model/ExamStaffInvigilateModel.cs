using BPM.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Exam.Core.Model
{
    [TableName("Exam_StaffInvigilates")]
    [Description("监考员信息")]
    public class ExamStaffInvigilateModel
    {
        public int KeyId { get; set; }
        public int StaffId { get; set; }
        public int Invigilated { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}
