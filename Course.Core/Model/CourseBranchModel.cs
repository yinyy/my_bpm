using BPM.Common.Data;
using System.ComponentModel;

namespace Course.Core.Model
{
    [TableName("Course_Branches")]
    [Description("专业方向")]
    public class CourseBranchModel
    {
        public int KeyId { get; set;}
        public string Title { get; set; }
        public string Teacher { get; set; }
        public string Description { get; set; }
    }
}
