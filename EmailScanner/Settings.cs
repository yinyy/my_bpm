using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace EmailScanner
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            #region 获取当前的参数配置
            txtEmail.Text = ConfigurationManager.AppSettings["email"];
            txtPassword.Text =StringToolkit.DecryptDES(ConfigurationManager.AppSettings["password"], "yinyy_danis");
            txtSmtp.Text = ConfigurationManager.AppSettings["smtp"];
            #endregion
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            ConfigurationManager.AppSettings["email"] = txtEmail.Text.Trim();
            ConfigurationManager.AppSettings["password"] = StringToolkit.EncryptDES(txtPassword.Text, "yinyy_danis");
            ConfigurationManager.AppSettings["smtp"] = txtSmtp.Text.Trim();

            MessageBox.Show("保存成功。");
        }

    }
}
