using SuperSocket.SocketBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 主板模拟程序2
{
    public partial class Form1 : Form
    {
        

        public Form1()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            var appServer = new AppServer();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            long time = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            
            var arr = new String[] { "q1gtb9mC8vmL6pYL", Convert.ToString(time), "12345678" }.OrderBy(z => z).ToArray();
            var arrString = string.Join("", arr);
            var sha1 = System.Security.Cryptography.SHA1.Create();
            var sha1Arr = sha1.ComputeHash(Encoding.UTF8.GetBytes(arrString));
            StringBuilder enText = new StringBuilder();
            foreach (var b in sha1Arr)
            {
                enText.AppendFormat("{0:x2}", b);
            }

            Console.WriteLine(time + ",  " + enText.ToString());
        }
    }
}
