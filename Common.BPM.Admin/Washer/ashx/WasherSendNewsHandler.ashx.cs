using BPM.Common;
using BPM.Core;
using BPM.Core.Bll;
using BPM.Core.Model;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.Media;
using Senparc.Weixin.MP.CommonAPIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Washer.Bll;

namespace BPM.Admin.Washer.ashx
{
    /// <summary>
    /// WasherSendNewsHandler 的摘要说明
    /// </summary>
    public class WasherSendNewsHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            UserBll.Instance.CheckUserOnlingState();

            User user = SysVisitor.Instance.CurrentUser;
            Department dept = DepartmentBll.Instance.Get(user.DepartmentId);

            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<object>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<object>>(json);
                rpm.CurrentContext = context;
            }

            switch (rpm.Action)
            {
                default:
                    MediaList_NewsResult result = MediaApi.GetNewsMediaList(AccessTokenContainer.TryGetAccessToken(dept.Appid, dept.Secret), rpm.Pageindex, rpm.Pagesize);

                    if (result.errcode == Senparc.Weixin.ReturnCode.请求成功)
                    {
                        context.Response.Write(JSONhelper.ToJson(new
                        {
                            total = result.total_count,
                            rows = result.item.Select(a => new
                            {
                                MediaId = a.media_id,
                                UpdateTime = ConvertTime(a.update_time),
                                ItemCount = a.content.news_item.Count,
                                ItemTitles = a.content.news_item.Select(b => b.title).Aggregate((s, i) =>
                                {
                                    return string.IsNullOrWhiteSpace(s) ? i : s + "<br/>" + i;
                                }),
                                Items = a.content.news_item.Select(b => new
                                {
                                    Title = b.title,
                                    ThumbMediaId = b.thumb_media_id,
                                    ShowConverPic = b.show_cover_pic,
                                    Author = b.author,
                                    Digest = b.digest,
                                    Content = b.content,
                                    Url = b.url,
                                    ContentSourceUrl = b.content_source_url
                                }).ToArray()
                            }).ToArray()
                        }));
                    }
                    else
                    {
                        context.Response.Write(JSONhelper.ToJson(new { total = 0 }));
                    }
                    break;
            }

            context.Response.Flush();
            context.Response.End();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private string ConvertTime(long time)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(time + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtStart.Add(toNow));
        }
    }
}