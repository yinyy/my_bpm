using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 主板模拟程序
{
    public partial class Form1 : Form
    {
        private Socket socket = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void SaveParams(object sender, EventArgs e)
        {
            string file = string.Format("{0}\\params.txt", Application.StartupPath);

            using (StreamWriter writer = new StreamWriter(file, false, Encoding.UTF8))
            {
                writer.WriteLine(tbBoardNumber.Text);
                writer.WriteLine(tbIpAddress.Text);
                writer.WriteLine(tbPort.Text);
                writer.WriteLine(tbCard.Text);
                writer.WriteLine(tbPhone.Text);
                writer.WriteLine(tbPassword.Text);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string file = string.Format("{0}\\params.txt", Application.StartupPath);
            if (File.Exists(file))
            {
                using (StreamReader reader = new StreamReader(file, Encoding.UTF8))
                {
                    string line;
                    if ((line = reader.ReadLine()) != null)
                    {
                        tbBoardNumber.Text = line;
                    }
                    if ((line = reader.ReadLine()) != null)
                    {
                        tbIpAddress.Text = line;
                    }
                    if ((line = reader.ReadLine()) != null)
                    {
                        tbPort.Text = line;
                    }
                    if ((line = reader.ReadLine()) != null)
                    {
                        tbCard.Text = line;
                    }
                    if ((line = reader.ReadLine()) != null)
                    {
                        tbPhone.Text = line;
                    }
                    if ((line = reader.ReadLine()) != null)
                    {
                        tbPassword.Text = line;
                    }
                }
            }

            tbBoardNumber.TextChanged += SaveParams;
            tbIpAddress.TextChanged += SaveParams;
            tbPort.TextChanged += SaveParams;
            tbCard.TextChanged += SaveParams;
            tbPhone.TextChanged += SaveParams;
            tbPassword.TextChanged += SaveParams;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (socket != null && socket.Connected)
            {
                socket.Close();
            }

            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(tbIpAddress.Text), Convert.ToInt32(tbPort.Text));
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(remoteEP);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (socket != null && socket.Connected)
            {
                socket.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (socket != null && socket.Connected)
            {
                byte[] buffer = { 0x00 };
                socket.Send(buffer);

                buffer = new byte[100];
                socket.Receive(buffer);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            long board = Convert.ToInt64(tbBoardNumber.Text);
            byte[] buffer = { 0x00, 0x00, 0x00, 0x00, 0x00, 0xC9, 0x04, (byte)((board & 0xff000000) >> 24), (byte)((board & 0xff0000) >> 16), (byte)((board & 0xff00) >> 8), (byte)((board & 0xff)) };
            socket.Send(buffer);

            buffer = new byte[100];
            socket.Receive(buffer);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            long board = Convert.ToInt64(tbBoardNumber.Text);
            long card = Convert.ToInt64(tbCard.Text);

            byte[] buffer = {0x00, 0x00, 0x00, 0x00, 0x00, 0x66, 0x08,
                (byte)((board & 0xff000000) >> 24), (byte)((board & 0xff0000) >> 16), (byte)((board & 0xff00) >> 8), (byte)((board & 0xff)),
                (byte)((card & 0xff000000) >> 24), (byte)((card& 0xff0000) >> 16), (byte)((card& 0xff00) >> 8), (byte)((card& 0xff))
            };

            socket.Send(buffer);
            HandleData();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            int time = (int)(DateTime.Now - startTime).TotalSeconds;
            long board = Convert.ToInt64(tbBoardNumber.Text);
            long id = Convert.ToInt64(tbId.Text);
            long cost = Convert.ToInt64(Convert.ToSingle(tbCost.Text) * 100);

            byte[] buffer = { (byte)((time & 0xff000000) >> 24), (byte)((time & 0xff0000) >> 16), (byte)((time & 0xff00) >> 8), (byte)(time & 0xff),
                0x00,0xCD, 0x0c,
                (byte)((board & 0xff000000) >> 24), (byte)((board & 0xff0000) >> 16), (byte)((board & 0xff00) >> 8), (byte)((board & 0xff)),
                (byte)((id & 0xff000000) >> 24), (byte)((id& 0xff0000) >> 16), (byte)((id& 0xff00) >> 8), (byte)((id& 0xff)),
                (byte)((cost & 0xff000000) >> 24), (byte)((cost& 0xff0000) >> 16), (byte)((cost& 0xff00) >> 8), (byte)((cost& 0xff))
            };

            socket.Send(buffer);

            buffer = new byte[100];
            int len = socket.Receive(buffer);

            cost = (buffer[15] << 24) + (buffer[16] << 16) + (buffer[17] << 8) + buffer[18];
            MessageBox.Show(string.Format("实际扣款：{0:#.00}", cost / 100.0));
        }

        private void HandleData()
        {
            byte[] buffer = new byte[100];
            int len = socket.Receive(buffer);

            foreach (byte b in buffer)
            {
                Console.Write("{0:x2} ", b);
            }
            Console.WriteLine();

            int command = (buffer[4] << 8) + buffer[5];
            if (command == 0x0065)
            {
                len = buffer[6];
                long board = (buffer[7] << 24) + (buffer[8] << 16) + (buffer[9] << 8) + buffer[10];
                long id = (buffer[11] << 24) + (buffer[12] << 16) + (buffer[13] << 8) + buffer[14];
                int kind = buffer[15];
                int status = buffer[16];
                long money = (buffer[17] << 24) + (buffer[18] << 16) + (buffer[19] << 8) + buffer[20];

                tbStatus.Text = string.Format("{0}/{1}", kind == 0x02 ? "维护卡" : kind == 0x01 ? "消费卡" : "未知卡", status == 0x01 ? "正常" : status == 0x02 ? "异常" : "未知");
                tbId.Text = id+"";
                tbMoney.Text = string.Format("{0:#.00}", money / 100.0);

                MessageBox.Show("洗车机开始工作！");
            }
            else
            {
                MessageBox.Show("返回的数据不正确。");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            long board = Convert.ToInt64(tbBoardNumber.Text);
            long phone = Convert.ToInt64(tbPhone.Text);
            long password = Convert.ToInt64(tbPassword.Text);

            byte[] buffer = {0x00, 0x00, 0x00, 0x00, 0x00, 0x67, 0x0e,
                (byte)((board & 0xff000000) >> 24), (byte)((board & 0xff0000) >> 16), (byte)((board & 0xff00) >> 8), (byte)((board & 0xff)),
                (byte)((phone & 0xff0000000000) >> 40), (byte)((phone& 0xff00000000) >> 32), (byte)((phone& 0xff000000) >> 24), (byte)((phone& 0xff0000)>>16), (byte)((phone& 0xff00)>>8), (byte)(phone& 0xff),
                (byte)((password& 0xff000000) >> 24), (byte)((password& 0xff0000)>>16), (byte)((password& 0xff00)>>8), (byte)(password& 0xff)
            };
            
            socket.Send(buffer);
            HandleData();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            long board = Convert.ToInt64(tbBoardNumber.Text);
            long card = Convert.ToInt64(tbCard.Text);
            long password = Convert.ToInt64(tbPassword.Text);
            
            byte[] buffer = {0x00, 0x00, 0x00, 0x00, 0x00, 0x65, 0x0c,
                (byte)((board & 0xff000000) >> 24), (byte)((board & 0xff0000) >> 16), (byte)((board & 0xff00) >> 8), (byte)((board & 0xff)),
                (byte)((card & 0xff000000) >> 24), (byte)((card& 0xff0000) >> 16), (byte)((card& 0xff00) >> 8), (byte)((card& 0xff)),
                (byte)((password& 0xff000000) >> 24), (byte)((password& 0xff0000)>>16), (byte)((password& 0xff00)>>8), (byte)(password& 0xff)
            };

            socket.Send(buffer);
            HandleData();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            HandleData();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string msg = "<xml><return_code><![CDATA[FAIL]]></return_code><return_msg><![CDATA[签名错误]]></return_msg><xml>";
            Match match = Regex.Match(msg, @"<return_msg><\!\[CDATA\[(\S+)\]\]></return_msg>");
            Console.WriteLine(match.Success + "    " + match.Groups[1].Value);
        }
    }
}