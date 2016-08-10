﻿using BPM.Common;
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
using BPM.BoardListenerLib;
using System.Threading;
using System.IO;
using System.Text;
using System.Web.SessionState;
using Washer.Extension;

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
                string appid = context.Session["appid"].ToString();
                string openid = context.Session["openid"].ToString();

                Department dept = DepartmentBll.Instance.GetByAppid(appid);
                WasherWeChatConsumeModel wxconsume = WasherWeChatConsumeBll.Instance.Get(dept.KeyId, openid);

                //余额支付洗车
                string boardNumber = context.Request.Params["board"];

                dynamic dobj = WasherValidatorBll.Instance.ValidateWxConsume(boardNumber, wxconsume.KeyId);
                if (dobj.Success == true)
                {
                    ReplyMessageBase replyMessage = new ReplyValidateMessage(boardNumber)
                    {
                        BalanceId = dobj.BalanceId,
                        Kind = TcpMessageBase.CardKind.Normal,
                        Money = dobj.RemainCoins,
                        Status = TcpMessageBase.CardStatus.Regular
                    };

                    byte[] buffer = replyMessage.ToByteArray();
                    buffer[4] = 0xff;

                    if (!string.IsNullOrWhiteSpace(dobj.ListenerIp))
                    {
                        SendData(dobj.ListenerIp, buffer);
                    }

                    CustomApi.SendText(AccessTokenContainer.TryGetAccessToken(dobj.Appid, dobj.Secret), dobj.OpenId, "机器已经启动");

                    context.Response.Write(JSONhelper.ToJson(new { Success = true }));
                }
                else
                {
                    context.Response.Write(JSONhelper.ToJson(dobj));
                }
            }
            else if (action == "PayWash")
            {
                //微信支付洗车
                string serial = context.Request.Params["serial"];

                dynamic dobj = WasherValidatorBll.Instance.ValidatePaySerial(serial);
                if (dobj.Success == true)
                {
                    ReplyMessageBase replyMessage = new ReplyValidateMessage(dobj.BoardNumber)
                    {
                        BalanceId = dobj.BalanceId,
                        Kind = TcpMessageBase.CardKind.Normal,
                        Money = dobj.RemainCoins,
                        Status = TcpMessageBase.CardStatus.Regular
                    };

                    byte[] buffer = replyMessage.ToByteArray();
                    buffer[4] = 0xff;

                    if (!string.IsNullOrWhiteSpace(dobj.ListenerIp))
                    {
                        SendData(dobj.ListenerIp, buffer);
                    }

                    CustomApi.SendText(AccessTokenContainer.TryGetAccessToken(dobj.Appid, dobj.Secret), dobj.OpenId, "机器已经启动");

                    context.Response.Write(JSONhelper.ToJson(new { Success = true }));
                }
                else
                {
                    context.Response.Write(JSONhelper.ToJson(dobj));
                }
            }
            else if (action == "ValidateCardAndPassword")
            {
                string boardNumber = context.Request.Params["boardNumber"];
                string cardNo = context.Request.Params["cardNo"];
                string password = context.Request.Params["password"];

                dynamic dobj = WasherValidatorBll.Instance.ValidateCard(boardNumber, cardNo, password);
                if (dobj.Success == true)
                {
                    jobj.Add("Kind", (int)TcpMessageBase.CardKind.Normal);
                    jobj.Add("Status", (int)TcpMessageBase.CardStatus.Regular);
                    jobj.Add("Money", dobj.Money);
                    jobj.Add("BalanceId", dobj.BalanceId);
                }
                else
                {
                    jobj.Add("Kind", (int)TcpMessageBase.CardKind.Normal);
                    jobj.Add("Status", (int)TcpMessageBase.CardStatus.Unusual);
                    jobj.Add("Money", 0);
                    jobj.Add("BalanceId", 0);
                }

                context.Response.Write(JSONhelper.ToJson(jobj));
            }
            else if (action == "ValidateCard")
            {
                string boardNumber = context.Request.Params["boardNumber"];
                string cardNo = context.Request.Params["cardNo"];

                dynamic dobj = WasherValidatorBll.Instance.ValidateCard(boardNumber, cardNo);
                if (dobj.Success == true)
                {
                    jobj.Add("Kind", (int)TcpMessageBase.CardKind.Normal);
                    jobj.Add("Status", (int)TcpMessageBase.CardStatus.Regular);
                    jobj.Add("Money", dobj.Money);
                    jobj.Add("BalanceId", dobj.BalanceId);
                }
                else
                {
                    jobj.Add("Kind", (int)TcpMessageBase.CardKind.Normal);
                    jobj.Add("Status", (int)TcpMessageBase.CardStatus.Unusual);
                    jobj.Add("Money", 0);
                    jobj.Add("BalanceId", 0);
                }

                context.Response.Write(JSONhelper.ToJson(jobj));
            }
            else if (action == "ValidatePhoneAndPassword")
            {
                string boardNumber = context.Request.Params["boardNumber"];
                string phone = context.Request.Params["phone"];
                string password = context.Request.Params["password"];

                dynamic dobj = WasherValidatorBll.Instance.ValidatePhone(boardNumber, phone, password);
                if (dobj.Success == true)
                {
                    jobj.Add("Kind", (int)TcpMessageBase.CardKind.Normal);
                    jobj.Add("Status", (int)TcpMessageBase.CardStatus.Regular);
                    jobj.Add("Money", dobj.Money);
                    jobj.Add("BalanceId", dobj.BalanceId);
                }
                else
                {
                    jobj.Add("Kind", (int)TcpMessageBase.CardKind.Normal);
                    jobj.Add("Status", (int)TcpMessageBase.CardStatus.Unusual);
                    jobj.Add("Money", 0);
                    jobj.Add("BalanceId", 0);
                }

                context.Response.Write(JSONhelper.ToJson(jobj));
            }
            else if (action == "TimeSync")
            {
                string boardNumber = context.Request.Params["boardNumber"];
                string clientIp = context.Request.Params["clientIp"];
                string listenerIp = context.Request.Params["listenerIp"];

                WasherDeviceModel device = WasherDeviceBll.Instance.GetByBoardNumber(boardNumber);
                if (device != null)
                {
                    device.UpdateTime = DateTime.Now;
                    device.IpAddress = clientIp;
                    device.ListenerIp = listenerIp;

                    WasherDeviceBll.Instance.Update(device);

                    jobj.Add("Success", true);
                    jobj.Add("BoardNumber", device.BoardNumber);
                    jobj.Add("Serial", device.SerialNumber);
                    jobj.Add("Address", string.Format("{0}-{1}-{2}-{3}", device.Region, device.Province, device.City, device.Address));
                    jobj.Add("IP", clientIp);

                    Department dept = DepartmentBll.Instance.Get(device.DepartmentId);
                    jobj.Add("DepartmentName", dept.DepartmentName);
                }
                else
                {
                    jobj.Add("Success", false);
                }

                context.Response.Write(JSONhelper.ToJson(jobj));
            }
            else if (action == "ReaderSetting")
            {
                string boardNumber = context.Request.Params["boardNumber"];
                WasherDeviceModel device = WasherDeviceBll.Instance.GetByBoardNumber(boardNumber);
                if (device == null)
                {
                    jobj.Add("ErrorCode", 1);
                }
                else
                {
                    jobj.Add("ErrorCode", 0);
                    jobj.Add("Values", (JArray)(JObject.Parse(device.Setting)["Params"]));
                    jobj.Add("Enabled", device.Enabled);
                }

                context.Response.Write(JSONhelper.ToJson(jobj));
            }
            else if (action == "SendSetting")
            {
                WasherDeviceModel device = WasherDeviceBll.Instance.Get(Convert.ToInt32(context.Request.Params["keyid"]));
                if (device == null)
                {
                    context.Response.Write("error");
                }
                else
                {
                    byte[] buffer = new byte[75];

                    long ticks = DateTime.Now.Ticks;

                    buffer[0] = (byte)((ticks >> 24) & 0xff);
                    buffer[1] = (byte)(((ticks >> 16) & 0xff));
                    buffer[2] = (byte)(((ticks >> 8) & 0xff));
                    buffer[3] = (byte)(ticks & 0xff);

                    buffer[4] = (byte)((202 >> 8) & 0xff);
                    buffer[5] = (byte)(202 & 0xff);

                    buffer[6] = 68;

                    long deviceID = Convert.ToInt32(device.BoardNumber);
                    buffer[7] = (byte)((deviceID >> 24) & 0xff);
                    buffer[8] = (byte)((deviceID >> 16) & 0xff);
                    buffer[9] = (byte)((deviceID >> 8) & 0xff);
                    buffer[10] = (byte)(deviceID & 0xff);

                    var obj = new { Coin = 0, Params = new int[32] };
                    obj = JsonConvert.DeserializeAnonymousType(device.Setting, obj);
                    for (int i = 0; i < obj.Params.Length; i++)
                    {
                        buffer[i * 2 + 11] = (byte)((obj.Params[i] >> 8) & 0xff);
                        buffer[i * 2 + 11 + 1] = (byte)(obj.Params[i] & 0xff);
                    }

                    if (!device.Enabled)
                    {
                        obj.Params[31] = (~0x0001) & obj.Params[31];
                    }

                    SendData(device.ListenerIp, buffer);

                    context.Response.Write("success");
                }
            }
            else if (action == "UploadStatus")
            {
                string boardNumber = context.Request.Params["boardNumber"];
                string status = "";

                using (Stream stream = context.Request.InputStream)
                {
                    byte[] buffer = new byte[context.Request.InputStream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    status = Encoding.UTF8.GetString(buffer);
                }

                WasherDeviceModel device = WasherDeviceBll.Instance.GetByBoardNumber(boardNumber);
                if (device != null)
                {
                    device.Status = status;

                    WasherDeviceBll.Instance.Update(device);

                    context.Response.Write("SUCCESS");
                }
                else
                {
                    context.Response.Write("ERROR");
                }
            }
            else if (action == "Account")
            {
                string boardNumber = context.Request.Params["boardNumber"];
                int ticks = Convert.ToInt32(context.Request.Params["ticks"]);
                int balanceId = Convert.ToInt32(context.Request.Params["balanceId"]);
                int payment = Convert.ToInt32(context.Request.Params["payment"]);

                jobj.Add("Remain", WasherDeviceLogBll.Instance.Clearing(ticks, boardNumber, balanceId, payment));
                jobj.Add("BalanceId", balanceId);

                context.Response.Write(JSONhelper.ToJson(jobj));
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
            new Thread(() =>
            {
                try
                {
                    IPAddress localhost = IPAddress.Parse(ip);
                    using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                    {
                        socket.Connect(IPAddress.Parse(ip), 6000);
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