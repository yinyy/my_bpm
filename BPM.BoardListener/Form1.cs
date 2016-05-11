using BPM.BoardListenerLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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


        public Form1()
        {
            InitializeComponent();
        }

        private void miStart_Click(object sender, EventArgs e)
        {
            miStart.Enabled = false;
            miStop.Enabled = true;
            miQuit.Enabled = false;
            IsRunning = true;
            Thread t1 = new Thread(new ThreadStart(() =>
            {
                IPAddress local = IPAddress.Parse(ConfigurationManager.AppSettings["address"]);
                IPEndPoint localEP = new IPEndPoint(local, Convert.ToInt32(ConfigurationManager.AppSettings["port"]));

                Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Bind(localEP);
                server.Listen(Int32.MaxValue);

                ClearStatusText();
                AddStatusMessage("服务器启动！");

                while (IsRunning)
                {
                    AddStatusMessage("等待客户端连接......");
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
                AddStatusMessage("服务器停止！");
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
                AddStatusMessage(remoteIP + " 已经接入。");

                StateObject so = new StateObject() { WorkSocket = client };
                client.BeginReceive(so.buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), so);
            }
            catch (Exception exp)
            {
                AddStatusMessage("Position 001 " + exp.Message);
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            StateObject so = ar.AsyncState as StateObject;
            Socket client = so.WorkSocket;

            try {
                int len = client.EndReceive(ar);
                if (len > 0)
                {
                    byte[] bs = so.buffer.Take(len).ToArray();
                    ReceiveDebug((client.RemoteEndPoint as IPEndPoint).Address.ToString(), bs);

                    ReceivedMessageBase receivedMessage = ReceivedMessageBase.Parse(bs);
                    ReplyMessageBase replyMessage = null;
                    if (receivedMessage != null && (replyMessage = receivedMessage.CreateReplyMessage()) != null)
                    {
                        if (receivedMessage.Command == MessageBase.CommandType.HeartBeat)
                        {
                            //心跳包
                        }
                        else if (receivedMessage.Command == MessageBase.CommandType.ValidateCardAndPassword)
                        {
                            ReplyValidateMessage rvm = replyMessage as ReplyValidateMessage;

                            ReceivedValidateCardAndPasswordMessage rvcpm = receivedMessage as ReceivedValidateCardAndPasswordMessage;
                            WasherDeviceModel device;
                            WasherCardModel card;
                            WasherConsumeModel consume;
                            if ((device = WasherDeviceBll.Instance.GetByBoardNumber(rvcpm.BoardNumber)) != null &&
                                (card = WasherCardBll.Instance.Get(device.DepartmentId, rvcpm.CardNo)) != null &&
                                card.ValidateEnd.Date.CompareTo(DateTime.Now.Date) >= 0 && card.Coins >= 5 &&
                                card.BinderId != null && (consume = WasherConsumeBll.Instance.Get(card.BinderId.Value)) != null &&
                                consume.Password == rvcpm.Password)
                            {
                                rvm.Kind = MessageBase.CardKind.Normal;
                                rvm.Status = MessageBase.CardStatus.Regular;
                                rvm.Money = (int)(card.Coins * 100);
                                rvm.CardId = card.KeyId;
                            }
                            else
                            {
                                rvm.Kind = MessageBase.CardKind.Normal;
                                rvm.Status = MessageBase.CardStatus.Unusual;
                                rvm.Money = 0;
                                rvm.CardId = 0;
                            }
                        }
                        else if (receivedMessage.Command == MessageBase.CommandType.ValidateCard)
                        {
                            ReplyValidateMessage rvm = replyMessage as ReplyValidateMessage;

                            ReceivedValidateCardMessage rvcm = receivedMessage as ReceivedValidateCardMessage;
                            WasherDeviceModel device;
                            WasherCardModel card;
                            if ((device = WasherDeviceBll.Instance.GetByBoardNumber(rvcm.BoardNumber)) != null &&
                                (card = WasherCardBll.Instance.Get(device.DepartmentId, rvcm.CardNo)) != null &&
                                card.ValidateEnd.Date.CompareTo(DateTime.Now.Date) >= 0 && card.Coins >= 5)
                            {
                                rvm.Kind = MessageBase.CardKind.Normal;
                                rvm.Status = MessageBase.CardStatus.Regular;
                                rvm.CardId = card.KeyId;
                                rvm.Money = (int)(card.Coins * 100);
                            }
                            else
                            {
                                rvm.Kind = MessageBase.CardKind.Normal;
                                rvm.Status = MessageBase.CardStatus.Unusual;
                                rvm.CardId = 0;
                                rvm.Money = 0;
                            }
                        }
                        else if (receivedMessage.Command == MessageBase.CommandType.ValidatePhoneAndPassword)
                        {
                            ReplyValidateMessage rvm = replyMessage as ReplyValidateMessage;

                            ReceivedValidatePhoneAndPasswordMessage rvppm = receivedMessage as ReceivedValidatePhoneAndPasswordMessage;
                            WasherDeviceModel device;
                            WasherConsumeModel consume;
                            float coins;

                            if ((device = WasherDeviceBll.Instance.GetByBoardNumber(receivedMessage.BoardNumber)) != null &&
                                (consume = WasherConsumeBll.Instance.Get(device.DepartmentId, rvppm.Phone)) != null &&
                                consume.Password == rvppm.Password && (coins = WasherConsumeBll.Instance.GetValidCoins(consume.KeyId)) >= 5)
                            {
                                rvm.Kind = MessageBase.CardKind.Normal;
                                rvm.Status = MessageBase.CardStatus.Regular;
                                rvm.CardId = WasherConsumeBll.Instance.GetValidCards(consume.KeyId).FirstOrDefault().KeyId;
                                rvm.Money = (int)(coins * 100);
                            }
                            else
                            {
                                rvm.Kind = MessageBase.CardKind.Normal;
                                rvm.Status = MessageBase.CardStatus.Unusual;
                                rvm.CardId = 0;
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
                            r.CardId = ram.CardId;

                            WasherDeviceModel device;
                            WasherCardModel card;
                            WasherSpendingModel spending;
                            //卡号为0表示微信支付的用户
                            if (ram.CardId != 0 && (device = WasherDeviceBll.Instance.GetByBoardNumber(ram.BoardNumber)) != null &&
                                    (card = WasherCardBll.Instance.Get(ram.CardId)) != null &&
                                    ((spending = WasherSpendingBll.Instance.Get(card.BinderId.Value, device.KeyId, ram.Ticks)) == null
                                    || spending.Time.AddSeconds(30) < DateTime.Now))
                            {
                                r.Payment = (int)(WasherCardBll.Instance.Deduction(ram.CardId, ram.Payment / 100.0f) * 100);

                                WasherSpendingBll.Instance.Add(new WasherSpendingModel()
                                {
                                    ConsumeId = card.BinderId.Value,
                                    DeviceId = device.KeyId,
                                    Time = DateTime.Now,
                                    Ticks = (int)ram.Ticks,
                                    Kind = "洗车",
                                    Coins = r.Payment/100f,
                                    Memo = ""
                                });
                            }
                            else
                            {
                                r.Payment = 0;
                            }
                        }
                        else if (receivedMessage.Command == MessageBase.CommandType.Operation)
                        {
                            ReplyOperationMessage o = replyMessage as ReplyOperationMessage;
                            o.Status = 1;
                        }

                        byte[] buffer = replyMessage.ToByteArray();
                        client.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), client);

                        SendDebug((client.RemoteEndPoint as IPEndPoint).Address.ToString(), buffer);//放到Callback中会不会好一些
                    }
                }
                else
                {
                    if (so.data.Count > 0)
                    {
                        AddStatusMessage((client.RemoteEndPoint as IPEndPoint).Address + " 收到 " + so.data.Count + "字节数据。");
                    }
                    client.Close();
                }

                client.BeginReceive(so.buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), so);
            }catch(Exception exp)
            {
                //client.Close();
                AddStatusMessage("Position 002 " + exp.Message);
            }
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
                        AddStatusMessage(remoteIP + " 断开连接。");
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

        delegate void AddStatusMessageDelegate(string message);
        private void AddStatusMessage(string message)
        {
            if (rtbStatus.InvokeRequired)
            {
                AddStatusMessageDelegate d = new AddStatusMessageDelegate(AddStatusMessage);
                Invoke(d, new object[] { message });
            }
            else {
                rtbStatus.Text = message + "\r\n" + rtbStatus.Text;
            }
        }

        private void miClear_Click(object sender, EventArgs e)
        {
            rtbStatus.Clear();
        }
        
        private void ReceiveDebug(string ip, byte[] bs)
        {
            StringBuilder sb = new StringBuilder();
            foreach(byte b in bs) 
            {
                sb.Append(string.Format("{0:x2} ", b));
            }
            AddStatusMessage(string.Format("从 {0} 接收 {1}", ip, sb.ToString()));
        }

        private void SendDebug(string ip, byte[] bs)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(string.Format("{0:x2} ", b));
            }
            AddStatusMessage(string.Format("向 {0} 发送 {1}", ip, sb.ToString()));
        }
    }
}
