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
    public class WasherCardLogDal : BaseRepository<WasherCardLogModel>
    {
        public static WasherCardLogDal Instance
        {
            get { return SingletonProvider<WasherCardLogDal>.Instance; }
        }

        internal string GetJson(int pageindex, int pagesize, string filter, string sort, string order)
        {
            return base.JsonDataForEasyUIdataGrid("V_CardLogs", pageindex, pagesize, filter,
                                                  sort, order);
        }

        public DataTable Export(string filter, string sort = "KeyId", string order = "desc")
        {
            string sortorder = sort + " " + order;
            string showfiels = "Time as 刷卡时间, (case when Name is null then '未注册用户' else Name end) as 持卡人, CardNo as 支付卡号, Kind as 卡类型, convert(decimal(10, 2), Coins/100.0) as 消费洗车币, Memo as 支付凭证, DepartmentName as 所属客户";

            var pcp = new ProcCustomPage("V_CardLogs")
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
