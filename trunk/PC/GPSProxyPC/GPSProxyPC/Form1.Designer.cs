namespace GPSProxyPC
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnOutput = new System.Windows.Forms.Button();
            this.btnInput = new System.Windows.Forms.Button();
            this.textBoxOutputPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxInputPort = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.richTextLog = new System.Windows.Forms.RichTextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnClose);
            this.groupBox1.Controls.Add(this.btnStart);
            this.groupBox1.Controls.Add(this.btnOutput);
            this.groupBox1.Controls.Add(this.btnInput);
            this.groupBox1.Controls.Add(this.textBoxOutputPort);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBoxInputPort);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(627, 86);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "GPS NMEA data provider";
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(533, 21);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(66, 51);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(430, 19);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 53);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnOutput
            // 
            this.btnOutput.Location = new System.Drawing.Point(330, 47);
            this.btnOutput.Name = "btnOutput";
            this.btnOutput.Size = new System.Drawing.Size(41, 23);
            this.btnOutput.TabIndex = 2;
            this.btnOutput.Text = "...";
            this.btnOutput.UseVisualStyleBackColor = true;
            this.btnOutput.Click += new System.EventHandler(this.btnOutput_Click);
            // 
            // btnInput
            // 
            this.btnInput.Location = new System.Drawing.Point(330, 19);
            this.btnInput.Name = "btnInput";
            this.btnInput.Size = new System.Drawing.Size(41, 23);
            this.btnInput.TabIndex = 2;
            this.btnInput.Text = "...";
            this.btnInput.UseVisualStyleBackColor = true;
            this.btnInput.Click += new System.EventHandler(this.btnInput_Click);
            // 
            // textBoxOutputPort
            // 
            this.textBoxOutputPort.Enabled = false;
            this.textBoxOutputPort.Location = new System.Drawing.Point(119, 48);
            this.textBoxOutputPort.Name = "textBoxOutputPort";
            this.textBoxOutputPort.Size = new System.Drawing.Size(205, 21);
            this.textBoxOutputPort.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "Output COM Port:";
            // 
            // textBoxInputPort
            // 
            this.textBoxInputPort.Enabled = false;
            this.textBoxInputPort.Location = new System.Drawing.Point(119, 21);
            this.textBoxInputPort.Name = "textBoxInputPort";
            this.textBoxInputPort.Size = new System.Drawing.Size(205, 21);
            this.textBoxInputPort.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "GPS COM Port:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.richTextLog);
            this.groupBox2.Location = new System.Drawing.Point(12, 121);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(627, 342);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Log";
            // 
            // richTextLog
            // 
            this.richTextLog.Location = new System.Drawing.Point(7, 21);
            this.richTextLog.Name = "richTextLog";
            this.richTextLog.ReadOnly = true;
            this.richTextLog.Size = new System.Drawing.Size(614, 315);
            this.richTextLog.TabIndex = 0;
            this.richTextLog.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(651, 475);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "GPS proxy";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnOutput;
        private System.Windows.Forms.Button btnInput;
        private System.Windows.Forms.TextBox textBoxOutputPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxInputPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RichTextBox richTextLog;
    }
}

