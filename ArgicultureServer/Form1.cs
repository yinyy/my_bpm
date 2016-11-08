using BPM.Agriculture.Bll;
using BPM.Agriculture.Model;
using Newtonsoft.Json;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.Containers;
using SuperSocket.WebSocket;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace ArgicultureServer
{
    public partial class Form1 : Form
    {
        private MqttClient mqttClient;
        private WebSocketServer webSocketServer;
        private bool IsRunning = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (IsRunning)
            {
                mqttClient.Disconnect();
                webSocketServer.Stop();

                button1.Text = "启动服务器";
                IsRunning = false;
            }
            else
            {
                // create client instance 
                mqttClient = new MqttClient("139.129.43.203");

                // register to message received 
                mqttClient.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

                string clientId = "AgricultureerverClient";
                mqttClient.Connect(clientId);

                // subscribe to the topic "/home/temperature" with QoS 0 
                mqttClient.Subscribe(new string[] { "/a/l/out" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
                mqttClient.Subscribe(new string[] { "/a/a/out" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });


                webSocketServer = new WebSocketServer();
                if (!webSocketServer.Setup("127.0.0.1", 8181))
                {
                    MessageBox.Show("服务器建立失败。");
                    return;
                }
                if (!webSocketServer.Start())
                {
                    MessageBox.Show("服务器启动失败。");
                    return;
                }
                webSocketServer.NewSessionConnected += WebSocketServer_NewSessionConnected;
                webSocketServer.NewMessageReceived += WebSocketServer_NewMessageReceived;
                webSocketServer.NewDataReceived += WebSocketServer_NewDataReceived;
                webSocketServer.SessionClosed += WebSocketServer_SessionClosed;

                button1.Text = "关闭服务器";
                IsRunning = true;
            }
        }

        private void WebSocketServer_SessionClosed(WebSocketSession session, SuperSocket.SocketBase.CloseReason value)
        {
            Console.WriteLine("closed");
        }

        private void WebSocketServer_NewDataReceived(WebSocketSession session, byte[] value)
        {
            Console.WriteLine("data");
        }

        private void WebSocketServer_NewMessageReceived(WebSocketSession session, string value)
        {
            byte[] buffer = new byte[value.Length / 2];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = Convert.ToByte(value.Substring(i * 2, 2), 16);
            }

            mqttClient.Publish("/a/l/in", buffer);
        }

        private void WebSocketServer_NewSessionConnected(WebSocketSession session)
        {
            Console.WriteLine("connected");
        }

        private void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            byte[] bs = e.Message;
            string msg = e.Message.Select(b => string.Format("{0:x2}", b)).Aggregate((p, s) => p + s);
            Console.WriteLine(string.Format("{0}===={1}", e.Topic, msg));

            string mac = msg.Substring(0, 16);
            string kind = msg.Substring(16, 2);

            AgricultureDeviceModel adm = AgricultureDeviceBll.Instance.Get(mac);
            var ss = AgricultureSubscriberBll.Instance.GetByDeviceId(adm.KeyId);
            string openids = "";
            if (ss.Count > 0)
            {
                openids = ss.Select(a => a.OpenId).Aggregate((a, b) => a + " " + b);
            }

            object jsonDevice;
            if (kind == "02")
            {
                int[] status = new int[Convert.ToByte(msg.Substring(18, 2), 16)];
                int value = Convert.ToInt16(msg.Substring(20, 2), 16);
                for (int i = 0; i < status.Length; i++)
                {
                    status[i] = value & 0x01;
                    value >>= 1;
                }

                jsonDevice = new { Mac = mac, Kind = adm.Kind, Status = status };
            }
            else if (kind == "03")
            {
                float temperature = Convert.ToInt16(msg.Substring(18, 4), 16) / 10.0f;
                int humidity = Convert.ToByte(msg.Substring(22, 2), 16);
                jsonDevice = new
                {
                    Mac = mac,
                    Kind = adm.Kind,
                    Temperature = temperature,
                    Humidity = humidity
                };

                if (!string.IsNullOrWhiteSpace(openids) && adm.Threshold != null && adm.Threshold.Length > 2)
                {
                    string message = "";
                    ThresholdModel[] threshold = JsonConvert.DeserializeObject<ThresholdModel[]>(adm.Threshold);
                    if (threshold[0].Low.Enabled && temperature < Convert.ToSingle(threshold[0].Low.Value))
                    {
                        message = string.Format("{0} 当前温度为 {1}℃，低于预设值 {2}℃。", adm.Title, temperature, threshold[0].Low.Value);
                    }
                    else if (threshold[0].High.Enabled && temperature > Convert.ToSingle(threshold[0].High.Value))
                    {
                        message = string.Format("{0} 当前温度为 {1}℃，高于预设值 {2}℃。", adm.Title, temperature, threshold[0].High.Value);
                    }
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        SendMessage(message, openids);
                    }

                    message = "";
                    if (threshold[1].Low.Enabled && humidity < Convert.ToByte(threshold[1].Low.Value))
                    {
                        message = string.Format("{0} 当前湿度为 {1}%，低于预设值 {2}%。", adm.Title, humidity, threshold[1].Low.Value);
                    }
                    else if (threshold[1].High.Enabled && humidity > Convert.ToByte(threshold[1].High.Value))
                    {
                        message = string.Format("{0} 当前湿度为 {1}%，高于预设值 {2}%。", adm.Title, humidity, threshold[1].High.Value);
                    }
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        SendMessage(message, openids);
                    }
                }
            }
            else if (kind == "04")
            {
                int humidity = Convert.ToByte(msg.Substring(18, 2), 16);
                jsonDevice = new { Mac = mac, Kind = adm.Kind, Humidity = humidity };

                if (!string.IsNullOrWhiteSpace(openids) && adm.Threshold != null && adm.Threshold.Length > 2)
                {
                    string message = "";
                    ThresholdModel[] threshold = JsonConvert.DeserializeObject<ThresholdModel[]>(adm.Threshold);
                    if (threshold[0].Low.Enabled && humidity < Convert.ToByte(threshold[0].Low.Value))
                    {
                        message = string.Format("{0} 当前湿度为 {1}%，低于预设值 {2}%。", adm.Title, humidity, threshold[0].Low.Value);
                    }
                    else if (threshold[0].High.Enabled && humidity > Convert.ToByte(threshold[0].High.Value))
                    {
                        message = string.Format("{0} 当前湿度为 {1}%，高于预设值 {2}%。", adm.Title, humidity, threshold[0].High.Value);
                    }
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        SendMessage(message, openids);
                    }
                }
            }
            else if (kind == "05")
            {
                int value = Convert.ToInt32(msg.Substring(18, 4), 16);
                jsonDevice = new { Mac = mac, Kind = adm.Kind, Value = value };

                if (!string.IsNullOrWhiteSpace(openids) && adm.Threshold != null && adm.Threshold.Length > 2)
                {
                    string message = "";
                    ThresholdModel[] threshold = JsonConvert.DeserializeObject<ThresholdModel[]>(adm.Threshold);
                    if (threshold[0].High.Enabled && value > Convert.ToInt32(threshold[0].High.Value))
                    {
                        message = string.Format("{0} 当前颗粒物浓度为 {1}μg/m³，高于预设值 {2}μg/m³。", adm.Title, value, threshold[0].High.Value);
                    }
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        SendMessage(message, openids);
                    }
                }
            }
            else if (kind == "06")
            {
                int[] values = new int[Convert.ToInt16(msg.Substring(18, 2), 16)];
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = Convert.ToInt16(msg.Substring(20 + i * 2, 2), 16);
                }

                jsonDevice = new { Mac = mac, Kind = adm.Kind, Values = values };
            }
            else
            {
                jsonDevice = new { };
            }

            foreach (var session in webSocketServer.GetAllSessions())
            {
                session.Send(JsonConvert.SerializeObject(jsonDevice));
            }
        }

        private void SendMessage(string message, string openids)
        {
            string[] openid = openids.Split(' ');
            foreach (string id in openid)
            {
                CustomApi.SendText(AccessTokenContainer.TryGetAccessToken(Toolkit.AppId, Toolkit.Secret), id, message);
            }
        }
    }
}
