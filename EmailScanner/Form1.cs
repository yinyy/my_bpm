using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net.Mail;

namespace EmailScanner
{
    public partial class Form1 : Form
    {
        private string log = System.Environment.CurrentDirectory + @"\log.txt";

        public Form1()
        {
            InitializeComponent();

            if (!File.Exists(log))
            {
                using (File.CreateText(log)) { }
            }
            
            scanning();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            scanning();
        }

        private void scanning()
        {
            using (StreamWriter sw = new StreamWriter(File.Open(log, FileMode.Append, FileAccess.Write)))
            {
                try
                {
                    DateTime now = DateTime.Now;
                    string nowstr = now.ToString("yyyyMMddHH:mm");

                    //sw.WriteLine(string.Format("[{0:yyyy-MM-dd HH:mm:ss}] 开始。", DateTime.Now));

                    DataClasses1DataContext db = new DataClasses1DataContext();

                    var q = (from aR in db.Logistics_Inquiry
                             where aR.SendNotification == null
                             join aD in db.Sys_Dics on aR.Inquirer equals aD.KeyId
                             orderby aR.Ended descending
                             select new { InquiryId = aR.KeyId, Time = aR.Ended, Inquirer = aD.Title, Email = aD.Remark, Created = aR.Published, Port=aR.Port, Cargo=aR.Cargo, Amount=aR.Amount }).ToArray();

                    taskList.Items.Clear();

                    int count = 1;
                    foreach (var i in q)
                    {
                        ListViewItem lvi = new ListViewItem(Convert.ToString(count++));
                        lvi.SubItems.Add(i.Time.ToString("yyyy年MM月dd日 HH:mm"));
                        lvi.SubItems.Add(i.Inquirer);
                        lvi.SubItems.Add(i.Email);

                        taskList.Items.Add(lvi);
                    }

                    #region 发送邮件
                    foreach (var i in q)
                    {
                        string t = i.Time.ToString("yyyyMMddHH:mm");
                        if (nowstr.CompareTo(t)>0)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append(string.Format("【{0:yyyy年MM月dd日 HH:mm:ss}】发布的询盘已经完成。询盘详细信息如下：<br/>", i.Created));
                            sb.Append(string.Format("询价人：{0}，目的港：{1}，柜数：{2}，货物名称：{3}。<br/><br/>", i.Inquirer, i.Port, i.Amount, i.Cargo));

                            sb.Append("报价详细信息如下：<br/>");
                            count = 1;
                            //var qq = from aR in db.Logistics_Feedback
                            //         where aR.InquiryId == i.InquiryId && aR.Price > 0
                            //         join aU in db.Sys_Users on aR.SupplyId equals aU.KeyId
                            //         join aD in db.Sys_Departments on aU.DepartmentId equals aD.KeyId
                            //         orderby aR.Price ascending
                            //         select new { Title = aD.DepartmentName, Price = aR.Price, Ship = aR.Ship, ETD = aR.ETD, ETA = aR.ETA, Memo = aR.Memo };
                            var qq = from aR in db.V_Inquiry_Quoted
                                     where aR.InquiryId == i.InquiryId
                                     join aU in db.Sys_Users on aR.SupplyId equals aU.KeyId
                                     join aD in db.Sys_Departments on aU.DepartmentId equals aD.KeyId
                                     orderby aR.NewIndex ascending
                                     select new { Title = aD.DepartmentName, Price = aR.Price, Ship = aR.Ship, ETD = aR.ETD, ETA = aR.ETA, Memo = aR.Memo };
                            foreach (var p in qq)
                            {
                                sb.Append(string.Format("{0}、报价公司：{1}，价格：{2}，船公司：{3}，ETD：{4}，ETA：{5}，备注：{6}。<br/>", count++, p.Title, p.Price, p.Ship, p.ETD, p.ETA, p.Memo));
                            }

                            sw.WriteLine(string.Format("[{0:yyyy-MM-dd HH:mm:ss}] 发邮件给{1}。", DateTime.Now, i.Email));

                            #region 发送邮件
                            MailAddress mfrom = new MailAddress(ConfigurationManager.AppSettings["email"], "石大胜华");
                            MailMessage mail = new MailMessage();

                            mail.From = mfrom;
                            mail.Subject = "询盘已完成。";
                            mail.Body = sb.ToString();
                            mail.IsBodyHtml = true;
                            mail.BodyEncoding = System.Text.Encoding.UTF8;

                            MailAddress tma = new MailAddress(i.Email, i.Inquirer);
                            mail.Bcc.Add(tma);

                            mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;

                            SmtpClient client = new SmtpClient();
                            client.Host = ConfigurationManager.AppSettings["smtp"];
                            client.UseDefaultCredentials = false;
                            client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["email"], ConfigurationManager.AppSettings["password"]);
                            client.DeliveryMethod = SmtpDeliveryMethod.Network;
                            client.SendCompleted += delegate(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
                            {
                                sw.WriteLine(string.Format("[{0:yyyy-MM-dd HH:mm:ss}] 发邮件给成功。", DateTime.Now));
                            };

                            //client.SendAsync(mail, this);
                            client.Send(mail);
                            #endregion

                            //更新数据库
                            Logistics_Inquiry inq = (from aR in db.Logistics_Inquiry
                                                     where aR.KeyId == i.InquiryId
                                                     select aR).FirstOrDefault();
                            inq.SendNotification = 1;
                            db.SubmitChanges();
                        }
                    }
                    #endregion

                    toolStripStatusLabel1.Text = string.Format("{0:yyyy年MM月dd日 HH:mm:ss} 更新。", now);
                }
                catch (Exception ee)
                {
                    toolStripStatusLabel1.Text = ee.Message;

                    sw.WriteLine(string.Format("[{0:yyyy-MM-dd HH:mm:ss}] {1}。", DateTime.Now, ee.Message));
                }
            }
        }

        private void 退出QToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认退出发件发送程序吗？", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                this.Close();
            }
        }

        private void 设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings sts = new Settings();
            sts.ShowDialog(this);
        }

        private void 打开日志ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("notepad.exe ", log);
        }
    }
}