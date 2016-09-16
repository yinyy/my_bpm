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

    
    public partial class Form1 : Form,IPresenter
    {
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
        private List<BoardListenerThread> listenerThreads = new List<BoardListenerThread>();

        public bool AutoStart { get; set; }

        public Form1()
        {
            InitializeComponent();

            log = LogManager.GetLogger(this.GetType());
        }
        
        private void miStart_Click(object sender, EventArgs e)
        {
            miStart.Enabled = false;
            miStop.Enabled = true;
            miQuit.Enabled = false;

            ClearStatusText();

            string local = ConfigurationManager.AppSettings["local"];
            string values = ConfigurationManager.AppSettings["departments"];
            bool syncHeartBeat = false, showHeartBeat = false;
            int heartBeatThreshold = 10;

            try
            {
                showHeartBeat = Convert.ToBoolean(ConfigurationManager.AppSettings["show_heartbeat"]);
                syncHeartBeat = Convert.ToBoolean(ConfigurationManager.AppSettings["save_heartbeat"]);
                heartBeatThreshold = Convert.ToInt32(ConfigurationManager.AppSettings["heartbeat_threshold"]);
            }
            catch(Exception ee)
            {
                PrintDebug("获取配置参数错误。", true);
            }
            
            foreach(string ps in values.Split(';'))
            {
                string[] p = ps.Split(',');
                BoardListenerThread t = new BoardListenerThread(this, local, p[2], Convert.ToInt32(p[1]), Convert.ToInt32(p[0]), showHeartBeat, syncHeartBeat, heartBeatThreshold);
                listenerThreads.Add(t);
                t.Start();
            }
        }

        private void miStop_Click(object sender, EventArgs e)
        {
            miStart.Enabled = true;
            miStop.Enabled = false;
            miQuit.Enabled = true;

            foreach(BoardListenerThread t in listenerThreads)
            {
                t.Stop();
            }
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

        delegate void AddDeviceDelegate(string serial, string board, string ip, string department, string address);
        public void AddDevice(string serial, string board, string ip, string department, string address)
        {
            if (deviceList.InvokeRequired)
            {
                AddDeviceDelegate d = new AddDeviceDelegate(AddDevice);
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
                lvi.SubItems.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
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

        delegate void RemoveDeviceDelegate(string ip);
        public void RemoveDevice(string ip)
        {
            if (deviceList.InvokeRequired)
            {
                RemoveDeviceDelegate d = new RemoveDeviceDelegate(RemoveDevice);
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

        delegate void UpdateDeviceDelegate(string ip);
        public void UpdateDevice(string ip)
        {
            if (deviceList.InvokeRequired)
            {
                RemoveDeviceDelegate d = new RemoveDeviceDelegate(UpdateDevice);
                Invoke(d, new object[] { ip });
            }
            else {
                for (int i = deviceList.Items.Count - 1; i >= 0; i--)
                {
                    if (deviceList.Items[i].SubItems[3].Text == ip)
                    {
                        deviceList.Items[i].SubItems[4].Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        break;
                    }
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

            //dynamic dobj = new { Success = true, Message = "dfadsfasdfasdfasdf" };
            //Console.WriteLine(dobj.Success);
        }

        private void miCurrentFolder_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", Application.StartupPath);
        }

        delegate void LogDelegate(string message, bool saveLog);
        public void PrintDebug(string message, bool saveLog)
        {
            if (rtbStatus.InvokeRequired)
            {
                LogDelegate d = new LogDelegate(PrintDebug);
                Invoke(d, new object[] { message, saveLog });
            }
            else {
                if (rtbStatus.Lines.Count() >= Convert.ToInt32(ConfigurationManager.AppSettings["lines"]))
                {
                    rtbStatus.Clear();
                }

                rtbStatus.AppendText("\r\n" + string.Format("[{0:yyyy/MM/dd HH:mm:ss:sss}]{1}", DateTime.Now, message));
                rtbStatus.ScrollToCaret();

                if (saveLog)
                {
                    log.Debug(message);
                }
            }
        }
    }
}