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
    public partial class UserControlTemperatureHumidity : UserControl
    {
        public delegate void On_SendMessageDelegate(string message);
        public event On_SendMessageDelegate On_SendMessage;

        public UserControlTemperatureHumidity()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string message = textBox1.Text.Trim();
            message += "03";

            int temperature = (int)(Convert.ToSingle(textBox2.Text.Trim()) * 10);
            int humidity = Convert.ToInt16(textBox3.Text.Trim());

            message += ((((temperature & 0xff00) >> 8) + 256) % 256).ToString("x2");
            message += (((temperature & 0xff) + 256) % 256).ToString("x2");
            message += (((humidity & 0xff) + 256) % 256).ToString("x2");

            On_SendMessage(message);
        }
    }
}
