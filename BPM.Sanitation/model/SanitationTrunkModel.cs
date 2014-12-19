using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using BPM.Common;
using BPM.Common.Data;

namespace Sanitation.Model
{
	[TableName("Sanitation_Trunk")]
	[Description("车辆表")]
	public class SanitationTrunkModel
	{
		/// <summary>
		/// 主键
		/// </summary>
		[Description("主键")]
		public int KeyId { get; set; }
      
		/// <summary>
		/// 品牌
		/// </summary>
		[Description("品牌")]
		public string Brand { get; set; }
      
		/// <summary>
		/// 型号
		/// </summary>
		[Description("型号")]
		public string Model { get; set; }
      
		/// <summary>
		/// 车牌号
		/// </summary>
		[Description("车牌号")]
		public string Plate { get; set; }
      
		/// <summary>
		/// 体积
		/// </summary>
		[Description("体积")]
		public decimal Volumn { get; set; }
      
		/// <summary>
		/// 备注
		/// </summary>
		[Description("备注")]
		public string Memo { get; set; }
      
				
		public override string ToString()
		{
			return JSONhelper.ToJson(this);
		}
	}
}