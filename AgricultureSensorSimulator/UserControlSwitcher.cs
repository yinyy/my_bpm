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
    public partial class UserControlSwitcher : UserControl
    {
        public delegate void On_SendMessageDelegate(string message);
        public event On_SendMessageDelegate On_SendMessage;

        public UserControlSwitcher()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string message = textBox1.Text.Trim();
            message += "02";
            message += ((int)numericUpDown1.Value).ToString("x2");

            int[] values = { checkBox1.Checked ? 1 : 0, checkBox2.Checked ? 1 : 0, checkBox3.Checked ? 1 : 0, checkBox4.Checked ? 1 : 0,
            checkBox5.Checked?1:0, checkBox6.Checked?1:0, checkBox7.Checked?1:0, checkBox8.Checked?1:0};

            int value = 0;
            for(int i = 0; i < numericUpDown1.Value; i++)
            {
                value |= values[i] << i;
            }

            message += value.ToString("x2");

            On_SendMessage(message);
        }
    }
}
