using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using GPSProxy.SerialPort;

namespace GPSProxyPC
{
    public delegate void DelegateStandardPattern(String para);

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();


            UpdateControls();
        }

        PortRedirector portRedt;

        IPort inPort;
        IPort outPort;  

        private void btnInput_Click(object sender, EventArgs e)
        {
            FormComSelect comDlg = new FormComSelect();
            comDlg.IsInputPort = true;
            DialogResult ret = comDlg.ShowDialog();
            if(DialogResult.OK == ret)
            {
                IPort newPort = comDlg.GetPort();
                if(newPort != null)
                {
                    if (inPort != null)
                        inPort.Dispose();

                    inPort = newPort;

                    textBoxInputPort.Text = comDlg.GetDisplayString();

                    inPort.Read += new PortReadEvent(inPort_Read);
                }
            }
        }

        void inPort_Read(IPort sender, byte[] data)
        {
            AppendLog(System.Text.Encoding.Default.GetString(data, 0, data.Length));
        }

        private void AppendLog(String msg)
        {
            if(this.InvokeRequired)
            {
                this.Invoke(new DelegateStandardPattern(AppendLog), msg);
            }
            else
            {
                richTextLog.Text += msg;
                richTextLog.SelectionStart = richTextLog.Text.Length;
            }
        }

        private void btnOutput_Click(object sender, EventArgs e)
        {
            FormComSelect comDlg = new FormComSelect();
            comDlg.IsInputPort = false;
            DialogResult ret = comDlg.ShowDialog();
            if (DialogResult.OK == ret)
            {
                IPort newPort = comDlg.GetPort();
                if (newPort != null)
                {
                    if (outPort != null)
                        outPort.Dispose();

                    outPort = newPort;

                    textBoxOutputPort.Text = comDlg.GetDisplayString();
                }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (IsPortWorking(inPort))
                StopProxy();
            else
                StartProxy();

            UpdateControls();
        }  

        private void UpdateControls()
        {
            bool bIsPortWorking = IsPortWorking(inPort);

            btnInput.Enabled = !bIsPortWorking;
            btnOutput.Enabled = !bIsPortWorking;

            btnStart.Text = bIsPortWorking ? "Stop" : "Start";
            
        }

        private bool IsPortWorking(IPort port)
        {
            bool bIsPortWorking = false;
            if (port != null && port.IsOpen())
            {
                bIsPortWorking = true;
            }

            return bIsPortWorking;
        }

        private void StartProxy()
        {
            if (portRedt != null)
            {
                portRedt.Dispose();
                portRedt = null;
            }

            portRedt = new PortRedirector(inPort, outPort);
            portRedt.Start();
        }

        private void StopProxy()
        {
            if (portRedt != null)
            {
                portRedt.Stop();
                portRedt.Dispose();
                portRedt = null;
            }
        }        

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopProxy();
        }
    }
}
