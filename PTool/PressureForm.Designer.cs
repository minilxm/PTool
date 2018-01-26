namespace PTool
{
    partial class PressureForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PressureForm));
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.tlpTitle = new System.Windows.Forms.TableLayoutPanel();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.lbTitle = new System.Windows.Forms.Label();
            this.picSetting = new System.Windows.Forms.PictureBox();
            this.tlpParameter = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.lbParaSetting = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cbPumpType = new System.Windows.Forms.ComboBox();
            this.tbPumpNo = new System.Windows.Forms.TextBox();
            this.tbToolingNo = new System.Windows.Forms.TextBox();
            this.tbToolingNo2 = new System.Windows.Forms.TextBox();
            this.tlpMain.SuspendLayout();
            this.tlpTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSetting)).BeginInit();
            this.tlpParameter.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Controls.Add(this.tlpTitle, 0, 0);
            this.tlpMain.Controls.Add(this.tlpParameter, 0, 1);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Margin = new System.Windows.Forms.Padding(0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 3;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 84F));
            this.tlpMain.Size = new System.Drawing.Size(1133, 735);
            this.tlpMain.TabIndex = 0;
            // 
            // tlpTitle
            // 
            this.tlpTitle.ColumnCount = 4;
            this.tlpTitle.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tlpTitle.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpTitle.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpTitle.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpTitle.Controls.Add(this.picLogo, 0, 0);
            this.tlpTitle.Controls.Add(this.lbTitle, 1, 0);
            this.tlpTitle.Controls.Add(this.picSetting, 3, 0);
            this.tlpTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpTitle.Location = new System.Drawing.Point(0, 0);
            this.tlpTitle.Margin = new System.Windows.Forms.Padding(0);
            this.tlpTitle.Name = "tlpTitle";
            this.tlpTitle.RowCount = 1;
            this.tlpTitle.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpTitle.Size = new System.Drawing.Size(1133, 44);
            this.tlpTitle.TabIndex = 0;
            this.tlpTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tlpTitle_MouseDown);
            this.tlpTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tlpTitle_MouseMove);
            this.tlpTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tlpTitle_MouseUp);
            // 
            // picLogo
            // 
            this.picLogo.Image = global::PTool.Properties.Resources.icon_logo;
            this.picLogo.Location = new System.Drawing.Point(5, 5);
            this.picLogo.Margin = new System.Windows.Forms.Padding(5);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(40, 34);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.TabIndex = 0;
            this.picLogo.TabStop = false;
            // 
            // lbTitle
            // 
            this.lbTitle.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lbTitle.AutoSize = true;
            this.lbTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbTitle.ForeColor = System.Drawing.Color.Black;
            this.lbTitle.Location = new System.Drawing.Point(53, 13);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(84, 17);
            this.lbTitle.TabIndex = 1;
            this.lbTitle.Text = "压力测试1.0";
            // 
            // picSetting
            // 
            this.picSetting.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.picSetting.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picSetting.Image = global::PTool.Properties.Resources.icon_setting;
            this.picSetting.Location = new System.Drawing.Point(1088, 5);
            this.picSetting.Margin = new System.Windows.Forms.Padding(5);
            this.picSetting.Name = "picSetting";
            this.picSetting.Size = new System.Drawing.Size(40, 34);
            this.picSetting.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picSetting.TabIndex = 2;
            this.picSetting.TabStop = false;
            // 
            // tlpParameter
            // 
            this.tlpParameter.BackColor = System.Drawing.SystemColors.HotTrack;
            this.tlpParameter.ColumnCount = 9;
            this.tlpParameter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tlpParameter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tlpParameter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tlpParameter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tlpParameter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tlpParameter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tlpParameter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tlpParameter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tlpParameter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tlpParameter.Controls.Add(this.label1, 0, 0);
            this.tlpParameter.Controls.Add(this.lbParaSetting, 0, 0);
            this.tlpParameter.Controls.Add(this.label2, 3, 0);
            this.tlpParameter.Controls.Add(this.label3, 5, 0);
            this.tlpParameter.Controls.Add(this.label4, 7, 0);
            this.tlpParameter.Controls.Add(this.cbPumpType, 2, 0);
            this.tlpParameter.Controls.Add(this.tbPumpNo, 4, 0);
            this.tlpParameter.Controls.Add(this.tbToolingNo, 6, 0);
            this.tlpParameter.Controls.Add(this.tbToolingNo2, 8, 0);
            this.tlpParameter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpParameter.Location = new System.Drawing.Point(0, 44);
            this.tlpParameter.Margin = new System.Windows.Forms.Padding(0);
            this.tlpParameter.Name = "tlpParameter";
            this.tlpParameter.RowCount = 1;
            this.tlpParameter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpParameter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 73F));
            this.tlpParameter.Size = new System.Drawing.Size(1133, 73);
            this.tlpParameter.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("SimSun", 12F);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(151, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "机器型号";
            // 
            // lbParaSetting
            // 
            this.lbParaSetting.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbParaSetting.AutoSize = true;
            this.lbParaSetting.Font = new System.Drawing.Font("SimSun", 12F);
            this.lbParaSetting.ForeColor = System.Drawing.Color.White;
            this.lbParaSetting.Location = new System.Drawing.Point(18, 28);
            this.lbParaSetting.Name = "lbParaSetting";
            this.lbParaSetting.Size = new System.Drawing.Size(88, 16);
            this.lbParaSetting.TabIndex = 0;
            this.lbParaSetting.Text = "泵参数设置";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(400, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 21);
            this.label2.TabIndex = 1;
            this.label2.Text = "产品序号";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("SimSun", 12F);
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(647, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 16);
            this.label3.TabIndex = 1;
            this.label3.Text = "工装编号1";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("SimSun", 12F);
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(897, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 16);
            this.label4.TabIndex = 1;
            this.label4.Text = "工装编号2";
            // 
            // cbPumpType
            // 
            this.cbPumpType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cbPumpType.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.cbPumpType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbPumpType.Font = new System.Drawing.Font("SimSun", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.cbPumpType.ForeColor = System.Drawing.Color.White;
            this.cbPumpType.FormattingEnabled = true;
            this.cbPumpType.Location = new System.Drawing.Point(253, 26);
            this.cbPumpType.Name = "cbPumpType";
            this.cbPumpType.Size = new System.Drawing.Size(119, 20);
            this.cbPumpType.TabIndex = 2;
            // 
            // tbPumpNo
            // 
            this.tbPumpNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPumpNo.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.tbPumpNo.Font = new System.Drawing.Font("SimSun", 12F);
            this.tbPumpNo.ForeColor = System.Drawing.Color.White;
            this.tbPumpNo.Location = new System.Drawing.Point(503, 23);
            this.tbPumpNo.Name = "tbPumpNo";
            this.tbPumpNo.Size = new System.Drawing.Size(119, 26);
            this.tbPumpNo.TabIndex = 3;
            // 
            // tbToolingNo
            // 
            this.tbToolingNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbToolingNo.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.tbToolingNo.Font = new System.Drawing.Font("SimSun", 12F);
            this.tbToolingNo.ForeColor = System.Drawing.Color.White;
            this.tbToolingNo.Location = new System.Drawing.Point(753, 23);
            this.tbToolingNo.Name = "tbToolingNo";
            this.tbToolingNo.Size = new System.Drawing.Size(119, 26);
            this.tbToolingNo.TabIndex = 3;
            // 
            // tbToolingNo2
            // 
            this.tbToolingNo2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbToolingNo2.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.tbToolingNo2.Font = new System.Drawing.Font("SimSun", 12F);
            this.tbToolingNo2.ForeColor = System.Drawing.Color.White;
            this.tbToolingNo2.Location = new System.Drawing.Point(1003, 23);
            this.tbToolingNo2.Name = "tbToolingNo2";
            this.tbToolingNo2.Size = new System.Drawing.Size(127, 26);
            this.tbToolingNo2.TabIndex = 3;
            // 
            // PressureForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1133, 735);
            this.Controls.Add(this.tlpMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PressureForm";
            this.Text = "PressureForm";
            this.Load += new System.EventHandler(this.PressureForm_Load);
            this.tlpMain.ResumeLayout(false);
            this.tlpTitle.ResumeLayout(false);
            this.tlpTitle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSetting)).EndInit();
            this.tlpParameter.ResumeLayout(false);
            this.tlpParameter.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.TableLayoutPanel tlpTitle;
        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Label lbTitle;
        private System.Windows.Forms.PictureBox picSetting;
        private System.Windows.Forms.TableLayoutPanel tlpParameter;
        private System.Windows.Forms.Label lbParaSetting;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbPumpType;
        private System.Windows.Forms.TextBox tbPumpNo;
        private System.Windows.Forms.TextBox tbToolingNo;
        private System.Windows.Forms.TextBox tbToolingNo2;
    }
}