namespace GPSProxyPC
{
    partial class FormComSelect
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
            this.label1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.comboBoxPortType = new System.Windows.Forms.ComboBox();
            this.panelFile = new System.Windows.Forms.Panel();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.textBoxFileName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panelComPort = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxComPortName = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxBaudRate = new System.Windows.Forms.ComboBox();
            this.panelWebServer = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxPathID = new System.Windows.Forms.TextBox();
            this.panelFile.SuspendLayout();
            this.panelComPort.SuspendLayout();
            this.panelWebServer.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Port Type:";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(70, 209);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(200, 209);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // comboBoxPortType
            // 
            this.comboBoxPortType.FormattingEnabled = true;
            this.comboBoxPortType.Items.AddRange(new object[] {
            "COM Port",
            "File",
            "Web Server"});
            this.comboBoxPortType.Location = new System.Drawing.Point(82, 13);
            this.comboBoxPortType.Name = "comboBoxPortType";
            this.comboBoxPortType.Size = new System.Drawing.Size(121, 20);
            this.comboBoxPortType.TabIndex = 3;
            this.comboBoxPortType.Text = "COM Port";
            this.comboBoxPortType.SelectedIndexChanged += new System.EventHandler(this.comboBoxPortType_SelectedIndexChanged);
            // 
            // panelFile
            // 
            this.panelFile.Controls.Add(this.btnSelectFile);
            this.panelFile.Controls.Add(this.textBoxFileName);
            this.panelFile.Controls.Add(this.label2);
            this.panelFile.Location = new System.Drawing.Point(12, 112);
            this.panelFile.Name = "panelFile";
            this.panelFile.Size = new System.Drawing.Size(321, 40);
            this.panelFile.TabIndex = 4;
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Location = new System.Drawing.Point(288, 9);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(33, 23);
            this.btnSelectFile.TabIndex = 2;
            this.btnSelectFile.Text = "...";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // textBoxFileName
            // 
            this.textBoxFileName.Location = new System.Drawing.Point(70, 9);
            this.textBoxFileName.Name = "textBoxFileName";
            this.textBoxFileName.Size = new System.Drawing.Size(212, 21);
            this.textBoxFileName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "File Name:";
            // 
            // panelComPort
            // 
            this.panelComPort.Controls.Add(this.comboBoxBaudRate);
            this.panelComPort.Controls.Add(this.label4);
            this.panelComPort.Controls.Add(this.comboBoxComPortName);
            this.panelComPort.Controls.Add(this.label3);
            this.panelComPort.Location = new System.Drawing.Point(12, 42);
            this.panelComPort.Name = "panelComPort";
            this.panelComPort.Size = new System.Drawing.Size(321, 64);
            this.panelComPort.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "COM Port:";
            // 
            // comboBoxComPortName
            // 
            this.comboBoxComPortName.FormattingEnabled = true;
            this.comboBoxComPortName.Items.AddRange(new object[] {
            "COM0",
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8",
            "COM9"});
            this.comboBoxComPortName.Location = new System.Drawing.Point(71, 10);
            this.comboBoxComPortName.Name = "comboBoxComPortName";
            this.comboBoxComPortName.Size = new System.Drawing.Size(121, 20);
            this.comboBoxComPortName.TabIndex = 1;
            this.comboBoxComPortName.Text = "COM7";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "Baud Rate:";
            // 
            // comboBoxBaudRate
            // 
            this.comboBoxBaudRate.FormattingEnabled = true;
            this.comboBoxBaudRate.Items.AddRange(new object[] {
            "4800",
            "9600"});
            this.comboBoxBaudRate.Location = new System.Drawing.Point(71, 36);
            this.comboBoxBaudRate.Name = "comboBoxBaudRate";
            this.comboBoxBaudRate.Size = new System.Drawing.Size(121, 20);
            this.comboBoxBaudRate.TabIndex = 1;
            this.comboBoxBaudRate.Text = "9600";
            // 
            // panelWebServer
            // 
            this.panelWebServer.Controls.Add(this.textBoxPathID);
            this.panelWebServer.Controls.Add(this.label5);
            this.panelWebServer.Location = new System.Drawing.Point(12, 158);
            this.panelWebServer.Name = "panelWebServer";
            this.panelWebServer.Size = new System.Drawing.Size(321, 35);
            this.panelWebServer.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "Path ID:";
            // 
            // textBoxPathID
            // 
            this.textBoxPathID.Location = new System.Drawing.Point(70, 7);
            this.textBoxPathID.Name = "textBoxPathID";
            this.textBoxPathID.Size = new System.Drawing.Size(212, 21);
            this.textBoxPathID.TabIndex = 2;
            // 
            // FormComSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(349, 244);
            this.Controls.Add(this.panelWebServer);
            this.Controls.Add(this.panelComPort);
            this.Controls.Add(this.panelFile);
            this.Controls.Add(this.comboBoxPortType);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormComSelect";
            this.ShowInTaskbar = false;
            this.Text = "Select COM Port";
            this.Load += new System.EventHandler(this.FormComSelect_Load);
            this.panelFile.ResumeLayout(false);
            this.panelFile.PerformLayout();
            this.panelComPort.ResumeLayout(false);
            this.panelComPort.PerformLayout();
            this.panelWebServer.ResumeLayout(false);
            this.panelWebServer.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox comboBoxPortType;
        private System.Windows.Forms.Panel panelFile;
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.TextBox textBoxFileName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelComPort;
        private System.Windows.Forms.ComboBox comboBoxBaudRate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxComPortName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panelWebServer;
        private System.Windows.Forms.TextBox textBoxPathID;
        private System.Windows.Forms.Label label5;
    }
}