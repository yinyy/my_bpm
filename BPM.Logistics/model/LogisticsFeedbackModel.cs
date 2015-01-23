using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using BPM.Common;
using BPM.Common.Data;

namespace BPM.Logistics.Model
{
	[TableName("Logistics_Feedback")]
	[Description("报价表")]
	public class LogisticsFeedbackModel
	{
		/// <summary>
		/// 主键
		/// </summary>
		[Description("主键")]
		public long KeyId { get; set; }
      
		/// <summary>
		/// 询盘编号
		/// </summary>
		[Description("询盘编号")]
		public long InquiryId { get; set; }
      
		/// <summary>
		/// 供应商编号
		/// </summary>
		[Description("供应商编号")]
		public long SupplyId { get; set; }
      
		/// <summary>
		/// 报价时间
		/// </summary>
		[Description("报价时间")]
		public DateTime Published { get; set; }
      
		/// <summary>
		/// 价格
		/// </summary>
		[Description("价格")]
		public decimal Price { get; set; }
      
		/// <summary>
		/// 船公司
		/// </summary>
		[Description("船公司")]
		public string Ship { get; set; }
      
		/// <summary>
		/// 航程
		/// </summary>
		[Description("ETD")]
		public string Etd{ get; set; }
      
		/// <summary>
		/// 备注
		/// </summary>
		[Description("备注")]
		public string Memo { get; set; }

        [Description("ETA")]
        public string Eta{ get; set; }

        [Description("记录报价的次数")]
        public int Counted { get; set; }
		public override string ToString()
		{
			return JSONhelper.ToJson(this);
		}
	}
}