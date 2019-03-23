using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SanitationServer2
{
    public partial class Form1 : Form
    {
        private string lastRecord = "";
        private int recordCount = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (btnConnect.Text == "连接")
            {
                if (serialPort1.IsOpen)
                {
                    serialPort1.Close();
                    serialPort1.DataReceived -= SerialPort1_DataReceived;
                }

                serialPort1.PortName = tbPort.Text.Trim();
                serialPort1.BaudRate = Convert.ToInt32(tbBaud.Text.Trim());
                serialPort1.Parity = System.IO.Ports.Parity.None;
                serialPort1.DataBits = 8;
                serialPort1.StopBits = System.IO.Ports.StopBits.One;
                serialPort1.DataReceived += SerialPort1_DataReceived;
                serialPort1.ReceivedBytesThreshold = 26;
                serialPort1.ReadTimeout = 200;

                serialPort1.Open();

                btnConnect.Text = "断开";
            }
            else
            {
                serialPort1.Close();
                serialPort1.DataReceived -= SerialPort1_DataReceived;

                btnConnect.Text = "连接";
            }
        }

        private void SerialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            byte[] buffer = new byte[serialPort1.BytesToRead];
            serialPort1.Read(buffer, 0, buffer.Length);

            if ((buffer[0] == 0xaa) && (buffer[buffer.Length - 1] == 0xbb))
            {
                #region 把分析数据的任务放到线程池去做
                ThreadPool.QueueUserWorkItem(new WaitCallback((obj) =>
                {
                    try
                    {
                        DateTime time = new DateTime(2000 + ((((buffer[1] + 256) % 256) << 8) + ((buffer[2] + 256) % 256)),
                            (((buffer[3] + 256) % 256) << 8) + ((buffer[4] + 256) % 256),
                            (((buffer[5] + 256) % 256) << 8) + ((buffer[6] + 256) % 256),
                            (((buffer[7] + 256) % 256) << 8) + ((buffer[8] + 256) % 256),
                            (((buffer[9] + 256) % 256) << 8) + ((buffer[10] + 256) % 256),
                            0);

                        string trunk = string.Format("{0}{1}{2}{3}{4}{5}",
                            (char)buffer[11], (char)buffer[12], (char)buffer[13], (char)buffer[14], (char)buffer[15], (char)buffer[16]);

                        float water1 = ((((buffer[17] + 256) % 256) << 8) + ((buffer[18] + 256) % 256)) / 100f;
                        float water2 = ((((buffer[19] + 256) % 256) << 8) + ((buffer[20] + 256) % 256)) / 10f;

                        string driver = string.Format("{0}{1}{2}{3}",
                            (char)buffer[21], (char)buffer[22], (char)buffer[23], (char)buffer[24]);


                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = time.ToString("yyyy/MM/dd HH:mm:ss");
                        lvi.SubItems.Add(driver);
                        lvi.SubItems.Add(trunk);
                        lvi.SubItems.Add(water1 + "");
                        lvi.SubItems.Add(water2 + "");

                        lock (lastRecord)
                        {
                            recordCount += 1;
                            SetRecordCount();

                            string current = string.Format("{0:yyyyMMddHHmmss}|{1}|{2}|{3}|{4}", time, driver, trunk, water1, water2);
                            if (cbRepeat.Checked || lastRecord != current)
                            {
                                lastRecord = current;
                                AddItemToListView(lvi);

                                #region 数据保存到数据库

                                #endregion
                            }
                        }                        
                    }
                    catch
                    {

                    }
                }));
                #endregion
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lock (lastRecord)
            {
                lvData.Items.Clear();
                lastRecord = "";
                recordCount = 0;

                SetRecordCount();
            }
        }

        delegate void AddItemToListViewCallback(ListViewItem item);
        private void AddItemToListView(ListViewItem item)
        {
            if (this.lvData.InvokeRequired)
            {
                AddItemToListViewCallback cb = new AddItemToListViewCallback(AddItemToListView);
                this.Invoke(cb, new object[] { item });
            }
            else
            {
                this.lvData.Items.Insert(0, item);
            }
        }

        delegate void SetRecordCountCallback();
        private void SetRecordCount()
        {
            if (this.statusStrip1.InvokeRequired)
            {
                SetRecordCountCallback cb = new SetRecordCountCallback(SetRecordCount);
                this.Invoke(cb);
            }
            else
            {
                this.tssbCount.Text = recordCount + "";
            }
        }
    }
}
