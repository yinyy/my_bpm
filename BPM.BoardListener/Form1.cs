using BPM.BoardListenerLib;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Washer.Bll;
using Washer.Model;

namespace BPM.BoardListener
{
    //https://msdn.microsoft.com/zh-cn/library/dxkwh6zw.aspx
    //http://www.cnblogs.com/ysyn/p/3399351.html
    //http://blog.sina.com.cn/s/blog_5f4ffa17010112h7.html
    
    public partial class Form1 : Form
    {
        private ManualResetEvent resetEvent = new ManualResetEvent(false);
        private bool IsRunning = false;
        private Dictionary<string, Socket> clients = new Dictionary<string, Socket>();
        private List<BoardClient> boards = new List<BoardClient>();

        private ILog log;//= LogManager.GetLogger("BPM.BoardListener.Form1");


        public Form1()
        {
            InitializeComponent();

            log = LogManager.GetLogger(this.GetType());
            //if (log.IsDebugEnabled)
            //{
            //    log.Debug("dfadfadfadfadsfsdf");
            //}
        }

        private void miStart_Click(object sender, EventArgs e)
        {
            miStart.Enabled = false;
            miStop.Enabled = true;
            miQuit.Enabled = false;
            IsRunning = true;

            //new Thread(new ThreadStart(()=>
            //{
            //    IPAddress localhost = IPAddress.Parse(ConfigurationManager.AppSettings["address"]);
            //    IPEndPoint localEP = new IPEndPoint(localhost, Convert.ToInt32(ConfigurationManager.AppSettings["port"]));

            //    Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //    server.Bind(localEP);
            //    server.Listen(Int32.MaxValue);

            //    while (IsRunning)
            //    {
            //        try
            //        {
            //            Socket aNewClient = server.Accept();
            //            BoardClient bc = new BoardClient(aNewClient);
            //            boards.Add(bc);
            //            bc.Start();
            //        }
            //        catch
            //        {

            //        }
            //    }
            //})).Start();


            Thread t1 = new Thread(new ThreadStart(() =>
            {
                IPAddress local = IPAddress.Parse(ConfigurationManager.AppSettings["address"]);
                IPEndPoint localEP = new IPEndPoint(local, Convert.ToInt32(ConfigurationManager.AppSettings["port"]));

                Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Bind(localEP);
                server.Listen(Int32.MaxValue);

                ClearStatusText();
                PrintDebug("服务器启动！");

                while (IsRunning)
                {
                    PrintDebug("等待客户端连接......");

                    resetEvent.Reset();
                    server.BeginAccept(new AsyncCallback(AcceptCallback), server);
                    resetEvent.WaitOne();
                }

                if (server.Connected)
                {
                    server.Shutdown(SocketShutdown.Receive);
                    Thread.Sleep(100);
                }
                server.Close();

                PrintDebug("服务器停止！");
            }));
            t1.Start();
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            resetEvent.Set();

            Socket server = ar.AsyncState as Socket;

            try
            {
                Socket client = server.EndAccept(ar);
                string remoteIP = (client.RemoteEndPoint as IPEndPoint).Address.ToString();
                PrintDebug(string.Format("{0} 已经接入。", remoteIP));

                StateObject so = new StateObject() { WorkSocket = client };
                client.BeginReceive(so.buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), so);
            }
            catch (Exception exp)
            {
                PrintDebug("Position 001 " + exp.Message);
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            StateObject so = ar.AsyncState as StateObject;
            Socket client = so.WorkSocket;

            try
            {
                int len = client.EndReceive(ar);
                if (len > 0)
                {
                    new Thread(new ThreadStart(()=>
                    {
                        string ipaddress = (client.RemoteEndPoint as IPEndPoint).Address.ToString();
                        byte[] bs = so.buffer.Take(len).ToArray();

                        ReceivedMessageBase receivedMessage = ReceivedMessageBase.Parse(bs);
                        ReplyMessageBase replyMessage = null;
                        if (receivedMessage != null && (replyMessage = receivedMessage.CreateReplyMessage()) != null)
                        {
                            PrintDebug(receivedMessage.ToString());

                            if (receivedMessage.Command == MessageBase.CommandType.HeartBeat)
                            {
                                //心跳包
                            }
                            else if (receivedMessage.Command == MessageBase.CommandType.ValidateCardAndPassword)
                            {
                                ReplyValidateMessage rvm = replyMessage as ReplyValidateMessage;

                                ReceivedValidateCardAndPasswordMessage rvcpm = receivedMessage as ReceivedValidateCardAndPasswordMessage;
                                WasherDeviceLogModel balance = WasherValidatorBll.Instance.ValidateCardAndPassword(rvcpm.BoardNumber, rvcpm.CardNo, rvcpm.Password);
                                if (balance != null)
                                {
                                    rvm.Kind = MessageBase.CardKind.Normal;
                                    rvm.Status = MessageBase.CardStatus.Regular;
                                    rvm.Money = balance.RemainCoins;
                                    rvm.BalanceId = balance.KeyId;
                                }
                                else
                                {
                                    rvm.Kind = MessageBase.CardKind.Normal;
                                    rvm.Status = MessageBase.CardStatus.Unusual;
                                    rvm.Money = 0;
                                    rvm.BalanceId = 0;
                                }
                            }
                            else if (receivedMessage.Command == MessageBase.CommandType.ValidateCard)
                            {
                                ReplyValidateMessage rvm = replyMessage as ReplyValidateMessage;

                                ReceivedValidateCardMessage rvcm = receivedMessage as ReceivedValidateCardMessage;
                                WasherDeviceLogModel balance = WasherValidatorBll.Instance.ValidateCardAndPassword(rvcm.BoardNumber, rvcm.CardNo);
                                if (balance != null)
                                {
                                    rvm.Kind = MessageBase.CardKind.Normal;
                                    rvm.Status = MessageBase.CardStatus.Regular;
                                    rvm.BalanceId = balance.KeyId;
                                    rvm.Money = balance.RemainCoins;
                                }
                                else
                                {
                                    rvm.Kind = MessageBase.CardKind.Normal;
                                    rvm.Status = MessageBase.CardStatus.Unusual;
                                    rvm.BalanceId = 0;
                                    rvm.Money = 0;
                                }
                            }
                            else if (receivedMessage.Command == MessageBase.CommandType.ValidatePhoneAndPassword)
                            {
                                ReplyValidateMessage rvm = replyMessage as ReplyValidateMessage;

                                ReceivedValidatePhoneAndPasswordMessage rvppm = receivedMessage as ReceivedValidatePhoneAndPasswordMessage;
                                WasherDeviceLogModel balance = WasherValidatorBll.Instance.ValidatePhoneAndPassword(rvppm.BoardNumber, rvppm.Phone, rvppm.Password);
                                if (balance != null)
                                {
                                    rvm.Kind = MessageBase.CardKind.Normal;
                                    rvm.Status = MessageBase.CardStatus.Regular;
                                    rvm.BalanceId = balance.KeyId;
                                    rvm.Money = balance.RemainCoins;
                                }
                                else
                                {
                                    rvm.Kind = MessageBase.CardKind.Normal;
                                    rvm.Status = MessageBase.CardStatus.Unusual;
                                    rvm.BalanceId = 0;
                                    rvm.Money = 0;
                                }
                            }
                            else if (receivedMessage.Command == MessageBase.CommandType.TimeSync)
                            {
                                //时间同步
                                //把设备号和Socket放到字典中
                                string boardNumber = replyMessage.BoardNumber;
                                if (clients.ContainsKey(boardNumber) && !clients[boardNumber].Connected)
                                {
                                    clients.Remove(boardNumber);
                                }

                                if (!clients.ContainsKey(boardNumber))
                                {
                                    clients.Add(boardNumber, client);
                                }

                                #region 更新设备的信息
                                WasherDeviceModel device = WasherDeviceBll.Instance.GetByBoardNumber(boardNumber);
                                if (device != null)
                                {
                                    device.UpdateTime = DateTime.Now;
                                    device.IpAddress = (client.RemoteEndPoint as IPEndPoint).Address.ToString();

                                    WasherDeviceBll.Instance.Update(device);
                                }
                                #endregion
                            }
                            else if (receivedMessage.Command == MessageBase.CommandType.ReaderSetting)
                            {
                                ReplyReadSettingMessage o = replyMessage as ReplyReadSettingMessage;

                                #region 从数据库读取设备的参数设置
                                WasherDeviceModel device = WasherDeviceBll.Instance.GetByBoardNumber(o.BoardNumber);
                                if (device == null)
                                {
                                    o.ErrorCode = 1;
                                }
                                else
                                {
                                    o.ErrorCode = 0;

                                    JObject jobj = JObject.Parse(device.Setting);
                                    JArray jarray = (JArray)jobj["Params"];
                                    for (int i = 0; i < jarray.Count; i++)
                                    {
                                        o.Values[i] = (int)jarray[i];
                                    }

                                    if (!device.Enabled)
                                    {
                                        o.Values[31] = (~0x0001) & o.Values[31];
                                    }
                                }
                                #endregion
                            }
                            else if (receivedMessage.Command == MessageBase.CommandType.UploadStatus)
                            {
                                ReplyUploadStatusMessage o = replyMessage as ReplyUploadStatusMessage;
                                o.Status = 1;

                                #region 把设备数据更新到数据库
                                WasherDeviceModel device = WasherDeviceBll.Instance.GetByBoardNumber(o.BoardNumber);
                                if (device != null)
                                {
                                    ReceivedUploadStatusMessage rusm = receivedMessage as ReceivedUploadStatusMessage;

                                    JArray bitArray = new JArray();
                                    uint initValue = 0x80000000;
                                    do
                                    {
                                        bitArray.Add((initValue & rusm.BitStatus) == 0 ? 0 : 1);
                                        initValue >>= 1;
                                    } while (initValue != 0);

                                    JObject jobj = new JObject();
                                    jobj.Add("Bits", bitArray);
                                    jobj.Add("Values", new JArray(rusm.ValueStatus));

                                    device.Status = JsonConvert.SerializeObject(jobj);

                                    WasherDeviceBll.Instance.Update(device);//00 00 00 00 00 CB 1C 00 01 86 A2 0F 01 20 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 C9
                                }
                                #endregion
                            }
                            else if (receivedMessage.Command == MessageBase.CommandType.Account)
                            {
                                ReceivedAccountMessage ram = receivedMessage as ReceivedAccountMessage;
                                ReplyAccountMessage r = replyMessage as ReplyAccountMessage;
                                r.Remain = WasherDeviceLogBll.Instance.Clearing(ram.Ticks, ram.BoardNumber, ram.BalanceId, ram.Payment);
                                r.BalanceId = ram.BalanceId;
                            }

                            byte[] buffer = replyMessage.ToByteArray();

                            //log.Debug("测试数据："+buffer.Select(a => string.Format("{0:x2} ", a)).Aggregate((r, v) => { return r + v; }));

                            if (receivedMessage.BoardNumber != null && clients.ContainsKey(receivedMessage.BoardNumber))
                            {
                                Socket clt = clients[receivedMessage.BoardNumber];
                                clt.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), clt);
                            }
                            else
                            {
                                client.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), client);
                            }

                            PrintDebug(replyMessage.ToString());
                        }
                        else
                        {
                            PrintDebug("异常接入。ReceivedMessage：" + 
                                (receivedMessage == null ? "空" : receivedMessage.ToString()) + "，ReplyMessage：" +
                                (replyMessage == null ? "空" : replyMessage.ToString()));
                        }
                    })).Start();
                }
                else
                {
                    if (so.data.Count > 0)
                    {
                        PrintDebug(string.Format("{0} 收到 {1} 字节数据。", (client.RemoteEndPoint as IPEndPoint).Address, so.data.Count));
                    }
                    //client.Close();
                }
            }
            catch (Exception exp)
            {
                //client.Close();
                PrintDebug("Position 002 " + exp.Message);
            }
            
            client.BeginReceive(so.buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), so);
        }

        private void SendCallback(IAsyncResult ar)
        {
            Socket client = ar.AsyncState as Socket;
            client.EndSend(ar);
        }

        private void miStop_Click(object sender, EventArgs e)
        {
            miStart.Enabled = true;
            miStop.Enabled = false;
            miQuit.Enabled = true;

            IsRunning = false;
            resetEvent.Set();

            #region 停止所有的连接
            new Thread(new ThreadStart(() =>
            {
                foreach (Socket client in clients.Values)
                {
                    if (client.Connected)
                    {
                        string remoteIP = (client.RemoteEndPoint as IPEndPoint).Address.ToString();
                        client.Close();
                        PrintDebug(remoteIP + " 断开连接。");
                    }
                }

                clients.Clear();
            })).Start();
            #endregion
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            miStop.Enabled = false;
        }

        private void miQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        delegate void ClearStatusTextDelegate();
        private void ClearStatusText()
        {
            if (rtbStatus.InvokeRequired)
            {
                ClearStatusTextDelegate d = new ClearStatusTextDelegate(ClearStatusText);
                Invoke(d);
            }
            else {
                rtbStatus.Clear();
            }
        }

        delegate void LogDelegate(string message);
        private void PrintDebug(string message)
        {
            if (rtbStatus.InvokeRequired)
            {
                LogDelegate d = new LogDelegate(PrintDebug);
                Invoke(d, new object[] { message });
            }
            else {
                if (string.IsNullOrWhiteSpace(rtbStatus.Text))
                {
                    rtbStatus.Text = message;
                }
                else
                {
                    rtbStatus.AppendText("\r\n" + message);
                }
                rtbStatus.ScrollToCaret();
                log.Debug(message);
            }
        }

        private void miClear_Click(object sender, EventArgs e)
        {
            rtbStatus.Clear();
        }

        private void 测试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string unifiedorder = "<xml><prepay_id><![CDATA[wx201411101639507cbf6ffd8b0779950874]]></prepay_id>";
            Match match = Regex.Match(unifiedorder, @"<prepay_id><\!\[CDATA\[(wx\S+)\]\]></prepay_id>");
            Console.WriteLine(match.Groups[1].Value+"");
        }
    }
}
