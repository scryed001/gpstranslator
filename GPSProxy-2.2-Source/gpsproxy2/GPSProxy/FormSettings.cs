/*
$Id: FormSettings.cs,v 1.3 2006/05/24 21:45:04 andrew_klopper Exp $

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
using GPSProxy.Extension;

namespace GPSProxy
{
	/// <summary>
	/// Summary description for FormSettings.
	/// </summary>
	public class FormSettings : System.Windows.Forms.Form
	{
		private const int marginTop = 4;
		private const int marginLeft = 4;
		private const int controlSpacing = 6;

		private class SettingButtonProperties
		{
			public Button button;
			public object data;

			public SettingButtonProperties(Button button, object data)
			{
				this.button = button;
				this.data = data;
			}
		}

		private class FileDialogData
		{
			public string filter;
			public int filterIndex;
			public string initialDir;

			public FileDialogData(string filter, int filterIndex, string initialDir)
			{
				this.filter = filter;
				this.filterIndex = filterIndex;
				this.initialDir = initialDir;
			}
		}

		private Settings settings;
		private Control[] controls;
		private ScrollablePanel[] controlPanels;
		private SettingButtonProperties[] settingButtonProperties;

		private SettingsValidator validator;

		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem menuItemOK;
		private Microsoft.WindowsCE.Forms.InputPanel inputPanel;
		private System.Windows.Forms.MenuItem menuItemCancel;
	
		public FormSettings(Settings settings, SettingsValidator validator)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Keep track of the validator.
			this.validator = validator;

			// This assumes all fonts on the form are the same size.
			Graphics g = CreateGraphics();
			int charWidth = (int)g.MeasureString("H", Font).Width;

			// Keep a reference to the supplied Settings object.
			this.settings = settings;

			// Create a tab page for each settings group.
			for (int i = 0; i < settings.Count; i++) 
			{
				Setting setting = settings[i];
				int j = 0;
				while ((j < tabControl.TabPages.Count) && (setting.Group != tabControl.TabPages[j].Text))
					j++;
				if (j >= tabControl.TabPages.Count) 
				{
					TabPage newPage = new TabPage();
					newPage.Text = setting.Group;
					tabControl.TabPages.Add(newPage);
				}
			}

			// Create a scrollable panel on each tab page.
			controlPanels = new ScrollablePanel[tabControl.TabPages.Count];
			for (int i = 0; i < tabControl.TabPages.Count; i++)
			{
				controlPanels[i] = new ScrollablePanel();
				controlPanels[i].Location = new Point(0, 0);
				controlPanels[i].Size = tabControl.TabPages[i].ClientSize;
				tabControl.TabPages[i].Controls.Add(controlPanels[i]);
			}

			// Create controls for each setting.
			controls = new Control[settings.Count];
			settingButtonProperties = new SettingButtonProperties[settings.Count];
			int[] controlOffset = new int[tabControl.TabPages.Count];
			for (int i = 0; i < settings.Count; i++)
			{
				Setting setting = settings[i];

				// Assemble the parameters for the setting.
				int j = 0;
				System.Collections.Hashtable parameters = new Hashtable();
				if (setting.Params.Length % 2 != 0)
					throw new Exception("Params array does not have an even number of elements");
				while (j < setting.Params.Length) 
				{
					parameters[setting.Params[j]] = setting.Params[j + 1];
					j += 2;
				}

				// Find the corresponding tab page and panel.
				j = 0;
				while ((j < tabControl.TabPages.Count) && (setting.Group != tabControl.TabPages[j].Text))
					j++;
				if (j >= tabControl.TabPages.Count)
					throw new Exception("Tab page not found. Should be impossible.");
				ScrollablePanel controlPanel = controlPanels[j];
				int maxControlWidth = controlPanel.ContentWidth - 2 * marginLeft;

				// Add the appropriate control.
				if (setting.Type == "boolean") 
				{
					// The caption is part of the check box control.
					CheckBox checkBox = new CheckBox();
					checkBox.Text = setting.Caption;
					checkBox.Width = maxControlWidth;
					checkBox.Checked = (bool)setting.Value;
					controls[i] = checkBox;
				}
				else
				{
					// Display a caption with the corresponding control underneath it.
					Label caption = new Label();
					caption.Left = marginLeft;
					caption.Top = marginTop + controlOffset[j];
					caption.Width = maxControlWidth;
					caption.Text = setting.Caption;
					controlOffset[j] += caption.Height;
					controlPanel.ContentControls.Add(caption);
					switch (setting.Type) 
					{
						case "string":
						case "integer":
						case "long":
						case "double":
						{
							int maxLength = parameters.Contains("maxlength") ? (int)parameters["maxlength"] : 0;
							TextBox textBox = new TextBox();
							if (maxLength > 0) 
							{
								textBox.MaxLength = maxLength;
								textBox.ClientSize = new Size((maxLength + 1) * charWidth, textBox.ClientSize.Height);
								if (textBox.Width > maxControlWidth)
									textBox.Width = maxControlWidth;
							}
							else
							{
								textBox.Width = maxControlWidth;
							}
							textBox.Text = setting.Value.ToString();
							textBox.GotFocus += new EventHandler(textBox_GotFocus);
							textBox.LostFocus += new EventHandler(textBox_LostFocus);
							controls[i] = textBox;
							break;
						}
						case "updown":
						{
							if (! parameters.Contains("min") || ! parameters.Contains("max"))
								throw new Exception("Missing 'min' and/or 'max' parameter(s)");
							int maxLength = parameters.Contains("maxlength") ? (int)parameters["maxlength"] : 0;
							int min = (int)parameters["min"];
							int max = (int)parameters["max"];
							NumericUpDown upDown = new NumericUpDown();
							upDown.Value = (int)setting.Value;
							upDown.Minimum = min;
							upDown.Maximum = max;
							if (maxLength > 0)
							{
								upDown.ClientSize = new Size((maxLength + 1) * charWidth + 30, upDown.ClientSize.Height);
								if (upDown.Width > maxControlWidth)
									upDown.Width = maxControlWidth;
							}
							else
							{
								upDown.Width = maxControlWidth;
							}
							controls[i] = upDown;
							break;
						}
						case "comportin":
						case "comportout":
						{
							Button button = new Button();
							button.Text = "..";
							button.Width = 20;
							button.Top = marginTop + controlOffset[j];
							button.Left = controlPanel.ContentWidth - marginLeft - button.Width;
							button.Click += new EventHandler(SettingButtonClick);
							settingButtonProperties[i] = new SettingButtonProperties(button, setting.Value);
							controlPanel.ContentControls.Add(button);

							TextBox textBox = new TextBox();
							textBox.ReadOnly = true;
							textBox.Width = button.Left - marginLeft - 2;
							textBox.Text = GetComPortDescription(setting.Value.ToString());
							controls[i] = textBox;
							break;
						}
						case "combobox":
						{
							if (! parameters.Contains("items"))
								throw new Exception("Missing 'items' parameter");
							object[] items = (object[])parameters["items"];
							ComboBox combo = new ComboBox();
							combo.Width = maxControlWidth;
							for (int k = 0; k < items.Length;  k++)
								combo.Items.Add(items[k]);
							combo.SelectedItem = setting.Value;
							controls[i] = combo;
							break;
						}
						case "openfile":
						case "savefile":
						case "exefile":
						{
							object data = null;
							if (setting.Type != "exefile")
							{
								data = new FileDialogData(parameters.Contains("filter") ? (string)parameters["filter"] : "All Files (*.*)|*.*",
									parameters.Contains("filterindex") ? (int)parameters["filterindex"] : 1,
									parameters.Contains("initialdir") ? (string)parameters["initialdir"] : "");
							}
							Button button = new Button();
							button.Text = "..";
							button.Width = 20;
							button.Top = marginTop + controlOffset[j];
							button.Left = controlPanel.ContentWidth - marginLeft - button.Width;
							button.Click += new EventHandler(SettingButtonClick);
							settingButtonProperties[i] = new SettingButtonProperties(button, data);
							controlPanel.ContentControls.Add(button);

							TextBox textBox = new TextBox();
							textBox.Width = button.Left - marginLeft - 2;
							textBox.Text = setting.Value.ToString();
							textBox.GotFocus += new EventHandler(textBox_GotFocus);
							textBox.LostFocus += new EventHandler(textBox_LostFocus);
							controls[i] = textBox;
							break;
						}
						default:
						{
							throw new Exception("Unknown setting type: " + setting.Type);
						}
					}
				}
				controls[i].Left = marginLeft;
				controls[i].Top = marginTop + controlOffset[j];
				controlOffset[j] += controls[i].Height + controlSpacing;
				controlPanel.ContentControls.Add(controls[i]);
			}

			// Set the heights of the control panel contents.
			for (int i = 0; i < controlPanels.Length; i++)
				controlPanels[i].ContentHeight = controlOffset[i];
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
			this.tabControl = new System.Windows.Forms.TabControl();
			this.mainMenu = new System.Windows.Forms.MainMenu();
			this.menuItemOK = new System.Windows.Forms.MenuItem();
			this.menuItemCancel = new System.Windows.Forms.MenuItem();
			this.inputPanel = new Microsoft.WindowsCE.Forms.InputPanel();
			// 
			// tabControl
			// 
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(240, 272);
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
			// inputPanel
			// 
			this.inputPanel.EnabledChanged += new System.EventHandler(this.inputPanel_EnabledChanged);
			// 
			// FormSettings
			// 
			this.Controls.Add(this.tabControl);
			this.Menu = this.mainMenu;
			this.Text = "Settings";
			this.Resize += new System.EventHandler(this.FormSettings_Resize);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FormSettings_Closing);

		}
		#endregion

		private void menuItemOK_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK;
		}

		private void menuItemCancel_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}

		private void FormSettings_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// Hack to handle the close/ok button in the top right corner of the window.
			if (DialogResult == DialogResult.None)
				DialogResult = DialogResult.OK;

			// Validate the form contents if OK was pressed.
			if (DialogResult == DialogResult.OK)
			{
				// Backup the old values so that they can be restored in the event of a validation error.
				object[] oldValues = new object[settings.Count];
				for (int i = 0; i < settings.Count; i++)
					oldValues[i] = settings[i].Value;

				// Copy the settings from the controls to the Settings object. Basic type validation
				// is performed here.
				for (int i = 0; ! e.Cancel && (i < settings.Count); i++) 
				{
					Setting setting = settings[i];
					switch (setting.Type) 
					{
						case "boolean":
							setting.Value = ((CheckBox)controls[i]).Checked;
							break;
						case "openfile":
						case "savefile":
						case "exefile":
						case "string":
							setting.Value = controls[i].Text;
							break;
						case "integer":
							try 
							{
								setting.Value = int.Parse(controls[i].Text);
							}
							catch
							{
								MessageBox.Show("Invalid numerical value for " + setting.Caption, "Error",
									MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
								e.Cancel = true;
							}
							break;
						case "long":
							try 
							{
								setting.Value = long.Parse(controls[i].Text);
							}
							catch
							{
								MessageBox.Show("Invalid numerical value for " + setting.Caption, "Error",
									MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
								e.Cancel = true;
							}
							break;
						case "double":
							try 
							{
								setting.Value = double.Parse(controls[i].Text);
							}
							catch
							{
								MessageBox.Show("Invalid floating point value for " + setting.Caption, "Error",
									MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
								e.Cancel = true;
							}
							break;
						case "updown":
							// If the control is blank, the minimum value will be returned here.
							setting.Value = (int)((NumericUpDown)controls[i]).Value;
							break;
						case "comportin":
						case "comportout":
						{
							// Copy the setting value from the button properties, as the text box only
							// displays a user-friendly description of the port.
							setting.Value = settingButtonProperties[i].data;
							break;
						}
						case "combobox":
						{
							setting.Value = ((ComboBox)controls[i]).SelectedItem;
							break;
						}
						default:
							throw new Exception("Unknown setting type: " + setting.Type);
					}
				}

				// If no errors occurred above, use the supplied validator function.
				if (! e.Cancel)
				{
					string errorMessage = "";
					if ((validator != null) && ! validator(settings, ref errorMessage))
					{
						MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand,
							MessageBoxDefaultButton.Button1);
						e.Cancel = true;
					}
				}

				// If a validation error occurred, restore the old settings values.
				if (e.Cancel) 
				{
					for (int i = 0; i < settings.Count; i++)
						settings[i].Value = oldValues[i];
				} 
			}

			// Move the focus to something other than a text box to ensure that the text box
			// LostFocus is not called after the input panel has been destroyed.
			tabControl.Focus();
		}

		private void SettingButtonClick(object Sender, EventArgs e)
		{
			// Find the corresponding button details.
			int i = 0;
			while ((i < settingButtonProperties.Length) &&
				((settingButtonProperties[i] == null) || (settingButtonProperties[i].button != Sender)))
				i++;
			if (i >= settingButtonProperties.Length)
				throw new Exception("Unable to find setting button properties");

			// Take the appropriate action.
			Setting setting = settings[i];
			switch (setting.Type) 
			{
				case "openfile":
				case "savefile":
				{
					FileDialogData data = (FileDialogData)settingButtonProperties[i].data;
					FileDialog dialog = setting.Type == "openfile" ? (FileDialog)(new OpenFileDialog()) :
						(FileDialog)(new SaveFileDialog());
					dialog.FileName = controls[i].Text;
					dialog.Filter = data.filter;
					dialog.FilterIndex = data.filterIndex;
					dialog.InitialDirectory = data.initialDir;
					if (dialog.ShowDialog() == DialogResult.OK)
						controls[i].Text = dialog.FileName;
					break;
				}
				case "exefile":
				{
					FormFileTreeView dialog = new FormFileTreeView();
					dialog.FileName = controls[i].Text;
					if (dialog.ShowDialog() == DialogResult.OK)
						controls[i].Text = dialog.FileName;
					break;
				}
				case "comportin":
				case "comportout":
				{
					FormComSelect dialog = new FormComSelect();
					dialog.ComPortSpec = settingButtonProperties[i].data.ToString();
					dialog.IsInputPort = setting.Type == "comportin";
					if (dialog.ShowDialog() == DialogResult.OK)
					{
						settingButtonProperties[i].data = dialog.ComPortSpec;
						controls[i].Text = GetComPortDescription(settingButtonProperties[i].data.ToString());
					}
					break;
				}
				default:
					throw new Exception("Invalid setting type: " + setting.Type);
			}
		}

		private void ResizeTabControl()
		{
			tabControl.Height = ClientSize.Height - (inputPanel.Enabled ? inputPanel.Bounds.Height : 0);
			for (int i = 0; i < tabControl.TabPages.Count; i++) 
				controlPanels[i].Height = tabControl.TabPages[i].ClientSize.Height;
		}

		private void inputPanel_EnabledChanged(object sender, System.EventArgs e)
		{
			ResizeTabControl();
		}

		private void textBox_GotFocus(object sender, EventArgs e)
		{
			// Nice in theory, but seems to cause frequent ObjectDisposedExceptions.
			// These can be triggered by opening and closing the input panel quickly,
			// so they may be due to the input panel closing while the resize event
			// is still in progress.
			inputPanel.Enabled = true;
		}

		private void textBox_LostFocus(object sender, EventArgs e)
		{
			inputPanel.Enabled = false;
		}

		private void FormSettings_Resize(object sender, System.EventArgs e)
		{
			ResizeTabControl();
		}

		private string GetComPortDescription(string portSpec)
		{
			string ret = "";
			StorableHashtable settings = new StorableHashtable(portSpec);
			if (settings.ContainsKey("type"))
			{
				switch (settings["type"].ToString())
				{
					case "comport":
						if (settings.ContainsKey("port_name"))
							ret = settings["port_name"].ToString();
						break;
					case "file":
						if (settings.ContainsKey("file_name"))
							ret = settings["file_name"].ToString();
						break;
				}
			}
			return ret;
		}
	}
}
