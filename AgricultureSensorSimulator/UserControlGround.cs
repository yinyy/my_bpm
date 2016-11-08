using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AgricultureSensorSimulator
{
    public partial class UserControlGround : UserControl
    {
        public delegate void On_SendMessageDelegate(string message);
        public event On_SendMessageDelegate On_SendMessage;

        public UserControlGround()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string message = textBox1.Text.Trim();
            message += "04";

            int humidity = Convert.ToInt16(textBox3.Text.Trim());
            
            message += (((humidity & 0xff) + 256) % 256).ToString("x2");

            On_SendMessage(message);
        }
    }
}
