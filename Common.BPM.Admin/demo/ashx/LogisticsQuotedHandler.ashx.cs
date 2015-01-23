using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using BPM.Logistics.Model;
using BPM.Logistics.Bll;

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
using BPM.Logistics;

namespace BPM.Admin.demo.ashx
{
    /// <summary>
    /// dbHandler 的摘要说明
    /// </summary>
    public class LogisticsQuotedHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            UserBll.Instance.CheckUserOnlingState();

            //int k;
            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<LogisticsFeedbackModel>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<LogisticsFeedbackModel>>(json);
                rpm.CurrentContext = context;
            }

            switch (rpm.Action)
            {
                case "current":
                    //TODO:这个地方还需要再测试
                    var pcp = new ProcCustomPage("V_Quoted_Detail")
                    {
                        PageIndex = rpm.Pageindex,
                        PageSize = rpm.Pagesize,
                        WhereString = " Status='已发送' and SupplyId =  " + PublicMethod.GetLong(SysVisitor.Instance.UserId),
                        OrderFields = rpm.Sort + " " + rpm.Order
                    };

                    int count = 0;
                    DataTable table = DbUtils.GetPageWithSp(pcp, out count);

                    context.Response.Write(JSONhelper.FormatJSONForEasyuiDataGrid(count, table));

                    break;
                case "quoted":
                    LogisticsFeedbackModel model = DbUtils.Get<LogisticsFeedbackModel>(rpm.KeyId);

                    //判断是否超过时间
                    LogisticsInquiryModel inquiry = DbUtils.Get<LogisticsInquiryModel>(model.InquiryId);

                    if (DateTime.Now.CompareTo(inquiry.Ended) > 0)
                    {
                        context.Response.Write(-1);
                    }
                    else
                    {
                        //先得到未更新前的排名
                        pcp = new ProcCustomPage("V_Quoted_Detail")
                        {
                            WhereString = string.Format(" InquiryId = {0} and NewIndex is not null ", inquiry.KeyId),
                            PageIndex = 1,
                            PageSize = 100
                        };

                        DataTable t1 = DbUtils.GetPageWithSp(pcp, out count);
                        Dictionary<int, int> oldOrder = new Dictionary<int, int>();
                        foreach (DataRow row in t1.Rows)
                        {
                            oldOrder.Add(Convert.ToInt32(row["SupplyId"]), Convert.ToInt32(row["NewIndex"]));
                        }

                        //更新本次报价
                        model.Price = rpm.Entity.Price;
                        model.Ship = rpm.Entity.Ship;
                        model.Etd = rpm.Entity.Etd;
                        model.Memo = rpm.Entity.Memo;
                        model.Published = DateTime.Now;
                        model.Eta = rpm.Entity.Eta;
                        model.Counted = model.Counted + 1;

                        context.Response.Write(LogisticsFeedbackBll.Instance.Update(model));

                        //获取新的排名
                        DataTable t2 = DbUtils.GetPageWithSp(pcp, out count);
                        User user = SysVisitor.Instance.CurrentUser;
                        foreach (DataRow row in t2.Rows)
                        {
                            int supplyId = Convert.ToInt32(row["SupplyId"]);
                            int newOrder = Convert.ToInt32(row["NewIndex"]);

                            if (!oldOrder.ContainsKey(supplyId) || oldOrder[supplyId] != newOrder || user.KeyId == supplyId)
                            {
                                //发送邮件
                                EmailHelper.SendEmail(ConfigurationManager.AppSettings["email"],
                                    ConfigurationManager.AppSettings["password"],
                                    ConfigurationManager.AppSettings["smtp"],
                                    Convert.ToString(row["Email"]), Convert.ToString(row["DepartmentName"]),
                                    string.Format(ConfigurationManager.AppSettings["quoted_email_subject"], DateTime.Now),
                                    string.Format(ConfigurationManager.AppSettings["quoted_email_body"],
                                    inquiry.Port, inquiry.Cargo, newOrder));
                            }
                        }
                    }
                    break;
                case "all":
                default:
                    string where = FilterTranslator.ToSql(rpm.Filter);
                    where = " Status='已完成' and SupplyId =  " + PublicMethod.GetLong(SysVisitor.Instance.UserId) + " and " + where;

                    //TODO:这个地方还需要再测试
                    pcp = new ProcCustomPage("V_Quoted_Detail")
                    {
                        PageIndex = rpm.Pageindex,
                        PageSize = rpm.Pagesize,
                        WhereString =where,
                        OrderFields = rpm.Sort + " " + rpm.Order
                    };

                    count = 0;
                    table = DbUtils.GetPageWithSp(pcp, out count);

                    context.Response.Write(JSONhelper.FormatJSONForEasyuiDataGrid(count, table));

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


        private bool SendEmail(string from, string password, string smtp, Dictionary<string, string> tos,string subject, string body)
        {
            MailAddress mfrom = new MailAddress(from, "石大胜华");
            MailMessage mail = new MailMessage();

            mail.From = mfrom;
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            mail.BodyEncoding = System.Text.Encoding.UTF8;

            foreach (string to in tos.Keys)
            {
                MailAddress t = new MailAddress(to, tos[to]);
                //mail.To.Add(t);
                mail.Bcc.Add(t);
            }

            mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
            
            SmtpClient client = new SmtpClient();
            client.Host = smtp;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(from, password);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.SendCompleted += client_SendCompleted;

            //client.SendAsync(mail, "test");
            try
            {
                client.Send(mail);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return true;
        }

        void client_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Console.WriteLine("发送完成！");
        }
    }
}