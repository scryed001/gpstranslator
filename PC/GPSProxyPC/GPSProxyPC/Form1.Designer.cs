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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnOutput = new System.Windows.Forms.Button();
            this.btnInput = new System.Windows.Forms.Button();
            this.textBoxOutputPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxInputPort = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(199, 440);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(147, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Upload Data";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(24, 440);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(147, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Download Data";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox1
            // 
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
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(531, 19);
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(651, 475);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "GPS proxy";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnOutput;
        private System.Windows.Forms.Button btnInput;
        private System.Windows.Forms.TextBox textBoxOutputPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxInputPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnStart;
    }
}

