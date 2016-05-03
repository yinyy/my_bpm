using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BPM.BoardListener
{
    public partial class Form1 : Form
    {
        private TcpListener listener;
        private Thread listenerThread;
        private bool isListen = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void miStart_Click(object sender, EventArgs e)
        {
            miStart.Enabled = false;
            miStop.Enabled = true;
            miQuit.Enabled = false;

            listenerThread = new Thread(() =>
            {
                isListen = true;

                listener = new TcpListener(new IPEndPoint(IPAddress.Parse(ConfigurationManager.AppSettings["ip"]), Convert.ToInt32(ConfigurationManager.AppSettings["port"])));
                listener.Start();

                try {
                    while (isListen)
                    {
                        Socket client = listener.AcceptSocket();
                        Task task = Task.Factory.StartNew(() =>
                        {

                        });
                    }
                }catch(Exception exp)
                {
                    Console.WriteLine(exp.Message);
                }
            });
            listenerThread.Start();
            Console.WriteLine("已经启用监听程序。");
        }

        private void miStop_Click(object sender, EventArgs e)
        {
            miStart.Enabled = true;
            miStop.Enabled = false;
            miQuit.Enabled = true;

            isListen = false;
            listener.Stop();
            listener = null;
            listenerThread = null;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            miStop.Enabled = false;
        }

        private void miQuit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
