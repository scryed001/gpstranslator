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
        }

        // upload the gps data
        IPort inPortFile;
        IPort outPortWebService;
        PortRedirector portRedtUpload;

        // down load the gps data
        IPort inPortWebService;
        IPort outPortCOM7;
        PortRedirector portRedtDownload;

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
    }
}
