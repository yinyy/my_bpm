using BPM.Common.Data;
using BPM.Common.Data.Filter;
using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Washer.Model;
using Washer.Toolkit;

namespace Washer.Dal
{
    public class WasherDeviceLogDal : BaseRepository<WasherDeviceLogModel>
    {
        public static WasherDeviceLogDal Instance
        {
            get { return SingletonProvider<WasherDeviceLogDal>.Instance; }
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort,
                              string order, string fields)
        {
            return base.JsonDataForEasyUIdataGrid("V_DeviceLogs", pageindex, pagesize, filterJson,
                                                  sort, order, fields);
        }

        public DataTable Export(string filter, string sort="KeyId", string order="desc")
        {
            string sortorder = sort + " " + order;
            string showfiels = "(case when ConsumeName is null then '未注册' else ConsumeName end) as 消费者, (convert(varchar(30), Started, 120) +' - '+substring(convert(varchar(30),  Ended, 120), 12, 8)) as 洗车时间, convert(varchar(100), SerialNumber) as 洗车机序列号, Address as 洗车地点,convert(decimal(10, 2), PayCoins/100.0) as 消费洗车币, Kind as 支付方式, CardNo as 支付卡号,convert(varchar(50), Ticks) as 支付凭证, DepartmentName as 所属客户";

            var pcp = new ProcCustomPage("V_DeviceLogs")
            {
                PageIndex = 1,
                PageSize = Int32.MaxValue,
                OrderFields = sortorder,
                WhereString = FilterTranslator.ToSql(filter),
                ShowFields = showfiels
            };
            int recordCount;

            return GetPageWithSp(pcp, out recordCount);
        }
    }
}