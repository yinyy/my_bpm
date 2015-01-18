using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Sanitation.Bll;
using Sanitation.Model;
using BPM.Core.Model;
using BPM.Core.Bll;
using BPM.Core.Dal;

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
                    dispatchId= Convert.ToInt32(context.Request.Params["dispatchId"]);

                    SanitationDispatchModel dispatchModel = SanitationDispatchBll.Instance.GetById(dispatchId);
                    if (dispatchModel == null)
                    {
                        context.Response.Write("error_dispatch");//没有本次任务，提示数据错误
                    }
                    else if (DateTime.Now.Date != dispatchModel.Time.Date)
                    {
                        context.Response.Write("error_date");//加水的时间不对。
                    }
                    else
                    {
                        int finished = SanitationDetailBll.Instance.Get(dispatchId).Count();//获取今天、当前任务的完成次数
                        SanitationTrunkModel trunkModel = SanitationTrunkBll.Instance.GetById(dispatchModel.TrunkId);
                        context.Response.Write(
                            string.Format("success_{0},{1},{2},{3},{4},{5},{6}",
                            dispatchModel.DriverId, 
                            dispatchModel.TrunkId,
                            trunkModel.Volumn, 
                            dispatchModel.Workload,
                            finished, dispatchModel.Potency, DicDal.Instance.Get(dispatchModel.KindId).Code));//如果正确，则返回当前任务的司机编号、车辆编号、任务次数、已完成次数
                    }

                    break;
                case "save":
                    dispatchId = Convert.ToInt32(context.Request.Params["dispatchId"]);
                    decimal volumn = Convert.ToDecimal(context.Request.Params["volumn"]);
                    string address = context.Request.Params["address"];

                    SanitationDetailModel m = new SanitationDetailModel();
                    m.Address = address;
                    m.Time = DateTime.Now;
                    m.Volumn = volumn;
                    m.DispatchId = dispatchId;

                    dispatchId = SanitationDetailBll.Instance.Add(m);
                    if (dispatchId >= 0)
                    {
                        context.Response.Write("success_save_" + dispatchId);
                    }
                    else
                    {
                        context.Response.Write("error_save");
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