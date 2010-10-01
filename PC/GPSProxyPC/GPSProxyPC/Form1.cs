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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            inPortFile = new FileBasedPort(@"G:\sunzhongkui\code\gpstranslator\gps.txt", "read");
            outPortWebService = new WebServiceBasedPort(2,"","PC", false, false);

            portRedtUpload = new PortRedirector(inPortFile, outPortWebService);

            inPortWebService = new WebServiceBasedPort(2, "", "PC", true, false);
            outPortCOM7 = new MSPort("COM7", 9600);
            portRedtDownload = new PortRedirector(inPortWebService, outPortCOM7);

            UpdateControls();
        }

        // upload the gps data
        IPort inPortFile;
        IPort outPortWebService;
        PortRedirector portRedtUpload;

        // down load the gps data
        IPort inPortWebService;
        IPort outPortCOM7;
        PortRedirector portRedtDownload;

        IPort inPort;
        IPort outPort;

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                portRedtUpload.Start();

            }
            catch (System.Exception ex)
            {
            	
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                portRedtDownload.Start();

            }
            catch (System.Exception ex)
            {

            }
        }

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
                }
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
            if(inPort !=null)
            {
                if (inPort.IsOpen())
                    inPort.Close();

                inPort.Open();
            }

            if (outPort != null)
            {
                if (outPort.IsOpen())
                    outPort.Close();

                outPort.Open();
            }

            UpdateControls();
        }

  


        private void UpdateControls()
        {
            bool bIsPortWorking = false;

            if(inPort != null && inPort.IsOpen())
            {
                bIsPortWorking = true;
            }

            btnInput.Enabled = !bIsPortWorking;
            btnOutput.Enabled = !bIsPortWorking;

            btnStart.Text = bIsPortWorking ? "Stop" : "Start";
            
        }
    }
}
