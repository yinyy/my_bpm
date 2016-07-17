using BPM.Common.Data;
using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Washer.Model;
using System.Data;
using BPM.Common.Data.Filter;

namespace Washer.Dal
{
    public class WasherOrderDal : BaseRepository<WasherOrderModel>
    {
        public static WasherOrderDal Instance
        {
            get { return SingletonProvider<WasherOrderDal>.Instance; }
        }

        public string GetJson(int pageindex, int pagesize, string filterJson, string sort = "Time",
                              string order = "desc")
        {
            return base.JsonDataForEasyUIdataGrid("V_Orders", pageindex, pagesize, filterJson, sort, order);
        }

        public DataTable Export(string filter, string sort = "Time", string order = "desc")
        {
            string sortorder = sort + " " + order;
            string showfiels = "Time as 时间, Serial as 订单号, Name as 付款方, Kind as 类型, Money as 支付金额, Status as 订单状态, TransactionId as 微信账单, Memo as 备注, DepartmentName as 所属客户";

            var pcp = new ProcCustomPage("V_Orders")
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
