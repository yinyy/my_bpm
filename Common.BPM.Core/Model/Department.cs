using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using BPM.Common.Data;
using BPM.Core.Dal;

namespace BPM.Core.Model
{
    [TableName("Sys_Departments")]
    [Description("部门管理")]
    public class Department
    {
        [DefaultValue(0)]
        public int KeyId { get; set; }

        [Description("部门名称")]
        public string DepartmentName { get; set; }

        [DefaultValue(0)]
        [Description("上级ID")]
        public int ParentId { get; set; }

        [DefaultValue(0)]
        [Description("排序")]
        public int Sortnum { get; set; }

        [Description("备注")]
        public string Remark { get; set; }

        [DbField(false)]
        public IEnumerable<Department> children
        {
            get { return DepartmentDal.Instance.GetChildren(KeyId); }
        }
       
        public string Brand { get; set; }

        public string Logo { get; set; }

        public string CardColor { get; set; }

        public string Setting { get; set; }

        public string Introduction { get; set; }

        public string Qrticket { get; set; }

        public string Appid { get; set; }
        public string Secret { get; set; }
        public string Aeskey { get; set; }
        public string Token { get; set; }
        public string Tag { get; set; }
        public string MerchantKey { get; set; }
        public string MerchantId { get; set; }
        /// <summary>
        /// tree 节点状态
        /// </summary>
        [DbField(false)]
        public string state
        {
            get
            {
                if (ParentId == 0)
                    return "open";
                return children.Any() ? "closed" : "open";
            }
        }
    }
}
