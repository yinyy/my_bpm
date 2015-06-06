using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Washer.Bll;
using Washer.Model;

namespace BPM.Admin.Washer.ashx
{
    /// <summary>
    /// WasherHandler 的摘要说明
    /// </summary>
    public class WasherHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            string action = context.Request.Params["action"];

            WasherCardModel card;
            WasherDeviceModel device;
            WasherConsumeModel consume;

            switch (action)
            {
                case "consume":
                    card = WasherCardBll.Instance.GetBySerial(context.Request.Params["card"]);
                    device = WasherDeviceBll.Instance.GetBySerial(context.Request.Params["device"]);

                    if (card != null && device != null)
                    {
                        consume = new WasherConsumeModel();
                        consume.CardId = card.KeyId;
                        consume.DeviceId = device.KeyId;
                        consume.Time = DateTime.Now;
                        consume.Money = Convert.ToDecimal(context.Request.Params["money"]);

                        context.Response.Write(WasherConsumeBll.Instance.Add(consume));
                    }
                    else
                    {
                        context.Response.Write(0);
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