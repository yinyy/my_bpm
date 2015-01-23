using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using BPM.Common;
using BPM.Common.Data;

namespace BPM.Logistics.Model
{
	[TableName("Logistics_Inquiry")]
	[Description("询盘表")]
	public class LogisticsInquiryModel
	{
		/// <summary>
		/// 主键
		/// </summary>
		[Description("主键")]
		public long KeyId { get; set; }
      
		/// <summary>
		/// 询价人
		/// </summary>
		[Description("询价人")]
		public int Inquirer { get; set; }
      
		/// <summary>
		/// 询价时间
		/// </summary>
		[Description("询价时间")]
		public DateTime Published { get; set; }
      
		/// <summary>
		/// 货物名称
		/// </summary>
		[Description("货物名称")]
		public string Cargo { get; set; }
      
		/// <summary>
		/// 柜数
		/// </summary>
		[Description("柜数")]
		public int Amount { get; set; }
      
		/// <summary>
		/// 包装
		/// </summary>
		[Description("包装")]
		public int Packing { get; set; }
      
		/// <summary>
		/// 吨数
		/// </summary>
		[Description("吨数")]
		public decimal Weight { get; set; }
      
		/// <summary>
		/// 目的港
		/// </summary>
		[Description("目的港")]
		public string Port { get; set; }
      
		/// <summary>
		/// 区域
		/// </summary>
		[Description("ETD")]
		public string Etd { get; set; }
      
		/// <summary>
		/// 船期与航程
		/// </summary>
		[Description("ETA")]
		public string Eta{ get; set; }
      
		/// <summary>
		/// 类型
		/// </summary>
		[Description("类型")]
		public int Kind { get; set; }
      
		/// <summary>
		/// 报价截止日期
		/// </summary>
		[Description("报价截止日期")]
		public DateTime Ended { get; set; }
      
		/// <summary>
		/// 免费箱使
		/// </summary>
		[Description("免费箱使")]
		public int Freebox { get; set; }
      
		/// <summary>
		/// 备注
		/// </summary>
		[Description("备注")]
		public string Memo { get; set; }
      
		/// <summary>
		/// 附件
		/// </summary>
		[Description("附件")]
		public string Attachment { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Description("状态")]
        public string Status{ get; set; }

        /// <summary>
        /// 供应商编号
        /// </summary>
        [Description("供应商编号")]
        public string SupplyIds{ get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        [Description("供应商名称")]
        public string SupplyNames{ get; set; }
				
		public override string ToString()
		{
			return JSONhelper.ToJson(this);
		}
	}
}