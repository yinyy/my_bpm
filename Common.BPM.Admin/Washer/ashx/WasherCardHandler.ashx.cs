using BPM.Common;
using BPM.Core;
using BPM.Core.Bll;
using BPM.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Washer.Bll;
using Washer.Model;

namespace BPM.Admin.Washer.ashx
{
    /// <summary>
    /// WasherCardHandler 的摘要说明
    /// </summary>
    public class WasherCardHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            
            UserBll.Instance.CheckUserOnlingState();

            User user = SysVisitor.Instance.CurrentUser;
            int departmentId = user.DepartmentId;

            string filter;
            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<WasherCardModel>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<WasherCardModel>>(json);
                rpm.CurrentContext = context;
            }

            WasherCardModel model;
            switch (rpm.Action)
            {
                case "add":
                    model = rpm.Entity;
                    model.DepartmentId = departmentId;
                    model.UserId = user.KeyId;
                    model.Status = 0;

                    context.Response.Write(WasherCardBll.Instance.Add(model));
                    break;
                case "edit":
                    model = WasherCardBll.Instance.GetById(rpm.KeyId);

                    model.Serial  = rpm.Entity.Serial;
                    model.CustomId = rpm.Entity.CustomId;
                    model.Memo = rpm.Entity.Memo;

                    context.Response.Write(WasherCardBll.Instance.Update(model));
                    break;
                case "del":
                    context.Response.Write(WasherCardBll.Instance.Delete(rpm.KeyId));
                    break;
                case "cstm":
                    context.Response.Write(WasherCustomBll.Instance.GetJsonByUserId(rpm.KeyId == 0 ? user.KeyId : rpm.KeyId));
                    break;
                case "list2":
                    filter = string.Format("{{'groupOp':'AND','rules':[{{'field':'DepartmentId','op':'eq','data':{0}}}, {{'field':'UserId','op':'eq','data':{1}}}],'groups':[{2}]}}", departmentId, user.KeyId, rpm.Filter);
                    context.Response.Write(WasherCardBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, filter, rpm.Sort, rpm.Order));
                    break;
                case "loss":
                    model = WasherCardBll.Instance.GetById(rpm.KeyId);
                    model.Status = 1;
                    context.Response.Write(WasherCardBll.Instance.Update(model));
                    break;
                case "relieve":
                    model = WasherCardBll.Instance.GetById(rpm.KeyId);
                    model.Status = 0;
                    context.Response.Write(WasherCardBll.Instance.Update(model));
                    break;
                default:
                    filter = string.Format("{{'groupOp':'AND','rules':[{{'field':'DepartmentId','op':'eq','data':{0}}}],'groups':[{1}]}}", departmentId, rpm.Filter);
                    context.Response.Write(WasherCardBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, filter, rpm.Sort, rpm.Order));
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