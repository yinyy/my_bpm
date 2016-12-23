﻿using BPM.BoardListenerLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BPM.BoardListener
{
    public class BoardListenerThread
    {
        private IPresenter presenter;
        private string localAddress;
        private string serverAddress;
        private int departmentId;
        private int port;
        private bool IsRunning;
        private ManualResetEvent resetEvent = new ManualResetEvent(false);
        private Dictionary<string, Socket> clients = new Dictionary<string, Socket>();
        private bool showHeartBeat, saveHeartBeat;
        private Dictionary<string, int> heartBeatCount = new Dictionary<string, int>();
        private int heartBeatThreshold;

        public BoardListenerThread(IPresenter presenter, string localAddress, string serverAddress, int port, int departmentId, bool showHeartBeat, bool saveHeartBeat, int heartBeatThreshold)
        {
            this.presenter = presenter;
            this.localAddress = localAddress;
            this.serverAddress = serverAddress;
            this.port = port;
            this.departmentId = departmentId;
            this.showHeartBeat = showHeartBeat;
            this.saveHeartBeat = saveHeartBeat;
            this.heartBeatThreshold = heartBeatThreshold;
        }

        public void Start()
        {
            IsRunning = true;

            new Thread(() =>
            {
                IPAddress local = IPAddress.Parse(localAddress);
                IPEndPoint localEP = new IPEndPoint(local, port);

                Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Bind(localEP);
                server.Listen(Int32.MaxValue);

                presenter.PrintDebug(string.Format("【{0:######}】服务器启动！", departmentId), true);

                while (IsRunning)
                {
                    presenter.PrintDebug(string.Format("【{0:######}】等待客户端连接！", departmentId), false);

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

                presenter.PrintDebug(string.Format("【{0:######}】服务器停止！", departmentId), true);
            }).Start();
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            resetEvent.Set();

            Socket server = ar.AsyncState as Socket;
            try
            {
                Socket client = server.EndAccept(ar);
                string remoteIP = (client.RemoteEndPoint as IPEndPoint).Address.ToString();

                presenter.PrintDebug(string.Format("【{0:######}】{1} 已经接入。", departmentId, remoteIP), true);

                StateObject so = new StateObject() { WorkSocket = client };
                client.BeginReceive(so.buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), so);
            }
            catch (Exception exp)
            {
                presenter.PrintDebug(string.Format("【{0:######}】异常位置1：{1}。\n{2}", departmentId, exp.Message, exp.StackTrace), true);
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            StateObject so = ar.AsyncState as StateObject;
            Socket client = so.WorkSocket;
            if (client == null || !client.Connected)
            {
                presenter.PrintDebug("某个连接被主动关闭。", false);
            }
            else {
                try
                {
                    int len = client.EndReceive(ar);
                    if (len > 0)
                    {
                        new Thread(() =>
                        {
                            string ipaddress = (client.RemoteEndPoint as IPEndPoint).Address.ToString();
                            byte[] bs = so.buffer.Take(len).ToArray();

                            ReceivedMessageBase receivedMessage = ReceivedMessageBase.Parse(bs);
                            if ((receivedMessage.Command != TcpMessageBase.CommandType.HeartBeat) || showHeartBeat)
                            {
                                presenter.PrintDebug(string.Format("【{0:######}】{1}", departmentId, receivedMessage.ToString()), (receivedMessage.Command != TcpMessageBase.CommandType.HeartBeat) || saveHeartBeat);
                            }

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

                                            try
                                            {
                                                clt.Send(new byte[] { });
                                            }
                                            catch
                                            {
                                                presenter.PrintDebug(string.Format("【{0:######}】连接可能已经断开", departmentId), true);
                                            }

                                            if (clt != null && clt.Connected)
                                            {
                                                //clt.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), clt);
                                                clt.Send(buffer);//由异步改为同步

                                                presenter.PrintDebug(string.Format("【{0:######}】{1}", departmentId, replyMessage.ToString()), true);
                                            }
                                            else
                                            {
                                                presenter.PrintDebug(string.Format("【{0:######}】连接可能已经断开2。", departmentId), true);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ReplyMessageBase replyMessage = null;
                                        if (receivedMessage.Command == TcpMessageBase.CommandType.HeartBeat)
                                        {
                                            //心跳包
                                            replyMessage = new ReplyHeartBeatMessage() { Address = ipaddress };

                                            presenter.UpdateDevice(ipaddress);

                                            if (heartBeatCount.ContainsKey(ipaddress))
                                            {
                                                heartBeatCount[ipaddress]++;
                                            }
                                            else
                                            {
                                                heartBeatCount.Add(ipaddress, 1);
                                            }

                                            if (heartBeatCount[ipaddress] > heartBeatThreshold)
                                            {
                                                heartBeatCount[ipaddress] = 0;

                                                new Thread(() =>
                                                {
                                                    SendMessageToServer(string.Format("http://{0}/Washer/ashx/WasherHandler.ashx?action={1}&clientIp={2}&deptId={3}",
                                                    serverAddress, "HeartBeat", ipaddress/*(client.RemoteEndPoint as IPEndPoint).Address.ToString()*/, departmentId));
                                                }).Start();
                                            }
                                        }
                                        else if (receivedMessage.Command == TcpMessageBase.CommandType.TimeSync)
                                        {
                                            //时间同步
                                            //把设备号和Socket放到字典中
                                            string boardNumber = receivedMessage.BoardNumber;
                                            if (!Form1.blackBoardList.Contains(boardNumber))
                                            {
                                                if (clients.ContainsKey(boardNumber))
                                                {
                                                    Socket clt = clients[boardNumber];
                                                    if (clt != client)
                                                    {
                                                        try
                                                        {
                                                            clt.Shutdown(SocketShutdown.Both);
                                                            clt.Close();
                                                        }
                                                        catch
                                                        {

                                                        }
                                                        clients.Remove(boardNumber);

                                                        presenter.PrintDebug(string.Format("【{0:######}】主板（{1}）时间同步，服务器主动关闭之前的TCP连接。", departmentId, boardNumber), true);
                                                    }
                                                }

                                                if (!clients.ContainsKey(boardNumber))
                                                {
                                                    clients.Add(boardNumber, client);
                                                }

                                                replyMessage = new ReplyTimeSyncMessage(boardNumber);

                                                new Thread(() =>
                                                {
                                                    byte[] buffer = SendMessageToServer(string.Format("http://{0}/Washer/ashx/WasherHandler.ashx?action={1}&boardNumber={2}&clientIp={3}&listenerIp={4}&port={5}&deptId={6}",
                                                    serverAddress, "TimeSync", boardNumber, ipaddress, localAddress, port, departmentId));
                                                    string str = Encoding.UTF8.GetString(buffer);

                                                    var obj = new { Success = false, BoardNumber = "", Serial = "", Address = "", DepartmentName = "", IP = "" };
                                                    obj = JsonConvert.DeserializeAnonymousType(str, obj);

                                                    if (obj.Success == true)
                                                    {
                                                        presenter.AddDevice(obj.Serial, obj.BoardNumber, obj.IP, obj.DepartmentName, obj.Address);
                                                    }
                                                }).Start();
                                            }
                                            else
                                            {
                                                presenter.PrintDebug(string.Format("【{0:######}】黑名单主板（{1}）[{2}]尝试时间同步。", departmentId, boardNumber, ipaddress), false);

                                                try
                                                {
                                                    client.Shutdown(SocketShutdown.Both);
                                                    client.Close();
                                                }
                                                catch
                                                {
                                                }
                                                finally
                                                {
                                                    presenter.PrintDebug(string.Format("【{0:######}】黑名单主板（{1}）[{2}]关闭连接。", departmentId, boardNumber, ipaddress), false);
                                                }
                                                client = null;
                                            }
                                        }
                                        else if (receivedMessage.Command == TcpMessageBase.CommandType.ReaderSetting)
                                        {
                                            byte[] buffer = SendMessageToServer(string.Format("http://{0}/Washer/ashx/WasherHandler.ashx?action={1}&boardNumber={2}&deptId={3}",
                                                serverAddress, "ReaderSetting", receivedMessage.BoardNumber, departmentId));

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
                                                buffer = SendMessageToServer(string.Format("http://{0}/Washer/ashx/WasherHandler.ashx?action={1}&boardNumber={2}&cardNo={3}&password={4}&deptId={5}",
                                                    serverAddress, "ValidateCardAndPassword", rvcpm.BoardNumber, rvcpm.CardNo, rvcpm.Password, departmentId));
                                            }
                                            else if (receivedMessage.Command == TcpMessageBase.CommandType.ValidateCard)
                                            {
                                                ReceivedValidateCardMessage rvcm = receivedMessage as ReceivedValidateCardMessage;
                                                buffer = SendMessageToServer(string.Format("http://{0}/Washer/ashx/WasherHandler.ashx?action={1}&boardNumber={2}&cardNo={3}&deptId={4}",
                                                        serverAddress, "ValidateCard", rvcm.BoardNumber, rvcm.CardNo, departmentId));
                                            }
                                            else if (receivedMessage.Command == TcpMessageBase.CommandType.ValidatePhoneAndPassword)
                                            {
                                                ReceivedValidatePhoneAndPasswordMessage rvcpm = receivedMessage as ReceivedValidatePhoneAndPasswordMessage;
                                                buffer = SendMessageToServer(string.Format("http://{0}/Washer/ashx/WasherHandler.ashx?action={1}&boardNumber={2}&phone={3}&password={4}&deptId={5}",
                                                    serverAddress, "ValidatePhoneAndPassword", rvcpm.BoardNumber, rvcpm.Phone, rvcpm.Password, departmentId));
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

                                            byte[] buffer = SendMessageToServer(string.Format("http://{0}/Washer/ashx/WasherHandler.ashx?action={1}&boardNumber={2}&deptId={3}",
                                                serverAddress, "UploadStatus", receivedMessage.BoardNumber, departmentId), data);
                                            string status = Encoding.UTF8.GetString(buffer);

                                            replyMessage = new ReplyUploadStatusMessage(receivedMessage.BoardNumber, "SUCCESS" == status ? 1 : 0);
                                        }
                                        else if (receivedMessage.Command == TcpMessageBase.CommandType.Account)
                                        {
                                            ReceivedAccountMessage ram = receivedMessage as ReceivedAccountMessage;
                                            byte[] buffer = SendMessageToServer(string.Format("http://{0}/Washer/ashx/WasherHandler.ashx?action={1}&boardNumber={2}&balanceId={3}&payment={4}&ticks={5}&deptId={6}",
                                                serverAddress, "Account", ram.BoardNumber, ram.BalanceId, ram.Payment, ram.Ticks, departmentId));
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
                                            //client.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), client);
                                            client.Send(buffer);

                                            if ((replyMessage.Command != TcpMessageBase.CommandType.HeartBeat) || showHeartBeat)
                                            {
                                                presenter.PrintDebug(string.Format("【{0:######}】{1}", departmentId, replyMessage.ToString()),
                                                    (replyMessage.Command != TcpMessageBase.CommandType.HeartBeat) || saveHeartBeat);
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception eee)
                            {
                                presenter.PrintDebug(string.Format("【{0:######}】异常位置2：{1}。\n{2}", departmentId, eee.Message, eee.StackTrace), true);
                            }
                        }).Start();
                    }
                    else
                    {
                        if (so.data.Count > 0)
                        {
                            presenter.PrintDebug(string.Format("【{0:######}】{1} 收到 {2} 字节数据。", departmentId, (client.RemoteEndPoint as IPEndPoint).Address, so.data.Count), true);
                        }
                        else
                        {
                            string ip = (client.RemoteEndPoint as IPEndPoint).Address.ToString();
                            presenter.PrintDebug(string.Format("【{0:######}】{1} 断开。", departmentId, ip), true);

                            client.Shutdown(SocketShutdown.Both);
                            client.Close();
                            client = null;

                            presenter.RemoveDevice(ip);

                            List<string> keys = new List<string>();
                            foreach (string k in clients.Keys)
                            {
                                if (!clients[k].Connected)
                                {
                                    keys.Add(k);
                                }
                            }
                            foreach (string k in keys)
                            {
                                clients.Remove(k);
                            }
                        }
                        //client.Close();
                    }

                    //TODO:放到这里不知道是否会存在问题
                    if (client != null && client.Connected)
                    {
                        client.BeginReceive(so.buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), so);
                    }
                    else
                    {
                        if (clients.ContainsValue(client))
                        {
                            string key = clients.Where(p => { return p.Value == client; }).Select(p => p.Key).FirstOrDefault();
                            if (key != null)
                            {
                                clients.Remove(key);
                            }
                        }
                    }
                }
                catch (Exception exp)
                {
                    //client.Close();
                    presenter.PrintDebug(string.Format("【{0:######}】异常位置3：{1}。\n{2}", departmentId, exp.Message, exp.StackTrace), true);
                }
            }

            //if (client != null && client.Connected)
            //{
            //    client.BeginReceive(so.buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), so);
            //}
        }

        //private void SendCallback(IAsyncResult ar)
        //{
        //    Socket client = ar.AsyncState as Socket;
        //    client.EndSend(ar);
        //}

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
                presenter.PrintDebug(string.Format("【{0:######}】异常位置4：{1}。\n{2}", departmentId, e.Message, e.StackTrace), true);
            }

            return null;
        }

        public void Stop()
        {
            IsRunning = false;

            #region 停止所有的连接
            new Thread(new ThreadStart(() =>
            {
                foreach (Socket client in clients.Values)
                {
                    if (client!=null && client.Connected)
                    {
                        string remoteIP = (client.RemoteEndPoint as IPEndPoint).Address.ToString();
                        client.Close();
                        presenter.PrintDebug(string.Format("【{0:######}】{1} 断开连接。", departmentId, remoteIP), true);
                    }
                }

                clients.Clear();
            })).Start();
            #endregion

            resetEvent.Set();
        }
    }
}
