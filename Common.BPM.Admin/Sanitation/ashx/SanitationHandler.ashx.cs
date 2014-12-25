using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Sanitation.Bll;
using Sanitation.Model;

namespace BPM.Admin.Sanitation.ashx
{
    /// <summary>
    /// SanitationHandler 的用途是：
    /// 1、SIM900A模块获取调度信息
    /// 2、保存加水信息
    /// </summary>
    public class SanitationHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action = context.Request.Params["action"];
            int dispatchId;

            switch (action)
            {
                case "get":
                    string time = context.Request.Params["time"];
                    dispatchId= Convert.ToInt32(context.Request.Params["dispatchId"]);
                    
                    DateTime st = DateTime.Parse(time);
                    if (DateTime.Now.Date != st.Date)
                    {
                        context.Response.Write("error_date");//加水时间不对
                    }
                    else
                    {
                        //检查是否有本次调度任务
                        if (SanitationDispatchBll.Instance.GetById(dispatchId) == null)
                        {
                            context.Response.Write("error_dispatch");
                        }
                        else
                        {
                            context.Response.Write("success_" + SanitationDetailBll.Instance.Get(st, dispatchId).Count());//如果正确，则查询今天已经加注的次数
                        }
                    }
                    break;
                case "save":
                    dispatchId = Convert.ToInt32(context.Request.Params["dispatchId"]);
                    int  driverId =Convert.ToInt32( context.Request.Params["driverId"]);
                    int trunkId= Convert.ToInt32(context.Request.Params["trunkId"]);
                    decimal volumn = Convert.ToDecimal(context.Request.Params["volumn"]);
                    string address = context.Request.Params["address"];

                    SanitationDetailModel m = new SanitationDetailModel();
                    m.Address = address;
                    m.DriverId = driverId;
                    m.Time = DateTime.Now;
                    m.TrunkId = trunkId;
                    m.Volumn = volumn;
                    m.ReferDispatchId = dispatchId;

                    dispatchId = SanitationDetailBll.Instance.Add(m);
                    if (dispatchId >= 0)
                    {
                        context.Response.Write("success_saved_" + dispatchId);
                    }
                    else
                    {
                        context.Response.Write("error_saved");
                    }
                    break;
                default:
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