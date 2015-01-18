using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using BPM.Common;
using BPM.Common.Data;

namespace Sanitation.Model
{
	[TableName("Sanitation_Dispatch")]
	[Description("调度单")]
	public class SanitationDispatchModel
	{
		/// <summary>
		/// 主键
		/// </summary>
		[Description("主键")]
		public int KeyId { get; set; }
      
		/// <summary>
		/// 日期
		/// </summary>
		[Description("日期")]
		public DateTime Time { get; set; }
      
		/// <summary>
		/// 姓名
		/// </summary>
		[Description("姓名")]
		public int DriverId { get; set; }
      
		/// <summary>
		/// 车辆
		/// </summary>
		[Description("车辆")]
		public int TrunkId { get; set; }
      
		/// <summary>
		/// 次数
		/// </summary>
		[Description("次数")]
		public int Workload { get; set; }

        [Description("加注类型")]
        public int KindId { get; set; }
        [Description("浓度")]
        public int Potency { get; set; }
		/// <summary>
		/// 备注
		/// </summary>
		[Description("备注")]
		public string Memo { get; set; }

        [Description("是否生效")]
        public string Enabled { get; set; }	

		public override string ToString()
		{
			return JSONhelper.ToJson(this);
		}
	}
}