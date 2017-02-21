using BPM.Common.Data;
using BPM.Common.Data.Filter;
using BPM.Common.Provider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Washer.Model;

namespace Washer.Dal
{
    public class WasherRewardDal : BaseRepository<WasherRewardModel>
    {
        public static WasherRewardDal Instance
        {
            get { return SingletonProvider<WasherRewardDal>.Instance; }
        }

        internal string GetJson(int pageindex, int pagesize, string filter, string sort, string order)
        {
            return base.JsonDataForEasyUIdataGrid("V_Rewards", pageindex, pagesize, filter, sort, order);
        }

        public DataTable Export(string filter, string sort = "KeyId", string order = "desc")
        {
            string sortorder = sort + " " + order;
            string showfiels = "Name as 会员姓名, Kind as 积分类型, Points as 积分, Time as 获取时间, Used as 已用, DepartmentName as 运营商, Memo as 备注";

            var pcp = new ProcCustomPage("V_Rewards")
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
