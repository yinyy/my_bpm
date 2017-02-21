using BPM.Common;
using BPM.Core.Bll;
using BPM.Core.Model;
using log4net;
using Newtonsoft.Json;
using SuperSocket.SocketBase;
using SuperSocket.WebSocket;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Washer.Bll;
using Washer.Model;
using Washer.Toolkit;

namespace WasherBusiness
{
    public partial class Form1 : Form
    {
        private DateTime timeStart = new DateTime(1970, 1, 1, 0, 0, 0);
        private ILog log;
        private int maxLoggerLines;
        private int thresholdHeartBeat;

        private WebSocketServer webSocketServer;
        private List<BoardAppServer> boardAppServers;

        private SessionCheckThread sessionCheckThread;

        private bool isAutoRoll = false;

        private class DeviceComparator : IComparer
        {
            public int Compare(object x, object y)
            {

                string v1 = string.Format("{0}-{1}", ((ListViewItem)x).SubItems[5], ((ListViewItem)x).SubItems[2]);
                string v2 = string.Format("{0}-{1}", ((ListViewItem)y).SubItems[5], ((ListViewItem)y).SubItems[2]);

                return v1.CompareTo(v2);
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            log = LogManager.GetLogger("LogFile");

            maxLoggerLines = Convert.ToInt32(ConfigurationManager.AppSettings["max_logger_lines"]);
            thresholdHeartBeat = Convert.ToInt32(ConfigurationManager.AppSettings["threshold_heart_beat"]);

            lvDevices.ListViewItemSorter = new DeviceComparator();
        }

        private void 启动服务器SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            #region 启动检查线程
            sessionCheckThread = new WasherBusiness.SessionCheckThread();
            sessionCheckThread.Start();
            #endregion

            #region 启动WebSocket
            string ip = ConfigurationManager.AppSettings["ip"];
            string webSocketPort = ConfigurationManager.AppSettings["web_socket_port"];
            this.webSocketServer = new WebSocketServer();
            if (!this.webSocketServer.Setup(ip, Convert.ToInt32(webSocketPort)))
            {
                PrintLogger(string.Format("创建WebSocket服务器失败。"), true);
                MessageBox.Show("创建WebSocket服务器失败。");

                return;
            }
            if (!this.webSocketServer.Start())
            {
                PrintLogger(string.Format("WebSocket服务器启动失败。"), true);
                MessageBox.Show("WebSocket服务器启动失败。");

                return;
            }

            this.webSocketServer.NewSessionConnected += (session) =>
            {
                PrintLogger(string.Format("【WebSocket】客户端接入。IP：{0}。", session.RemoteEndPoint.Address.ToString()), true);
            };
            this.webSocketServer.NewDataReceived += (session, buffer) =>
            {
                PrintLogger(string.Format("【WebSocket】二进制消息。IP：{0}，消息：{1}。", session.RemoteEndPoint.Address.ToString(),
                    buffer.Select(b => string.Format("{0:X2", b)).Aggregate((a, b) =>
                    {
                        if (string.IsNullOrEmpty(a))
                        {
                            return b;
                        }

                        return a + " " + b;
                    })), true);
            };
            this.webSocketServer.NewMessageReceived += (session, message) =>
            {
                PrintLogger(string.Format("【WebSocket】文本消息。IP：{0}，消息：{1}。", session.RemoteEndPoint.Address.ToString(), message), true);

                var o = new { Action = "", Data="" };
                o = JsonConvert.DeserializeAnonymousType(message, o);
                if (o.Action == "start_machine")
                {
                    var o2 = new { DepartmentId=0, BalanceId=0, Coins=0, BoardNumber="" };
                    o2 = JsonConvert.DeserializeAnonymousType(o.Data, o2);

                    PrintLogger(string.Format("【WebSocket】解密数据。主板编号：{0}。", Aes.Decrypt( o2.BoardNumber)), true);

                    foreach (var svr in boardAppServers)
                    {
                        if (svr.DepartmentId == o2.DepartmentId)
                        {
                            var sn = svr.GetSessions(s => s.BoardNumber == Aes.Decrypt( o2.BoardNumber)).FirstOrDefault();
                            if(sn!=null && sn.Connected)
                            {
                                byte[] buffer = CreateBuffer(RequestCommand.CardAndPassword, 14, Aes.Decrypt( o2.BoardNumber), o2.BalanceId, o2.Coins);
                                sn.Send(buffer, 0, buffer.Length);
                            }
                            break;
                        }
                    }

                    session.Close();
                }else if (o.Action == "download_params")
                {
                    WasherDeviceModel device = WasherDeviceBll.Instance.Get(Convert.ToInt32(o.Data));
                    foreach(var svr in boardAppServers)
                    {
                        if (svr.DepartmentId == device.DepartmentId)
                        {
                            var sn = svr.GetSessions(s => s.BoardNumber == device.BoardNumber).FirstOrDefault();
                            if (sn != null && sn.Connected)
                            {
                                HandleConfig(sn, null, device);
                            }
                        }
                    }
                }
            };
            this.webSocketServer.SessionClosed += (session, reason) =>
            {
                PrintLogger(string.Format("【WebSocket】客户端关闭。IP：{0}。", session.RemoteEndPoint.Address.ToString()), true);
            };
            #endregion

            sessionCheckThread.Add(webSocketServer);


            #region 启动主板监听程序
            this.boardAppServers = new List<BoardAppServer>();

            string deptsConfig = ConfigurationManager.AppSettings["depts"];
            foreach (string config in deptsConfig.Split(';'))
            {
                string[] cs = config.Split(',');
                int deptId = Convert.ToInt32(cs[0]);
                int port = Convert.ToInt32(cs[1]);

                Department dept = DepartmentBll.Instance.Get(deptId);

                BoardAppServer boardAppServer = new BoardAppServer(deptId, dept.DepartmentName);
                if (!boardAppServer.Setup(ip, port))
                {
                    PrintLogger(string.Format("【{0}】创建服务器失败。", deptId), true);
                    continue;
                }

                if (!boardAppServer.Start())
                {
                    PrintLogger(string.Format("【{0}】启动服务器失败。", deptId), true);
                    continue;
                }

                boardAppServers.Add(boardAppServer);

                boardAppServer.NewSessionConnected += (session) =>
                {
                    PrintLogger(string.Format("【{0}】客户端接入。IP：{1}。", deptId, session.RemoteEndPoint.Address.ToString()), false);
                };
                boardAppServer.SessionClosed += (session, reason) =>
                {
                    RemoveDevice(session.BoardNumber);

                    PrintLogger(string.Format("【{0}】客户端关闭。IP：{1}。", deptId, session.RemoteEndPoint.Address.ToString()), false);
                };
                boardAppServer.NewRequestReceived += (session, request) =>
                {
                    switch (request.Command)
                    {
                        case RequestCommand.Balance:
                            HandleBalance(session, request);
                            break;
                        case RequestCommand.CardAndPassword:
                            HandleCardAndPassword(session, request);
                            break;
                        case RequestCommand.CardOnly:
                            HandleCard(session, request);
                            break;
                        case RequestCommand.Config:
                            HandleConfig(session, request);
                            break;
                        case RequestCommand.Heart:
                            HandleHeart(session, request);
                            break;
                        case RequestCommand.PhoneAndPassword:
                            HandlePhoneAndPassword(session, request);
                            break;
                        case RequestCommand.Sync:
                            HandleTimeSync(session, request);
                            break;
                        case RequestCommand.Upload:
                            HandleUpload(session, request);
                            break;
                        case RequestCommand.Temp:
                            HandleTemp(session, request);
                            break;
                    }
                };

                sessionCheckThread.Add(boardAppServer);
            }
            #endregion

            启动服务器SToolStripMenuItem.Enabled = false;
            关闭服务器CToolStripMenuItem.Enabled = true;
            退出QToolStripMenuItem.Enabled = false;
        }

        private void HandleUpload(BoardAppSession s, BoardRequestInfo r)
        {
            PrintLogger(string.Format("【{0}】上传状态，接收。IP：{1}，主板编号：{2}，位状态：【{3}】，值状态：【{4}】。",
                ((BoardAppServer)s.AppServer).DepartmentId, s.RemoteEndPoint.Address.ToString(), r.BoardNumber,
                r.BitStatus.Select((b, idx) => string.Format("第{0:00}位:{1}", idx + 1, b ? 1 : 0)).Aggregate((a, v) =>
                   {
                       if (string.IsNullOrEmpty(a))
                       {
                           return v;
                       }

                       return a + "," + v;
                   }),
                r.ValueStatus.Select((v, idx) => string.Format("第{0:00}值:{1}", idx + 1, v)).Aggregate((a, v) =>
                    {
                        if (string.IsNullOrEmpty(a))
                        {
                            return v;
                        }

                        return a + "," + v;
                    })), true);

            WasherDeviceModel device = WasherDeviceBll.Instance.Get(((BoardAppServer)s.AppServer).DepartmentId, r.BoardNumber);
            if (device == null)
            {
                PrintLogger(string.Format("【{0}】上传状态，验证。非法主板编号。", ((BoardAppServer)s.AppServer).DepartmentId), true);
            }
            else
            {
                device.Status = JSONhelper.ToJson(new
                {
                    Bits = r.BitStatus.Select(b => b ? 1 : 0).ToArray(),
                    Values = r.ValueStatus.ToArray()
                });
                int rtnValue = WasherDeviceBll.Instance.Update(device);

                byte[] buffer = CreateBuffer(RequestCommand.Upload, 1);
                buffer[7] = 0xff;

                s.Send(buffer, 0, buffer.Length);

                PrintLogger(string.Format("【{0}】上传状态，回复。", ((BoardAppServer)s.AppServer).DepartmentId), true);
            }
        }

        private void HandleTimeSync(BoardAppSession s, BoardRequestInfo r)
        {
            PrintLogger(string.Format("【{0}】时间同步，接收。IP：{1}，主板编号：{2}。", ((BoardAppServer)s.AppServer).DepartmentId, s.RemoteEndPoint.Address.ToString(), r.BoardNumber), true);

            WasherDeviceModel device = WasherDeviceBll.Instance.Get(((BoardAppServer)s.AppServer).DepartmentId, r.BoardNumber);
            if (device == null)
            {
                PrintLogger(string.Format("【{0}】时间同步，验证。非法主板编号。", ((BoardAppServer)s.AppServer).DepartmentId), true);
            }
            else
            {
                s.BoardNumber = r.BoardNumber;

                byte[] buffer = CreateBuffer(RequestCommand.Sync, 0x06);

                DateTime now = DateTime.Now;
                //当前时间
                buffer[7] = (byte)(now.Year - 2000);
                buffer[8] = (byte)now.Month;
                buffer[9] = (byte)now.Day;
                buffer[10] = (byte)now.Hour;
                buffer[11] = (byte)now.Minute;
                buffer[12] = (byte)now.Second;

                s.Send(buffer, 0, buffer.Length);

                s.LastUpdateTime = DateTime.Now;

                //更新数据库的状态
                WasherDeviceBll.Instance.UpdateOnlineTime(((BoardAppServer)s.AppServer).DepartmentId, r.BoardNumber, s.RemoteEndPoint.Address.ToString());

                //添加到设备列表中
                UpdateDevice(device.KeyId, device.SerialNumber, device.BoardNumber, s.RemoteEndPoint.Address.ToString(), DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss"), ((BoardAppServer)s.AppServer).DepartmentName, device.Address);

                PrintLogger(string.Format("【{0}】时间同步，回复。更新服务器。", ((BoardAppServer)s.AppServer).DepartmentId), true);
            }
        }

        private void HandlePhoneAndPassword(BoardAppSession s, BoardRequestInfo r)
        {
            PrintLogger(string.Format("【{0}】电话密码，接收。IP：{1}，主板编号：{2}，手机号码：{3}，密码：{4}。", ((BoardAppServer)s.AppServer).DepartmentId, s.RemoteEndPoint.Address.ToString(), r.BoardNumber, r.Telphone, r.Password), true);

            byte[] buffer = CreateBuffer(RequestCommand.CardAndPassword, 10, r.BoardNumber, 0);
            WasherDeviceModel device;
            WasherConsumeModel consume;
            int coins;

            if ((device = WasherDeviceBll.Instance.Get(((BoardAppServer)s.AppServer).DepartmentId, r.BoardNumber)) == null)
            {
                PrintLogger(string.Format("【{0}】电话密码，验证。非法主板编号。", ((BoardAppServer)s.AppServer).DepartmentId), true);
            }
            else if ((consume = WasherConsumeBll.Instance.Get(((BoardAppServer)s.AppServer).DepartmentId, r.Telphone)) == null)
            {
                PrintLogger(string.Format("【{0}】电话密码，验证。非法手机号码。", ((BoardAppServer)s.AppServer).DepartmentId), true);
            }
            else if (consume.Password != r.Password)
            {
                PrintLogger(string.Format("【{0}】电话密码，验证。密码错误。", ((BoardAppServer)s.AppServer).DepartmentId), true);
            }
            else if ((coins = WasherConsumeBll.Instance.GetValidCoins(consume.KeyId)) <= 0)
            {
                PrintLogger(string.Format("【{0}】电话密码，验证。余额不足。", ((BoardAppServer)s.AppServer).DepartmentId), true);
            }
            else
            {
                WasherDeviceLogModel balance = new WasherDeviceLogModel();
                balance.CardId = WasherCardBll.Instance.GetValidCards(consume.KeyId).OrderBy(a => a.ValidateEnd).FirstOrDefault().KeyId;
                balance.ConsumeId = consume.KeyId;
                balance.DeviceId = device.KeyId;
                balance.Kind = "电话密码";
                balance.Memo = "";
                balance.PayCoins = 0;
                balance.RemainCoins = coins;
                balance.Started = DateTime.Now;
                balance.IsShow = true;

                if ((balance.KeyId = WasherDeviceLogBll.Instance.Add(balance)) <= 0)
                {
                    PrintLogger(string.Format("【{0}】电话密码，验证。操作失败。", ((BoardAppServer)s.AppServer).DepartmentId), true);
                }
                else
                {
                    buffer = CreateBuffer(RequestCommand.CardAndPassword, 14, r.BoardNumber, balance.KeyId, coins);

                    PrintLogger(string.Format("【{0}】电话密码，验证。消费编号：{1}，余额：{2:0.00}元。", ((BoardAppServer)s.AppServer).DepartmentId, balance.KeyId, coins / 100.0), true);
                }
            }

            s.Send(buffer, 0, buffer.Length);

            PrintLogger(string.Format("【{0}】电话密码，回复。", ((BoardAppServer)s.AppServer).DepartmentId), true);
        }

        private void HandleHeart(BoardAppSession s, BoardRequestInfo r)
        {
            PrintLogger(string.Format("【{0}】心跳同步，接收。IP：{1}。", ((BoardAppServer)s.AppServer).DepartmentId, s.RemoteEndPoint.Address.ToString()), true);

            s.Send(new byte[] { 0x00 }, 0, 1);

            if (s.LastUpdateTime != null && s.LastUpdateTime.AddMinutes(1).AddSeconds(30).CompareTo(DateTime.Now) < 0)
            {
                s.LastUpdateTime = DateTime.Now;

                //更新列表中的时间
                UpdateDevice(s.BoardNumber, s.RemoteEndPoint.Address.ToString());

                //更新设备的最后更新时间
                WasherDeviceBll.Instance.UpdateOnlineTime(((BoardAppServer)s.AppServer).DepartmentId, s.BoardNumber);
            }

            PrintLogger(string.Format("【{0}】心跳同步，回复。", ((BoardAppServer)s.AppServer).DepartmentId), true);
        }
        private void HandleConfig(BoardAppSession s, BoardRequestInfo r, WasherDeviceModel device=null)
        {
            if (r != null)
            {
                PrintLogger(string.Format("【{0}】读取设置，接收。IP：{1}，主板编号：{2}。", ((BoardAppServer)s.AppServer).DepartmentId, s.RemoteEndPoint.Address.ToString(), r.BoardNumber), true);
            }
            else
            {
                PrintLogger(string.Format("【{0}】读取设置，接收。", ((BoardAppServer)s.AppServer).DepartmentId), true);
            }

            byte[] buffer = CreateBuffer(RequestCommand.Config, 1);
            device = device ?? WasherDeviceBll.Instance.Get(((BoardAppServer)s.AppServer).DepartmentId, r.BoardNumber);
            if (device == null)
            {
                PrintLogger(string.Format("【{0}】读取设置，验证。非法主板编号。", ((BoardAppServer)s.AppServer).DepartmentId), true);
            }
            else
            {
                try
                {
                    var o = new { Coin = 0, Params = new int[1] };
                    o = JsonConvert.DeserializeAnonymousType(device.Setting, o);
                    buffer = CreateBuffer(RequestCommand.Config, 68, device.BoardNumber);

                    for (int i = 0; i < 32; i++)
                    {
                        buffer[11 + i * 2] = (byte)((o.Params[i] & 0xff00) >> 8);
                        buffer[11 + i * 2 + 1] = (byte)(o.Params[i] & 0xff);
                    }

                    if (!device.Enabled)
                    {
                        buffer[buffer.Length - 1] = (byte)((~0x01) & buffer[buffer.Length - 1]);
                    }

                    PrintLogger(string.Format("【{0}】读取设置，验证。设置项：{1}。",
                    ((BoardAppServer)s.AppServer).DepartmentId,
                   buffer.Skip(11).Select((v, idx) => string.Format("第{0:00}值:{1}", idx + 1, v)).Aggregate((a, v) =>
                   {
                       if (string.IsNullOrEmpty(a))
                       {
                           return v;
                       }

                       return a + "," + v;
                   })), true);
                }
                catch
                {
                    PrintLogger(string.Format("【{0}】读取设置，验证。设置项错误。", ((BoardAppServer)s.AppServer).DepartmentId), true);
                }

                s.Send(buffer, 0, buffer.Length);

                PrintLogger(string.Format("【{0}】读取设置，回复。", ((BoardAppServer)s.AppServer).DepartmentId), true);
            }
        }

        private void HandleCard(BoardAppSession s, BoardRequestInfo r)
        {
            PrintLogger(string.Format("【{0}】刷卡，接收。IP：{1}，主板编号：{2}，卡号：{3}。", ((BoardAppServer)s.AppServer).DepartmentId, s.RemoteEndPoint.Address.ToString(), r.BoardNumber, r.Card), true);

            byte[] buffer = CreateBuffer(RequestCommand.CardAndPassword, 10, r.BoardNumber, 0);
            WasherDeviceModel device;
            WasherCardModel card;

            if ((device = WasherDeviceBll.Instance.Get(((BoardAppServer)s.AppServer).DepartmentId, r.BoardNumber)) == null)
            {
                PrintLogger(string.Format("【{0}】刷卡，验证。非法主板编号。", ((BoardAppServer)s.AppServer).DepartmentId), true);
            }
            else if ((card=WasherCardBll.Instance.Get(((BoardAppServer)s.AppServer).DepartmentId, r.Card)) == null)
            {
                PrintLogger(string.Format("【{0}】刷卡，验证。洗车卡不存在。", ((BoardAppServer)s.AppServer).DepartmentId), true);
            }
            else if (card.ValidateEnd.CompareTo(DateTime.Now)<=0)
            {
                PrintLogger(string.Format("【{0}】刷卡，验证。洗车卡已过有效期。", ((BoardAppServer)s.AppServer).DepartmentId), true);
            }
            else if (card.Coins<=0)
            {
                PrintLogger(string.Format("【{0}】刷卡，验证。余额不足。", ((BoardAppServer)s.AppServer).DepartmentId), true);
            }
            else
            {
                WasherDeviceLogModel balance = new WasherDeviceLogModel();
                balance.CardId = card.KeyId;
                balance.ConsumeId = card.BinderId;
                balance.DeviceId = device.KeyId;
                balance.Kind = "刷卡";
                balance.Memo = "";
                balance.PayCoins = 0;
                balance.RemainCoins = card.Coins;
                balance.Started = DateTime.Now;
                balance.IsShow = true;

                if ((balance.KeyId = WasherDeviceLogBll.Instance.Add(balance)) <= 0)
                {
                    PrintLogger(string.Format("【{0}】刷卡，验证。操作失败。", ((BoardAppServer)s.AppServer).DepartmentId), true);
                }
                else
                {
                    buffer = CreateBuffer(RequestCommand.CardAndPassword, 14, r.BoardNumber, balance.KeyId, card.Coins);

                    PrintLogger(string.Format("【{0}】刷卡，验证。消费编号：{1}，余额：{2:0.00}元。", ((BoardAppServer)s.AppServer).DepartmentId, balance.KeyId, card.Coins / 100.0), true);
                }
            }

            s.Send(buffer, 0, buffer.Length);

            PrintLogger(string.Format("【{0}】刷卡，回复。", ((BoardAppServer)s.AppServer).DepartmentId), true);
        }

        private void HandleCardAndPassword(BoardAppSession s, BoardRequestInfo r)
        {
            PrintLogger(string.Format("【{0}】卡号密码，接收。IP：{1}，主板编号：{2}，卡号：{3}，密码：{4}。", ((BoardAppServer)s.AppServer).DepartmentId, s.RemoteEndPoint.Address.ToString(), r.BoardNumber, r.Card, r.Password), true);

            byte[] buffer = CreateBuffer(RequestCommand.CardAndPassword, 10, r.BoardNumber, 0);
            WasherDeviceModel device;
            WasherCardModel card = null;
            WasherConsumeModel consume;

            if ((device = WasherDeviceBll.Instance.Get(((BoardAppServer)s.AppServer).DepartmentId, r.BoardNumber)) == null)
            {
                PrintLogger(string.Format("【{0}】卡号密码，验证。非法主板编号。", ((BoardAppServer)s.AppServer).DepartmentId), true);
            }
            else if ((card = WasherCardBll.Instance.Get(((BoardAppServer)s.AppServer).DepartmentId, r.Card)) == null)
            {
                PrintLogger(string.Format("【{0}】卡号密码，验证。洗车卡不存在。", ((BoardAppServer)s.AppServer).DepartmentId), true);
            }
            else if (card.BinderId == null)
            {
                PrintLogger(string.Format("【{0}】卡号密码，验证。洗车卡未绑定。", ((BoardAppServer)s.AppServer).DepartmentId), true);
            }
            else if ((consume = WasherConsumeBll.Instance.Get(card.BinderId.Value)) == null)
            {
                PrintLogger(string.Format("【{0}】卡号密码，验证。绑定的消费者不存在。", ((BoardAppServer)s.AppServer).DepartmentId), true);
            }
            else if (consume.Password != r.Password)
            {
                PrintLogger(string.Format("【{0}】卡号密码，验证。洗车卡密码错误。", ((BoardAppServer)s.AppServer).DepartmentId), true);
            }
            else if (card.ValidateEnd.CompareTo(DateTime.Now) <= 0)
            {
                PrintLogger(string.Format("【{0}】卡号密码，验证。洗车卡已过有效期。", ((BoardAppServer)s.AppServer).DepartmentId), true);
            }
            else if (card.Coins <= 0)
            {
                PrintLogger(string.Format("【{0}】卡号密码，验证。余额不足。", ((BoardAppServer)s.AppServer).DepartmentId), true);
            }
            else
            {
                WasherDeviceLogModel balance = new WasherDeviceLogModel();
                balance.CardId = card.KeyId;
                balance.ConsumeId = card.BinderId;
                balance.DeviceId = device.KeyId;
                balance.Kind = "卡号密码";
                balance.Memo = "";
                balance.PayCoins = 0;
                balance.RemainCoins = card.Coins;
                balance.Started = DateTime.Now;
                balance.IsShow = true;

                if ((balance.KeyId = WasherDeviceLogBll.Instance.Add(balance)) <= 0)
                {
                    PrintLogger(string.Format("【{0}】卡号密码，验证。操作失败。", ((BoardAppServer)s.AppServer).DepartmentId), true);
                }
                else
                {
                    buffer = CreateBuffer(RequestCommand.CardAndPassword, 14, r.BoardNumber, balance.KeyId, card.Coins);

                    PrintLogger(string.Format("【{0}】卡号密码，验证。消费编号：{1}，余额：{2:0.00}元。", ((BoardAppServer)s.AppServer).DepartmentId, balance.KeyId, card.Coins / 100.0), true);
                }
            }

            s.Send(buffer, 0, buffer.Length);

            PrintLogger(string.Format("【{0}】卡号密码，回复。", ((BoardAppServer)s.AppServer).DepartmentId), true);
        }

        private void HandleBalance(BoardAppSession s, BoardRequestInfo r)
        {
            PrintLogger(string.Format("【{0}】消费结算，接收。IP：{1}，主板编号：{2}，消费编号：{3}，结算金额：{4:0.00}元。", ((BoardAppServer)s.AppServer).DepartmentId, s.RemoteEndPoint.Address.ToString(), r.BoardNumber, r.BalanceId, r.Payment / 100.0), true);

            byte[] buffer = CreateBuffer2(RequestCommand.Balance, 12, r.BoardNumber, r.BalanceId, 0);
            WasherDeviceModel device;
            WasherDeviceLogModel balance;

            if ((device = WasherDeviceBll.Instance.Get(((BoardAppServer)s.AppServer).DepartmentId, r.BoardNumber)) == null)
            {
                s.Send(buffer, 0, buffer.Length);

                PrintLogger(string.Format("【{0}】消费结算，验证。非法主板编号。", ((BoardAppServer)s.AppServer).DepartmentId), true);
            }
            else if ((balance = WasherDeviceLogBll.Instance.Get(r.BalanceId)) == null)
            {
                s.Send(buffer, 0, buffer.Length);

                PrintLogger(string.Format("【{0}】消费结算，验证。非法消费编号。", ((BoardAppServer)s.AppServer).DepartmentId), true);
            }
            //else if (balance.Ticks != null)
            //{
            //    PrintLogger(string.Format("【{0}】消费结算，验证。已被结算，凭证：{1}。", ((BoardAppServer)s.AppServer).DepartmentId, balance.Ticks), true);
            //}
            else if (r.Payment < 0)
            {
                s.Send(buffer, 0, buffer.Length);

                PrintLogger(string.Format("【{0}】消费结算，验证。结算金额小于0。", ((BoardAppServer)s.AppServer).DepartmentId), true);
            }
            else
            {
                if (balance.CardId == 0)/*这代表的是微信支付*/
                {
                    balance.PayCoins = r.Payment;
                }
                else
                {
                    balance.PayCoins = WasherCardBll.Instance.Deduction(balance.CardId, r.Payment, (int)r.Ticks);
                }
                balance.Ticks = (int)r.Ticks;
                balance.Ended = DateTime.Now;

                if (WasherDeviceLogBll.Instance.Update(balance) < 0)
                {
                    s.Send(buffer, 0, buffer.Length);

                    PrintLogger(string.Format("【{0}】消费结算，验证。操作失败。", ((BoardAppServer)s.AppServer).DepartmentId), true);
                }
                else
                {
                    buffer = CreateBuffer2(RequestCommand.Balance, 12, r.BoardNumber, r.BalanceId, balance.PayCoins);

                    PrintLogger(string.Format("【{0}】消费结算，验证。消费编号：{1}，实际消费：{2:0.00}元。", ((BoardAppServer)s.AppServer).DepartmentId, r.BalanceId, balance.PayCoins / 100.0), true);

                    s.Send(buffer, 0, buffer.Length);

                    PrintLogger(string.Format("【{0}】消费结算，回复。", ((BoardAppServer)s.AppServer).DepartmentId), true);

                    #region 余额支付洗车送积分
                    WasherDepartmentSetting setting = JsonConvert.DeserializeObject<WasherDepartmentSetting>(DepartmentBll.Instance.Get(device.DepartmentId).Setting);
                    int point = 0;
                    if ((balance.Kind == "余额洗车" || balance.Kind == "电话密码" || balance.Kind == "卡号密码" || balance.Kind == "刷卡")
                        && balance.ConsumeId != null
                        && (point = balance.PayCoins * setting.PayWashCar.Vip / 100 / 100) > 0)
                    {
                        WasherRewardModel reward = new WasherRewardModel();
                        reward.ConsumeId = balance.ConsumeId.Value;
                        reward.Expired = false;
                        reward.Kind = "会员洗车送积分";
                        reward.Memo = JSONhelper.ToJson(new { BalanceId = balance.KeyId, Kind= balance.Kind, Money = string.Format("{0:0.00}", balance.PayCoins / 100.0), Percent = setting.PayWashCar.Vip });
                        reward.Points = point;
                        reward.Time = DateTime.Now;
                        reward.Used = 0;

                        reward.KeyId = WasherRewardBll.Instance.Add(reward);
                    }
                    #endregion

                    #region 如果是外部服务，则需要根据设置判断是否需要回调
                    if (balance.Kind.StartsWith("外部服务"))
                    {
                        var o= new { Desc="",Tag="",Echostr="",Rid=""};
                        o = JsonConvert.DeserializeAnonymousType(balance.Memo, o);

                        WasherOutsiderModel outsider = WasherOutsiderBll.Instance.Get(device.DepartmentId, o.Tag);
                        if (!string.IsNullOrEmpty(outsider.Url))
                        {
                            new Thread(() =>
                            {
                                string url = string.Format("{0}{1}echostr={2}&rid={3}", outsider.Url, outsider.Url.IndexOf('?') == -1 ? "?" : "&", o.Echostr, o.Rid);

                                System.Net.WebRequest wReq = System.Net.WebRequest.Create(url);
                                System.Net.WebResponse wResp = wReq.GetResponse();
                                System.IO.Stream respStream = wResp.GetResponseStream();

                                respStream.Close();
                                wResp.Close();
                            }).Start();
                        }
                    }
                    #endregion
                }
            }
        }

        private void HandleTemp(BoardAppSession s, BoardRequestInfo r)
        {
            PrintLogger(string.Format("【{0}】临时结算，接收。IP：{1}，主板编号：{2}，消费编号：{3}，结算金额：{4:0.00}元。", ((BoardAppServer)s.AppServer).DepartmentId, s.RemoteEndPoint.Address.ToString(), r.BoardNumber, r.BalanceId, r.Payment / 100.0), true);

            WasherDeviceLogModel balance = WasherDeviceLogBll.Instance.Get(r.BalanceId);
            if (balance == null)
            {
                PrintLogger(string.Format("【{0}】消费结算，验证。非法消费编号。", ((BoardAppServer)s.AppServer).DepartmentId), true);
            }
            else
            {
                balance.TempTime = DateTime.Now;
                balance.TempCoins = r.Payment;

                WasherDeviceLogBll.Instance.Update(balance);

                PrintLogger(string.Format("【{0}】消费结算，回复。", ((BoardAppServer)s.AppServer).DepartmentId), true);
            }
        }

        private byte[] CreateBuffer(RequestCommand requestCommand, byte dataLength, string boardNumber = null)
        {
            byte[] buffer = new byte[4 + 2 + 1 + dataLength];

            long time = (DateTime.Now.Ticks - timeStart.Ticks) / 1000;

            //时间戳
            for (int i = 0; i < 4; i++)
            {
                buffer[3 - i] = (byte)((time & (0xff << i * 8)) >> (0xff >> i * 8));
            }

            //命令
            buffer[4] = (byte)((((int)requestCommand) & 0xff00) >> 8);
            buffer[5] = (byte)(((int)requestCommand) & 0xff);

            //长度
            buffer[6] = dataLength;

            if (boardNumber != null)
            {
                long code = Convert.ToInt64(boardNumber);
                for (int i = 0; i < 4; i++)
                {
                    buffer[7 + i] = (byte)((code & (0xff << (3 - i) * 8)) >> (3 - i) * 8);
                }
            }

            return buffer;
        }

        private byte[] CreateBuffer(RequestCommand requestCommand, byte dataLength, string boardNumber, long balanceId, int coin = 0)
        {
            byte[] buffer = CreateBuffer(requestCommand, dataLength, boardNumber);

            if (balanceId == 0)
            {
                buffer[15] = 0x01;
                buffer[16] = 0x02;
            }
            else
            {
                buffer[15] = 0x01;
                buffer[16] = 0x01;
            }

            for (int i = 0; i < 4; i++)
            {
                buffer[11 + i] = (byte)((balanceId & (0xff << (3 - i) * 8)) >> (3 - i) * 8);
            }

            if (coin > 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    buffer[17 + i] = (byte)((coin & (0xff << (3 - i) * 8)) >> (3 - i) * 8);
                }
            }

            return buffer;
        }

        /// <summary>
        /// 创建结算回复的字节数组
        /// </summary>
        /// <param name="requestCommand"></param>
        /// <param name="dataLength"></param>
        /// <param name="boardNumber"></param>
        /// <param name="balanceId"></param>
        /// <param name="coin"></param>
        /// <returns></returns>
        private byte[] CreateBuffer2(RequestCommand requestCommand, byte dataLength, string boardNumber, long balanceId, int coin)
        {
            byte[] buffer = CreateBuffer(requestCommand, dataLength, boardNumber);
            
            for (int i = 0; i < 4; i++)
            {
                buffer[11 + i] = (byte)((balanceId & (0xff << (3 - i) * 8)) >> (3 - i) * 8);
            }

            if (coin > 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    buffer[15 + i] = (byte)((coin & (0xff << (3 - i) * 8)) >> (3 - i) * 8);
                }
            }

            return buffer;
        }

        delegate void PrintLoggerDelegate(string message, bool saveLog);
        public void PrintLogger(string message, bool saveLog)
        {
            if (rtbLog.InvokeRequired)
            {
                PrintLoggerDelegate d = new PrintLoggerDelegate(PrintLogger);
                Invoke(d, new object[] { message, saveLog });
            }
            else
            {
                if (rtbLog.Lines.Count() >= maxLoggerLines)
                {
                    rtbLog.Clear();
                }

                rtbLog.AppendText(string.Format("[{0:yyyy/MM/dd HH:mm:ss:sss}]{1}\r\n", DateTime.Now, message));

                if (isAutoRoll)
                {
                    rtbLog.ScrollToCaret();
                }

                if (saveLog)
                {
                    log.Debug(message);
                }
            }
        }

        delegate void AddDeviceDelegate(int kid, string serial, string code, string ip, string time, string belonged, string address);
        public void UpdateDevice(int kid, string serial, string code, string ip, string time, string belonged, string address)
        {
            if (rtbLog.InvokeRequired)
            {
                AddDeviceDelegate d = new AddDeviceDelegate(UpdateDevice);
                Invoke(d, new object[] { kid, serial, code, ip, time, belonged, address });
            }
            else
            {
                for (int i = 0; i < lvDevices.Items.Count; i++)
                {
                    if (Convert.ToInt32(lvDevices.Items[i].Tag) == kid)
                    {
                        lvDevices.Items.RemoveAt(i);
                        break;
                    }
                }

                ListViewItem lvi = new ListViewItem();
                lvi.Tag = kid;
                lvi.Text = "";
                lvi.SubItems.Add(serial);
                lvi.SubItems.Add(code);
                lvi.SubItems.Add(ip.Split('.').Select(a=>string.Format("   {0}",a)).Select(a=>a.Substring(a.Length-3)).Aggregate((a, b) => string.Format("{0}.{1}", a, b)));
                lvi.SubItems.Add(time);
                lvi.SubItems.Add(belonged);
                lvi.SubItems.Add(address);

                lvDevices.Items.Add(lvi);

                lvDevices.Sort();

                for (int i = 0; i < lvDevices.Items.Count; i++)
                {
                    lvDevices.Items[i].Text = (i + 1).ToString();
                }
            }
        }

        delegate void UpdateDeviceDelegate(string board, string ip);
        public void UpdateDevice(string  board, string ip)
        {
            if (rtbLog.InvokeRequired)
            {
                UpdateDeviceDelegate d = new UpdateDeviceDelegate(UpdateDevice);
                Invoke(d, new object[] {board, ip });
            }
            else
            {
                ListViewItem current = null;
                for (int i = 0; i < lvDevices.Items.Count; i++)
                {
                    if (lvDevices.Items[i].SubItems[2].Text == board)
                    {
                        current = lvDevices.Items[i];
                        break;
                    }
                }

                if (current != null)
                {
                    current.SubItems[3].Text = ip.Split('.').Select(a => string.Format("   {0}", a)).Select(a => a.Substring(a.Length - 3)).Aggregate((a, b) => string.Format("{0}.{1}", a, b));
                    current.SubItems[4].Text = DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss");
                }
            }
        }

        delegate void RemoveDeviceDelegate(string board);
        public void RemoveDevice(string board)
        {
            if (rtbLog.InvokeRequired)
            {
                RemoveDeviceDelegate d = new RemoveDeviceDelegate(RemoveDevice);
                Invoke(d, new object[] { board });
            }
            else
            {
                for (int i = 0; i < lvDevices.Items.Count; i++)
                {
                    if (lvDevices.Items[i].SubItems[2].Text == board)
                    {
                        lvDevices.Items.RemoveAt(i);
                        break;
                    }
                }

                for (int i = 0; i < lvDevices.Items.Count; i++)
                {
                    lvDevices.Items[i].Text = (i + 1).ToString();
                }
            }
        }

        private void 关闭服务器CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.boardAppServers != null)
            {
                foreach(var svr in this.boardAppServers)
                {
                    foreach(var s in svr.GetAllSessions())
                    {
                        if (s.Connected)
                        {
                            s.Close();
                        }
                    }

                    svr.Stop();
                }

                this.boardAppServers = null;
            }

            if (this.webSocketServer != null)
            {
                foreach(var s in this.webSocketServer.GetAllSessions())
                {
                    if (s.Connected)
                    {
                        s.Close();
                    }
                }

                this.webSocketServer.Stop();
                this.webSocketServer = null;
            }

            启动服务器SToolStripMenuItem.Enabled = true;
            关闭服务器CToolStripMenuItem.Enabled = false;
            退出QToolStripMenuItem.Enabled = true;
        }

        private void 退出QToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 清空CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbLog.Clear();
        }

        private void 测试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string card = "201400990099";
            string enc = Aes.Encrypt(card);
            Console.WriteLine(enc);

            Console.WriteLine(Aes.Decrypt(enc));
        }

        private void 自动滚动ToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            isAutoRoll = 自动滚动ToolStripMenuItem.Checked;
        }
    }
}
