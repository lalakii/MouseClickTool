namespace def
{
    partial class MouseClickTool
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnUrl = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.delayVal = new System.Windows.Forms.TextBox();
            this.hotkeys = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.clickType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Label();
            this.btnMin = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnUrl);
            this.groupBox1.Location = new System.Drawing.Point(10, 62);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(337, 67);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "说明";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(217, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "本软件为开源软件，欢迎使用！";
            // 
            // btnUrl
            // 
            this.btnUrl.AutoSize = true;
            this.btnUrl.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnUrl.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUrl.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btnUrl.Location = new System.Drawing.Point(9, 41);
            this.btnUrl.Name = "btnUrl";
            this.btnUrl.Size = new System.Drawing.Size(272, 18);
            this.btnUrl.TabIndex = 2;
            this.btnUrl.Text = "https://github.com/lalakii/MouseClickTool";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.delayVal);
            this.groupBox3.Controls.Add(this.hotkeys);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.clickType);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.btnStart);
            this.groupBox3.Location = new System.Drawing.Point(10, 135);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(337, 104);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "功能";
            // 
            // delayVal
            // 
            this.delayVal.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.delayVal.Location = new System.Drawing.Point(126, 24);
            this.delayVal.MaxLength = 10;
            this.delayVal.Name = "delayVal";
            this.delayVal.Size = new System.Drawing.Size(75, 27);
            this.delayVal.TabIndex = 1;
            this.delayVal.Text = "1000";
            // 
            // hotkeys
            // 
            this.hotkeys.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.hotkeys.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.hotkeys.FormattingEnabled = true;
            this.hotkeys.Location = new System.Drawing.Point(126, 62);
            this.hotkeys.Name = "hotkeys";
            this.hotkeys.Size = new System.Drawing.Size(75, 28);
            this.hotkeys.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label3.Location = new System.Drawing.Point(45, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "全局热键：";
            // 
            // clickType
            // 
            this.clickType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.clickType.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.clickType.FormattingEnabled = true;
            this.clickType.Items.AddRange(new object[] {
            "左键(Left)",
            "右键(Right)"});
            this.clickType.Location = new System.Drawing.Point(212, 24);
            this.clickType.Name = "clickType";
            this.clickType.Size = new System.Drawing.Size(119, 28);
            this.clickType.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label4.Location = new System.Drawing.Point(5, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(121, 20);
            this.label4.TabIndex = 3;
            this.label4.Text = "间隔(毫秒/ms)：";
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnStart.Location = new System.Drawing.Point(212, 53);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(119, 47);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "开始";
            this.btnStart.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            this.btnClose.AutoSize = true;
            this.btnClose.Font = new System.Drawing.Font("宋体", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnClose.Location = new System.Drawing.Point(312, 21);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(40, 28);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "×";
            // 
            // btnMin
            // 
            this.btnMin.AutoSize = true;
            this.btnMin.Font = new System.Drawing.Font("宋体", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnMin.Location = new System.Drawing.Point(270, 16);
            this.btnMin.Name = "btnMin";
            this.btnMin.Size = new System.Drawing.Size(37, 40);
            this.btnMin.TabIndex = 5;
            this.btnMin.Text = "-";
            // 
            // MouseClickTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(358, 243);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnMin);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.groupBox3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MouseClickTool";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MouseClickTool";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label btnUrl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox delayVal;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.ComboBox clickType;
        private System.Windows.Forms.Label btnClose;
        private System.Windows.Forms.Label btnMin;
        private System.Windows.Forms.ComboBox hotkeys;
        private System.Windows.Forms.Label label3;
    }
}

