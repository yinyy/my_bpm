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
    public partial class UserControlFineParticulateMatter : UserControl
    {
        public delegate void On_SendMessageDelegate(string message);
        public event On_SendMessageDelegate On_SendMessage;

        private void button1_Click(object sender, EventArgs e)
        {
            string message = textBox1.Text.Trim();
            message += "05";

            int potency = Convert.ToInt16(textBox3.Text.Trim());
            message += ((((potency & 0xff00) >> 8) + 256) % 256).ToString("x2");
            message+= (((potency & 0xff) + 256) % 256).ToString("x2");
            
            On_SendMessage(message);
        }

        public UserControlFineParticulateMatter()
        {
            InitializeComponent();
        }
    }
}
