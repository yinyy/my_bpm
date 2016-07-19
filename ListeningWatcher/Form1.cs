using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ListeningWatcher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            checkProcess();
        }

        private void checkProcess()
        {
            Process[] process = Process.GetProcessesByName("BPM.BoardListener");
            if (process.Length == 0)
            {
                Process.Start(tbProgram.Text.Trim(), " autostart ");
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (btnStart.Text == "开始")
            {
                timer1.Enabled = true;
                btnStart.Text = "结束";
            }
            else
            {
                timer1.Enabled = false;
                btnStart.Text = "开始";
            }
        }

        private void btnProgram_Click(object sender, EventArgs e)
        {

        }
    }
}
