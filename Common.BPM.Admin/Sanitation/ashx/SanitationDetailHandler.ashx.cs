using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Sanitation.Model;
using Sanitation.Bll;
using Newtonsoft.Json;

using Omu.ValueInjecter;
using BPM.Core;
using BPM.Core.Bll;
using BPM.Common;
using Newtonsoft.Json.Linq;
using BPM.Common.Data;
using BPM.Common.Data.Filter;
using System.Data;

namespace BPM.Admin.Sanitation.ashx
{
    /// <summary>
    /// dbHandler 的摘要说明
    /// </summary>
    public class SanitationDetailHandler : IHttpHandler,IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            UserBll.Instance.CheckUserOnlingState();

            //int k;
            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<SanitationDetailModel>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<SanitationDetailModel>>(json);
                rpm.CurrentContext = context;
            }

            DateTime start = DateTime.Parse("2014-11-01");

            switch (rpm.Action)
            {
                case "add":
                    context.Response.Write(SanitationDetailBll.Instance.Add(rpm.Entity));
                    break;
                case "edit":
                    SanitationDetailModel d = new SanitationDetailModel();
                    d.InjectFrom(rpm.Entity);
                    d.KeyId = rpm.KeyId;
                    context.Response.Write(SanitationDetailBll.Instance.Update(d));
                    break;
                case "delete":
                    context.Response.Write(SanitationDetailBll.Instance.Delete(rpm.KeyId));
                    break;
                case "month":
                    DateTime now = DateTime.Now;
                    List<object> datas = new List<object>();

                    do
                    {
                        datas.Add(new { text = now.ToString("yyyy年MM月"), value = now.ToString("yyyy-MM") });
                        now = now.AddMonths(-1);
                    } while (now > start);

                    context.Response.Write(JSONhelper.ToJson(datas.ToArray()));
                    break;
                case "quarter":
                    now = DateTime.Now;
                    datas = new List<object>();

                    do
                    {
                        datas.Add(new { text = string.Format("{0:yyyy}年{1}", now, now.Quarter("large")), value = string.Format("{0:yyyy}-{1}", now, now.Quarter("small")) });
                        now = now.AddMonths(-3);
                    } while (now > start);

                    context.Response.Write(JSONhelper.ToJson(datas.ToArray()));
                    break;
                case "year":
                    datas = new List<object>();

                    for (int i = DateTime.Now.Year; i >= 2014; i--)
                    {
                        datas.Add(new { text = string.Format("{0}年", i), value = i });
                    }

                    context.Response.Write(JSONhelper.ToJson(datas.ToArray()));
                    break;
                case "address":
                    context.Response.Write(JSONhelper.ToJson(DicBll.Instance.GetListBy("device").Select(a => new { Code = a.Code, Title = a.Title }).OrderBy(a => a.Title).ToArray()));
                    break;
                case "export":
                    var pcp = new ProcCustomPage("V_Detail")
                    {
                        ShowFields = "Name as 姓名, Code as 编号, Plate as 车牌号, Time as 时间, Volumn as 加注量, Title as 加注地点",
                        PageIndex = 1,
                        PageSize = 9999999,
                        OrderFields = "Time desc, Name, Plate",
                        WhereString = rpm.JsonEntity != null ? FilterTranslator.ToSql(rpm.JsonEntity) : ""
                    };
                    int recordCount;
                    DataTable dt = DbUtils.GetPageWithSp(pcp, out recordCount);

                    GridViewExportUtil.Export(DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".xls", dt);
                    break;
                default:
                    context.Response.Write(SanitationDetailBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, rpm.Filter, rpm.Sort, rpm.Order));
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