namespace AgricultureSensorSimulator
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.userControlPwm1 = new AgricultureSensorSimulator.UserControlPwm();
            this.userControlFineParticulateMatter1 = new AgricultureSensorSimulator.UserControlFineParticulateMatter();
            this.userControlGround1 = new AgricultureSensorSimulator.UserControlGround();
            this.userControlTemperatureHumidity1 = new AgricultureSensorSimulator.UserControlTemperatureHumidity();
            this.userControlSwitcher1 = new AgricultureSensorSimulator.UserControlSwitcher();
            this.userControlBattery1 = new AgricultureSensorSimulator.UserControlBattery();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(730, 345);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "启动服务器";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // userControlPwm1
            // 
            this.userControlPwm1.Location = new System.Drawing.Point(279, 177);
            this.userControlPwm1.Name = "userControlPwm1";
            this.userControlPwm1.Size = new System.Drawing.Size(261, 167);
            this.userControlPwm1.TabIndex = 4;
            this.userControlPwm1.On_SendMessage += new AgricultureSensorSimulator.UserControlPwm.On_SendMessageDelegate(this.On_SendMessage);
            // 
            // userControlFineParticulateMatter1
            // 
            this.userControlFineParticulateMatter1.Location = new System.Drawing.Point(12, 177);
            this.userControlFineParticulateMatter1.Name = "userControlFineParticulateMatter1";
            this.userControlFineParticulateMatter1.Size = new System.Drawing.Size(261, 160);
            this.userControlFineParticulateMatter1.TabIndex = 3;
            this.userControlFineParticulateMatter1.On_SendMessage += new AgricultureSensorSimulator.UserControlFineParticulateMatter.On_SendMessageDelegate(this.On_SendMessage);
            // 
            // userControlGround1
            // 
            this.userControlGround1.Location = new System.Drawing.Point(546, 12);
            this.userControlGround1.Name = "userControlGround1";
            this.userControlGround1.Size = new System.Drawing.Size(260, 159);
            this.userControlGround1.TabIndex = 2;
            this.userControlGround1.On_SendMessage += new AgricultureSensorSimulator.UserControlGround.On_SendMessageDelegate(this.On_SendMessage);
            // 
            // userControlTemperatureHumidity1
            // 
            this.userControlTemperatureHumidity1.Location = new System.Drawing.Point(278, 12);
            this.userControlTemperatureHumidity1.Name = "userControlTemperatureHumidity1";
            this.userControlTemperatureHumidity1.Size = new System.Drawing.Size(262, 158);
            this.userControlTemperatureHumidity1.TabIndex = 1;
            this.userControlTemperatureHumidity1.On_SendMessage += new AgricultureSensorSimulator.UserControlTemperatureHumidity.On_SendMessageDelegate(this.On_SendMessage);
            // 
            // userControlSwitcher1
            // 
            this.userControlSwitcher1.Location = new System.Drawing.Point(12, 12);
            this.userControlSwitcher1.Name = "userControlSwitcher1";
            this.userControlSwitcher1.Size = new System.Drawing.Size(260, 159);
            this.userControlSwitcher1.TabIndex = 0;
            this.userControlSwitcher1.On_SendMessage += new AgricultureSensorSimulator.UserControlSwitcher.On_SendMessageDelegate(this.On_SendMessage);
            // 
            // userControlBattery1
            // 
            this.userControlBattery1.Location = new System.Drawing.Point(546, 177);
            this.userControlBattery1.Name = "userControlBattery1";
            this.userControlBattery1.Size = new System.Drawing.Size(262, 162);
            this.userControlBattery1.TabIndex = 6;
            this.userControlBattery1.On_SendMessage += new AgricultureSensorSimulator.UserControlBattery.On_SendMessageDelegate(this.On_SendMessage2);
            this.userControlBattery1.Load += new System.EventHandler(this.userControlBattery1_Load);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(817, 379);
            this.Controls.Add(this.userControlBattery1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.userControlPwm1);
            this.Controls.Add(this.userControlFineParticulateMatter1);
            this.Controls.Add(this.userControlGround1);
            this.Controls.Add(this.userControlTemperatureHumidity1);
            this.Controls.Add(this.userControlSwitcher1);
            this.Name = "Form1";
            this.Text = "Zigbee模拟器";
            this.ResumeLayout(false);

        }

        #endregion

        private UserControlSwitcher userControlSwitcher1;
        private UserControlTemperatureHumidity userControlTemperatureHumidity1;
        private UserControlGround userControlGround1;
        private UserControlFineParticulateMatter userControlFineParticulateMatter1;
        private UserControlPwm userControlPwm1;
        private System.Windows.Forms.Button button1;
        private UserControlBattery userControlBattery1;
    }
}

