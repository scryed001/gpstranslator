/*
$Id: FormMain.cs,v 1.5 2006/05/25 10:14:36 andrew_klopper Exp $

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

namespace GPSProxy
{
	using System;
	using System.Drawing;
	using System.Collections;
	using System.Windows.Forms;
	using System.Threading;
	using System.Runtime.InteropServices;
	using System.IO;
	using System.Reflection;
	using OpenNETCF;
	using OpenNETCF.Drawing;
	using OpenNETCF.Windows.Forms;
	using OpenNETCF.Threading;
	using OpenNETCF.Win32;
	using GPSProxy.Extension;
	using Nini.Config;

	/// <summary>
	/// Summary description for FormMain.
	/// </summary>
	public class FormMain : System.Windows.Forms.Form, IApplication, IMessageFilter
	{
		private const string version = "2.2";

		private class ExtensionDetails
		{
			public string ExtensionDLL;
			public Type ExtensionType;
			public GPSProxyExtensionAttribute ExtensionAttribute;

			public ExtensionDetails(string extensionDLL, Type extensionType, GPSProxyExtensionAttribute extensionAttribute)
			{
				ExtensionDLL = extensionDLL;
				ExtensionType = extensionType;
				ExtensionAttribute = extensionAttribute;
			}
		}

		private class ActiveExtensionInfo: IComparable
		{
			public ExtensionDetails ExtensionDetails;
			public int ExtensionID;
			public IExtension Extension;
			public Control Control;
			public Button MenuButton;

			public int CompareTo(Object obj)
			{
				int ret = ExtensionDetails.ExtensionAttribute.Precedence.CompareTo(
					((ActiveExtensionInfo)obj).ExtensionDetails.ExtensionAttribute.Precedence);
				if (ret == 0)
					return ExtensionID.CompareTo(((ActiveExtensionInfo)obj).ExtensionID);
				else
					return ret;
			}
		}

		private const int WM_USER = 0x0400;

		private const int WM_UPDATELOG = WM_USER;
		private const int WM_WAKEUP = WM_USER + 1;
		private const int WM_GPSFIX = WM_USER + 2;
		private const int WM_GPSSAT = WM_USER + 3;

		private const int maxLogTextLength = 16384;
		private const string wakeupEventName = "GPSPROXY_WAKEUP";
		private const string wakeupExecutable = "SetEvent.exe";
		private const string virtualComPortDll = "VirtualComPort.dll";
		private const string virtualComPortFriendlyName = "GPSProxy Virtual COM Port";

		private bool started = false;

		private IntPtr hwnd;

		private bool shuttingDown = false;
		private EventWaitHandle wakeupEvent = new EventWaitHandle(false, EventResetMode.AutoReset, wakeupEventName);

		private string displayString = String.Empty;
		private Mutex displayStringMutex = new Mutex();

		private IGPSFix newFix;
		private Mutex newFixMutex = new Mutex();

		private IGPSSatelliteVehicle[] newSatelliteVehicles;
		private Mutex newSatelliteVehiclesMutex = new Mutex();

		private string executablePath;
		private string extensionPath;
		private IConfigSource configSource;
		private Nini.Config.IConfig configSettings;

		private ExtensionDetails[] providerExtensionDetails;
		private ExtensionDetails[] consumerExtensionDetails;

		private ActiveExtensionInfo gpsProvider;
		private ActiveExtensionInfo[] activeExtensions;
		private ActiveExtensionInfo[] sortedActiveExtensions;

		private RegistryKey virtualComPortRootKey;
		private VirtualComPort[] virtualComPorts;

		private FormExtensions extensionsDialog;
		private bool repopulatingExtensionsDialog = false;

		private FormVirtualComPorts virtualComPortsDialog;
		private bool repopulatingVirtualComPortsDialog = false;

		private ScrollablePanel mainMenuPanel;
		private Button AboutButton;
		private Button SettingsButton;
		private Button ExtensionsButton;
		private Button VirtualComPortsButton;
		private Button ExitButton;

		private ScrollablePanel pageMenuPanel;
		private System.Windows.Forms.Timer reconnectTimer;

		private StreamWriter logStream;

		private EventHandler resizeEventHandler;

		private Settings settings =
			new Settings(new Setting[] {
										   new Setting("autoStart", "Start Proxy Automatically on Launch", "Launch", "boolean", false),
										   new Setting("runExternalApp", "Run External App after Proxy Startup", "Launch", "boolean", false),
										   new Setting("externalApp", "External Application", "Launch", "exefile", ""),
										   new Setting("autoReconnect", "Reconnect after Power-on", "Power", "boolean", false),
										   new Setting("reconnectDelay", "Power-on Reconnect Delay (Seconds)", "Power", "updown", 8, "maxlength", 2, "min", 1, "max", 30),
										   new Setting("logToFile", "Save Log to File", "Log", "boolean", false),
										   new Setting("logFile", "Log File Name", "Log", "savefile", "", "filter", "Log files (*.txt)|*.txt")
									   });

		private enum NOTIFICATION_EVENT: int
		{
			NOTIFICATION_EVENT_NONE = 0,
			NOTIFICATION_EVENT_TIME_CHANGE = 1,
			NOTIFICATION_EVENT_SYNC_END = 2,
			NOTIFICATION_EVENT_ON_AC_POWER = 3,
			NOTIFICATION_EVENT_OFF_AC_POWER = 4,
			NOTIFICATION_EVENT_NET_CONNECT = 5,
			NOTIFICATION_EVENT_NET_DISCONNECT = 6,
			NOTIFICATION_EVENT_DEVICE_CHANGE = 7,
			NOTIFICATION_EVENT_IR_DISCOVERED = 8,
			NOTIFICATION_EVENT_RS232_DETECTED = 9,
			NOTIFICATION_EVENT_RESTORE_END = 10,
			NOTIFICATION_EVENT_WAKEUP = 11,
			NOTIFICATION_EVENT_TZ_CHANGE = 12,
			NOTIFICATION_EVENT_MACHINE_NAME_CHANGE = 13
		}
		
		[DllImport("coredll.dll")]
		private static extern int CeRunAppAtEvent(string pwszAppName, NOTIFICATION_EVENT lWhichEvent);

		private System.Windows.Forms.Button buttonPage;
		private System.Windows.Forms.Button buttonMenu;
		private OpenNETCF.Windows.Forms.NotifyIcon notifyIcon;
		private System.Windows.Forms.Panel pluginPanel;
		private System.Windows.Forms.Panel logPanel;
		private OpenNETCF.Windows.Forms.TextBoxEx logTextBox;
		private System.Windows.Forms.ContextMenu contextMenuLog;
		private System.Windows.Forms.MenuItem menuItemCopy;
		private System.Windows.Forms.Button buttonStartStop;

		public FormMain()
		{
			// Set the resize event handler.
			resizeEventHandler = new EventHandler(FormMain_Resize);
			Resize += resizeEventHandler;

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Get the window handle.
			Capture = true;
			hwnd = OpenNETCF.Win32.Win32Window.GetCapture();
			Capture = false;

			// Add a message filter.
			ApplicationEx.AddMessageFilter(this);

			// Load the configuration.
			executablePath = ApplicationEx.StartupPath;
			extensionPath = Path.Combine(executablePath, "Extensions");
			string iniFileName = Path.Combine(executablePath, "GPSProxy.ini");
			if (! File.Exists(iniFileName))
			{
				try
				{
					File.Copy(Path.Combine(executablePath, "GPSProxy.ini.default"), iniFileName, false);
				}
				catch
				{
				}
			}
			if (! File.Exists(iniFileName))
				using (File.Create(iniFileName))
				{
				}
			configSource = new IniConfigSource(iniFileName);
			configSource.Alias.AddAlias("On", true);
			configSource.Alias.AddAlias("Off", false);
			configSource.Alias.AddAlias("True", true);
			configSource.Alias.AddAlias("False", false);
			if (configSource.Configs["Settings"] == null)
				configSource.AddConfig("Settings");

			configSettings = configSource.Configs["Settings"];
			settings["autoStart"].Value = configSettings.GetBoolean("Start Proxy Automatically on Launch", (bool)settings["autoStart"].Value);
			settings["runExternalApp"].Value = configSettings.GetBoolean("Run External App after Proxy Startup", (bool)settings["runExternalApp"].Value);
			settings["externalApp"].Value = configSettings.GetString("External Application", (string)settings["externalApp"].Value);
			settings["autoReconnect"].Value = configSettings.GetBoolean("Reconnect after Power-on", (bool)settings["autoReconnect"].Value);
			settings["reconnectDelay"].Value = configSettings.GetInt("Power-on Reconnect Delay", (int)settings["reconnectDelay"].Value);

			// Create the main and page menu panels.
			mainMenuPanel = new ScrollablePanel();
			mainMenuPanel.Visible = false;
			Controls.Add(mainMenuPanel);
			mainMenuPanel.Size = pluginPanel.ClientSize;
			mainMenuPanel.BringToFront();

			pageMenuPanel = new ScrollablePanel();
			pageMenuPanel.Visible = false;
			Controls.Add(pageMenuPanel);
			pageMenuPanel.Size = pluginPanel.ClientSize;
			pageMenuPanel.BringToFront();

			// Add the main menu buttons.
			AboutButton = AddMenuButton(mainMenuPanel, "About...",
				new EventHandler(AboutButton_Click));
			SettingsButton = AddMenuButton(mainMenuPanel, "Settings...",
				new EventHandler(SettingsButton_Click), true);
			ExtensionsButton = AddMenuButton(mainMenuPanel, "Extensions...",
				new EventHandler(ExtensionsButton_Click));
			VirtualComPortsButton = AddMenuButton(mainMenuPanel, "Virtual COM Ports...",
				new EventHandler(VirtualComPortsButton_Click));
			ExitButton = AddMenuButton(mainMenuPanel, "Exit",
				new EventHandler(ExitButton_Click), true);

			// Load the extension DLLs.
			LoadExtensions();

			// Activate extensions according to the configuration file.
			ActivateExtensions();

			// NOTE: do not attempt to show an message boxes before this point, as this triggers
			// other form events that expect the activeExtensions array to have been created.

			// Load the virtual COM port drivers.
			virtualComPortRootKey = Registry.LocalMachine.CreateSubKey(@"Drivers\GPSProxyVirtualCOMPorts");
			if (virtualComPortRootKey == null)
			{
				MessageBox.Show(@"Unable to open/create registry key: " + virtualComPortRootKey.Name,
					"Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
			}
			else
			{
				LoadVirtualComPortDrivers();
			}

			// Create and populate the extensions dialog.
			extensionsDialog = new FormExtensions();
			extensionsDialog.ComboBoxProvider.SelectedIndexChanged += new EventHandler(ComboBoxProvider_SelectedIndexChanged);
			extensionsDialog.ListBoxExtensions.SelectedIndexChanged += new EventHandler(ListBoxExtensions_SelectedIndexChanged);
			extensionsDialog.ButtonConfigureProvider.Click += new EventHandler(ButtonConfigureProvider_Click);
			extensionsDialog.ButtonAddExtension.Click += new EventHandler(ButtonAddExtension_Click);
			extensionsDialog.ButtonRemoveExtension.Click += new EventHandler(ButtonRemoveExtension_Click);
			extensionsDialog.ButtonConfigureExtension.Click += new EventHandler(ButtonConfigureExtension_Click);
			RepopulateExtensionsDialog();

			// Create and populate the virtual COM port dialog.
			virtualComPortsDialog = new FormVirtualComPorts();
			virtualComPortsDialog.VirtualComPortListBox.SelectedIndexChanged += new EventHandler(VirtualComPortListBox_SelectedIndexChanged);
			virtualComPortsDialog.ButtonAdd.Click += new EventHandler(ButtonAddVirtualComPort_Click);
			virtualComPortsDialog.ButtonRemove.Click += new EventHandler(ButtonRemoveVirtualComPort_Click);
			RepopulateVirtualComPortsDialog();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if (disposing)
			{
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
			this.buttonPage = new System.Windows.Forms.Button();
			this.buttonMenu = new System.Windows.Forms.Button();
			this.buttonStartStop = new System.Windows.Forms.Button();
			this.notifyIcon = new OpenNETCF.Windows.Forms.NotifyIcon();
			this.pluginPanel = new System.Windows.Forms.Panel();
			this.logPanel = new System.Windows.Forms.Panel();
			this.logTextBox = new OpenNETCF.Windows.Forms.TextBoxEx();
			this.contextMenuLog = new System.Windows.Forms.ContextMenu();
			this.menuItemCopy = new System.Windows.Forms.MenuItem();
			this.reconnectTimer = new System.Windows.Forms.Timer();
			// 
			// buttonPage
			// 
			this.buttonPage.Location = new System.Drawing.Point(0, 256);
			this.buttonPage.Size = new System.Drawing.Size(88, 38);
			this.buttonPage.Text = "Page";
			this.buttonPage.Click += new System.EventHandler(this.buttonPage_Click);
			// 
			// buttonMenu
			// 
			this.buttonMenu.Location = new System.Drawing.Point(88, 256);
			this.buttonMenu.Size = new System.Drawing.Size(64, 38);
			this.buttonMenu.Text = "Menu";
			this.buttonMenu.Click += new System.EventHandler(this.buttonMenu_Click);
			// 
			// buttonStartStop
			// 
			this.buttonStartStop.Location = new System.Drawing.Point(152, 256);
			this.buttonStartStop.Size = new System.Drawing.Size(88, 38);
			this.buttonStartStop.Text = "Start";
			this.buttonStartStop.Click += new System.EventHandler(this.buttonStartStop_Click);
			// 
			// notifyIcon
			// 
			// TODO: Code generation for 'this.notifyIcon.IconHandle' failed because of Exception 'Invalid Primitive Type: System.IntPtr. Only CLS compliant primitive types can be used. Consider using CodeObjectCreateExpression.'.
			this.notifyIcon.Text = "GPSProxy";
			this.notifyIcon.Click += new System.EventHandler(this.notifyIcon_Click);
			// 
			// pluginPanel
			// 
			this.pluginPanel.Size = new System.Drawing.Size(240, 256);
			// 
			// logPanel
			// 
			this.logPanel.Controls.Add(this.logTextBox);
			this.logPanel.Size = new System.Drawing.Size(240, 256);
			// 
			// logTextBox
			// 
			this.logTextBox.ContextMenu = this.contextMenuLog;
			this.logTextBox.Multiline = true;
			this.logTextBox.ReadOnly = true;
			this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.logTextBox.Size = new System.Drawing.Size(240, 256);
			this.logTextBox.Style = OpenNETCF.Windows.Forms.TextBoxStyle.Default;
			this.logTextBox.Text = "";
			// 
			// contextMenuLog
			// 
			this.contextMenuLog.MenuItems.Add(this.menuItemCopy);
			// 
			// menuItemCopy
			// 
			this.menuItemCopy.Text = "Copy";
			this.menuItemCopy.Click += new System.EventHandler(this.menuItemCopy_Click);
			// 
			// reconnectTimer
			// 
			this.reconnectTimer.Tick += new System.EventHandler(this.reconnectTimer_Tick);
			// 
			// FormMain
			// 
			this.ClientSize = new System.Drawing.Size(240, 294);
			this.Controls.Add(this.logPanel);
			this.Controls.Add(this.pluginPanel);
			this.Controls.Add(this.buttonStartStop);
			this.Controls.Add(this.buttonMenu);
			this.Controls.Add(this.buttonPage);
			this.Text = "GPSProxy";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FormMain_Closing);
			this.Load += new System.EventHandler(this.FormMain_Load);

		}
		#endregion

		#region IApplication Members

		public string ExecutablePath
		{
			get { return executablePath; }
		}

		public string ExtensionPath
		{
			get { return extensionPath; }
		}

		public void BringToForeground()
		{
			OpenNETCF.Win32.Win32Window.SetForegroundWindow(hwnd);	
		}

		public void ShowExtension(IExtension sender)
		{
			// Inefficient as we go through the extensions again when the button click is faked,
			// but it will do.
			int i = 0;
			while ((i < activeExtensions.Length) && (activeExtensions[i].Extension != sender))
				i++;
			if ((i < activeExtensions.Length) && (activeExtensions[i].MenuButton != null))
				PageMenuItem_Click(activeExtensions[i].MenuButton, null);
		}

		void GPSProxy.Extension.IApplication.LogMessage(IExtension sender, string message)
		{
			ExtensionDetails details = GetExtensionDetails(sender.GetType());
			LogMessage(details.ExtensionAttribute.ExtensionName + ": " + message);
		}

		public IPort GetPort(IExtension sender, string portSpec)
		{
			StorableHashtable settings = new StorableHashtable(portSpec);
			if (! settings.ContainsKey("type"))
				throw new Exception("COM port type not specified");
			switch ((string)settings["type"])
			{
				// Regular COM port.
				case "comport":
					if (! settings.ContainsKey("port_name"))
						throw new Exception("No COM port name specified");
					if (! settings.ContainsKey("baud_rate"))
						throw new Exception("No baud rate specified");
					return new OpenNETCFSerialPort((string)settings["port_name"], int.Parse((string)settings["baud_rate"]));

				// File
				case "file":
					if (! settings.ContainsKey("file_name"))
						throw new Exception("No file name specified");
					if (! settings.ContainsKey("file_mode"))
						throw new Exception("No file mode specified");
					return new FileBasedPort((string)settings["file_name"], (string)settings["file_mode"]);

				// Oops!
				default:
					throw new Exception("Invalid COM port type: " + (string)settings["type"]);
			}
		}

		public bool ShowSettingsDialog(IExtension sender, Settings settings, SettingsValidator validator)
		{
			FormSettings settingsDialog = new FormSettings(settings, validator);
			try
			{
				return settingsDialog.ShowDialog() == DialogResult.OK;
			}
			finally
			{
				settingsDialog.Dispose();
			}
		}

		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>

		static void Main() 
		{
			ApplicationEx.Run(new FormMain());
		}

		private void FormMain_Load(object sender, System.EventArgs e)
		{
			notifyIcon.Visible = true;

			// Start the power monitoring thread.
			Thread powerMonitorThread = new Thread(new ThreadStart(PowerMonitorThread));
			powerMonitorThread.Priority = ThreadPriority.Highest;
			powerMonitorThread.Start();

			// Start the proxy if required.
			if ((bool)settings["autoStart"].Value)
				startProxy();
		}

		private void FormMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// Nuke the resize event as we end up with ObjectDisposed exceptions on
			// activeExtensions if we don't.
			Resize -= resizeEventHandler;

			// Stop the proxy.
			stopProxy();

			// Get rid of the notification icon.
			notifyIcon.Visible = false;

			// Shut down the power monitoring thread.
			shuttingDown = true;
			wakeupEvent.Set();

			// Deactive the extensions.
			DeactivateExtensions();

			// Unload the virtual COM ports.
			UnloadVirtualComPortDrivers();

			// Clean up other resources.
			if (extensionsDialog != null)
				extensionsDialog.Dispose();
			if (virtualComPortsDialog != null)
				virtualComPortsDialog.Dispose();
			if (virtualComPortRootKey != null)
				virtualComPortRootKey.Dispose();
		}

		private Button AddMenuButton(ScrollablePanel menuPanel, string caption,	EventHandler click)
		{
			return AddMenuButton(menuPanel, caption, click, false);
		}
		
		private Button AddMenuButton(ScrollablePanel menuPanel, string caption,	EventHandler click, bool beginGroup)
		{
			int offset = menuPanel.ContentHeight;
			if (offset > 0)
				offset += beginGroup ? 4 : 2;
			else
				offset = 2;

			Button button = new Button();
			button.Text = caption;
			button.Location = new Point(2, offset);
			button.Size = new Size(menuPanel.ContentWidth - 4, 30);
			button.Click += click;
			menuPanel.ContentControls.Add(button);

			menuPanel.ContentHeight = button.Bottom;

			return button;
		}

		private void ShowMenuPanel(ScrollablePanel menuPanel)
		{
			//menuPanel.BringToFront();
			menuPanel.Visible = true;
		}

		private void ResizeMenuPanel(ScrollablePanel menuPanel, Size size)
		{
			menuPanel.Size = size;
			foreach (Control control in menuPanel.ContentControls)
				control.Width = menuPanel.ContentWidth - 4;
		}

		private void HideMenuPanel(ScrollablePanel menuPanel)
		{
			menuPanel.Visible = false;
		}

		private void UpdateLog() 
		{
			if (logStream != null)
			{
				try
				{
					logStream.Write(displayString);
				}
				catch
				{
				}
			}
			if (logTextBox.Text.Length > maxLogTextLength) 
				logTextBox.Text = string.Empty;
			logTextBox.Text += displayString;
			displayString = string.Empty;
			logTextBox.SelectionStart = logTextBox.Text.Length;
			logTextBox.ScrollToCaret();
		}

		private void LogMessage(string message)
		{
			if (displayStringMutex.WaitOne())
			{
				try
				{
					displayString += message + "\r\n";
					PostMessage(WM_UPDATELOG);
				}
				finally
				{
					displayStringMutex.ReleaseMutex();
				}
			}
		}
		
		private void PowerMonitorThread()
		{
			string cmdLine = Path.Combine(executablePath, wakeupExecutable);
			if (CeRunAppAtEvent(cmdLine, NOTIFICATION_EVENT.NOTIFICATION_EVENT_WAKEUP) != 0)
			{
				while (wakeupEvent.WaitOne() && ! shuttingDown) 
					PostMessage(WM_WAKEUP);
				if (CeRunAppAtEvent(cmdLine, NOTIFICATION_EVENT.NOTIFICATION_EVENT_NONE) == 0)
					LogMessage("ERROR: CeRunAppAtEvent(NOTIFICATION_EVENT_NONE) failed");
			}
			else
				LogMessage("ERROR: CeRunAppAtEvent(NOTIFICATION_EVENT_WAKEUP) failed");
		}	

		private void buttonPage_Click(object sender, System.EventArgs e)
		{
			if (mainMenuPanel.Visible)
				HideMenuPanel(mainMenuPanel);
			if (pageMenuPanel.Visible)
				HideMenuPanel(pageMenuPanel);
			else
				ShowMenuPanel(pageMenuPanel);
		}

		private void buttonMenu_Click(object sender, System.EventArgs e)
		{
			if (pageMenuPanel.Visible)
				HideMenuPanel(pageMenuPanel);
			if (mainMenuPanel.Visible)
				HideMenuPanel(mainMenuPanel);
			else
				ShowMenuPanel(mainMenuPanel);
		}

		private void buttonStartStop_Click(object sender, System.EventArgs e)
		{
			if (pageMenuPanel.Visible)
				HideMenuPanel(pageMenuPanel);
			if (mainMenuPanel.Visible)
				HideMenuPanel(mainMenuPanel);
			if (started)
				stopProxy();
			else
				startProxy();
		}

		private void notifyIcon_Click(object sender, System.EventArgs e)
		{
			BringToForeground();
		}

		private void AboutButton_Click(object sender, System.EventArgs e)
		{
			HideMenuPanel(mainMenuPanel);
			if (reconnectTimer.Enabled)
				return;
			FormAbout about = new FormAbout();
			try
			{
				about.Version = version;
				about.ShowDialog();
			}
			finally
			{
				about.Dispose();
			}		
		}

		private void SettingsButton_Click(object sender, System.EventArgs e)
		{
			HideMenuPanel(mainMenuPanel);
			if (reconnectTimer.Enabled)
				return;

			FormSettings dialog = new FormSettings(settings, null);
			try
			{
				if (dialog.ShowDialog() == DialogResult.OK)
				{
					configSettings.Set("Start Proxy Automatically on Launch", (bool)settings["autoStart"].Value);
					configSettings.Set("Run External App after Proxy Startup", (bool)settings["runExternalApp"].Value);
					configSettings.Set("External Application", (string)settings["externalApp"].Value);
					configSettings.Set("Reconnect after Power-on", (bool)settings["autoReconnect"].Value);
					configSettings.Set("Power-on Reconnect Delay", (int)settings["reconnectDelay"].Value);
					configSource.Save();
				}
			}
			finally
			{
				dialog.Dispose();
			}
		}

		private void ExtensionsButton_Click(object sender, System.EventArgs e)
		{
			HideMenuPanel(mainMenuPanel);
			if (reconnectTimer.Enabled)
				return;

			// Show the extensions dialog.
			extensionsDialog.ShowDialog();
		}

		private void VirtualComPortsButton_Click(object sender, System.EventArgs e)
		{
			HideMenuPanel(mainMenuPanel);
			if (reconnectTimer.Enabled)
				return;

			// Show the virtual COM ports dialog.
			virtualComPortsDialog.ShowDialog();
		}

		private void ExitButton_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void menuItemCopy_Click(object sender, System.EventArgs e)
		{
			logTextBox.Focus();
			logTextBox.SelectAll();
			logTextBox.Copy();
		}

		private void startProxy()
		{
			reconnectTimer.Enabled = false;
			if (! started)
			{
				// Check that a provider has been defined.
				if (gpsProvider == null)
				{
					MessageBox.Show("No GPS Provider has been selected.", "Error", MessageBoxButtons.OK,
						MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
					return;
				}

				// Open the log file if required.
				if ((bool)settings["logToFile"].Value)
				{
					try
					{
						logStream = new StreamWriter((string)settings["logFile"].Value);
					}
					catch (Exception e)
					{
						MessageBox.Show("Error opening log file for writing: " + (string)settings["logFile"].Value + ": " + e.Message,
							"Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
						return;
					}
				}

				// Start the extensions.
				int lastStartedIndex = -1;
				try
				{
					for (int i = 0; i < sortedActiveExtensions.Length; i++)
					{
						try
						{
							sortedActiveExtensions[i].Extension.Start();
							lastStartedIndex = i;
						}
						catch (Exception e2)
						{
							throw new Exception("The '" + sortedActiveExtensions[i].ExtensionDetails.ExtensionAttribute.ExtensionName +
								"' extension failed to start: " + e2.Message);
						}
					}

					// Start the provider.
					try
					{
						gpsProvider.Extension.Start();
					}
					catch (Exception e2)
					{
						try
						{
							gpsProvider.Extension.Stop();
						}
						catch
						{
						}
						throw new Exception("The '" + gpsProvider.ExtensionDetails.ExtensionAttribute.ExtensionName +
							"' provider failed to start: " + e2.Message);
					}

					// Success.
					started = true;
				}
				catch (Exception e)
				{
					// Display an error message.
					MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK,
						MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);

					// Stop all of the started extensions (excluding the failed one) in reverse order.
					for (int i = lastStartedIndex; i >= 0; i--)
					{
						try
						{
							sortedActiveExtensions[i].Extension.Stop();
						}
						catch (Exception e2)
						{
							MessageBox.Show("The '" + sortedActiveExtensions[i].ExtensionDetails.ExtensionAttribute.ExtensionName +
								"' extension failed to stop: " + e2.Message, "Error", MessageBoxButtons.OK,
								MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
						}
					}
				}

				// Update the user interface if we started successfully.
				if (started)
				{
					buttonStartStop.Text = "Stop";
					AboutButton.Enabled = false;
					SettingsButton.Enabled = false;
					ExtensionsButton.Enabled = false;
					VirtualComPortsButton.Enabled = false;
					if ((bool)settings["runExternalApp"].Value)
					{
						try
						{
							OpenNETCF.Diagnostics.Process.Start((string)settings["externalApp"].Value);
						}
						catch (Exception e)
						{
							LogMessage("Error starting " + (string)settings["externalApp"].Value + ": " + e.Message);
						}
					}
				}
			}
		}

		private void stopProxy()
		{
			reconnectTimer.Enabled = false;
			if (started)
			{
				// Stop the provider.
				try
				{
					gpsProvider.Extension.Stop();
				}
				catch (Exception e)
				{
					// Display an error message.
					MessageBox.Show("The '" + gpsProvider.ExtensionDetails.ExtensionAttribute.ExtensionName +
						"' provider failed to start: " + e.Message, "Error", MessageBoxButtons.OK,
						MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
				}

				// Stop all of the extensions in reverse order.
				for (int i = sortedActiveExtensions.Length - 1; i >= 0; i--)
				{
					try
					{
						sortedActiveExtensions[i].Extension.Stop();
					}
					catch (Exception e)
					{
						MessageBox.Show("The '" + sortedActiveExtensions[i].ExtensionDetails.ExtensionAttribute.ExtensionName +
							"' extension failed to stop: " + e.Message, "Error", MessageBoxButtons.OK,
							MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
					}
				}

				// Close the log file if one was opened.
				if (logStream != null)
				{
					logStream.Close();
					logStream = null;
				}

				// Update the user interface.
				buttonStartStop.Text = "Start";
				AboutButton.Enabled = true;
				SettingsButton.Enabled = true;
				ExtensionsButton.Enabled = true;
				VirtualComPortsButton.Enabled = true;
				
				started = false;
			}
		}

		private void Wakeup()
		{
			if ((bool)settings["autoReconnect"].Value && started)
			{
				stopProxy();
				reconnectTimer.Interval = (int)settings["reconnectDelay"].Value * 1000;
				reconnectTimer.Enabled = true;
			}
		}

		private void LoadExtensions()
		{
			Type extensionAttributeType = typeof(GPSProxyExtensionAttribute);
			Type extensionInterfaceType = typeof(IExtension);
			ArrayList extensionList = new ArrayList();
			int numProviderExtensions = 0;
			string[] dllFiles = Directory.GetFiles(extensionPath, "*.dll");
			foreach (string dll in dllFiles)
			{
				// Skip DLLs that are unnecessarily copied across during the Visual Studio deployment process.
				string baseName = Path.GetFileName(dll).ToLower();
				if ((baseName == "gpsproxy.extension.dll") || (baseName == "gpsproxy.common.dll"))
					continue;

				// Load the extension DLL and check that it contains a type with the specified attribute that
				// implements the specified interface.
				try 
				{
					Assembly assembly = Assembly.LoadFrom(dll);
 					if (assembly == null)
						throw new Exception(dll + " is not a valid assembly");

					Type extensionType = null;
					foreach (Type type in assembly.GetTypes())
					{
						if (type.IsDefined(extensionAttributeType, false))
						{
							extensionType = type;
							break;
						}
					}
					if (extensionType == null)
						throw new Exception("Did not find any types with the " + extensionAttributeType.Name + " attribute in " + dll);

					bool validExtension = false;
					foreach (Type interfaceType in extensionType.GetInterfaces())
					{
						if (extensionInterfaceType.Equals(interfaceType))
						{
							GPSProxyExtensionAttribute attribute =
								(GPSProxyExtensionAttribute)Attribute.GetCustomAttribute(extensionType,
									extensionAttributeType);
							extensionList.Add(new ExtensionDetails(Path.GetFileName(dll), extensionType,
								attribute));
							validExtension = true;
							if (attribute.IsProvider)
								numProviderExtensions++;
							break;
						}
					}
					if (! validExtension)
						throw new Exception(extensionType.FullName + " in " + dll + " does not implement the IExtension interface");
				}
				catch (Exception e)
				{
					MessageBox.Show("Error loading extension: " + e.Message, "Error",
						MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
				}
			}

			// Create arrays for the provider and consumer extensions.
			int p = 0;
			int c = 0;
			providerExtensionDetails = new ExtensionDetails[numProviderExtensions];
			consumerExtensionDetails = new ExtensionDetails[extensionList.Count - numProviderExtensions];
			for (int i = 0; i < extensionList.Count; i++)
			{
				if (((ExtensionDetails)extensionList[i]).ExtensionAttribute.IsProvider)
					providerExtensionDetails[p++] = (ExtensionDetails)extensionList[i];
				else
					consumerExtensionDetails[c++] = (ExtensionDetails)extensionList[i];
			}
		}

		private ExtensionDetails GetExtensionDetails(Type extensionType)
		{
			int i = 0;
			while ((i < providerExtensionDetails.Length) && ! providerExtensionDetails[i].ExtensionType.Equals(extensionType))
				i++;
			if (i < providerExtensionDetails.Length)
				return providerExtensionDetails[i];
			i = 0;
			while ((i < consumerExtensionDetails.Length) && ! consumerExtensionDetails[i].ExtensionType.Equals(extensionType))
				i++;
			if (i < consumerExtensionDetails.Length)
				return consumerExtensionDetails[i];
			throw new Exception("Invalid extension type: " + extensionType.Name);
		}

		private void RebuildPageMenu()
		{
			// Clear the current menu contents.
			pageMenuPanel.ContentControls.Clear();
			pageMenuPanel.ContentHeight = 0;

			// Add the GPS provider to the menu if required.
			Button firstExtensionButton = null;
			if ((gpsProvider != null) && (gpsProvider.Control != null))
			{
				// Can't reuse controls, so we need to create new ones each time.
				gpsProvider.MenuButton = AddMenuButton(pageMenuPanel, 
					gpsProvider.ExtensionDetails.ExtensionAttribute.ExtensionName,
					new EventHandler(PageMenuItem_Click));
				if (firstExtensionButton == null)
					firstExtensionButton = gpsProvider.MenuButton;
			}

			// Add entries for any of the active extensions that require them.
			for (int i = 0; i < activeExtensions.Length; i++)
			{
				if (activeExtensions[i].Control != null)
				{
					activeExtensions[i].MenuButton = AddMenuButton(pageMenuPanel,
						activeExtensions[i].ExtensionDetails.ExtensionAttribute.ExtensionName,
						new EventHandler(PageMenuItem_Click));
					if (firstExtensionButton == null)
						firstExtensionButton = activeExtensions[i].MenuButton;
				}
			}

			// Add the log page to the context menu.
			AddMenuButton(pageMenuPanel, "Log", new EventHandler(LogMenuItem_Click), true);

			// Show the first page on the page menu.
			if (firstExtensionButton == null)
				LogMenuItem_Click(null, null);
			else
				PageMenuItem_Click(firstExtensionButton, null);
		}

		private ActiveExtensionInfo ActivateExtension(ExtensionDetails extensionDetails, int extensionID, string configSection)
		{
			// TODO: error recovery from exceptions during ExtensionInit?
			ActiveExtensionInfo extensionInfo = new ActiveExtensionInfo();
			extensionInfo.ExtensionID = extensionID;
			extensionInfo.ExtensionDetails = extensionDetails;

			extensionInfo.Extension = (IExtension)Activator.CreateInstance(extensionDetails.ExtensionType);
			if (configSource.Configs[configSection] == null)
				configSource.AddConfig(configSection);
			extensionInfo.Extension.ExtensionInit(this, extensionID, new NiniIConfigWrapper(configSource.Configs[configSection]));

			if (extensionInfo.ExtensionDetails.ExtensionAttribute.HasUserInterface)
			{
				extensionInfo.Control = extensionInfo.Extension.GetUserInterface();
				extensionInfo.Control.Visible = false;
				pluginPanel.Controls.Add(extensionInfo.Control);
				extensionInfo.Control.Location = new Point(0, 0);
				extensionInfo.Extension.ResizeUserInterface(pluginPanel.ClientSize);
			}

			if (extensionInfo.ExtensionDetails.ExtensionAttribute.IsProvider)
			{
				extensionInfo.Extension.NewGPSFix += new GPSFixEvent(Extension_NewGPSFix);
				extensionInfo.Extension.NewGPSSatelliteData += new GPSSatelliteDataEvent(Extension_NewGPSSatelliteData);
			}

			return extensionInfo;
		}

		private void DeactivateExtension(ActiveExtensionInfo extensionInfo)
		{
			if (extensionInfo.Control != null)
				pluginPanel.Controls.Remove(extensionInfo.Control);
			if (extensionInfo.MenuButton != null)
				pageMenuPanel.ContentControls.Remove(extensionInfo.MenuButton);
			extensionInfo.Extension.ExtensionDispose();
		}

		private void DeactivateExtensions()
		{
			if (gpsProvider != null)
			{
				DeactivateExtension(gpsProvider);
				gpsProvider = null;
			}
			if (activeExtensions != null)
			{
				for (int i = 0; i < activeExtensions.Length; i++)
				{
					if (activeExtensions[i] != null)
						DeactivateExtension(activeExtensions[i]);
				}
				activeExtensions = null;
			}
		}

		private void ActivateExtensions()
		{
			// First unload all currently loaded extensions, if any.
			DeactivateExtensions();

			// Create a GPS provider object, if one was specified.
			string gpsProviderDll = configSource.Configs["Settings"].GetString("GPS Provider Dll", "");
			if (gpsProviderDll != "") 
			{
				// Find the corresponding extension details.
				int i = 0;
				while ((i < providerExtensionDetails.Length) && (providerExtensionDetails[i].ExtensionDLL != gpsProviderDll))
					i++;
				if (i >= providerExtensionDetails.Length)
					throw new Exception("Invalid GPS provider DLL setting: " + gpsProviderDll);

				// Create the extension.
				gpsProvider = ActivateExtension(providerExtensionDetails[i], -1, gpsProviderDll);
			}

			// Read in the active extension list.
			int numActiveExtensions = configSource.Configs["Settings"].GetInt("Num Active Extensions", 0);
			activeExtensions = new ActiveExtensionInfo[numActiveExtensions];
			for (int i = 0; i < numActiveExtensions; i++) 
			{
				string section = "Active Extension " + i;
				Nini.Config.IConfig config = configSource.Configs[section];
				if (config == null)
					throw new Exception("Missing configuration file section: " + section);

				int extensionID = config.GetInt("Extension ID", 0);
				string extensionDll = config.GetString("Extension Dll", "");

				// Find the corresponding extension details.
				int j = 0;
				while ((j < consumerExtensionDetails.Length) && (consumerExtensionDetails[j].ExtensionDLL != extensionDll))
					j++;
				if (j >= consumerExtensionDetails.Length)
					throw new Exception("Invalid GPS extension DLL setting for active extension " + i + ": " + extensionDll);
				ExtensionDetails extensionDetails = consumerExtensionDetails[j];

				// Ensure that the extension ID is unique.
				j = 0;
				while ((j < i) && (activeExtensions[j].ExtensionID != extensionID))
					j++;
				if (j < i)
					throw new Exception("Active extensions " + i + " and " + j + " have a duplicate extension ID: " + extensionID);

				// Create and configure the extension.
				activeExtensions[i] = ActivateExtension(extensionDetails, extensionID,
					"Extension " + extensionID);
			}

			UpdateSortedActiveExtensions();
			RebuildPageMenu();
		}

		private void UpdateActiveExtensionConfig()
		{
			// First clear out the existing active extension configuration.
			int i = 0;
			while (i < configSource.Configs.Count)
			{
				if (configSource.Configs[i].Name.StartsWith("Active Extension "))
					configSource.Configs.RemoveAt(i);
				else
					i++;
			}

			// Write out the new configuration.
			configSource.Configs["Settings"].Set("Num Active Extensions", activeExtensions.Length);
			for (i = 0; i < activeExtensions.Length; i++) 
			{
				Nini.Config.IConfig newConfig = configSource.AddConfig("Active Extension " + i);
				newConfig.Set("Extension ID", activeExtensions[i].ExtensionID);
				newConfig.Set("Extension Dll", activeExtensions[i].ExtensionDetails.ExtensionDLL);
			}

			// Clean out configuration sections for extension IDs that are no longer present.
			i = 0;
			while (i < configSource.Configs.Count)
			{
				string configName = configSource.Configs[i].Name;
				if (configName.StartsWith("Extension ")) 
				{
					int j = 0;
					while ((j < activeExtensions.Length) && (configName != "Extension " + activeExtensions[j].ExtensionID))
						j++;
					if (j >= activeExtensions.Length)
						configSource.Configs.RemoveAt(i);
					else
						i++;
				}
				else
				{
					i++;
				}
			}

			foreach (Nini.Config.IConfig config in configSource.Configs)
			{
				if (config.Name.StartsWith("Extension "))
				{
					i = 0;
					while ((i < activeExtensions.Length) && (config.Name != "Extension " + activeExtensions[i].ExtensionID))
						i++;
					if (i >= activeExtensions.Length)
						configSource.Configs.Remove(config);
				}
			}

			// Save the configuration.
			configSource.Save();
		}

		private void LogMenuItem_Click(object sender, System.EventArgs e)
		{
			HideMenuPanel(pageMenuPanel);
			pluginPanel.Hide();
			logPanel.Show();
		}

		private void PageMenuItem_Click(object sender, System.EventArgs e)
		{
			HideMenuPanel(pageMenuPanel);

			// Locate the corresponding extension.
			int i = 0;
			while ((i < activeExtensions.Length) && (activeExtensions[i].MenuButton != sender))
				i++;
			if (i < activeExtensions.Length)
			{
				// Show the plugin panel.
				logPanel.Hide();
				pluginPanel.Show();

				// Hide all other extension forms.
				for (int j = 0; j < activeExtensions.Length; j++) 
				{
					if ((j != i) && (activeExtensions[j].Control != null))
						activeExtensions[j].Control.Hide();
				}

				// Show this extension form.
				activeExtensions[i].Control.Show();
			}
		}

		private void RepopulateExtensionsDialog()
		{
			repopulatingExtensionsDialog = true;
			try
			{
				extensionsDialog.ComboBoxProvider.Items.Clear();
				for (int i = 0; i < providerExtensionDetails.Length; i++)
				{
					extensionsDialog.ComboBoxProvider.Items.Add(providerExtensionDetails[i].ExtensionAttribute.ExtensionName);
					if ((gpsProvider != null) && (providerExtensionDetails[i] == gpsProvider.ExtensionDetails))
						extensionsDialog.ComboBoxProvider.SelectedIndex = extensionsDialog.ComboBoxProvider.Items.Count - 1;
				}

				extensionsDialog.ListBoxExtensions.Items.Clear();
				for (int i = 0; i < activeExtensions.Length; i++)
					extensionsDialog.ListBoxExtensions.Items.Add(activeExtensions[i].ExtensionDetails.ExtensionAttribute.ExtensionName);
			}
			finally
			{
				repopulatingExtensionsDialog = false;
			}

			updateExtensionsDialogButtonState();
		}

		private void updateExtensionsDialogButtonState()
		{
			extensionsDialog.ButtonConfigureProvider.Enabled = (extensionsDialog.ComboBoxProvider.SelectedIndex >= 0) &&
				gpsProvider.ExtensionDetails.ExtensionAttribute.RequiresConfiguration;
			extensionsDialog.ButtonRemoveExtension.Enabled =  extensionsDialog.ListBoxExtensions.SelectedIndex >= 0;
			extensionsDialog.ButtonConfigureExtension.Enabled =  (extensionsDialog.ListBoxExtensions.SelectedIndex >= 0) &&
				activeExtensions[extensionsDialog.ListBoxExtensions.SelectedIndex].ExtensionDetails.ExtensionAttribute.RequiresConfiguration;
		}

		private void ComboBoxProvider_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (! repopulatingExtensionsDialog)
			{
				// Unload the current provider and load the new one.
				if (gpsProvider != null)
					DeactivateExtension(gpsProvider);

				int selected = extensionsDialog.ComboBoxProvider.SelectedIndex;
				gpsProvider = ActivateExtension(providerExtensionDetails[selected], -1,
					providerExtensionDetails[selected].ExtensionDLL);

				// Update the saved configuration.
				configSource.Configs["Settings"].Set("GPS Provider Dll", providerExtensionDetails[selected].ExtensionDLL);
				configSource.Save();

				// Update the user interface.
				RebuildPageMenu();
				RepopulateExtensionsDialog();
			}
		}

		private void ListBoxExtensions_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (! repopulatingExtensionsDialog)
				updateExtensionsDialogButtonState();
		}

		private void ButtonConfigureProvider_Click(object sender, EventArgs e)
		{
			if (gpsProvider != null)
			{
				gpsProvider.Extension.ShowConfigurationDialog();
				configSource.Save();
			}
		}

		private void ButtonAddExtension_Click(object sender, EventArgs e)
		{
			FormExtensionSelect dialog = new FormExtensionSelect();
			try
			{
				for (int i = 0; i < consumerExtensionDetails.Length; i++)
					dialog.ComboBoxExtension.Items.Add(consumerExtensionDetails[i].ExtensionAttribute.ExtensionName);
				if (consumerExtensionDetails.Length > 0)
					dialog.ComboBoxExtension.SelectedIndex = 0;
				if (dialog.ShowDialog() == DialogResult.OK) 
				{
					// Ensure that single-instance extensions are not added more than once.
					int selected = dialog.ComboBoxExtension.SelectedIndex;
					if (! consumerExtensionDetails[selected].ExtensionAttribute.AllowMultipleInstances) 
					{
						int i = 0;
						while ((i < activeExtensions.Length) && (activeExtensions[i].ExtensionDetails != consumerExtensionDetails[selected]))
							i++;
						if (i < activeExtensions.Length)
						{
							MessageBox.Show("Multiple instances of this extension are not allowed.",
								"Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
							return;
						}
					}

					// Determine the next available extension ID.
					int extensionID = 0;
					for (int i = 0; i < activeExtensions.Length; i++)
						if (activeExtensions[i].ExtensionID > extensionID)
							extensionID = activeExtensions[i].ExtensionID;
					extensionID++;

					// Activate the extension.
					ActiveExtensionInfo extensionInfo = ActivateExtension(consumerExtensionDetails[selected],
						extensionID, "Extension " + extensionID);

					// Add it to the active extension array.
					ActiveExtensionInfo[] newInfo = new ActiveExtensionInfo[activeExtensions.Length + 1];
					activeExtensions.CopyTo(newInfo, 0);
					newInfo[activeExtensions.Length] = extensionInfo;
					activeExtensions = newInfo;

					// Update the saved configuration.
					UpdateActiveExtensionConfig();

					// Update the user interface.
					UpdateSortedActiveExtensions();
					RebuildPageMenu();
					RepopulateExtensionsDialog();
				}
			}
			finally
			{
				dialog.Dispose();
			}
		}

		private void ButtonRemoveExtension_Click(object sender, EventArgs e)
		{
			int i = extensionsDialog.ListBoxExtensions.SelectedIndex;
			if (i >= 0)
			{
				// Dispose of the extension.
				DeactivateExtension(activeExtensions[i]);

				// Remove it from the active extension array.
				ActiveExtensionInfo[] newInfo = new ActiveExtensionInfo[activeExtensions.Length - 1];
				for (int j = 0; j < i; j++) 
					newInfo[j] = activeExtensions[j];
				for (int j = i + 1; j < activeExtensions.Length; j++)
					newInfo[j - 1] = activeExtensions[j];
				activeExtensions = newInfo;

				// Update the saved configuration.
				UpdateActiveExtensionConfig();

				// Update the user interface.
				UpdateSortedActiveExtensions();
				RebuildPageMenu();
				RepopulateExtensionsDialog();
			}
		}

		private void ButtonConfigureExtension_Click(object sender, EventArgs e)
		{
			int i = extensionsDialog.ListBoxExtensions.SelectedIndex;
			if (i >= 0)
			{
				activeExtensions[i].Extension.ShowConfigurationDialog();
				configSource.Save();
			}
		}

		private void Extension_NewGPSFix(IExtension sender, IGPSFix fix)
		{
			if (newFixMutex.WaitOne())
			{
				newFix = fix;
				newFixMutex.ReleaseMutex();
				PostMessage(WM_GPSFIX);
			}
		}

		private void NewGPSFix()
		{
			IGPSFix fix = null;
			if (newFixMutex.WaitOne())
			{
				fix = newFix;
				newFix = null;
				newFixMutex.ReleaseMutex();
			}
			if (fix != null)
			{
				for (int i = 0; i < activeExtensions.Length; i++)
				{
					try
					{
						activeExtensions[i].Extension.ProcessGPSFix(fix);
					}
					catch (Exception ex)
					{
						((IApplication)this).LogMessage(activeExtensions[i].Extension, ex.Message);
					}
				}
			}
		}

		private void Extension_NewGPSSatelliteData(IExtension sender, IGPSSatelliteVehicle[] vehicles)
		{
			if (newSatelliteVehiclesMutex.WaitOne())
			{
				newSatelliteVehicles = vehicles;
				newSatelliteVehiclesMutex.ReleaseMutex();
				PostMessage(WM_GPSSAT);
			}
		}

		private void NewGPSSatelliteData()
		{
			IGPSSatelliteVehicle[] vehicles = null;
			if (newSatelliteVehiclesMutex.WaitOne())
			{
				vehicles = newSatelliteVehicles;
				newSatelliteVehicles = null;
				newSatelliteVehiclesMutex.ReleaseMutex();
			}
			if (vehicles != null)
			{
				for (int i = 0; i < activeExtensions.Length; i++)
				{
					try
					{
						activeExtensions[i].Extension.ProcessGPSSatelliteData(vehicles);
					}
					catch(Exception ex)
					{
						((IApplication)this).LogMessage(activeExtensions[i].Extension, ex.Message);
					}
				}
			}
		}

		private void PostMessage(int msg)
		{
			OpenNETCF.Win32.Win32Window.PostMessage(hwnd, msg, IntPtr.Zero, IntPtr.Zero);
		}

		#region IMessageFilter Members

		public bool PreFilterMessage(ref Microsoft.WindowsCE.Forms.Message m)
		{
			// All this is to get things to execute asynchronously from the user interface thread without
			// using Invoke(), which blocks.
			if (m.HWnd == hwnd)
			{
				switch (m.Msg)
				{
					case WM_UPDATELOG:
						UpdateLog();
						return true;
					case WM_WAKEUP:
						Wakeup();
						return true;
					case WM_GPSFIX:
						NewGPSFix();
						return true;
					case WM_GPSSAT:
						NewGPSSatelliteData();
						return true;
				}
			}
			return false;
		}

		#endregion

		private void FormMain_Resize(object sender, System.EventArgs e)
		{
			//if (Screen.PrimaryScreen.Bounds.Width > Screen.PrimaryScreen.Bounds.Height)

			// Move and resize the buttons.
			buttonPage.Top = ClientSize.Height - buttonPage.Height;
			buttonPage.Width = ClientSize.Width / 3;
			buttonMenu.Location = new Point(buttonPage.Width, buttonPage.Top);
			buttonMenu.Width = buttonPage.Width;
			buttonStartStop.Location = new Point(buttonMenu.Left + buttonMenu.Width,
				buttonPage.Top);
			buttonStartStop.Width = ClientSize.Width - buttonPage.Width - buttonMenu.Width;

			Size newSize = ClientSize;
			newSize.Height = newSize.Height - buttonPage.Height;

			logPanel.Size = newSize;
			logTextBox.Size = logPanel.ClientSize;

			pluginPanel.Size = newSize;
			for (int i = 0; i < activeExtensions.Length; i++)
			{
				if (activeExtensions[i].Control != null)
					activeExtensions[i].Extension.ResizeUserInterface(pluginPanel.ClientSize);
			}

			ResizeMenuPanel(mainMenuPanel, newSize);
			ResizeMenuPanel(pageMenuPanel, newSize);
		}

		private void reconnectTimer_Tick(object sender, System.EventArgs e)
		{
			reconnectTimer.Enabled = false;
			startProxy();
		}

		private void UpdateSortedActiveExtensions()
		{
			sortedActiveExtensions = new ActiveExtensionInfo[activeExtensions.Length];
			activeExtensions.CopyTo(sortedActiveExtensions, 0);
			Array.Sort(sortedActiveExtensions);
		}

		private void LoadVirtualComPortDrivers()
		{
			// First clear out the virtual COM port key.
			string[] subKeyNames = virtualComPortRootKey.GetSubKeyNames();
			foreach (string keyName in subKeyNames)
				virtualComPortRootKey.DeleteSubKeyTree(keyName);

			// Load the list of virtual COM ports to be created.
			string[] virtualComPortNames;
			string virtualComPortSetting = configSettings.GetString("Virtual COM Ports", "").Trim();
			if (virtualComPortSetting == "")
			{
				virtualComPortNames = new string[0];
			}
			else
			{
				virtualComPortNames = System.Text.RegularExpressions.Regex.Split(virtualComPortSetting,
					@"\s*,\s*", System.Text.RegularExpressions.RegexOptions.None);
			}

			// Create the virtual COM ports.
			int i = 0;
			virtualComPorts = new VirtualComPort[virtualComPortNames.Length];
			foreach (string portName in virtualComPortNames)
			{
				try
				{
					virtualComPorts[i] = new VirtualComPort(virtualComPortRootKey,
						virtualComPortDll, virtualComPortFriendlyName, portName);
					if (! virtualComPorts[i].DriverLoaded)
						MessageBox.Show("Error loading virtual COM port driver for " + portName,
							"Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
					i++;
				}
				catch (Exception e)
				{
					virtualComPorts[i] = null;
					MessageBox.Show("Error creating virtual COM port object for " + portName + ": " +
						e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand,
						MessageBoxDefaultButton.Button1);
				}
			}

			// Resize the virtual COM port array if we had any catastrophic failures.
			if (i < virtualComPortNames.Length)
			{
				VirtualComPort[] newPorts = new VirtualComPort[i];
				for (int j = 0; j < i; j++)
					newPorts[j] = virtualComPorts[j];
				virtualComPorts = newPorts;
			}
		}

		private void UnloadVirtualComPortDrivers()
		{
			if (virtualComPorts != null)
			{
				for (int i = 0; i < virtualComPorts.Length; i++)
				{
					if (virtualComPorts[i] != null)
						virtualComPorts[i].Dispose();
				}
				virtualComPorts = null;
			}
		}

		private void RepopulateVirtualComPortsDialog()
		{
			repopulatingVirtualComPortsDialog = true;
			try
			{
				virtualComPortsDialog.VirtualComPortListBox.Items.Clear();
				for (int i = 0; i < virtualComPorts.Length; i++)
					virtualComPortsDialog.VirtualComPortListBox.Items.Add(virtualComPorts[i].PortName +
						(virtualComPorts[i].DriverLoaded ? "" : " - not loaded"));
			}
			finally
			{
				repopulatingVirtualComPortsDialog = false;
			}

			UpdateVirtualComPortsDialogButtonState();
		}

		private void UpdateVirtualComPortsDialogButtonState()
		{
			virtualComPortsDialog.ButtonRemove.Enabled =  virtualComPortsDialog.VirtualComPortListBox.SelectedIndex >= 0;
		}

		private void VirtualComPortListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (repopulatingVirtualComPortsDialog)
				return;
			UpdateVirtualComPortsDialogButtonState();
		}

		private void ButtonAddVirtualComPort_Click(object sender, EventArgs e)
		{
			VirtualComPort newPort;
			try
			{
				newPort = new VirtualComPort(virtualComPortRootKey, virtualComPortDll,
					virtualComPortFriendlyName);
				if (! newPort.DriverLoaded)
				{
					MessageBox.Show("Failed to load driver for new virtual COM port. There may not be a free COM port number.",
						"Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
					newPort = null;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to create new virtual COM port object: " + ex.Message, "Error",
					MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
				newPort = null;
			}
			if (newPort != null)
			{
				// Successfully loaded the driver for a new virtual COM port. Add it to the array.
				VirtualComPort[] newPorts = new VirtualComPort[virtualComPorts.Length + 1];
				virtualComPorts.CopyTo(newPorts, 0);
				newPorts[virtualComPorts.Length] = newPort;
				virtualComPorts = newPorts;

				// Update the saved configuration.
				UpdateVirtualComPortsConfiguration();

				// Update the user interface.
				RepopulateVirtualComPortsDialog();
			}
		}

		private void ButtonRemoveVirtualComPort_Click(object sender, EventArgs e)
		{
			int i = virtualComPortsDialog.VirtualComPortListBox.SelectedIndex;
			if (i >= 0)
			{
				// Dispose of the COM port.
				if (virtualComPorts[i] != null)
					virtualComPorts[i].Dispose();

				// Remove it from the array.
				VirtualComPort[] newPorts = new VirtualComPort[virtualComPorts.Length - 1];
				for (int j = 0; j < i; j++) 
					newPorts[j] = virtualComPorts[j];
				for (int j = i + 1; j < virtualComPorts.Length; j++)
					newPorts[j - 1] = virtualComPorts[j];
				virtualComPorts = newPorts;

				// Update the saved configuration.
				UpdateVirtualComPortsConfiguration();

				// Update the user interface.
				RepopulateVirtualComPortsDialog();
			}
		}

		private void UpdateVirtualComPortsConfiguration()
		{
			string virtualComPortSetting = "";
			for (int i = 0; i < virtualComPorts.Length; i++)
				virtualComPortSetting += (virtualComPortSetting == "" ? "" : ",")
					+ virtualComPorts[i].PortName;
			configSettings.Set("Virtual COM Ports", virtualComPortSetting);
			configSource.Save();
		}
	}
}