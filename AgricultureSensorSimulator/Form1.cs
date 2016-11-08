using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace AgricultureSensorSimulator
{
    public partial class Form1 : Form
    {
        private MqttClient mqttClient;

        public Form1()
        {
            InitializeComponent();
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "启动服务器")
            {
                // create client instance 
                mqttClient = new MqttClient("139.129.43.203");

                // register to message received 
                mqttClient.MqttMsgPublishReceived += MqttClient_MqttMsgPublishReceived; ;

                string clientId = "AgricultureLinkItOneSimulatorClient";
                mqttClient.Connect(clientId);

                // subscribe to the topic "/home/temperature" with QoS 0 
                mqttClient.Subscribe(new string[] { "/a/l/in" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
                mqttClient.Subscribe(new string[] { "/a/a/out" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });

                button1.Text = "关闭服务器";
            }
            else
            {
                mqttClient.Disconnect();

                button1.Text = "启动服务器";
            }
        }

        private void MqttClient_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        private void On_SendMessage(string message)
        {
            if (mqttClient == null)
            {
                return;
            }

            message = message.Replace(":", "");

            byte[] buffer = new byte[message.Length / 2];
            for(int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = Convert.ToByte(message.Substring(i * 2, 2), 16);
            }

            mqttClient.Publish("/a/l/out", buffer);
        }

        private void On_SendMessage2(string message)
        {
            if (mqttClient == null)
            {
                return;
            }

            message = message.Replace(":", "");

            byte[] buffer = new byte[message.Length / 2];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = Convert.ToByte(message.Substring(i * 2, 2), 16);
            }

            mqttClient.Publish("/a/l/in", buffer);
        }

        private void userControlBattery1_Load(object sender, EventArgs e)
        {

        }
    }
}
