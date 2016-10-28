using BPM.BoardListenerLib;
using BPM.Common;
using BPM.Core.Bll;
using BPM.Core.Model;
using Senparc.Weixin;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Washer.Bll;
using Washer.Model;

namespace BPM.Admin.Extra
{
    public partial class WashCar : System.Web.UI.Page
    {
        protected string validateMessage = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string code = Request.Params["code"];
                int deptId = Convert.ToInt16(Request.Params["d"]);
                string boardNumber = Request.Params["b"];
                Department dept = DepartmentBll.Instance.Get(deptId);

                if (string.IsNullOrEmpty(code))
                {
                    Response.Redirect(OAuthApi.GetAuthorizeUrl(dept.Appid, string.Format("http://xc.senlanjidian.com/Extra/WashCar.aspx?d={0}&b={1}", deptId, boardNumber), "0", Senparc.Weixin.MP.OAuthScope.snsapi_userinfo));
                    return;
                }

                OAuthAccessTokenResult result = OAuthApi.GetAccessToken(dept.Appid, dept.Secret, code);
                if (result.errcode != ReturnCode.请求成功)
                {
                    validateMessage = "wechat_error";
                    return;
                }

                string url;
                #region 获取对应客户的验证URL
                using (StreamReader reader = new StreamReader(new FileStream(Server.MapPath("~/Extra/url.data"), FileMode.Open)))
                {
                    while ((url = reader.ReadLine()) != null)
                    {
                        if (url.StartsWith(deptId + ","))
                        {
                            url = url.Substring(url.IndexOf(',') + 1);
                            break;
                        }
                    }
                }
                #endregion

                if (url == null)
                {
                    validateMessage = "configuration_error";
                    return;
                }

                string openid = result.openid;
                #region 验证openid是否是已经合法用户
                WebRequest wReq = WebRequest.Create(string.Format("{0}?o={1}", url, openid));
                using (StreamReader reader = new StreamReader(wReq.GetResponse().GetResponseStream()))
                {
                    validateMessage = reader.ReadLine().Trim();
                }
                #endregion

                if (!validateMessage.StartsWith("success"))
                {
                    return;
                }

                string token = validateMessage.Substring(validateMessage.IndexOf(',') + 1);

                #region 验证是否是合法的设备
                if (WasherDeviceBll.Instance.Get(deptId, boardNumber) == null)
                {
                    validateMessage = "device_error";
                    return;
                }
                #endregion

                #region 创建该用户对应的洗车卡
                WasherCardModel card = new WasherCardModel();
                card.CardNo = WasherCardBll.GetNextCouponCardNo(token, dept.KeyId, openid);
                card.Coins = 500;//setting.Coupon.Coins;
                card.DepartmentId = dept.KeyId;
                card.Kind = "Coupon";
                card.Memo = openid;
                card.Password = "";
                card.ValidateFrom = DateTime.Now;
                card.ValidateEnd = DateTime.Now.AddDays(7);
                #endregion

                if (WasherCardBll.Instance.Add(card) <= 0)
                {
                    validateMessage = "save_card_error";
                    return;
                }

                dynamic dobj = WasherValidatorBll.Instance.ValidateCard(deptId, boardNumber, card.CardNo);
                if (dobj.Success != true)
                {
                    validateMessage = "error,"+dobj.Message;
                    return;
                }

                var replyMessage = new ReplyValidateMessage(boardNumber)
                {
                    BalanceId = dobj.BalanceId,
                    Kind = TcpMessageBase.CardKind.Normal,
                    Status = TcpMessageBase.CardStatus.Regular,
                    Money = dobj.Money
                };
                WasherDeviceModel device = WasherDeviceBll.Instance.Get(deptId, boardNumber);
                
                byte[] buffer = replyMessage.ToByteArray();
                buffer[4] = 0xff;

                SendData(device.ListenerIp, buffer);

                validateMessage = "success";
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