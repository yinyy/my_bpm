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
using Omu.ValueInjecter;


namespace BPM.Admin.Washer.ashx
{
    /// <summary>
    /// WasherCustomHandler 的摘要说明
    /// </summary>
    public class WasherCustomHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            UserBll.Instance.CheckUserOnlingState();

            User user = SysVisitor.Instance.CurrentUser;
            int departmentId = user.DepartmentId;

            string filter;
            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<WasherCustomModel>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<WasherCustomModel>>(json);
                rpm.CurrentContext = context;
            }


            WasherCustomModel model;
            switch (rpm.Action)
            {
                case "add":
                    model = rpm.Entity;
                    model.DepartmentId = departmentId;
                    model.UserId = user.KeyId;

                    context.Response.Write(WasherCustomBll.Instance.Add(model));
                    break;
                case "edit":
                    model = WasherCustomBll.Instance.GetById(rpm.KeyId);
                    model.Name = rpm.Entity.Name;
                    model.Gender = rpm.Entity.Gender;
                    model.Memo = rpm.Entity.Memo;
                    model.Card = rpm.Entity.Card;

                    context.Response.Write(WasherCustomBll.Instance.Update(model));
                    break;
                case "del":
                    context.Response.Write(WasherCustomBll.Instance.Delete(rpm.KeyId));
                    break;
                case "list2":
                    filter = string.Format("{{'groupOp':'AND','rules':[{{'field':'DepartmentId','op':'eq','data':{0}}}, {{'field':'UserId','op':'eq','data':{1}}}],'groups':[{2}]}}", departmentId, user.KeyId, rpm.Filter);
                    context.Response.Write(WasherCustomBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, filter, rpm.Sort, rpm.Order));
                    break;
                case "consume":
                    context.Response.Write(WasherConsumeBll.Instance.GetConsumeJsonByCustomId(rpm.KeyId));
                    break;
                case "recharge":
                    context.Response.Write(WasherRechargeBll.Instance.GetRechargeJsonByCustomId(rpm.KeyId));
                    break;
                default:
                    filter = string.Format("{{'groupOp':'AND','rules':[{{'field':'DepartmentId','op':'eq','data':{0}}}],'groups':[{1}]}}", departmentId, rpm.Filter);
                    context.Response.Write(WasherCustomBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, filter, rpm.Sort, rpm.Order));
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