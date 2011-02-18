/*
$Id: FormComSelect.cs,v 1.2 2006/05/24 13:56:16 andrew_klopper Exp $

Copyright 2005-2006 Andrew Rowland Klopper (http://gpsproxy.sourceforge.net/)

This file is part of GPSProxy.

GPSProxy is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

GPSProxy is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with GPSProxy; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using OpenNETCF.Win32;

namespace GPSProxy
{
	/// <summary>
	/// Summary description for FormComSelect.
	/// </summary>
	public class FormComSelect : System.Windows.Forms.Form
	{
		private Regex comPortRegex = new Regex(@"^COM[0-9]+:$");

		private class ComPortDetails
		{
			public string Name;
			public string FriendlyName;

			public ComPortDetails(string name, string friendlyName)
			{
				Name = name;
				FriendlyName = friendlyName;
			}

			override public string ToString()
			{
				return FriendlyName.Equals(String.Empty) ? Name : Name + " (" + FriendlyName + ")";
			}

			public ComPortDetails Clone()
			{
				return new ComPortDetails(Name, FriendlyName);
			}
		}

		public string ComPortSpec 
		{
			get
			{
				StorableHashtable ret = new StorableHashtable();
				if (comboBoxPortType.SelectedIndex == 0) 
				{
					// COM port
					ret["type"] = "comport";
					ret["port_name"] = ((ComPortDetails)comboBoxComPortName.SelectedItem).Name;
					ret["baud_rate"] = comboBoxBaudRate.SelectedItem.ToString();
				}
                else if (comboBoxPortType.SelectedIndex == 1)
                {
                    // File
                    ret["type"] = "file";
                    ret["file_name"] = textBoxFileName.Text;
                    ret["file_mode"] = IsInputPort ? "read" : "write";
                }
                else
                {
                    // Web server
                    ret["type"] = "webserver";
                    ret["path_id"] = textBoxPathID.Text;
                    ret["is_inputport"] = IsInputPort.ToString();
                }
				return ret.ToString();
			}
			set
			{
				StorableHashtable settings = new StorableHashtable(value);

				// Fail silently if we don't get the parameters we are expecting.
				comboBoxPortType.SelectedIndex = 0;
				comboBoxComPortName.SelectedIndex = - 1;
				comboBoxBaudRate.SelectedIndex = -1;

				if (settings.ContainsKey("type"))
				{
					switch (settings["type"].ToString()) 
					{
						case "comport":
							comboBoxPortType.SelectedIndex = 0;
							if (settings.ContainsKey("port_name"))
							{
								for (int i = 0; i < comPorts.Length; i++)
								{
									if (comPorts[i].Name == settings["port_name"].ToString()) 
									{
										comboBoxComPortName.SelectedIndex = i;
										break;
									}
								}
							}
							if (settings.ContainsKey("baud_rate"))
							{
								for (int i = 0; i < baudRates.Length; i++)
								{
									if (baudRates[i].ToString() == settings["baud_rate"].ToString())
									{
										comboBoxBaudRate.SelectedIndex = i;
										break;
									}
								}
							}
							break;
						case "file":
							comboBoxPortType.SelectedIndex = 1;
							if (settings.ContainsKey("file_name")) 
							{
								textBoxFileName.Text = settings["file_name"].ToString();
							}
							else
							{
								textBoxFileName.Text = "";
							}
							break;
                        case "webserver":
                            comboBoxPortType.SelectedIndex = 2;

                            if (settings.ContainsKey("path_id"))
                            {
                                textBoxPathID.Text = settings["path_id"].ToString();
                            }
                            else
                            {
                                textBoxPathID.Text = "0";
                            }
                            break;
					}
				}

				if ((comboBoxComPortName.SelectedIndex < 0) && (comPorts.Length > 0))
					comboBoxComPortName.SelectedIndex = 0;
				if (comboBoxBaudRate.SelectedIndex < 0)
					comboBoxBaudRate.SelectedIndex = 0;
			}
		}

		public bool IsInputPort = true;

		private int[] baudRates = {4800, 9600, 14400, 19200, 38400, 57600, 115200};
		private ComPortDetails[] comPorts;

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBoxPortType;
		private System.Windows.Forms.Panel panelComPort;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboBoxComPortName;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox comboBoxBaudRate;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBoxFileName;
		private System.Windows.Forms.MainMenu mainMenu;
		private Microsoft.WindowsCE.Forms.InputPanel inputPanel;
		private System.Windows.Forms.MenuItem menuItemOK;
		private System.Windows.Forms.MenuItem menuItemCancel;
		private System.Windows.Forms.Panel panelFile;
        private Panel panelWebServer;
        private Label label6;
        private TextBox textBoxPathID;
		private System.Windows.Forms.Button buttonSelectFileName;
	
		public FormComSelect()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Configure panel location and visibility.
			panelFile.Location = panelComPort.Location;
			panelFile.Visible = false;
            panelWebServer.Location = panelComPort.Location;
            panelWebServer.Visible = false;

			comboBoxPortType.SelectedIndex = 0;

			// Enumerate the COM ports.
			comPorts = EnumerateComPorts();

			// Configure the COM port name combo.
			for (int i = 0; i < comPorts.Length; i++) 
				comboBoxComPortName.Items.Add(comPorts[i]);
			if (comPorts.Length > 0)
				comboBoxComPortName.SelectedIndex = 0;

			// Configure the BAUD rate combo.
			for (int i = 0; i < baudRates.Length; i++) 
				comboBoxBaudRate.Items.Add(baudRates[i]);
			comboBoxBaudRate.SelectedIndex = 0;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if (disposing)
			{
				// Make sure the input panel is disposed when the form is disposed to work around
				// an input panel bug that causes it to remember previously added event handlers
				// even if the form that added them was disposed.
				inputPanel.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxPortType = new System.Windows.Forms.ComboBox();
            this.panelComPort = new System.Windows.Forms.Panel();
            this.comboBoxBaudRate = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxComPortName = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panelFile = new System.Windows.Forms.Panel();
            this.buttonSelectFileName = new System.Windows.Forms.Button();
            this.textBoxFileName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.mainMenu = new System.Windows.Forms.MainMenu();
            this.menuItemOK = new System.Windows.Forms.MenuItem();
            this.menuItemCancel = new System.Windows.Forms.MenuItem();
            this.inputPanel = new Microsoft.WindowsCE.Forms.InputPanel();
            this.panelWebServer = new System.Windows.Forms.Panel();
            this.textBoxPathID = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.panelComPort.SuspendLayout();
            this.panelFile.SuspendLayout();
            this.panelWebServer.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(4, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 20);
            this.label1.Text = "Port Type:";
            // 
            // comboBoxPortType
            // 
            this.comboBoxPortType.Items.Add("COM Port");
            this.comboBoxPortType.Items.Add("File");
            this.comboBoxPortType.Items.Add("Web Server");
            this.comboBoxPortType.Location = new System.Drawing.Point(72, 4);
            this.comboBoxPortType.Name = "comboBoxPortType";
            this.comboBoxPortType.Size = new System.Drawing.Size(164, 22);
            this.comboBoxPortType.TabIndex = 2;
            this.comboBoxPortType.SelectedIndexChanged += new System.EventHandler(this.comboBoxPortType_SelectedIndexChanged);
            // 
            // panelComPort
            // 
            this.panelComPort.Controls.Add(this.comboBoxBaudRate);
            this.panelComPort.Controls.Add(this.label3);
            this.panelComPort.Controls.Add(this.comboBoxComPortName);
            this.panelComPort.Controls.Add(this.label2);
            this.panelComPort.Location = new System.Drawing.Point(0, 28);
            this.panelComPort.Name = "panelComPort";
            this.panelComPort.Size = new System.Drawing.Size(240, 60);
            // 
            // comboBoxBaudRate
            // 
            this.comboBoxBaudRate.Location = new System.Drawing.Point(72, 32);
            this.comboBoxBaudRate.Name = "comboBoxBaudRate";
            this.comboBoxBaudRate.Size = new System.Drawing.Size(84, 22);
            this.comboBoxBaudRate.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(4, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 20);
            this.label3.Text = "Baud Rate:";
            // 
            // comboBoxComPortName
            // 
            this.comboBoxComPortName.Location = new System.Drawing.Point(72, 4);
            this.comboBoxComPortName.Name = "comboBoxComPortName";
            this.comboBoxComPortName.Size = new System.Drawing.Size(164, 22);
            this.comboBoxComPortName.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(4, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 20);
            this.label2.Text = "COM Port:";
            // 
            // panelFile
            // 
            this.panelFile.Controls.Add(this.buttonSelectFileName);
            this.panelFile.Controls.Add(this.textBoxFileName);
            this.panelFile.Controls.Add(this.label4);
            this.panelFile.Location = new System.Drawing.Point(0, 88);
            this.panelFile.Name = "panelFile";
            this.panelFile.Size = new System.Drawing.Size(240, 32);
            // 
            // buttonSelectFileName
            // 
            this.buttonSelectFileName.Location = new System.Drawing.Point(216, 4);
            this.buttonSelectFileName.Name = "buttonSelectFileName";
            this.buttonSelectFileName.Size = new System.Drawing.Size(20, 20);
            this.buttonSelectFileName.TabIndex = 0;
            this.buttonSelectFileName.Text = "..";
            this.buttonSelectFileName.Click += new System.EventHandler(this.buttonSelectFileName_Click);
            // 
            // textBoxFileName
            // 
            this.textBoxFileName.Location = new System.Drawing.Point(72, 4);
            this.textBoxFileName.Name = "textBoxFileName";
            this.textBoxFileName.Size = new System.Drawing.Size(140, 21);
            this.textBoxFileName.TabIndex = 1;
            this.textBoxFileName.GotFocus += new System.EventHandler(this.textBoxFileName_GotFocus);
            this.textBoxFileName.LostFocus += new System.EventHandler(this.textBoxFileName_LostFocus);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(4, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 20);
            this.label4.Text = "File Name:";
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.Add(this.menuItemOK);
            this.mainMenu.MenuItems.Add(this.menuItemCancel);
            // 
            // menuItemOK
            // 
            this.menuItemOK.Text = "OK";
            this.menuItemOK.Click += new System.EventHandler(this.menuItemOK_Click);
            // 
            // menuItemCancel
            // 
            this.menuItemCancel.Text = "Cancel";
            this.menuItemCancel.Click += new System.EventHandler(this.menuItemCancel_Click);
            // 
            // panelWebServer
            // 
            this.panelWebServer.Controls.Add(this.textBoxPathID);
            this.panelWebServer.Controls.Add(this.label6);
            this.panelWebServer.Location = new System.Drawing.Point(0, 135);
            this.panelWebServer.Name = "panelWebServer";
            this.panelWebServer.Size = new System.Drawing.Size(240, 25);
            // 
            // textBoxPathID
            // 
            this.textBoxPathID.Location = new System.Drawing.Point(72, 2);
            this.textBoxPathID.Name = "textBoxPathID";
            this.textBoxPathID.Size = new System.Drawing.Size(140, 21);
            this.textBoxPathID.TabIndex = 2;
            this.textBoxPathID.GotFocus += new System.EventHandler(this.textBoxPathID_GotFocus);
            this.textBoxPathID.LostFocus += new System.EventHandler(this.textBoxPathID_LostFocus);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(0, 3);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(76, 20);
            this.label6.Text = "Path ID:";
            // 
            // FormComSelect
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.panelWebServer);
            this.Controls.Add(this.panelFile);
            this.Controls.Add(this.panelComPort);
            this.Controls.Add(this.comboBoxPortType);
            this.Controls.Add(this.label1);
            this.Menu = this.mainMenu;
            this.Name = "FormComSelect";
            this.Text = "Select COM Port";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.FormComSelect_Closing);
            this.panelComPort.ResumeLayout(false);
            this.panelFile.ResumeLayout(false);
            this.panelWebServer.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void FormComSelect_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// Hack to handle the close/ok button in the top right corner of the window.
			if (DialogResult == DialogResult.None)
				DialogResult = DialogResult.OK;

			// Validate the form contents if OK was pressed.
			if (DialogResult == DialogResult.OK) 
			{
				if (comboBoxPortType.SelectedIndex == 0) 
				{
					// COM port
					if (comboBoxComPortName.SelectedIndex < 0)
					{
						MessageBox.Show("No COM port selected.", "Error", MessageBoxButtons.OK,
							MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
						e.Cancel = true;
						return;
					}

					// Baud rate
					if (comboBoxBaudRate.SelectedIndex < 0)
					{
						MessageBox.Show("No baud rate selected.", "Error", MessageBoxButtons.OK,
							MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
						e.Cancel = true;
						return;
					}
				}
                else if (comboBoxPortType.SelectedIndex == 1)
                {
                    // File
                    if ((IsInputPort && (!File.Exists(textBoxFileName.Text))) ||
                        (!IsInputPort && (textBoxFileName.Text == "")))
                    {
                        MessageBox.Show("Invalid file name.", "Error", MessageBoxButtons.OK,
                            MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                        e.Cancel = true;
                        return;
                    }
                }
                else
                {
                    // web server

                    bool bValid = true;
                    try
                    {
                        Int32 id = Int32.Parse(textBoxPathID.Text.Trim());
                        if (id < 0)
                            bValid = false;
                    }
                    catch (System.Exception)
                    {
                        bValid = false;
                    }

                    if(!bValid)
                    {
                        MessageBox.Show("The path id should be equal to or greater than 0", "Error", MessageBoxButtons.OK,
                            MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                        e.Cancel = true;
                        return;
                    }

                }
			}
		}

		private void comboBoxPortType_SelectedIndexChanged(object sender, System.EventArgs e)
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

		private void menuItemOK_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK;
		}

		private void menuItemCancel_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}

		private void textBoxFileName_GotFocus(object sender, System.EventArgs e)
		{
			inputPanel.Enabled = true;
		}

		private void textBoxFileName_LostFocus(object sender, System.EventArgs e)
		{
			inputPanel.Enabled = false;
		}

        private void textBoxPathID_GotFocus(object sender, EventArgs e)
        {
            inputPanel.Enabled = true;
        }

        private void textBoxPathID_LostFocus(object sender, EventArgs e)
        {
            inputPanel.Enabled = false;
        }

		private void buttonSelectFileName_Click(object sender, System.EventArgs e)
		{
			FileDialog dialog;
			if (IsInputPort)
				dialog = new OpenFileDialog();
			else
				dialog = new SaveFileDialog();
			dialog.FileName = textBoxFileName.Text;
			//dialog.Filter = data.filter;
			//dialog.FilterIndex = data.filterIndex;
			//dialog.InitialDirectory = data.initialDir;
			if (dialog.ShowDialog() == DialogResult.OK)
				textBoxFileName.Text = dialog.FileName;
		}

		private ComPortDetails[] EnumerateComPorts()
		{
			// Create a list of all COM port names that we think we might need, just in
			// case we miss one while looking through the registry for available ports.
			int i;
			ArrayList comPorts = new ArrayList();
			for (i = 0; i < 10; i++) 
				comPorts.Add(new ComPortDetails("COM" + i + ":", ""));

			// Try and find descriptions for those COM ports that are actually installed.
			RegistryKey driversKey = Registry.LocalMachine.OpenSubKey(@"Drivers\Active");
			if (driversKey != null) 
			{
				try 
				{
					// Iterate through the active driver subkeys.
					foreach (string subkeyName in driversKey.GetSubKeyNames())
					{
						RegistryKey subkey = driversKey.OpenSubKey(subkeyName);
						if (subkey != null) 
						{
							try
							{
								string driverName = (string)subkey.GetValue("Name", "");
								if (comPortRegex.IsMatch(driverName)) 
								{
									// Look for a friendly name under the driver key. If none is
									// found, use the driver name as the friendly name.
									string key = subkey.GetValue("Key", "").ToString();
									int pos = key.LastIndexOf(@"\");
									string friendlyName = pos >= 0 ? key.Remove(0, pos + 1) : key;
									RegistryKey driverKey = Registry.LocalMachine.OpenSubKey(key);
									if (driverKey != null)
									{
										try
										{
											friendlyName = driverKey.GetValue("FriendlyName", friendlyName).ToString();
										}
										finally
										{
											driverKey.Dispose();
										}
									}

									// Update or add the port details.
									i = 0;
									while ((i < comPorts.Count) && (((ComPortDetails)comPorts[i]).Name != driverName))
										i++;
									if (i < comPorts.Count) 
									{
										((ComPortDetails)comPorts[i]).FriendlyName = friendlyName;
									} 
									else
									{
										comPorts.Add(new ComPortDetails(driverName, friendlyName));
									}
								}
							}
							finally
							{
								subkey.Dispose();
							}
						}
					}
				}
				finally
				{
					driversKey.Dispose();
				}
			}

			// Microsoft Bluetooth serial port enumerator contributed by Lim Wei Sian.
			driversKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Bluetooth\Serial\Ports");
			if (driversKey != null) 
			{
				try 
				{
					foreach (string subkeyName in driversKey.GetSubKeyNames())
					{
						RegistryKey subkey = driversKey.OpenSubKey(subkeyName);
						if (subkey != null) 
						{
							try
							{
								string driverName = subkey.GetValue("Port", "").ToString() + ":";
								if (comPortRegex.IsMatch(driverName))
								{
									string friendlyName = "Microsoft Bluetooth Serial";

									// Update or add the port details.
									i = 0;
									while ((i < comPorts.Count) && (((ComPortDetails)comPorts[i]).Name != driverName))
										i++;
									if (i < comPorts.Count) 
									{
										((ComPortDetails)comPorts[i]).FriendlyName = friendlyName;
									} 
									else
									{
										comPorts.Add(new ComPortDetails(driverName, friendlyName));
									}
								}
							}
							finally
							{
								subkey.Dispose();
							}
						}
					}
				}
				finally
				{
					driversKey.Dispose();
				}
			}
			
			// Return the list.
			ComPortDetails[] ret = new ComPortDetails[comPorts.Count];
			for (i = 0; i < comPorts.Count; i++)
				ret[i] = (ComPortDetails)comPorts[i];
			return ret;
		}

	}
}
