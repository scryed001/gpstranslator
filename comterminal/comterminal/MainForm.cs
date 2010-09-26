using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenNETCF.IO.Serial;

namespace comterminal
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            serialPort = new Port("COM1");
            serialPort.DataReceived += new Port.CommEvent(serialPort_DataReceived);
        }

        private delegate void UpdateRecerivedDataInvokeDelegate(String data);

        void serialPort_DataReceived()
        {
            byte[] data = serialPort.Input;

            DispalyRecerivedData(Encoding.ASCII.GetString(data, 0, data.Length));
        }

        void DispalyRecerivedData(String data)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdateRecerivedDataInvokeDelegate(DispalyRecerivedData), data);
            }
            else
            {
                receivedText.Text += data;
            }
        }

        private Port serialPort;

        private void MainForm_Load(object sender, EventArgs e)
        {
            UpdateControl();
        }

        private void menuClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPort.IsOpen)
                    serialPort.Close();
                else
                {
                    int baudRate = int.Parse(baudRateBox.Text);

                    serialPort.PortName = comPortBox.Text;
                    serialPort.Settings.BaudRate = (BaudRates)baudRate;
                    serialPort.Settings.ByteSize = 8;
                    serialPort.Settings.Parity = Parity.none;
                    serialPort.Settings.StopBits = StopBits.one;

                    serialPort.Open();
                }
            }
            catch (System.Exception)
            {

            }

            UpdateControl();
        }

        public void UpdateControl()
        {
            if (serialPort.IsOpen)
                btnOpen.Text = "Close";
            else
                btnOpen.Text = "Open";

            btnSend.Enabled = serialPort.IsOpen;
            comPortBox.Enabled = !serialPort.IsOpen;
            baudRateBox.Enabled = !serialPort.IsOpen;
        }

        private void MainForm_Closed(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
                serialPort.Close();

            serialPort.Dispose();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (sentText.Text.Length == 0)
                return;

            if(serialPort.IsOpen)
            {
                byte[] data = System.Text.Encoding.ASCII.GetBytes(sentText.Text); 
                serialPort.Output = data;
            }
        }
    }
}