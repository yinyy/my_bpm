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
            string code;
            string plate;
            double lng;
            double lat;
            int pipe; 
            SanitationDispatchModel dispatch;

            switch (action)
            {
                case "save":
                    code = context.Request.Params["driver"];
                    plate = context.Request.Params["trunk"];
                    float volumn = Convert.ToSingle(context.Request.Params["volumn"]);
                    string address = context.Request.Params["address"];
                    int kind = Convert.ToInt32(context.Request.Params["kind"]);
                    int potency = Convert.ToInt32(context.Request.Params["potency"]);

                    dispatch = new SanitationDispatchModel();
                    dispatch.DriverId = SanitationDriverBll.Instance.GetByCode(code).KeyId;
                    dispatch.Kind = kind;
                    dispatch.Potency = 5;
                    dispatch.Status = 0;
                    dispatch.TrunkId = SanitationTrunkBll.Instance.GetByPlate(plate).KeyId;
                    dispatch.Time = DateTime.Now;
                    dispatch.Address = DicDal.Instance.GetWhere(new { Code = address }).FirstOrDefault().Title;

                    context.Response.Write("SavedSuccess:" + SanitationDispatchBll.Instance.Add(dispatch));
                    context.Response.Flush();
                    break;
                case "current"://车载设备获得当前任务的方法
                    code = context.Request.Params["code"];
                    plate = context.Request.Params["plate"];

                    dispatch = SanitationDispatchBll.Instance.Current(code, plate);

                    context.Response.Write(string.Format("current:{0}", dispatch == null ? 0 : dispatch.KeyId));
                    break;
                case "sign"://车载设备签到的方法
                    code = context.Request.Params["code"];
                    plate = context.Request.Params["plate"];
                    lng=Convert.ToDouble( context.Request.Params["lng"]);
                    lng /= 100.0;
                    lat = Convert.ToDouble(context.Request.Params["lat"]);
                    lat /= 100.0;
                    pipe = Convert.ToInt32(context.Request.Params["pipe"]);

                    dispatch = SanitationDispatchBll.Instance.Current(code, plate);
                    dispatch.Signed = DateTime.Now;
                    dispatch.Destination = string.Format("{0},{1}",lng,lat);
                    dispatch.Working = pipe;
                    dispatch.Region = 1;//TODO:这个将来需要通过计算验证是在区域外签到还是区域内签到的
                    dispatch.Status = 1;

                    context.Response.Write(string.Format("signed:{0}", SanitationDispatchBll.Instance.Update(dispatch)));
                    break;
                default:
                    string result = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (string.IsNullOrEmpty(result))
                    {
                        result = context.Request.ServerVariables["REMOTE_ADDR"];
                    }
                    if (string.IsNullOrEmpty(result))
                    {
                        result = context.Request.UserHostAddress;
                    }
                    if (string.IsNullOrEmpty(result))
                    {
                        result = "0.0.0.0";
                    }

                    context.Response.Write(result);
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