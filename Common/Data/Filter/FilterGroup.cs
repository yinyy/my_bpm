using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPM.Common.Data.Filter
{
    public class FilterGroup
    {
        /// <summary>
        /// 筛选条件组合方式 and or
        /// </summary>
        public GroupOp groupOp { get; set; }
        public IList<FilterRule> Rules { get; set; }
        public IList<FilterGroup> Groups { get; set; }
    }

    public enum GroupOp
    { 
        AND,
        OR
    }
}
