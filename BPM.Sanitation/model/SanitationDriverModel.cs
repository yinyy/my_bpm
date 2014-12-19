using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using BPM.Common;
using BPM.Common.Data;

namespace Sanitation.Model
{
	[TableName("Sanitation_Driver")]
	[Description("司机表")]
	public class SanitationDriverModel
	{
		/// <summary>
		/// 主键
		/// </summary>
		[Description("主键")]
		public int KeyId { get; set; }
      
		/// <summary>
		/// 姓名
		/// </summary>
		[Description("姓名")]
		public string Name { get; set; }
      
		/// <summary>
		/// 性别
		/// </summary>
		[Description("性别")]
		public string Gender { get; set; }
      
		/// <summary>
		/// 电话
		/// </summary>
		[Description("电话")]
		public string Telphone { get; set; }
      
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