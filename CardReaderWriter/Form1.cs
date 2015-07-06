using Fleck;
using log4net;
using RFID.Public;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace CardReaderWriter
{
    public partial class Form1 : Form
    {
        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);
        delegate void DisplayOperationCallBack(string status, string epc, string data, string code);

        public Form1()
        {
            InitializeComponent();

            FetchSerialPorts();

            cbVersion.SelectedIndex = 1;
            cbBaud.SelectedIndex = 3;

            if (cbBaud.Items.Count > 0)
            {
                cbPort.SelectedIndex = 0;
            }
        }

        private void FetchSerialPorts()
        {
            cbPort.Items.Clear();
            foreach (String port in SerialPort.GetPortNames())
            {
                cbPort.Items.Add(port);
            }
        }

        private void OnMessage(IWebSocketConnection socket, string message)
        {
            JObject jobj = JObject.Parse(message);
            if (jobj["action"].ToString() == "write")
            {
                //处理写卡流程
                string data = jobj["data"].ToString().Trim();
                string start = jobj["start"].ToString().Trim();
                int men = Convert.ToInt32(jobj["mem"].ToString().Trim());

                string epcString;
                string value = RecognizeCard(out epcString);
                if (value.StartsWith("error"))
                {
                    log.Debug(value);

                    socket.Send(value);
                }
                else
                {
                    //把message编程16进制字符串
                    StringBuilder sb = new StringBuilder();
                    foreach (char c in data.ToCharArray())
                    {
                        sb.Append(Convert.ToString((byte)c, 16).PadLeft(2, '0'));
                    }

                    string hexdata = sb.ToString().ToUpper();
                    value = WriteData(epcString, men, start, hexdata);
                    if (value.StartsWith("error"))
                    {
                        log.Debug(string.Format("写卡失败，卡号：{0}，代码：{1}。", epcString, value));
                        DisplayOperation("写卡失败", epcString, "写卡失败", value);

                        socket.Send(value);
                    }
                    else
                    {
                        log.Debug(string.Format("写卡成功，卡号：{0}，数据：{1}。", epcString, hexdata));
                        DisplayOperation("写卡成功", epcString, hexdata, "");

                        socket.Send("success_" + hexdata);
                    }
                }
            }
            else if (jobj["action"].ToString() == "read")
            {
                //处理读卡流程
                string epcString;
                string tagString = RecognizeCard(out epcString);
                if (tagString.StartsWith("error"))
                {
                    log.Debug(tagString);

                    socket.Send(tagString);
                }
                else
                {
                    log.Debug(string.Format("读卡成功，卡号：{0}，数据：{1}。", epcString, epcString));
                    DisplayOperation("读卡成功", epcString, epcString, "");

                    socket.Send("success_" + epcString);

                    //int len = 0;
                    //string value = ReadDataLength(epcString, out len);
                    //if (value.StartsWith("error"))
                    //{
                    //    log.Debug(string.Format("读卡失败，卡号：{0}，代码：{1}。", epcString, value));
                    //    DisplayOperation("读卡失败", epcString, "", value);

                    //    socket.Send(value);
                    //}
                    //else
                    //{
                    //    value = ReadData(epcString, len);
                    //    if (value.StartsWith("error"))
                    //    {
                    //        log.Debug(string.Format("读卡失败，卡号：{0}，代码：{1}。", epcString, value));
                    //        DisplayOperation("读卡失败", epcString, "", value);

                    //        socket.Send(value);
                    //    }
                    //    else
                    //    {
                    //        log.Debug(string.Format("读卡成功，卡号：{0}，数据：{1}。", epcString, value));
                    //        DisplayOperation("读卡成功", epcString, value, "");

                    //        socket.Send("read_" + value);
                    //    }
                    //}
                }
            }
            else
            {
                //处理其它流程
                log.Debug(string.Format("其它操作，{0}。", message));
                DisplayOperation("其它操作", "", message, "");

                socket.Send(message);
            }
        }

        private void OnClosed(IWebSocketConnection socket)
        {
            log.Debug("关闭WebSocket。");
        }

        private void OnOpen(IWebSocketConnection socket)
        {
            log.Debug("打开WebSocket。");
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            WebSocketServer server = new WebSocketServer(string.Format("ws://0.0.0.0:{0}", txtPort.Text.Trim()));
            server.Start(socket =>
            {
                socket.OnOpen = () => OnOpen(socket);
                socket.OnClose = () => OnClosed(socket);
                socket.OnMessage = message => OnMessage(socket, message);
            });

            slWebSocket.Text = "WebSocket正在运行......";
            btnStart.Enabled = false;

            log.Debug("启动WebSocketServer。");
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            //如果是在正在进行循环操作时断开连接，则将循环标志置为false
            //单标签循环识别、防碰撞识别
            if (RFID.Public.DemoPublic.LoopEnable == true)
            {
                RFID.Public.DemoPublic.LoopEnable = false;
                System.Threading.Thread.Sleep(10);
                PublicFunction.Stop();
                System.Threading.Thread.Sleep(100);
            }

            //循环读
            if (DemoPublic.LoopRead == true)
            {
                System.Threading.Thread.Sleep(100);
            }

            //循环写
            if (DemoPublic.LoopWrite == true)
            {
                System.Threading.Thread.Sleep(100);
            }

            if (DemoPublic.EPCThread != null)
            {
                DemoPublic.EPCThread.Abort();
            }

            if (DemoPublic.UhfReaderDisconnect(ref RFID.Public.DemoPublic.hCom, 0))
            {
                RFID.Public.DemoPublic.Enabel_flg = false;

                slConnection.Text = "设备已经断开连接。";
                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;

                lblPower.Text = "";
                lblRate.Text = "";

                //OpenPort.Enabled = true;
                //OpenPort.Text = "连接";
                //ClosePort.Enabled = false;
                //tabControl1.Enabled = false;
                //BtSetPower.Enabled = false;
                //BtReadPower.Enabled = false;
                //BtSetFre.Enabled = false;
                //BtReadFre.Enabled = false;
                //if (ComboBox_AlreadyOpenCOM.Items.Count != 0)
                //{
                //    ComboBox_AlreadyOpenCOM.Items.RemoveAt(0);
                //}
                //connStatus = false;
                //this.TSM2.Enabled = false;
                //this.TSM3.Enabled = false;
            }
            else
            {
                slConnection.Text = "设备连接失败。";
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            RFID.Public.DemoPublic.JRM_version = cbVersion.Text.ToString();
            RFID.Public.DemoPublic.sPort = cbPort.Text.ToString();
            RFID.Public.DemoPublic.Baud = Int32.Parse(cbBaud.Text.ToString());
            RFID.Public.PublicFunction.SelectConnectBaud(RFID.Public.DemoPublic.Baud);

            if (DemoPublic.CheckVersion())
            {
                MessageBox.Show("请选择设备类型。");
                return;
            }
            if (DemoPublic.CheckCOM())
            {
                MessageBox.Show("请选择串口。");
                return;
            }
            if (DemoPublic.CheckBaud())
            {
                MessageBox.Show("请选择波特率。");
                return;
            }

            DemoPublic.UhfReaderDisconnect(ref RFID.Public.DemoPublic.hCom, 0);

            Cursor.Current = Cursors.WaitCursor;
            if (RFID.Public.PublicFunction.ConnectJRM())
            {
                Cursor.Current = Cursors.Default;

                System.Threading.Thread.Sleep(30);

                if (RFID.Public.PublicFunction.GetPower())
                {
                    lblPower.Text = (DemoPublic.Power).ToString() + "dBm";
                }
                else
                {
                    log.Debug("读取功率失败。");
                }

                System.Threading.Thread.Sleep(30);

                if (RFID.Public.PublicFunction.GetFrequency())
                {
                    lblRate.Text = DemoPublic.sFreI.ToString() + "." + DemoPublic.sFreD.ToString() + "-" + DemoPublic.eFreI.ToString() + "." + DemoPublic.eFreD.ToString() + "MHz";
                }
                else
                {
                    log.Debug("读取频率失败。");
                }


                //sLbFre.Text = "920.125Mhz - 924.875Mhz";
                //this.StatusBar1.Panels[0].Text = "读取工作频率成功";

                DemoPublic.selected = false;


                slConnection.Text = "已经连接，工作中......";
                btnDisconnect.Enabled = true;
                btnConnect.Enabled = false;

                //OpenPort.Text = "已连接";
                //ClosePort.Enabled = true;
                //OpenPort.Enabled = false;
                //tabControl1.Enabled = true;
                //connStatus = true;
                //BtSetPower.Enabled = true;
                //BtReadPower.Enabled = true;
                //BtSetFre.Enabled = true;
                //BtReadFre.Enabled = true;
                //this.StatusBar1.Panels[1].Text = this.ComboxCom.Text;
                //ComboBox_AlreadyOpenCOM.Items.Add(ComboxCom.Text);
                //ComboBox_AlreadyOpenCOM.SelectedIndex = ComboBox_AlreadyOpenCOM.SelectedIndex + 1;

                DemoPublic.Enabel_flg = true;
            }
            else
            {
                slConnection.Text = "设备连接失败。";
            }
        }

        private void DisplayOperation(string status, string epc, string data, string code)
        {
            if (this.lvOperation.InvokeRequired)
            {
                DisplayOperationCallBack docb = new DisplayOperationCallBack(DisplayOperation);
                this.Invoke(docb, new object[] { status, epc, data, code });
            }
            else
            {
                //txtOperation.Text = string.Format("[{0:yyyy-MM-dd HH:mm:ss}]{1}，{2}。{3}{4}", DateTime.Now, operation,message, Environment.NewLine, txtOperation.Text);
                ListViewItem lvi = new ListViewItem(string.Format("{0:yyyy年MM月dd日 HH:mm:ss}", DateTime.Now));
                lvi.SubItems.Add(status);
                lvi.SubItems.Add(epc);
                lvi.SubItems.Add(data);
                lvi.SubItems.Add(code);

                lvOperation.Items.Insert(0, lvi);
            }
        }

        private string RecognizeCard(out string epc)
        {
            epc = "";

            if (!DemoPublic.Enabel_flg)
            {
                //MessageBox.Show("请先进行连接。");
                return "error_connect";
            }

            ////在非“单步识别”的情况下，每次点击“识别标签”按钮时，标签数量置零并且清空数据窗口；
            ////在“单步识别”的情况下，如果识别不是“单步识别”则将标签数量置零并且清空数据窗口，否则标签数量不置零并且数据窗口不清空。
            //if (DemoPublic.OldInventoryFlg == false)
            //{
            //    DemoPublic.TagNum = 0;
            //}

            int startAddress = 0x01;
            int readLength = 7;

            PublicFunction.addr = Convert.ToByte(startAddress);
            PublicFunction.len = Convert.ToByte(readLength);
            int bbac = PublicFunction.len * 4;

            //单步识别
            DemoPublic.OldInventoryFlg = true;

            byte[] buiilen = new byte[1];
            byte[] buii = new byte[255];
            string tagString = "";
            string pcString = "";
            string epcString = "";
            int rssi = 0;
            int dcsb = PublicFunction.InventorySingle(buiilen, buii);

            if (dcsb != 0)
            {
                int aaa = dcsb * 2;
                tagString = PublicFunction.ByteArrayToHexString(buii);
                tagString = tagString.Substring(0, aaa);

                pcString = tagString.Substring(0, 4);

                try { epcString = tagString.Substring(4, Convert.ToInt32(PublicFunction.len) * 4); }
                catch { epcString = tagString.Substring(4, tagString.Length - 6); }

                epc = epcString;

                rssi = Convert.ToInt16(tagString.Substring(tagString.Length - 2, 2), 16);
                if (rssi > 127)
                {
                    rssi = -((-rssi) & 0xff);
                }

                log.Debug(string.Format("读到RFID卡：PC号：{0}，EPC标签号：{1}，RSSI：{2}dBm，完整标签号：{3}。", pcString, epcString, rssi, tagString));
            }
            else
            {
                log.Debug("读卡错误。");

                return "error_recognize";
            }

            return tagString;
        }


        private string WriteData(string epc, int mem, string start, string data)
        {
            if (!DemoPublic.Enabel_flg)
            {
                //MessageBox.Show("请先进行连接");
                return "error_connect";
            }

            string hexdata = data;
            if (hexdata.Length % 4 != 0)
            {
                hexdata += "00";
            }

            DemoPublic.sPwd = "00000000";
            DemoPublic.sTag = epc;
            DemoPublic.sAddress = start;
            DemoPublic.sCnt = Convert.ToString(hexdata.Length / 4);

            DemoPublic.sData = hexdata;

            DemoPublic.bBank = (byte)mem;

            byte Pc_len = (byte)(DemoPublic.sTag.Length / 2);
            byte[] bAdd = new byte[2];

            if (Convert.ToUInt16(DemoPublic.sAddress, 10) > 127)
            {
                bAdd[0] = (byte)((Convert.ToUInt16(DemoPublic.sAddress, 10) >> 7) | 0x80);
                bAdd[1] = (byte)((Convert.ToUInt16(DemoPublic.sAddress, 10) >> 7) & 0x7F);
                Pc_len = (byte)(Pc_len + 1);
            }
            else
            {
                bAdd[0] = (byte)(Convert.ToUInt16(DemoPublic.sAddress, 10) >> 7);
            }

            Pc_len += (byte)(DemoPublic.sData.Length / 2 + 9);
            byte cuowu = new byte();
            byte[] aWriteData = new byte[255];
            aWriteData = HexStringToByteArray(hexdata);

            try
            {
                int aaag = PublicFunction.JRMWriteDataByxzEPC(epc, DemoPublic.sPwd, DemoPublic.bBank, DemoPublic.sAddress, byte.Parse(DemoPublic.sCnt), aWriteData, cuowu);
                if (aaag != 0)
                {
                    return epc;
                }
                else
                {
                    return "error_write";
                }
            }
            catch (Exception e)
            {
                return "error_" + e.Message;
            }
        }

        //private string ReadDataLength(string epc, out int len)
        //{
        //    len = 0;

        //    if (!DemoPublic.Enabel_flg)
        //    {
        //        //MessageBox.Show("请先进行连接");
        //        return "error_connect";
        //    }

        //    DemoPublic.sPwd = "00000000";
        //    PublicFunction.addr = Convert.ToByte("00");
        //    PublicFunction.len = Convert.ToByte("1");

        //    DemoPublic.TagNum = 0;

        //    DemoPublic.sTag = epc;
        //    DemoPublic.sAddress = "00";
        //    DemoPublic.sCnt = "1";
        //    DemoPublic.bBank = 3;

        //    byte Pc_len = (byte)(DemoPublic.sTag.Length / 2);
        //    byte[] bAdd = new byte[2];
        //    byte[] readersj = new byte[255];
        //    byte cuowu = 0;

        //    if (Convert.ToUInt16(DemoPublic.sAddress, 10) > 127)
        //    {
        //        bAdd[0] = (byte)((Convert.ToUInt16(DemoPublic.sAddress, 10) >> 7) | 0x80);
        //        bAdd[1] = (byte)((Convert.ToUInt16(DemoPublic.sAddress, 10) >> 7) & 0x7F);
        //        Pc_len = (byte)(Pc_len + 1);
        //    }
        //    else
        //    {
        //        bAdd[0] = (byte)(Convert.ToUInt16(DemoPublic.sAddress, 10) >> 7);
        //    }

        //    try
        //    {
        //        int aaag = PublicFunction.ReadxzData(epc, DemoPublic.sPwd, DemoPublic.bBank, int.Parse(DemoPublic.sAddress), int.Parse(DemoPublic.sCnt), readersj, cuowu);
        //        if (readersj[1] != 0x00)
        //        {
        //            return "error_length";
        //        }

        //        len = readersj[0];
        //        return "success";
        //    }
        //    catch (Exception e)
        //    {
        //        return "error_" + e.Message;
        //    }
        //}

        //private string ReadData(string epc, int len)
        //{
        //    if (!DemoPublic.Enabel_flg)
        //    {
        //        //MessageBox.Show("请先进行连接");
        //        return "error_connect";
        //    }

        //    DemoPublic.sPwd = "00000000";
        //    PublicFunction.addr = Convert.ToByte("01");
        //    PublicFunction.len = Convert.ToByte(len);

        //    DemoPublic.TagNum = 0;

        //    DemoPublic.sTag = epc;
        //    DemoPublic.sAddress = "01";
        //    DemoPublic.sCnt = Convert.ToString(len);
        //    DemoPublic.bBank = 3;

        //    byte Pc_len = (byte)(DemoPublic.sTag.Length / 2);
        //    byte[] bAdd = new byte[2];
        //    byte[] readersj = new byte[255];
        //    byte cuowu = 0;

        //    if (Convert.ToUInt16(DemoPublic.sAddress, 10) > 127)
        //    {
        //        bAdd[0] = (byte)((Convert.ToUInt16(DemoPublic.sAddress, 10) >> 7) | 0x80);
        //        bAdd[1] = (byte)((Convert.ToUInt16(DemoPublic.sAddress, 10) >> 7) & 0x7F);
        //        Pc_len = (byte)(Pc_len + 1);
        //    }
        //    else
        //    {
        //        bAdd[0] = (byte)(Convert.ToUInt16(DemoPublic.sAddress, 10) >> 7);
        //    }

        //    try
        //    {
        //        int aaag = PublicFunction.ReadxzData(epc, DemoPublic.sPwd, DemoPublic.bBank, int.Parse(DemoPublic.sAddress), int.Parse(DemoPublic.sCnt), readersj, cuowu);
        //        int rfdi2 = aaag * 2;
        //        string uii_str = PublicFunction.ByteArrayToHexString(readersj);
        //        uii_str = uii_str.Substring(0, rfdi2);

        //        StringBuilder sb = new StringBuilder();
        //        for (int i = 0; i < uii_str.Length; i += 2)
        //        {
        //            int x = Convert.ToInt16(uii_str.Substring(i, 2), 16);
        //            if (x == 0)
        //            {
        //                break;
        //            }

        //            sb.Append((char)x);
        //        }

        //        string s = sb.ToString().Trim();
        //        if (s.Length == 0)
        //        {
        //            return "error_length";
        //        }

        //        return sb.ToString().Trim();
        //    }
        //    catch(Exception e)
        //    {
        //        return "error_" + e.Message;
        //    }
        //}

        private byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }

        private void btnLogger_Click(object sender, EventArgs e)
        {
            string logfile = Environment.CurrentDirectory + "\\log4netfile.txt";
            System.Diagnostics.Process.Start("notepad.exe ", logfile);
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            string epcString;
            string tagString = RecognizeCard(out epcString);
            if (tagString.StartsWith("error"))
            {
                MessageBox.Show("发生错误。");
            }
            else
            {
                //string value = ReadData(epcString);
                //MessageBox.Show(value.Substring(value.IndexOf('_') + 1));
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
            }
            else
            {
                notifyIcon1.Visible = false;
            }
        }

        private void tsmiShow_Click(object sender, EventArgs e)
        {
            Visible = true;
            WindowState = FormWindowState.Normal;
        }

        private void tsmiClose_Click(object sender, EventArgs e)
        {
            btnDisconnect_Click(null, null);
            Close();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                tsmiShow_Click(null, null);
            }
        }
    }
}
