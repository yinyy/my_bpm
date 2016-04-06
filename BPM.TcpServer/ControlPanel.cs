using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BPM.TcpServer
{
    public partial class ControlPanel : Form
    {
        private TcpListener tcpListener=null;
        private Thread tcpThread = null;
        private int tcpPort;
        private HttpListener httpListener= null;
        private Thread httpThread = null;
        private string urlPrefix;


        public ControlPanel()
        {
            InitializeComponent();

            tcpPort = Convert.ToInt32(ConfigurationManager.AppSettings["port"]);
            urlPrefix = ConfigurationManager.AppSettings["url_prefix"];
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (btnStart.Text == "启动")
            {
                btnStart.Text = "停止";

                IPAddress address = new IPAddress(new byte[] { 172, 16, 46, 100 });
                tcpListener = new TcpListener(address, tcpPort);
                tcpListener.Start();

                tcpThread = new Thread(() =>
                {
                    while (true)
                    {
                        if (tcpListener == null)
                        {
                            break;
                        }

                        try
                        {
                            TcpClient client = tcpListener.AcceptTcpClient();
                            client.GetStream().Write(new byte[] { (byte)'a', (byte)'b', (byte)'c', (byte)'d', (byte)'e' }, 0, 5);
                            client.GetStream().Flush();
                            client.Close();

                            Console.WriteLine("finished");
                        }
                        catch (Exception exp)
                        {
                            Console.WriteLine(exp.Message);
                        }
                    }
                    Console.WriteLine("Tcp Over");
                });
                tcpThread.Start();

                //netsh http add urlacl url=http://+:7134/ user=Tester
                //netsh http delete urlacl url=http://+:7134/
                httpListener = new HttpListener();
                httpListener.Prefixes.Add(urlPrefix);
                httpListener.Start();

                httpThread = new Thread(() =>
                {
                    while (true)
                    {
                        if (httpListener == null)
                        {
                            break;
                        }

                        try
                        {
                            HttpListenerContext context = httpListener.GetContext();
                            HttpListenerResponse response = context.Response;
                            StreamWriter writer = new StreamWriter(response.OutputStream);

                            writer.Write(JsonConvert.SerializeObject(new { Success = true, Message = "测试效果怎么样！" }));
                            writer.Flush();
                            writer.Close();

                            response.Close();
                            
                            Console.WriteLine("One Request");
                        }catch(Exception exp)
                        {
                            Console.WriteLine(exp.Message);
                        }
                    }

                    Console.WriteLine("Http Over");
                });
                httpThread.Start();
            }
            else
            {
                btnStart.Text = "启动";

                CloseServer();
            }
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            CloseServer();

            Close();
        }

        private void CloseServer()
        {
            if (tcpListener != null)
            {
                tcpListener.Stop();
                tcpListener = null;
            }

            if (httpListener != null)
            {
                httpListener.Close();
                httpListener = null;
            }
        }
    }
}
