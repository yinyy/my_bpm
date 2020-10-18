using BPM.Common;
using BPM.Core.Bll;
using BPM.Core.Model;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.CommonAPIs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;
using Washer.Bll;
using Washer.Model;
using WeChat.Utils;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading;
using System.IO;
using System.Text;
using System.Web.SessionState;
using Washer.Extension;
using WebSocket4Net;
using Washer.Toolkit;
using Senparc.Weixin.MP.Containers;

namespace BPM.Admin.Washer.ashx
{
    /// <summary>
    /// WasherHandler 的摘要说明
    /// </summary>
    public class WasherHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            JObject jobj = new JObject();

            context.Response.ContentType = "text/plain";
            string action = context.Request.Params["action"];

            if (action == "PayCoins")
            {
                int deptId = Convert.ToInt32(context.Session["deptId"].ToString());
                string openid = context.Session["openid"].ToString();
                string boardNumber = context.Request.Params["board"];

                Department dept = DepartmentBll.Instance.Get(deptId);
                WasherWeChatConsumeModel wxconsume;
                WasherConsumeModel consume;
                WasherDeviceModel device;
                int coins;

                if (string.IsNullOrWhiteSpace(boardNumber))
                {
                    context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "参数错误" }));
                    CustomApi.SendText(AccessTokenContainer.TryGetAccessToken(dept.Appid, dept.Secret), openid, string.Format("参数错误。"));
                }
                else if ((device = WasherDeviceBll.Instance.Get(dept.KeyId, boardNumber)) == null)
                {
                    context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "设备不存在" }));
                    CustomApi.SendText(AccessTokenContainer.TryGetAccessToken(dept.Appid, dept.Secret), openid, string.Format("扫码设备未登记。"));
                }
                else if ((wxconsume = WasherWeChatConsumeBll.Instance.Get(dept.KeyId, openid)) == null)
                {
                    context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "微信用户不存在" }));
                    CustomApi.SendText(AccessTokenContainer.TryGetAccessToken(dept.Appid, dept.Secret), openid, string.Format("微信用户信息错误。"));
                }
                else if ((consume = WasherConsumeBll.Instance.GetByBinder(wxconsume)) == null)
                {
                    context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "未绑定个人信息" }));
                    CustomApi.SendText(AccessTokenContainer.TryGetAccessToken(dept.Appid, dept.Secret), openid, string.Format("未绑定个人信息。"));
                }
                else if ((coins = WasherConsumeBll.Instance.GetValidCoins(consume.KeyId)) <= 0)
                {
                    context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "洗车币余额小于0" }));
                    CustomApi.SendText(AccessTokenContainer.TryGetAccessToken(dept.Appid, dept.Secret), openid, string.Format("可用洗车币余额为0。"));
                }
                else
                {
                    var o = new { MaxPayCoins = 0 };
                    o = JsonConvert.DeserializeAnonymousType(consume.Setting, o);

                    WasherDeviceLogModel balance = new WasherDeviceLogModel();
                    balance.CardId = WasherCardBll.Instance.GetValidCards(consume.KeyId).OrderBy(a => a.ValidateEnd).FirstOrDefault().KeyId;
                    balance.ConsumeId = consume.KeyId;
                    balance.DeviceId = device.KeyId;
                    balance.Kind = "余额洗车";
                    balance.Memo = "";
                    balance.PayCoins = 0;
                    balance.RemainCoins = o.MaxPayCoins == 0 ? coins : Math.Min(o.MaxPayCoins, coins);
                    balance.Started = DateTime.Now;
                    balance.IsShow = true;

                    if ((balance.KeyId = WasherDeviceLogBll.Instance.Add(balance)) < 0)
                    {
                        context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "写入设备日志错误" }));
                    }
                    else
                    {
                        var o2 = new
                        {
                            Action = "start_machine",
                            Data = JsonConvert.SerializeObject(new
                            {
                                DepartmentId = deptId,
                                BoardNumber = Aes.Encrypt(boardNumber),
                                BalanceId = balance.KeyId,
                                Coins = balance.RemainCoins
                            })
                        };

                        WebSocket webSocket = new WebSocket("ws://139.129.43.203:5500");
                        webSocket.Opened += (s0, e0) =>
                        {
                            webSocket.Send(JsonConvert.SerializeObject(o2));
                            CustomApi.SendText(AccessTokenContainer.TryGetAccessToken(dept.Appid, dept.Secret), openid, string.Format("机器已经启动，预支付：{0:0.00}元。结算后，余额将返还账户。", balance.RemainCoins / 100.0));

                            try { webSocket.Close(); } catch { }
                        };
                        webSocket.Error += (s0, e0) =>
                        {
                            CustomApi.SendText(AccessTokenContainer.TryGetAccessToken(dept.Appid, dept.Secret), openid, "启动洗车机时发生异常。");

                            try { webSocket.Close(); } catch { }
                        };
                        webSocket.Open();

                        context.Response.Write(JSONhelper.ToJson(new { Success = true }));
                    }
                }
            }
            else if (action == "Validate")
            {
                bool passed = true;

                int deptId = 0;
                string openid;
                string boardNumber = context.Request.Params["board"];

                if (context.Session["deptId"] == null)
                {
                    deptId = 70;
                }
                else
                {
                    deptId = Convert.ToInt32(context.Session["deptId"].ToString());
                }
                if (context.Session["openid"] == null)
                {
                    openid = "oiVK2uH3zgJLC6iGMoB6iuDKDW1M";
                }
                else
                {
                    openid = context.Session["openid"].ToString();
                }

                Department dept = DepartmentBll.Instance.Get(deptId);

                #region 验证微信或洗车卡使用时间
                //获取设备参数
                WasherDeviceModel device = WasherDeviceBll.Instance.Get(deptId, boardNumber);

                var setting = new
                {
                    Deadline = new
                    {
                        Wechat = new
                        {
                            Start = "08:00:00",
                            End = "20:59:59"
                        },
                        Member = new
                        {
                            Start = "00:00:00",
                            End = "23:59:59"
                        }
                    }
                };
                setting = JsonConvert.DeserializeAnonymousType(device.Setting, setting);

                if (setting != null && setting.Deadline != null)
                {
                    DateTime now = DateTime.Now;
                    string nowStr = now.ToString("HH:mm:ss");

                    bool useCard = Convert.ToBoolean(context.Request.Params["card"]);
                    if (useCard)
                    {
                        if (nowStr.CompareTo(setting.Deadline.Member.Start) < 0 || nowStr.CompareTo(setting.Deadline.Member.End) > 0)
                        {
                            CustomApi.SendText(AccessTokenContainer.TryGetAccessToken(dept.Appid, dept.Secret), openid, string.Format("当前不在会员支付洗车时间。会员支付洗车时间为：{0} - {1}。", setting.Deadline.Member.Start, setting.Deadline.Member.End));
                            context.Response.Write(-1);

                            passed = false;
                        }
                    }
                    else
                    {
                        if (nowStr.CompareTo(setting.Deadline.Wechat.Start) < 0 || nowStr.CompareTo(setting.Deadline.Wechat.End) > 0)
                        {
                            CustomApi.SendText(AccessTokenContainer.TryGetAccessToken(dept.Appid, dept.Secret), openid, string.Format("当前不在微信支付洗车时间。微信支付洗车时间为：{0} - {1}。", setting.Deadline.Wechat.Start, setting.Deadline.Wechat.End));
                            context.Response.Write(-2);

                            passed = false;
                        }
                    }
                }
                #endregion

                if (passed)
                {
                    #region 验证时间戳
                    double scanTime = Convert.ToDouble(context.Request.Params["ts"]);
                    double overTime = (TimeZone.CurrentTimeZone.ToLocalTime(DateTime.Now.AddMinutes(-5)) - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1))).TotalSeconds;
                    if (overTime <= scanTime)
                    {
                        context.Response.Write(1);
                    }
                    else
                    {
                        CustomApi.SendText(AccessTokenContainer.TryGetAccessToken(dept.Appid, dept.Secret), openid, "请求已过期，请重新扫码。");

                        context.Response.Write(0);
                    }
                    #endregion
                }
            }

            context.Response.Flush();
            context.Response.End();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private void SendData(string ip, byte[] buffer)
        {
            int port = 6000;
            if (ip.IndexOf(':') != -1)
            {
                port = Convert.ToInt32(ip.Substring(ip.IndexOf(':') + 1));
                ip = ip.Substring(0, ip.IndexOf(':'));
            }

            new Thread(() =>
            {
                try
                {
                    //IPAddress localhost = IPAddress.Parse(ip);
                    using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                    {
                        socket.Connect(IPAddress.Parse(ip), port);
                        socket.Send(buffer);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }).Start();
        }
    }
}