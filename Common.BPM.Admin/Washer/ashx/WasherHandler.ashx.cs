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
            switch (action)
            {
                case "send_command":
                    int balanceId = Convert.ToInt32(context.Request.Params["BalanceId"]);
                    WasherDeviceLogModel deviceLog = WasherDeviceLogBll.Instance.Get(balanceId);
                    if (deviceLog == null)
                    {
                        context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "设备日志不存在。" }));
                    }
                    else if (deviceLog.Ended.Ticks>0)
                    {
                        context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "设备日志已经过期。" }));
                    }
                    else
                    {
                        WasherDeviceModel device = WasherDeviceBll.Instance.Get(deviceLog.DeviceId);
                        int boardNumber = Convert.ToInt32(device.BoardNumber);

                        IPAddress localhost = IPAddress.Parse("127.0.0.1");
                        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        byte[] buffer = new byte[21];
                        try
                        {
                            socket.Connect(IPAddress.Parse(ConfigurationManager.AppSettings["tcp_server"]), Convert.ToInt32(ConfigurationManager.AppSettings["tcp_server_port"]));

                            //0-3：时间戳
                            //4-5：命令
                            buffer[4] = 0x00;
                            buffer[5] = 101;
                            //6：数据量
                            buffer[6] = 12;
                            //7-10：设备编号
                            buffer[7] = (byte)((boardNumber >> 24) & 0xff);
                            buffer[8] = (byte)((boardNumber >> 16) & 0xff);
                            buffer[9] = (byte)((boardNumber >> 8) & 0xff);
                            buffer[10] = (byte)(boardNumber & 0xff);
                            //11-14：记录序号
                            buffer[11] = (byte)((deviceLog.CardId >> 24) & 0xff);
                            buffer[12] = (byte)((deviceLog.CardId >> 16) & 0xff);
                            buffer[13] = (byte)((deviceLog.CardId >> 8) & 0xff);
                            buffer[14] = (byte)(deviceLog.CardId & 0xff);
                            //15：卡类型
                            buffer[15] = 0x01;
                            //16：卡状态
                            buffer[16] = 0x01;
                            //17-20：金额
                            buffer[17] = (byte)((deviceLog.RemainCoins >> 24) & 0xff);
                            buffer[18] = (byte)((deviceLog.RemainCoins >> 16) & 0xff);
                            buffer[19] = (byte)((deviceLog.RemainCoins >> 8) & 0xff);
                            buffer[20] = (byte)(deviceLog.RemainCoins & 0xff);

                            socket.Send(buffer);
                            socket.Close();
                            
                            WasherConsumeModel consume = WasherConsumeBll.Instance.Get(deviceLog.ConsumeId);
                            WasherWeChatConsumeModel wxconsume = WasherWeChatConsumeBll.Instance.Get(consume.BinderId.Value);
                            Department dept = DepartmentBll.Instance.Get(consume.DepartmentId);
                            
                            CustomApi.SendText(AccessTokenContainer.TryGetAccessToken(dept.Appid, dept.Secret), wxconsume.OpenId, "机器已经启动");

                            context.Response.Write(JSONhelper.ToJson(new { Success =true }));
                        }
                        catch
                        {
                            context.Response.Write(JSONhelper.ToJson(new { Success = false, Message="Socket通信错误。" }));
                        }
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