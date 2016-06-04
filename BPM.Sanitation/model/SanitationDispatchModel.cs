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
      
		[Description("加注类型")]
        public int Kind { get; set; }
        [Description("浓度")]
        public int Potency { get; set; }
        /// <summary>
        /// 0：表示加注完成
        /// 1：表示签到完成
        /// </summary>
        [Description("状态")]
        public int Status { get; set; }
        /// <summary>
        /// 签到的位置坐标
        /// </summary>
        [Description("目的地")]
        public string Destination { get; set; }
        /// <summary>
        /// 签到的区域
        /// </summary>
        [Description("签到区域")]
        public int Region { get; set; }
        [Description("签到时间")]
        public DateTime Signed { get; set; }
        /// <summary>
        /// 0：表示粗管
        /// 1：表示细管
        /// </summary>
        [Description("管子类型")]
        public int Working { get; set; }
        [Description("加水地点")]
        public string Address { get; set; }

        [Description("加水容积")]
        public float Volumn { get; set; }

        public string Memo { get; set; }
		public override string ToString()
		{
			return JSONhelper.ToJson(this);
		}
	}
}