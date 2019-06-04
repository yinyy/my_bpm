using BPM.Common;
using BPM.Core;
using BPM.Core.Bll;
using BPM.Core.Model;
using Newtonsoft.Json;
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

            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<WasherCardModel>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<WasherCardModel>>(json);
                rpm.CurrentContext = context;
            }

            //Kind
            //Inner:内部发行卡
            //Sale:外部售卖卡
            //Coupon:优惠卡
            //Convertor:兑换卡

            WasherCardModel model;
            string filter;
            switch (rpm.Action)
            {
                case "add":
                    model = rpm.Entity;
                    WasherCardModel c2 = WasherCardBll.Instance.Get(departmentId, model.CardNo);
                    if (c2 == null)
                    {
                        model.DepartmentId = departmentId;
                        model.Memo = "";
                        //model.Coins *= 100;

                        context.Response.Write(WasherCardBll.Instance.Add(model));
                    }
                    else
                    {
                        context.Response.Write("-1");
                    }
                    break;
                case "del":
                    context.Response.Write(WasherCardBll.Instance.Delete(rpm.KeyId));
                    break;
                case "edit":
                    model = WasherCardBll.Instance.Get(rpm.KeyId);
                    model.ValidateFrom = rpm.Entity.ValidateFrom;
                    model.ValidateEnd = rpm.Entity.ValidateEnd;
                    model.Kind = rpm.Entity.Kind;
                    model.Coins = rpm.Entity.Coins;

                    context.Response.Write(WasherCardBll.Instance.Update(model));
                    break;
                case "consume":
                    filter = string.Format("{{\"groupOp\":\"AND\",\"rules\":[{{\"field\":\"CardId\",\"op\":\"eq\",\"data\":\"{0}\"}}, {{\"field\":\"Coins\",\"op\":\"lt\",\"data\":\"0\"}}],\"groups\":[]}}", rpm.KeyId);
                    context.Response.Write(WasherCardLogBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, filter, rpm.Sort, rpm.Order));
                    break;
                case "recharge":
                    filter = string.Format("{{\"groupOp\":\"AND\",\"rules\":[{{\"field\":\"CardId\",\"op\":\"eq\",\"data\":\"{0}\"}}, {{\"field\":\"Coins\",\"op\":\"gt\",\"data\":\"0\"}}],\"groups\":[]}}", rpm.KeyId);
                    context.Response.Write(WasherCardLogBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, filter, rpm.Sort, rpm.Order));
                    break;
                //case "coupon":
                //    model = new WasherCardModel();
                //    model.Binded = DateTime.Now;
                //    model.BinderId = 6;
                //    model.CardNo = string.Format("Coupon_{0}_{1:x}", 69, DateTime.Now.Ticks);
                //    model.Coins = 10000;
                //    model.DepartmentId = 69;
                //    model.Kind = "Coupon";
                //    model.Memo = "";
                //    model.Password = "123456";
                //    model.ValidateFrom = DateTime.Now;
                //    model.ValidateEnd = DateTime.Now.AddYears(1);

                //    context.Response.Write(WasherCardBll.Instance.Add(model));
                //    break;
                case "batch":
                    var o = new { Start=0, End=0};
                    o = JsonConvert.DeserializeAnonymousType(rpm.JsonEntity, o);
                    string prefix = rpm.Entity.CardNo.Substring(0, rpm.Entity.CardNo.IndexOf('*'));
                    string suffix = rpm.Entity.CardNo.Substring(rpm.Entity.CardNo.LastIndexOf('*') + 1);
                    int len = rpm.Entity.CardNo.Length - prefix.Length - suffix.Length;
                    Random r = new Random();
                    int count = 0;

                    for(int i = o.Start; i <= o.End && i.ToString().Length<=len; i++)
                    {
                        string cardNo = string.Format("{0}{1}{2}", prefix, i.ToString().PadLeft(len, '0'), suffix);
                        if (!WasherCardBll.Instance.Exits(cardNo)) {
                            try
                            {
                                WasherCardModel card = new WasherCardModel();
                                card.CardNo = cardNo;
                                card.Password = Convert.ToString(r.Next(100000, 999999));
                                card.ValidateFrom = rpm.Entity.ValidateFrom;
                                card.ValidateEnd = rpm.Entity.ValidateEnd;
                                card.Coins = rpm.Entity.Coins;
                                card.Binded = null;
                                card.BinderId = null;
                                card.DepartmentId = departmentId;
                                card.Memo = "";
                                card.Kind = rpm.Entity.Kind;
                                card.Locked = null;

                                if (WasherCardBll.Instance.Add(card) != -1)
                                {
                                    count++;
                                }
                            }
                            catch
                            {

                            }
                        }
                    }

                    context.Response.Write(count);
                    break;
                case "export":
                    if (user.IsAdmin)
                    {
                        GridViewExportUtil.Export(DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".xls", WasherCardBll.Instance.Export(rpm.Filter, rpm.Sort, rpm.Order));
                    }
                    else
                    {
                        filter = string.Format("{{\"groupOp\":\"AND\",\"rules\":[{{\"field\":\"DepartmentId\",\"op\":\"eq\",\"data\":\"{0}\"}}],\"groups\":[{1}]}}", departmentId, rpm.Filter);
                        GridViewExportUtil.Export(DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".xls", WasherCardBll.Instance.Export(filter, rpm.Sort, rpm.Order));
                    }
                    break;
                case "empCard":
                    {
                        WasherConsumeModel washerConsume = new WasherConsumeModel();
                        washerConsume.DepartmentId = departmentId;
                        washerConsume.Gender = "未知";
                        washerConsume.Memo = "";
                        washerConsume.Name = rpm.Entity.EmpName;
                        washerConsume.Password = "000000";
                        washerConsume.Points = 0;
                        washerConsume.Telphone = rpm.Entity.Phone;

                        Department department = DepartmentBll.Instance.Get(departmentId);
                        WasherDepartmentSetting setting = JsonConvert.DeserializeObject<WasherDepartmentSetting>(department.Setting);
                        washerConsume.Setting = JSONhelper.ToJson(new { MaxPayCoins = setting.PayWashCar.MaxPayCoins });

                        if ((washerConsume.KeyId = WasherConsumeBll.Instance.Add(washerConsume)) <= 0)
                        {
                            context.Response.Write("-1");
                            break;
                        }

                        model = rpm.Entity;
                        WasherCardModel c3 = WasherCardBll.Instance.Get(departmentId, model.CardNo);
                        if (c3 == null)
                        {
                            model.DepartmentId = departmentId;
                            model.BinderId = washerConsume.KeyId;
                            model.Binded = DateTime.Now;
                            model.Memo = "";

                            context.Response.Write(WasherCardBll.Instance.Add(model));
                        }
                        else
                        {
                            context.Response.Write("-1");
                        }
                    }
                    break;
                case "batchEmpCard":
                    if (string.IsNullOrEmpty(rpm.Entity.Memo))
                    {
                        context.Response.Write("-1");
                    }
                    else
                    {
                        Department department = DepartmentBll.Instance.Get(departmentId);
                        WasherDepartmentSetting setting = JsonConvert.DeserializeObject<WasherDepartmentSetting>(department.Setting);

                        string[] datas = rpm.Entity.Memo.Split(';');
                        foreach(String data in datas)
                        {
                            if (string.IsNullOrEmpty(data))
                            {
                                continue;
                            }

                            string[] ds = data.Split(',');
                            //      0           1       2           3       4       5       6
                            //财务部-张三,12345678901,1234567890,000000,20190604,20200603,100000
                            WasherConsumeModel washerConsume = new WasherConsumeModel();
                            washerConsume.DepartmentId = departmentId;
                            washerConsume.Gender = "未知";
                            washerConsume.Memo = "";
                            washerConsume.Name = ds[0];
                            washerConsume.Password = "000000";
                            washerConsume.Points = 0;
                            washerConsume.Telphone = ds[1];
                            washerConsume.Setting = JSONhelper.ToJson(new { MaxPayCoins = setting.PayWashCar.MaxPayCoins });

                            if ((washerConsume.KeyId = WasherConsumeBll.Instance.Add(washerConsume)) <= 0)
                            {
                                continue;
                            }

                            WasherCardModel c3 = WasherCardBll.Instance.Get(departmentId, ds[2]);
                            if (c3 == null)
                            {
                                c3 = new WasherCardModel();
                                c3.DepartmentId = departmentId;
                                c3.BinderId = washerConsume.KeyId;
                                c3.Binded = DateTime.Now;
                                c3.Memo = "";
                                c3.CardNo = ds[2];
                                c3.Password = ds[3];
                                c3.ValidateFrom = DateTime.Parse(ds[4]);
                                c3.ValidateEnd = DateTime.Parse(ds[5]);
                                c3.Coins = Convert.ToInt32(ds[6]);
                                c3.Kind = "Inner";

                                WasherCardBll.Instance.Add(c3);
                            }
                        }
                    }

                    context.Response.Write(1);
                    break;
                default:
                    if (user.IsAdmin)
                    {
                        context.Response.Write(WasherCardBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, rpm.Filter, rpm.Sort, rpm.Order));
                    }
                    else
                    {
                        filter = string.Format("{{\"groupOp\":\"AND\",\"rules\":[{{\"field\":\"DepartmentId\",\"op\":\"eq\",\"data\":\"{0}\"}}],\"groups\":[{1}]}}",departmentId, rpm.Filter);
                        context.Response.Write(WasherCardBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, filter, rpm.Sort, rpm.Order));
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