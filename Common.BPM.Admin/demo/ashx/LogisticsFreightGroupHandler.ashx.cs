using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using BPM.Core;
using BPM.Core.Bll;
using BPM.Core.Dal;
using BPM.Core.Model;
using Omu.ValueInjecter;
using System.Configuration;
using BPM.Logistics.Bll;
using BPM.Logistics.model;

namespace BPM.Admin.demo.ashx
{
    /// <summary>
    /// LogisticsFreightGroupHandler 的摘要说明
    /// </summary>
    public class LogisticsFreightGroupHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            if (SysVisitor.Instance.IsGuest)
            {
                context.Response.Write(
                    new JsonMessage { Success = false, Data = "-99", Message = "登录已过期，请重新登录" }.ToString()
                    );
                context.Response.End();
            }

            string action = context.Request.Params["action"];
            switch (action)
            {
                case "hdfz":
                    string categoryCode = ConfigurationManager.AppSettings["freight_group_code"];
                    string dicJson = DicBll.Instance.DicJson(categoryCode);
                    context.Response.Write(dicJson);
                    break;
                case "freight":
                    context.Response.Write(DepartmentBll.Instance.GetFreightForDatagrid(Convert.ToInt32(ConfigurationManager.AppSettings["freight_department_parent_id"])));
                    break;
                case "line":
                    context.Response.Write(LogisticsFreightGroupBll.Instance.GetFreightByDic(Convert.ToInt32(context.Request.Params["id"])));
                    break;
                case "save":
                    int dicId = Convert.ToInt32(context.Request.Params["did"]);
                    string dids = context.Request.Params["dids"];

                    //先删除原有的分组信息
                    LogisticsFreightGroupBll.Instance.DeleteGroup(dicId);

                    //把新数据添加进去
                    foreach (string id in dids.Split(','))
                    {
                        LogisticsFreightGroupModel model = new LogisticsFreightGroupModel()
                        {
                            DepartmentId = Convert.ToInt32(id),
                            DicId = dicId
                        };

                        LogisticsFreightGroupBll.Instance.Add(model);
                    }

                    context.Response.Write("success");
                    break;
            }
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