namespace comterminal
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

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
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comPortBox = new System.Windows.Forms.ComboBox();
            this.baudRateBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.sentText = new System.Windows.Forms.TextBox();
            this.receivedText = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.receiveHex = new System.Windows.Forms.CheckBox();
            this.sendHex = new System.Windows.Forms.CheckBox();
            this.menuClose = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuClose);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(4, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 20);
            this.label1.Text = "COM Port";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(4, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 20);
            this.label2.Text = "Baud Rate";
            // 
            // comPortBox
            // 
            this.comPortBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.comPortBox.Items.Add("COM1");
            this.comPortBox.Items.Add("COM2");
            this.comPortBox.Items.Add("COM3");
            this.comPortBox.Items.Add("COM4");
            this.comPortBox.Items.Add("COM5");
            this.comPortBox.Items.Add("COM6");
            this.comPortBox.Items.Add("COM7");
            this.comPortBox.Items.Add("COM8");
            this.comPortBox.Items.Add("COM9");
            this.comPortBox.Items.Add("COM10");
            this.comPortBox.Location = new System.Drawing.Point(100, 12);
            this.comPortBox.Name = "comPortBox";
            this.comPortBox.Size = new System.Drawing.Size(100, 22);
            this.comPortBox.TabIndex = 2;
            this.comPortBox.Text = "COM3";
            // 
            // baudRateBox
            // 
            this.baudRateBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.baudRateBox.Items.Add("4800");
            this.baudRateBox.Items.Add("9600");
            this.baudRateBox.Location = new System.Drawing.Point(100, 39);
            this.baudRateBox.Name = "baudRateBox";
            this.baudRateBox.Size = new System.Drawing.Size(100, 22);
            this.baudRateBox.TabIndex = 2;
            this.baudRateBox.Text = "4800";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(4, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 20);
            this.label3.Text = "Sent data";
            // 
            // sentText
            // 
            this.sentText.Location = new System.Drawing.Point(3, 91);
            this.sentText.Multiline = true;
            this.sentText.Name = "sentText";
            this.sentText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.sentText.Size = new System.Drawing.Size(233, 40);
            this.sentText.TabIndex = 4;
            // 
            // receivedText
            // 
            this.receivedText.Location = new System.Drawing.Point(4, 164);
            this.receivedText.Multiline = true;
            this.receivedText.Name = "receivedText";
            this.receivedText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.receivedText.Size = new System.Drawing.Size(233, 90);
            this.receivedText.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(4, 138);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 20);
            this.label4.Text = "Received Data";
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(123, 68);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(56, 20);
            this.btnOpen.TabIndex = 7;
            this.btnOpen.Text = "Open";
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(185, 68);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(51, 20);
            this.btnSend.TabIndex = 8;
            this.btnSend.Text = "Send";
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // receiveHex
            // 
            this.receiveHex.Enabled = false;
            this.receiveHex.Location = new System.Drawing.Point(100, 137);
            this.receiveHex.Name = "receiveHex";
            this.receiveHex.Size = new System.Drawing.Size(100, 20);
            this.receiveHex.TabIndex = 9;
            this.receiveHex.Text = "hex";
            // 
            // sendHex
            // 
            this.sendHex.Enabled = false;
            this.sendHex.Location = new System.Drawing.Point(66, 68);
            this.sendHex.Name = "sendHex";
            this.sendHex.Size = new System.Drawing.Size(50, 20);
            this.sendHex.TabIndex = 9;
            this.sendHex.Text = "hex";
            // 
            // menuClose
            // 
            this.menuClose.Text = "Close";
            this.menuClose.Click += new System.EventHandler(this.menuClose_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.sendHex);
            this.Controls.Add(this.receiveHex);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.receivedText);
            this.Controls.Add(this.sentText);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.baudRateBox);
            this.Controls.Add(this.comPortBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Menu = this.mainMenu1;
            this.Name = "MainForm";
            this.Text = "COM Ternimal";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Closed += new System.EventHandler(this.MainForm_Closed);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comPortBox;
        private System.Windows.Forms.ComboBox baudRateBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox sentText;
        private System.Windows.Forms.TextBox receivedText;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.CheckBox receiveHex;
        private System.Windows.Forms.CheckBox sendHex;
        private System.Windows.Forms.MenuItem menuClose;
    }
}

