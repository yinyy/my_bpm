using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BPM.Core.Model;
using BPM.Common;
using BPM.Common.Data;
using BPM.Common.Data.Filter;
using BPM.Common.Provider;

namespace BPM.Core.Dal
{
    public class LogDal : BaseRepository<LogModel>
    {
        public static LogDal Instance
        {
            get { return SingletonProvider<LogDal>.Instance; }
        }

        public string JsonDataForEasyUIdataGrid(int pageindex, int pagesize, string filterJSON)
        {
            var pcp = new ProcCustomPage("V_logs")
            {
                PageIndex = pageindex,
                PageSize = pagesize,
                OrderFields = "keyid desc",
                WhereString = FilterTranslator.ToSql(filterJSON)
            };
            int recordCount;
            DataTable dt = base.GetPageWithSp(pcp, out recordCount);
            return JSONhelper.FormatJSONForEasyuiDataGrid(recordCount, dt);

        }

        public IEnumerable<LogModel> GetList(int days)
        {
            string sql = "select * from sys_logs where datediff(d,OperationTime,getdate()) > " + days;
            return DbUtils.ExecuteReader<LogModel>(sql,null);
        }

        /// <summary>
        /// 清除所有操作日志
        /// </summary>
        public void RemoveAll()
        {
            string sql = "truncate table sys_logs TRUNCATE TABLE sys_logdetails";
            DbUtils.ExecuteNonQuery(sql, null);
        }
    }
}
