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
    public class WasherCardDal : BaseRepository<WasherCardModel>
    {
        public static WasherCardDal Instance
        {
            get { return SingletonProvider<WasherCardDal>.Instance; }
        }

        internal string GetJson(int pageindex, int pagesize, string filterJson, string sort, string order)
        {
            return base.JsonDataForEasyUIdataGrid("V_Cards", pageindex, pagesize, filterJson,
                                                  sort, order);
        }

        public WasherCardModel Get(int departmentId, string cardNo)
        {
            return GetWhere(new { departmentId = departmentId, CardNo = cardNo }).FirstOrDefault();
        }

        public DataTable Export(string filter, string sort = "KeyId", string order = "desc")
        {
            string sortorder = sort + " " + order;
            string showfiels = "CardNo as 卡号, Password as 密码, (case when Kind='Inner' then '内部发行卡' when Kind='Sale' then '售卖卡' when Kind='Coupon' then '优惠卡' when Kind='Convertor' then '兑换卡' end) as 卡类型, ValidateFrom as 有效期开始, ValidateEnd as 有效期结束, Binded as 绑卡时间, Name as 持卡人, Coins as '卡余额（分）', Status as '卡状态', DepartmentName as 所属客户";

            var pcp = new ProcCustomPage("V_Cards")
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
