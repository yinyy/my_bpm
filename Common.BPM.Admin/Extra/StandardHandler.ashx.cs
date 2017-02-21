using BPM.Common;
using BPM.Core.Bll;
using BPM.Core.Model;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using Washer.Bll;
using Washer.Extension;
using Washer.Model;
using Washer.Toolkit;
using WebSocket4Net;

namespace BPM.Admin.Extra
{
    /// <summary>
    /// StandardHandler 的摘要说明
    /// </summary>
    public class StandardHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            string signature = context.Request.Params["signature"];
            string timestamp = context.Request.Params["timestamp"];
            string nonce = context.Request.Params["nonce"];
            string echostr = context.Request.Params["echostr"];
            string moneyStr = context.Request.Params["money"];
            string outTag = context.Request.Params["tag"];
            string inTag = context.Request.Params["itag"];
            string board = context.Request.Params["board"];
            string rid = context.Request.Params["rid"];

            Department dept = null;
            WasherOutsiderModel outsider = null;
            WasherDeviceModel device = null;
            WasherDeviceLogModel balance = null;
            int money = 0;

            bool wsClosed = false;

            if (string.IsNullOrWhiteSpace(signature) ||
                string.IsNullOrWhiteSpace(timestamp) ||
                string.IsNullOrWhiteSpace(nonce) ||
                string.IsNullOrEmpty(echostr)||
                string.IsNullOrWhiteSpace(moneyStr) ||
                string.IsNullOrWhiteSpace(outTag)||
                string.IsNullOrWhiteSpace(inTag)||
                string.IsNullOrWhiteSpace(board)||
                string.IsNullOrWhiteSpace(rid))
            {
                context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "missing parameter." }));
            }else if((dept = DepartmentBll.Instance.GetByTag(inTag))==null)
            {
                context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "inner tag error." }));
            }else if((outsider = WasherOutsiderBll.Instance.Get(dept.KeyId, outTag)) == null)
            {
                context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "outer tag error." }));
            }/*else if (!CheckTimestamp(Convert.ToInt64(timestamp)))
            {
                context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "timestamp error." }));
            }
            else if (!CheckSignature(signature, outsider.Token, timestamp, nonce))
            {
                context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "signature error." }));
            }*/else if((device=WasherDeviceBll.Instance.Get(dept.KeyId, board))==null)
            {
                context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "board error." }));
            }else if ((money=GetMoney(moneyStr)) <= 0)
            {
                context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "money error." }));
            }
            else
            {
                //将支付信息写入设备日志
                balance = new WasherDeviceLogModel();
                balance.CardId = 0;
                balance.ConsumeId = null;
                balance.DeviceId = device.KeyId;
                balance.Kind = string.Format("外部服务{0}", outTag);
                balance.Memo = JSONhelper.ToJson(new { Desc="外部服务", Tag=outTag, Echostr=echostr, Rid=rid });
                balance.PayCoins = 0;
                balance.RemainCoins = Convert.ToInt32(moneyStr);
                balance.Started = DateTime.Now;
                balance.IsShow = true;

                balance.KeyId = WasherDeviceLogBll.Instance.Add(balance);

                var o = new
                {
                    Action = "start_machine",
                    Data = JsonConvert.SerializeObject(new
                    {
                        DepartmentId = dept.KeyId,
                        BoardNumber = Aes.Encrypt(board),
                        BalanceId = balance.KeyId,
                        Coins = balance.RemainCoins
                    })
                };

                WebSocket webSocket = new WebSocket("ws://139.129.43.203:5500");
                webSocket.Opened += (s0, e0) =>
                {
                    webSocket.Send(JsonConvert.SerializeObject(o));
                    try { webSocket.Close(); } catch { }

                    context.Response.Write(JSONhelper.ToJson(new { Success = true, Message = "ok." }));

                    wsClosed = true;
                };
                webSocket.Error += (s0, e0) =>
                {
                    try { webSocket.Close(); } catch { }

                    context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "start error." }));

                    wsClosed = true;
                };
                webSocket.Open();

                //wsClosed = true;
            }

            while (!wsClosed)
            {
                Thread.Sleep(200);
            }


            //#region 如果是外部服务，则需要根据设置判断是否需要回调
            //if (balance.Kind.StartsWith("外部服务"))
            //{
            //    var o = new { Desc = "", Tag = "", Echostr = "", Rid = "" };
            //    o = JsonConvert.DeserializeAnonymousType(balance.Memo, o);

            //    outsider = WasherOutsiderBll.Instance.Get(device.DepartmentId, o.Tag);
            //    if (!string.IsNullOrEmpty(outsider.Url))
            //    {
            //        new Thread(() =>
            //        {
            //            string url = string.Format("{0}{1}echostr={2}&rid={3}", outsider.Url, outsider.Url.IndexOf('?') == -1 ? "?" : "&", o.Echostr, o.Rid);

            //            System.Net.WebRequest wReq = System.Net.WebRequest.Create(url);
            //            System.Net.WebResponse wResp = wReq.GetResponse();
            //            System.IO.Stream respStream = wResp.GetResponseStream();

            //            respStream.Close();
            //            wResp.Close();
            //        }).Start();
            //    }
            //}
            //#endregion
        }

        private int GetMoney(string moneyStr)
        {
            int value = 0;
            Int32.TryParse(moneyStr, out value);

            if (value < 0)
            {
                value = 0;
            }

            return value;
        }

        private bool CheckTimestamp(long timestamp)
        {
            long time = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            return Math.Abs(time - timestamp) <= 30;
        }

        private bool CheckSignature(string signature, string token, string timestamp, string nonce)
        {
            return signature == GetSignature(token, timestamp, nonce);
        }

        private string GetSignature(string token, string timestamp, string nonce)
        {
            var arr = new[] { token, timestamp, nonce }.OrderBy(z => z).ToArray();
            var arrString = string.Join("", arr);
            var sha1 = System.Security.Cryptography.SHA1.Create();
            var sha1Arr = sha1.ComputeHash(Encoding.UTF8.GetBytes(arrString));
            StringBuilder enText = new StringBuilder();
            foreach (var b in sha1Arr)
            {
                enText.AppendFormat("{0:x2}", b);
            }

            return enText.ToString();
        }
            //http://127.0.0.1:9582/Extra/StandardHandler.ashx?signature=473c528dbec76e92c41365a7c2e0f186c4834480&timestamp=1486818830&nonce=123&echostr=abc&tag=lwpicc&itag=Senlan&board=100001&money=1000&rid=1

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}