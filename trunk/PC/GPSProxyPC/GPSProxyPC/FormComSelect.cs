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
    public partial class FormComSelect : Form
    {
        public FormComSelect()
        {
            InitializeComponent();
        }

        public bool IsInputPort = true;

        private void FormComSelect_Load(object sender, EventArgs e)
        {
            // Configure panel location and visibility.
            panelFile.Location = panelComPort.Location;
            panelFile.Visible = false;
            panelWebServer.Location = panelComPort.Location;
            panelWebServer.Visible = false;

            comboBoxPortType.SelectedIndex = 0;
        }             

        private void comboBoxPortType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxPortType.SelectedIndex == 0)
            {
                panelComPort.Visible = true;
                panelFile.Visible = false;
                panelWebServer.Visible = false;
            }
            else if (comboBoxPortType.SelectedIndex == 1)
            {
                panelFile.Visible = true;
                panelComPort.Visible = false;
                panelWebServer.Visible = false;
            }
            else
            {
                panelFile.Visible = false;
                panelComPort.Visible = false;
                panelWebServer.Visible = true;
            }
        }

        public IPort GetPort()
        {
            IPort newPort = null;
            if (comboBoxPortType.SelectedIndex == 0)
            {
                newPort = new MSPort(comboBoxComPortName.Text, Int32.Parse(comboBoxBaudRate.Text));
            }
            else if (comboBoxPortType.SelectedIndex == 1)
            {
                String fileMode = IsInputPort ? "read" : "write";

                newPort = new FileBasedPort(textBoxFileName.Text, fileMode);
            }
            else
            {
                try
                {
                    Int32 pathID = Int32.Parse(textBoxPathID.Text);
                    newPort = new WebServiceBasedPort(pathID, "", "PC", IsInputPort, false);
                }
                catch (System.Exception )
                {
                    // There is exception when the textBoxPathID.Text isn't a number.
                }
            }
            return newPort;
        }

        public String GetDisplayString()
        {
            String str = null;

            if (comboBoxPortType.SelectedIndex == 0)
            {
                str = comboBoxComPortName.Text;
            }
            else if (comboBoxPortType.SelectedIndex == 1)
            {
                str = textBoxFileName.Text;
            }
            else
            {
                str = "Path ID: " + textBoxPathID.Text;
            }

            return str;
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            FileDialog dialog;
            if (IsInputPort)
                dialog = new OpenFileDialog();
            else
                dialog = new SaveFileDialog();
            dialog.FileName = textBoxFileName.Text;


            if (dialog.ShowDialog() == DialogResult.OK)
                textBoxFileName.Text = dialog.FileName;
        }
    }
}
