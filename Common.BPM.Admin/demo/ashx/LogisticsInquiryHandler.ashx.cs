using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using BPM.Logistics.Model;
using BPM.Logistics.Bll;
using BPM.Logistics.ext;
using Omu.ValueInjecter;
using BPM.Core;
using BPM.Core.Bll;
using BPM.Common;
using BPM.Common.Data;
using BPM.Core.Model;
using System.Configuration;
using System.Net.Mail;
using System.Data;
using BPM.Common.Data.Filter;
using BPM.Logistics.Util;

namespace BPM.Admin.demo.ashx
{
    /// <summary>
    /// dbHandler 的摘要说明
    /// </summary>
    public class LogisticsInquiryHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            UserBll.Instance.CheckUserOnlingState();

            //int k;
            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<LogisticsInquiryModel>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<LogisticsInquiryModel>>(json);
                rpm.CurrentContext = context;
            }

            switch (rpm.Action)
            {
                case "add":
                    context.Response.Write(LogisticsInquiryBll.Instance.Add(rpm.Entity));
                    break;
                case "edit":
                    LogisticsInquiryModel d = new LogisticsInquiryModel();
                    d.InjectFrom(rpm.Entity);
                    d.KeyId = rpm.KeyId;
                    context.Response.Write(LogisticsInquiryBll.Instance.Update(d));
                    break;
                case "delete":
                    //先删除Logistic_Feedback中的信息
                    DbUtils.DeleteWhere<LogisticsFeedbackModel>(new { InquiryId = rpm.KeyId });
                    //再删除Logistic_Inquiry中的信息
                    context.Response.Write(LogisticsInquiryBll.Instance.Delete(rpm.KeyId));
                    break;
                case "dics":
                    context.Response.Write(DicBll.Instance.GetDicListBy(rpm.KeyId));
                    break;
                case "depart_tree":
                    context.Response.Write(JSONhelper.ToJson(DicBll.Instance.GetListBy(ConfigurationManager.AppSettings["freight_group_code"]).
                        OrderBy(c => c.Sortnum).Select(c => new
                    {
                        id = c.KeyId,
                        text = c.Title,
                        children = DepartmentBll.Instance.GetFreight(LogisticsFreightGroupBll.Instance.GetListByDic(c.KeyId).Select(f => f.DepartmentId).ToArray()).Select(dd => new
                        {
                            id = dd.KeyId,
                            text = dd.DepartmentName
                        }).ToArray()
                    }).ToArray()));
                    break;
                case "send":
                    try
                    {
                        //先清除Logistic_Feedback中的记录
                        DbUtils.DeleteWhere<LogisticsFeedbackModel>(new { InquiryId = rpm.KeyId });
                        //在Logistic_Feedback中增加记录
                        LogisticsInquiryModel lim = DbUtils.Get<LogisticsInquiryModel>(rpm.KeyId);
                        Dictionary<string, string> emails = new Dictionary<string, string>();
                        foreach (int did in LogisticsFreightGroupBll.Instance.GetListByDic(Convert.ToInt32(lim.SupplyIds)).Select(dd=>dd.DepartmentId))
                        {
                            foreach (User u in DbUtils.GetWhere<User>(new { DepartmentId = did, IsDisabled = false }))
                            {
                                LogisticsFeedbackModel lfm = new LogisticsFeedbackModel();
                                lfm.InquiryId = rpm.KeyId;
                                lfm.SupplyId = u.KeyId;
                                lfm.Memo = "";

                                LogisticsFeedbackBll.Instance.Add(lfm);

                                //添加到要发送的Email列表
                                emails.Add(u.Email, u.TrueName);
                            }
                        }
                        //发送邮件
                        EmailHelper.SendEmail(ConfigurationManager.AppSettings["email"],
                            ConfigurationManager.AppSettings["password"],
                            ConfigurationManager.AppSettings["smtp"],
                            emails,
                            string.Format(ConfigurationManager.AppSettings["inquiry_email_subject"], DateTime.Now),
                            string.Format(ConfigurationManager.AppSettings["inquiry_email_body"], lim.Port, lim.Amount, lim.Cargo));
                        //更新Logistics_Inquiry中的状态
                        lim.Status = "已发送";
                        LogisticsInquiryBll.Instance.Update(lim);

                        context.Response.Write(1);
                    }
                    catch
                    {
                        context.Response.Write(0);
                    }
                    break;
                case "admin_delete":
                    context.Response.Write(LogisticsInquiryBll.Instance.AdminDelete(rpm.KeyId));
                    break;
                case "detail":
                    context.Response.Write(LogisticsFeedbackBll.Instance.Detail(rpm.KeyId));
                    break;
                default:
                    string where = FilterTranslator.ToSql(rpm.Filter);
                    where = " NewIndex = 1 and " + where;

                    //TODO:这个地方还需要再测试
                    var pcp = new ProcCustomPage("V_Inquiry_Detail")
                    {
                        PageIndex = rpm.Pageindex,
                        PageSize = rpm.Pagesize,
                        WhereString = where,
                        OrderFields = rpm.Sort + " " + rpm.Order
                    };

                    int count = 0;
                    DataTable table = DbUtils.GetPageWithSp(pcp, out count);

                    context.Response.Write(JSONhelper.FormatJSONForEasyuiDataGrid(count, table));
                    //context.Response.Write(LogisticsInquiryBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, rpm.Filter, rpm.Sort, rpm.Order));
                    //context.Response.Write(ViewInquiryDetailBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, rpm.Filter, rpm.Sort, rpm.Order));
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