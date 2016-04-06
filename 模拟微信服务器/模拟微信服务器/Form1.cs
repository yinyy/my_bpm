using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace 模拟微信服务器
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            rtbResponse.Text = "";

            string url = string.Format("http://localhost:9582/PublicPlatform/DefaultHandler.ashx");
            //string url = string.Format("http://139.129.43.203//PublicPlatform/DefaultHandler.ashx");

            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Method = "POST";
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.UTF8))
                {
                    writer.Write(rtbMessage.Text);
                    writer.Flush();
                }

                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        rtbResponse.Text = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ee)
            {
                rtbResponse.Text = ee.Message;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            rtbMessage.Text = "<xml><ToUserName><![CDATA[gh_e94577a0a594]]></ToUserName> <FromUserName><![CDATA[oIC6ejjI1izi372jxZ0R7LnD8p5o]]></FromUserName> <CreateTime>1457182986</CreateTime> <MsgType><![CDATA[event]]></MsgType> <Event><![CDATA[CLICK]]></Event> <EventKey><![CDATA[m121_card]]></EventKey> </xml>";
            btnSend_Click(null, null);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            rtbMessage.Text = "<xml><ToUserName><![CDATA[gh_e94577a0a594]]></ToUserName> <FromUserName><![CDATA[oIC6ejjI1izi372jxZ0R7LnD8p5o]]></FromUserName> <CreateTime>1457187279</CreateTime> <MsgType><![CDATA[event]]></MsgType> <Event><![CDATA[subscribe]]></Event> <EventKey><![CDATA[]]></EventKey> </xml>";
            btnSend_Click(null, null);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            rtbMessage.Text = "<xml><ToUserName><![CDATA[gh_e94577a0a594]]></ToUserName> <FromUserName><![CDATA[oIC6ejjI1izi372jxZ0R7LnD8p5o]]></FromUserName> <CreateTime>1457225307</CreateTime> <MsgType><![CDATA[event]]></MsgType> <Event><![CDATA[subscribe]]></Event> <EventKey><![CDATA[channel69]]></EventKey> <Ticket><![CDATA[gQFB8ToAAAAAAAAAASxodHRwOi8vd2VpeGluLnFxLmNvbS9xLzBIVUExalRtQ0JseHZGMXpCVjNrAAIEDbjBVgMEAAAAAA==]]></Ticket> </xml>";
            btnSend_Click(null, null);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            rtbMessage.Text = "<xml><ToUserName><![CDATA[gh_e94577a0a594]]></ToUserName> <FromUserName><![CDATA[oIC6ejjI1izi372jxZ0R7LnD8p5o]]></FromUserName> <CreateTime>1457225307</CreateTime> <MsgType><![CDATA[event]]></MsgType> <Event><![CDATA[subscribe]]></Event> <EventKey><![CDATA[consume14]]></EventKey> <Ticket><![CDATA[gQFB8ToAAAAAAAAAASxodHRwOi8vd2VpeGluLnFxLmNvbS9xLzBIVUExalRtQ0JseHZGMXpCVjNrAAIEDbjBVgMEAAAAAA==]]></Ticket> </xml>";
            btnSend_Click(null, null);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            rtbMessage.Text = "<xml><ToUserName><![CDATA[gh_e94577a0a594]]></ToUserName><FromUserName><![CDATA[oIC6ejjI1izi372jxZ0R7LnD8p5o]]></FromUserName><CreateTime>1458982587</CreateTime><MsgType><![CDATA[event]]></MsgType><Event><![CDATA[scancode_waitmsg]]></Event><EventKey><![CDATA[m111_wash_car]]></EventKey><ScanCodeInfo><ScanType><![CDATA[qrcode]]></ScanType><ScanResult><![CDATA[as-asd123-2341234123-adaf]]></ScanResult></ScanCodeInfo></xml>";
            btnSend_Click(null, null);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            rtbMessage.Text = string.Format("{{\"UnionId\":\"{0}\",\"OpenId\":\"{1}\",\"Money\":5,\"DeviceSerial\":\"{2}\",\"Payment\":\"scanqrcode\"}}", "oIn5Vw4nqPV_YQnOwKHDJBu4r4Lg", "oIC6ejjI1izi372jxZ0R7LnD8p5o", "as-asd123-2341234123-adaf");
            button8_Click(null, null);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            rtbMessage.Text = "putcoin";
            button8_Click(null, null);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            rtbResponse.Text = "";

            string url = string.Format("http://localhost:9582/Pay/DefaultHandler.ashx");
            //string url = string.Format("http://139.129.43.203//PublicPlatform/DefaultHandler.ashx");

            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Method = "POST";
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.UTF8))
                {
                    writer.Write(rtbMessage.Text);
                    writer.Flush();
                }

                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        rtbResponse.Text = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ee)
            {
                rtbResponse.Text = ee.Message;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {

        }
    }
}