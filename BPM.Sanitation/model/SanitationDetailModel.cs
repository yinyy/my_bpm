using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using BPM.Common;
using BPM.Common.Data;

namespace Sanitation.Model
{
	[TableName("Sanitation_Detail")]
	[Description("加水详情")]
	public class SanitationDetailModel
	{
		/// <summary>
		/// 主键
		/// </summary>
		[Description("主键")]
		public int KeyId { get; set; }

        [Description("参考SanitationDispatch的KeyId")]
        public int ReferDispatchId { get; set; }

		/// <summary>
		/// 姓名
		/// </summary>
		[Description("姓名")]
		public int DriverId { get; set; }
      
		/// <summary>
		/// 车牌号
		/// </summary>
		[Description("车牌号")]
		public int TrunkId { get; set; }
      
		/// <summary>
		/// 时间
		/// </summary>
		[Description("时间")]
		public DateTime Time { get; set; }
      
		/// <summary>
		/// 加注量
		/// </summary>
		[Description("加注量")]
		public decimal Volumn { get; set; }
      
		/// <summary>
		/// 地点
		/// </summary>
		[Description("地点")]
		public string Address { get; set; }
      
				
		public override string ToString()
		{
			return JSONhelper.ToJson(this);
		}
	}
}