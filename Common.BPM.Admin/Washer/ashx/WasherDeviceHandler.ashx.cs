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
using BPM.Core.Dal;

namespace BPM.Admin.Washer.ashx
{
    //设备当前的状态，0表示状态位置，1表示当前正在工作，2表示空闲

    /// <summary>
    /// WasherDeviceHandler 的摘要说明
    /// </summary>
    public class WasherDeviceHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            UserBll.Instance.CheckUserOnlingState();

            User user = SysVisitor.Instance.CurrentUser;
            int departmentId = user.DepartmentId;

            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<WasherDeviceModel>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<WasherDeviceModel>>(json);
                rpm.CurrentContext = context;
            }

            WasherDeviceModel model;
            switch (rpm.Action)
            {
                case "add":
                    model = rpm.Entity;

                    model.Title = "";
                    model.Address = "";
                    model.Status = 0;
                    model.Position = "";
                    model.Updated = DateTime.Now;
                    model.Memo2 = "";
                    model.Deleted = false;
                  
                    context.Response.Write(WasherDeviceBll.Instance.Add(model));
                    break;
                case "edit":
                    model = WasherDeviceBll.Instance.GetById(rpm.KeyId);

                    model.Serial = rpm.Entity.Serial;
                    model.DepartmentId = rpm.Entity.DepartmentId;
                    model.Memo = rpm.Entity.Memo;
        
                    context.Response.Write(WasherDeviceBll.Instance.Update(model));
                    break;
                case "edit2":
                    model = WasherDeviceBll.Instance.GetById(rpm.KeyId);

                    model.Title = rpm.Entity.Title;
                    model.Address = rpm.Entity.Address;
                    model.Memo2 = rpm.Entity.Memo2;

                    context.Response.Write(WasherDeviceBll.Instance.Update(model));
                    break;
                case "del":
                    context.Response.Write(WasherDeviceBll.Instance.Delete(rpm.KeyId));
                    break;
                case "del2":
                    model = WasherDeviceBll.Instance.GetById(rpm.KeyId);
                    model.Deleted = true;

                    context.Response.Write(WasherDeviceBll.Instance.Update(model));
                    break;
                case "dept":
                    context.Response.Write(JSONhelper.ToJson(DepartmentDal.Instance.GetAll().OrderBy(a => a.DepartmentName).Select(a => new { KeyId = a.KeyId, DepartmentName = a.DepartmentName })));
                    break;
                case "list2":
                    string filter = string.Format("{{'groupOp':'AND','rules':[{{'field':'DepartmentId','op':'eq','data':{0}}}, {{'field':'Deleted','op':'eq','data':0}}],'groups':[{1}]}}", departmentId, rpm.Filter);
                    context.Response.Write(WasherDeviceBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, filter, rpm.Sort, rpm.Order));
                    break;
                default:
                    context.Response.Write(WasherDeviceBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, rpm.Filter, rpm.Sort, rpm.Order));
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