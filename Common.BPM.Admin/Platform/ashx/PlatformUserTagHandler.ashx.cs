using BPM.Common;
using BPM.Core;
using Newtonsoft.Json;
using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.UserTag;
using Senparc.Weixin.MP.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace BPM.Admin.Platform.ashx
{
    /// <summary>
    /// PlatformUserTagHandler 的摘要说明
    /// </summary>
    public class PlatformUserTagHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<TagJson_Tag>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<TagJson_Tag>>(json);
                rpm.CurrentContext = context;
            }

            switch (rpm.Action)
            {
                case "add":
                    CreateTagResult result = UserTagApi.Create(Config.SenparcWeixinSetting.WeixinAppId, rpm.Entity.name);
                    if (result.errcode == ReturnCode.请求成功)
                    {
                        context.Response.Write(1);
                    }
                    else
                    {
                        context.Response.Write(JsonConvert.SerializeObject(new {Data = -1, Message = result.errmsg}));
                    }
                    break;
                case "edit":
                    WxJsonResult result2 = UserTagApi.Update(Config.SenparcWeixinSetting.WeixinAppId, rpm.KeyId, rpm.Entity.name);
                    if (result2.errcode == ReturnCode.请求成功)
                    {
                        context.Response.Write(1);
                    }
                    else
                    {
                        context.Response.Write(JsonConvert.SerializeObject(new { Data = -1, Message = result2.errmsg }));
                    }
                    break;
                case "del":
                    WxJsonResult result3 = UserTagApi.Delete(Config.SenparcWeixinSetting.WeixinAppId, rpm.KeyId);
                    if (result3.errcode == ReturnCode.请求成功)
                    {
                        context.Response.Write(1);
                    }
                    else
                    {
                        context.Response.Write(JsonConvert.SerializeObject(new { Data = -1, Message = result3.errmsg }));
                    }
                    break;
                default:
                    TagJson tagJson = UserTagApi.Get(Config.SenparcWeixinSetting.WeixinAppId);
                    if (tagJson.errcode == ReturnCode.请求成功)
                    {
                        List<TagJson_Tag> tags = tagJson.tags;
                        var obj = new { total = tags.Count(), rows = tags.ToArray() };
                        context.Response.Write(JsonConvert.SerializeObject(obj));
                    }
                    else
                    {
                        var obj = new { total = 0};
                        context.Response.Write(JsonConvert.SerializeObject(obj));
                    }
                    
                    break;
            }

            context.Response.End();
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