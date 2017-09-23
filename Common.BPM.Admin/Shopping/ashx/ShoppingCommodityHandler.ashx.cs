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
using Washer.Toolkit;

namespace BPM.Admin.Shopping.ashx
{
    /// <summary>
    /// ShoppingCommodityHandler 的摘要说明
    /// </summary>
    public class ShoppingCommodityHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            UserBll.Instance.CheckUserOnlingState();

            User user = SysVisitor.Instance.CurrentUser;
            int departmentId = user.DepartmentId;

            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<ShoppingCommodityModel>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<ShoppingCommodityModel>>(json);
                rpm.CurrentContext = context;
            }

            ShoppingCommodityModel model;
            string filter;
            switch (rpm.Action)
            {
                case "add":
                    model = rpm.Entity;
                    model.DepartmentId = departmentId;
                    if (ShoppingCommodityBll.Instance.Add(model)>0)
                    {
                        for(int i = 0; i < model.Gallery.Length; i++)
                        {
                            ShoppingGalleryModel g = new ShoppingGalleryModel()
                            {
                                CommodityId = model.KeyId, Picture = model.Gallery[i], Sorting = i
                            };
                            ShoppingGalleryBll.Instance.Add(g);
                        }

                        context.Response.Write(JSONhelper.ToJson(new ResponseObject() { Success=true}));
                    }else
                    {
                        context.Response.Write(JSONhelper.ToJson(new ResponseObject() { Success=false, Message="添加商品信息错误。"}));
                    }
                    break;
                default:
                    if (user.IsAdmin)
                    {
                        context.Response.Write(ShoppingCommodityBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, rpm.Filter, rpm.Sort, rpm.Order));
                    }
                    else
                    {
                        filter = string.Format("{{\"groupOp\":\"AND\",\"rules\":[{{\"field\":\"DepartmentId\",\"op\":\"eq\",\"data\":\"{0}\"}}],\"groups\":[{1}]}}", user.DepartmentId, rpm.Filter);
                        context.Response.Write(ShoppingCommodityBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, filter, rpm.Sort, rpm.Order));
                    }
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