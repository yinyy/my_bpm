﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using BPM.Core.Bll;
using BPM.Common;
using BPM.Common.Data;

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
            var pcp = new ProcCustomPage("V_Quoted_Analyse")
                    {
                        PageIndex = 1,
                        PageSize = 999999,
                        OrderFields = "TrueName asc"
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