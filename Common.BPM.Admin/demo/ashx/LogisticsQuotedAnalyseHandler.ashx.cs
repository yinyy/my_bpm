using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using BPM.Core.Bll;
using BPM.Common;
using BPM.Common.Data;
using BPM.Common.Data.Filter;
using BPM.Core;

namespace BPM.Admin.demo.ashx
{
    /// <summary>
    /// LogisticsQuotedAnalyse 的摘要说明
    /// </summary>
    public class LogisticsQuotedAnalyseHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            UserBll.Instance.CheckUserOnlingState();

            string where = FilterTranslator.ToSql(context.Request["Filter"]);
            var pcp = new ProcCustomPage("V_Quoted_Analyse")
                    {
                        PageIndex = 1,
                        PageSize = 999999,
                        OrderFields = "TrueName asc",
                        WhereString = where
            };

            int count = 0;

            DataTable table = DbUtils.GetPageWithSp(pcp, out count);
            context.Response.Write(JSONhelper.FormatJSONForEasyuiDataGrid(count, table));
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}