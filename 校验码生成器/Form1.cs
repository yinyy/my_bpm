using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 校验码生成器
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string command = textBox1.Text.Trim();
            if(command==""){
                MessageBox.Show("请输入命令。");
                return ;
            }

            if(command.Length%2!=0){
                MessageBox.Show("命令格式错误。");
                return ;
            }

            byte sum = 0;
            for (int i = 0; i < command.Length; i += 2)
            {
                byte x = Convert.ToByte(command.Substring(i, 2), 16);
                sum += x;
            }

            sum = (byte)(~sum + 1);

            string cs = Convert.ToString(sum, 16);
            if (cs.Length == 1)
            {
                cs = "0" + cs;
            }

            cs = (command + cs).ToUpper();
            textBox2.Text = cs;

            //复制到剪贴板
            Clipboard.SetData(DataFormats.Text, cs);
        }
    }
}
