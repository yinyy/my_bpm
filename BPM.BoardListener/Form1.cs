using BPM.BoardListenerLib;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

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
        //private Socket server;

        private class DeviceSorter : IComparer
        {
            public int Compare(object x, object y)
            {
                string s1 = (x as ListViewItem).SubItems[1].Text;
                string s2 = (y as ListViewItem).SubItems[1].Text;

                return s1.CompareTo(s2);
            }
        }

        private ILog log;//= LogManager.GetLogger("BPM.BoardListener.Form1");

        public bool AutoStart { get; set; }

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
                    new Thread(new ThreadStart(() =>
                    {
                        string ipaddress = (client.RemoteEndPoint as IPEndPoint).Address.ToString();
                        byte[] bs = so.buffer.Take(len).ToArray();

                        ReceivedMessageBase receivedMessage = ReceivedMessageBase.Parse(bs);
                        PrintDebug(receivedMessage.ToString());

                        try
                        {
                            if (receivedMessage.Command != TcpMessageBase.CommandType.Unknown)
                            {
                                if (receivedMessage.Command == TcpMessageBase.CommandType.Send)
                                {
                                    client.Close();
                                    client = null;

                                    //发送命令
                                    ReplyMessageBase replyMessage = new ReplySendMessage(receivedMessage.Meta);
                                    byte[] buffer = replyMessage.ToByteArray();

                                    if (clients.ContainsKey(receivedMessage.BoardNumber))
                                    {
                                        Socket clt = clients[receivedMessage.BoardNumber];
                                        if (clt.Connected)
                                        {
                                            clt.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), clt);

                                            PrintDebug(replyMessage.ToString());
                                        }
                                    }
                                }
                                else
                                {
                                    ReplyMessageBase replyMessage = null;
                                    if (receivedMessage.Command == TcpMessageBase.CommandType.HeartBeat)
                                    {
                                        //心跳包
                                        replyMessage = new ReplyHeartBeatMessage() { Address = ipaddress};
                                    }
                                    else if (receivedMessage.Command == TcpMessageBase.CommandType.ReaderSetting)
                                    {
                                        byte[] buffer = SendMessageToServer(string.Format("http://{0}/Washer/ashx/WasherHandler.ashx?action={1}&boardNumber={2}",
                                            ConfigurationManager.AppSettings["server_address"], "ReaderSetting", receivedMessage.BoardNumber));

                                        string str = Encoding.UTF8.GetString(buffer);
                                        var obj = new { ErrorCode = 0, Values = new int[32], Enabled = true };
                                        obj = JsonConvert.DeserializeAnonymousType(str, obj);

                                        replyMessage = new ReplyReadSettingMessage(receivedMessage.BoardNumber);
                                        (replyMessage as ReplyReadSettingMessage).ErrorCode = obj.ErrorCode;
                                        (replyMessage as ReplyReadSettingMessage).Values = obj.Values;
                                        if (!obj.Enabled)
                                        {
                                            (replyMessage as ReplyReadSettingMessage).Values[31] = (~0x0001) & (replyMessage as ReplyReadSettingMessage).Values[31];
                                        }
                                    }
                                    else if (receivedMessage.Command == TcpMessageBase.CommandType.ValidateCardAndPassword
                                    || receivedMessage.Command == TcpMessageBase.CommandType.ValidateCard
                                    || receivedMessage.Command == TcpMessageBase.CommandType.ValidatePhoneAndPassword)
                                    {
                                        byte[] buffer = null;

                                        if (receivedMessage.Command == TcpMessageBase.CommandType.ValidateCardAndPassword)
                                        {
                                            ReceivedValidateCardAndPasswordMessage rvcpm = receivedMessage as ReceivedValidateCardAndPasswordMessage;
                                            buffer = SendMessageToServer(string.Format("http://{0}/Washer/ashx/WasherHandler.ashx?action={1}&boardNumber={2}&cardNo={3}&password={4}",
                                                ConfigurationManager.AppSettings["server_address"], "ValidateCardAndPassword", rvcpm.BoardNumber, rvcpm.CardNo, rvcpm.Password));
                                        }
                                        else if (receivedMessage.Command == TcpMessageBase.CommandType.ValidateCard)
                                        {
                                            ReceivedValidateCardMessage rvcm = receivedMessage as ReceivedValidateCardMessage;
                                            buffer = SendMessageToServer(string.Format("http://{0}/Washer/ashx/WasherHandler.ashx?action={1}&boardNumber={2}&cardNo={3}",
                                                    ConfigurationManager.AppSettings["server_address"], "ValidateCard", rvcm.BoardNumber, rvcm.CardNo));
                                        }
                                        else if (receivedMessage.Command == TcpMessageBase.CommandType.ValidatePhoneAndPassword)
                                        {
                                            ReceivedValidatePhoneAndPasswordMessage rvcpm = receivedMessage as ReceivedValidatePhoneAndPasswordMessage;
                                            buffer = SendMessageToServer(string.Format("http://{0}/Washer/ashx/WasherHandler.ashx?action={1}&boardNumber={2}&phone={3}&password={4}",
                                                ConfigurationManager.AppSettings["server_address"], "ValidatePhoneAndPassword", rvcpm.BoardNumber, rvcpm.Phone, rvcpm.Password));
                                        }

                                        if (buffer != null)
                                        {
                                            var jobj = new { Kind = 0, Status = 0, Money = 0, BalanceId = 0 };
                                            jobj = JsonConvert.DeserializeAnonymousType(Encoding.UTF8.GetString(buffer), jobj);

                                            replyMessage = new ReplyValidateMessage(receivedMessage.BoardNumber)
                                            {
                                                BalanceId = jobj.BalanceId,
                                                Kind = (TcpMessageBase.CardKind)jobj.Kind,
                                                Status = (TcpMessageBase.CardStatus)jobj.Status,
                                                Money = jobj.Money
                                            };
                                        }
                                    }
                                    else if (receivedMessage.Command == TcpMessageBase.CommandType.TimeSync)
                                    {
                                        //时间同步
                                        //把设备号和Socket放到字典中
                                        string boardNumber = receivedMessage.BoardNumber;
                                        if (clients.ContainsKey(boardNumber) && !clients[boardNumber].Connected)
                                        {
                                            clients.Remove(boardNumber);
                                        }

                                        if (!clients.ContainsKey(boardNumber))
                                        {
                                            clients.Add(boardNumber, client);
                                        }

                                        replyMessage = new ReplyTimeSyncMessage(boardNumber);

                                        new Thread(() =>
                                        {
                                            byte[] buffer = SendMessageToServer(string.Format("http://{0}/Washer/ashx/WasherHandler.ashx?action={1}&boardNumber={2}&clientIp={3}&listenerIp={4}",
                                            ConfigurationManager.AppSettings["server_address"], "TimeSync", boardNumber, (client.RemoteEndPoint as IPEndPoint).Address.ToString(), ConfigurationManager.AppSettings["address"]));
                                            string str = Encoding.UTF8.GetString(buffer);

                                            var obj = new { Success = false, BoardNumber = "", Serial = "", Address = "", DepartmentName = "", IP = "" };
                                            obj = JsonConvert.DeserializeAnonymousType(str, obj);

                                            if (obj.Success == true)
                                            {
                                                AddDevieInfo(obj.Serial, obj.BoardNumber, obj.IP, obj.DepartmentName, obj.Address);
                                            }
                                        }).Start();
                                    }
                                    else if (receivedMessage.Command == TcpMessageBase.CommandType.UploadStatus)
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

                                        string data = JsonConvert.SerializeObject(jobj);

                                        byte[] buffer = SendMessageToServer(string.Format("http://{0}/Washer/ashx/WasherHandler.ashx?action={1}&boardNumber={2}",
                                            ConfigurationManager.AppSettings["server_address"], "UploadStatus", receivedMessage.BoardNumber), data);
                                        string status = Encoding.UTF8.GetString(buffer);

                                        replyMessage = new ReplyUploadStatusMessage(receivedMessage.BoardNumber, "SUCCESS" == status ? 1 : 0);
                                    }
                                    else if (receivedMessage.Command == TcpMessageBase.CommandType.Account)
                                    {
                                        ReceivedAccountMessage ram = receivedMessage as ReceivedAccountMessage;
                                        byte[] buffer = SendMessageToServer(string.Format("http://{0}/Washer/ashx/WasherHandler.ashx?action={1}&boardNumber={2}&balanceId={3}&payment={4}&ticks={5}",
                                            ConfigurationManager.AppSettings["server_address"], "Account", ram.BoardNumber, ram.BalanceId, ram.Payment, ram.Ticks));
                                        string str = Encoding.UTF8.GetString(buffer);
                                        if (!string.IsNullOrWhiteSpace(str))
                                        {
                                            var jobj = new { Remain = 0, BalanceId = 0 };
                                            jobj = JsonConvert.DeserializeAnonymousType(str, jobj);

                                            replyMessage = new ReplyAccountMessage(receivedMessage.BoardNumber)
                                            {
                                                BalanceId = jobj.BalanceId,
                                                Remain = jobj.Remain
                                            };
                                        }
                                    }

                                    if (replyMessage != null)
                                    {
                                        byte[] buffer = replyMessage.ToByteArray();
                                        client.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), client);

                                        PrintDebug(replyMessage.ToString());
                                    }
                                }
                            }
                        }
                        catch (Exception eee)
                        {
                            PrintDebug("Position 003 " + eee.Message);
                        }
                    })).Start();
                }
                else
                {
                    if (so.data.Count > 0)
                    {
                        PrintDebug(string.Format("{0} 收到 {1} 字节数据。", (client.RemoteEndPoint as IPEndPoint).Address, so.data.Count));
                    }
                    else
                    {
                        string ip = (client.RemoteEndPoint as IPEndPoint).Address.ToString();
                        PrintDebug(string.Format("{0} 断开。", ip));
                        client.Close();

                        RemoveDevieInfo(ip);
                    }
                    //client.Close();
                }
            }
            catch (Exception exp)
            {
                //client.Close();
                PrintDebug("Position 002 " + exp.Message);
            }

            if (client != null && client.Connected)
            {
                client.BeginReceive(so.buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), so);
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
            //resetEvent.Reset();

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

            resetEvent.Set();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            miStop.Enabled = false;

            deviceList.ListViewItemSorter = new DeviceSorter();

            if (AutoStart)
            {
                miStart_Click(null, null);
            }
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
                rtbStatus.AppendText("\r\n" + string.Format("[{0:yyyy/MM/dd HH:mm:ss:sss}]{1}", DateTime.Now, message));
                rtbStatus.ScrollToCaret();
                log.Debug(message);
            }
        }

        delegate void AddDeviceInfoDelegate(string serial, string board, string ip, string department, string address);
        private void AddDevieInfo(string serial, string board, string ip, string department, string address)
        {
            if (deviceList.InvokeRequired)
            {
                AddDeviceInfoDelegate d = new AddDeviceInfoDelegate(AddDevieInfo);
                Invoke(d, new object[] { serial, board, ip, department, address });
            }
            else {
                for(int i = deviceList.Items.Count - 1; i >= 0; i--)
                {
                    if (deviceList.Items[i].SubItems[1].Text == serial)
                    {
                        deviceList.Items.Remove(deviceList.Items[i]);
                    }
                }
                
                ListViewItem lvi = new ListViewItem("");
                lvi.SubItems.Add(serial);
                lvi.SubItems.Add(board);
                lvi.SubItems.Add(ip);
                lvi.SubItems.Add(department);
                lvi.SubItems.Add(address);

                deviceList.Items.Add(lvi);

                deviceList.Sort();

                long index = 0;
                foreach (ListViewItem t in deviceList.Items)
                {
                    t.Text = ++index + "";
                }
            }
        }

        delegate void RemoveDeviceInfoDelegate(string ip);
        private void RemoveDevieInfo(string ip)
        {
            if (deviceList.InvokeRequired)
            {
                RemoveDeviceInfoDelegate d = new RemoveDeviceInfoDelegate(RemoveDevieInfo);
                Invoke(d, new object[] { ip});
            }
            else {
                for (int i = deviceList.Items.Count - 1; i >= 0; i--)
                {
                    if (deviceList.Items[i].SubItems[3].Text == ip)
                    {
                        deviceList.Items.Remove(deviceList.Items[i]);
                    }
                }

                deviceList.Sort();

                long index = 0;
                foreach (ListViewItem t in deviceList.Items)
                {
                    t.Text = ++index + "";
                }
            }
        }

        private void miClear_Click(object sender, EventArgs e)
        {
            rtbStatus.Clear();
        }

        private void 测试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //string unifiedorder = "<xml><prepay_id><![CDATA[wx201411101639507cbf6ffd8b0779950874]]></prepay_id>";
            //Match match = Regex.Match(unifiedorder, @"<prepay_id><\!\[CDATA\[(wx\S+)\]\]></prepay_id>");
            //Console.WriteLine(match.Groups[1].Value+"");

            dynamic dobj = new { Success = true, Message = "dfadsfasdfasdfasdf" };
            Console.WriteLine(dobj.Success);
        }

        private byte[] SendMessageToServer(string url, string data = null)
        {
            WebRequest request = HttpWebRequest.Create(url);
            if (data == null)
            {
                request.Method = "GET";
            }
            else
            {
                request.Method = "POST";

                byte[] buffer = Encoding.UTF8.GetBytes(data);
                request.ContentLength = buffer.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Flush();
                }
            }

            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        byte[] buffer = new byte[1024];
                        int len = stream.Read(buffer, 0, buffer.Length);

                        return buffer.Take(len).ToArray();
                    }
                }
            }
            catch (Exception e)
            {
                PrintDebug("SendMessageToServer:" + e.Message);
            }

            return null;
        }

        private void miCurrentFolder_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", Application.StartupPath);
        }
    }
}