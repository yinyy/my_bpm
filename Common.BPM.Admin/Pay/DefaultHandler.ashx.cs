using BPM.Core.Bll;
using BPM.Core.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using Washer.Bll;
using Washer.Model;

namespace BPM.Admin.Pay
{
    /// <summary>
    /// DefaultHandler 的摘要说明
    /// </summary>
    public class DefaultHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            using (StreamReader reader = new StreamReader(context.Request.InputStream))
            {
                string str = reader.ReadToEnd().Trim();

                var msg = new { UnionId = "", OpenId = "", Money = 0f, DeviceSerial = "", Payment="" };
                msg = JsonConvert.DeserializeAnonymousType(str, msg);

                if (msg.Payment == "scanqrcode")
                {
                    #region 记录订单信息
                    WasherOrderModel order = new WasherOrderModel();
                    order.Memo = string.Format("Device: {0}", msg.DeviceSerial);
                    order.Money = msg.Money;
                    order.OpenId = msg.OpenId;
                    order.Payment = WasherOrderBll.Payment.ScanQrcode;
                    order.Time = DateTime.Now;
                    order.UnionId = msg.UnionId;
                    if (WasherOrderBll.Instance.Add(order) <= 0)
                    {
                        return;
                    }
                    #endregion

                    #region 如果用户是会员，则增加洗车积分
                    WasherConsumeModel consume = WasherConsumeBll.Instance.Get(msg.UnionId, msg.OpenId);
                    var deptSetting = new { Point = new { WashCar = 0 } };
                    if (consume != null)
                    {
                        Department dept = DepartmentBll.Instance.Get(consume.DepartmentId);
                        deptSetting = JsonConvert.DeserializeAnonymousType(dept.Setting, deptSetting);

                        WasherRewardModel reward = new WasherRewardModel();
                        reward.ConsumeId = consume.KeyId;
                        reward.Kind = WasherRewardBll.Kind.WashCar;
                        reward.Memo = "";
                        reward.Points = deptSetting.Point.WashCar;
                        reward.Time = DateTime.Now;
                        WasherRewardBll.Instance.Add(reward);

                        consume.Points += deptSetting.Point.WashCar;
                        WasherConsumeBll.Instance.Update(consume);
                    }
                    #endregion

                    #region 设备开设工作
                    string message = WeChat.Utils.WeChatToolkit.SendCommand(ConfigurationManager.AppSettings["tcp_server_http_url"],
                        JsonConvert.SerializeObject(new { Command = "", Message = "" }));
                    #endregion

                    context.Response.Write(string.Format("设备准备就绪。\n您本次新增 {0} 积分，累计 {1} 积分。", deptSetting.Point.WashCar, consume.Points));
                }
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