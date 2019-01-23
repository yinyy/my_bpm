using BPM.Common.Data;
using System;
using System.ComponentModel;

namespace Course.Core.Model
{
    [TableName("Course_Staff_Branch")]
    [Description("学生选择专业方向")]
    public class CourseStaffBranchModel
    {
        public int KeyId { get; set;}
        public int StaffId { get; set; }
        public int BranchId { get; set; }
        public string Introduction { get; set; }
        public int Sorted { get; set; }
        public int Accepted { get; set; }
        public DateTime ChooseTime { get; set; }
        public DateTime AcceptTime { get; set; }
    }
}
