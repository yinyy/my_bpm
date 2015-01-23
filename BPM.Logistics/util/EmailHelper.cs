using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace BPM.Logistics.Util
{
    public class EmailHelper
    {
        public static bool SendEmail(string from, string password, string smtp, Dictionary<string, string> tos, string subject, string body)
        {
            MailAddress mfrom = new MailAddress(from, "石大胜华");
            MailMessage mail = new MailMessage();

            mail.From = mfrom;
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            mail.BodyEncoding = System.Text.Encoding.UTF8;

            foreach (string to in tos.Keys)
            {
                if (string.IsNullOrWhiteSpace(to))
                {
                    continue;
                }

                MailAddress t = new MailAddress(to, tos[to]);
                //mail.To.Add(t);
                mail.Bcc.Add(t);
            }

            mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;

            SmtpClient client = new SmtpClient();
            client.Host = smtp;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(from, password);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.SendCompleted += client_SendCompleted;

            //client.SendAsync(mail, "test");
            try
            {
                client.SendAsync(mail, "test");
                //client.Send(mail);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return true;
        }

        public static bool SendEmail(string from, string password, string smtp, string to, string name, string subject, string body)
        {
            MailAddress mfrom = new MailAddress(from, "石大胜华");
            MailMessage mail = new MailMessage();

            mail.From = mfrom;
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            mail.BodyEncoding = System.Text.Encoding.UTF8;

            MailAddress t = new MailAddress(to, name);
            //mail.To.Add(t);
            mail.Bcc.Add(t);

            mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;

            SmtpClient client = new SmtpClient();
            client.Host = smtp;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(from, password);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.SendCompleted += client_SendCompleted;

            //client.SendAsync(mail, "test");
            try
            {
                //client.Send(mail);
                client.SendAsync(mail, "test");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return true;
        }

        public static void client_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Console.WriteLine("发送完成！");
        }
    }
}