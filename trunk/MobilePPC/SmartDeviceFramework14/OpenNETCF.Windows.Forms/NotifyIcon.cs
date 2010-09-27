//==========================================================================================
//
//		OpenNETCF.Windows.Forms.NotifyIcon
//		Copyright (c) 2003-2004, OpenNETCF.org
//
//		This library is free software; you can redistribute it and/or modify it under 
//		the terms of the OpenNETCF.org Shared Source License.
//
//		This library is distributed in the hope that it will be useful, but 
//		WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
//		FITNESS FOR A PARTICULAR PURPOSE. See the OpenNETCF.org Shared Source License 
//		for more details.
//
//		You should have received a copy of the OpenNETCF.org Shared Source License 
//		along with this library; if not, email licensing@opennetcf.org to request a copy.
//
//		If you wish to contact the OpenNETCF Advisory Board to discuss licensing, please 
//		email licensing@opennetcf.org.
//
//		For general enquiries, email enquiries@opennetcf.org or visit our website at:
//		http://www.opennetcf.org
//
//==========================================================================================
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;
using OpenNETCF.Win32;

#if !NDOC
using Microsoft.WindowsCE.Forms;
#endif

namespace OpenNETCF.Windows.Forms
{
	/// <summary>
	/// Specifies a component that creates an icon in the status area
	/// </summary>
	/// <remarks>Icons in the status area are short cuts to processes that are running in the background of a computer, such as a virus protection program or a volume control.
	/// These processes do not come with their own user interfaces.
	/// The <see cref="NotifyIcon"/> class provides a way to program in this functionality.
	/// The Icon property defines the icon that appears in the status area.
	/// Pop-up menus for an icon are addressed with the ContextMenu property.
	/// The <see cref="Text"/> property assigns ToolTip text (Tooltips are not supported by the Pocket PC interface).
	/// In order for the icon to show up in the status area, the <see cref="Visible"/> property must be set to true.</remarks>
#if DESIGN
	[ToolboxItemFilter("NETCF",ToolboxItemFilterType.Require),
	ToolboxItemFilter("System.CF.Windows.Forms", ToolboxItemFilterType.Custom)]
#endif
	public sealed class NotifyIcon : System.ComponentModel.Component
	{
		//count unique id - used to identify control
		private static uint id = 10;

		#region Fields

		//byte array used for marshalling
		private byte[] m_data;
		
#if !DESIGN
#if !NDOC
		//messagewindow
		private NotifyIconMessageWindow m_msgwnd;
#endif
#endif

		//flags
		private NotifyIconFlags m_flags;

		//icon - not currently implemented
		//private Icon m_icon;

		//visible
		private bool m_visible;

		//doubleclick
		private bool m_doubleclick  = false;

		#endregion


		#region Constructor
		/// <summary>
		/// Initializes a new instance of the NotifyIcon class.
		/// </summary>
		public NotifyIcon()
		{
#if !DESIGN
#if !NDOC
			//create new messagewindow
			m_msgwnd = new NotifyIconMessageWindow(this);
#endif
#endif
			//create new array
			m_data = new byte[152];

			//write standard contents to data array

			//write cbSize to the array
			BitConverter.GetBytes((int)m_data.Length).CopyTo(m_data, 0);


			//write id to the array
			BitConverter.GetBytes(id).CopyTo(m_data, 8);
			//increment id in case another icon is created
			id++;

			//write a default icon
			IntPtr hIcon = IntPtr.Zero;
			hIcon = ExtractIconEx(System.Reflection.Assembly.GetCallingAssembly().GetName().CodeBase, 0, 0, ref hIcon, 1);
			BitConverter.GetBytes((int)hIcon).CopyTo(m_data, 20);
			
			//write flags
			BitConverter.GetBytes((uint)NotifyIconFlags.Icon).CopyTo(m_data, 12);

			//write notification message
			BitConverter.GetBytes((int)WM.NOTIFY).CopyTo(m_data, 16);

#if !DESIGN
#if !NDOC
			//write hWnd to the array
			BitConverter.GetBytes((int)m_msgwnd.Hwnd).CopyTo(m_data, 4);
#endif
#endif
		}
		#endregion

		#region Dispose

		/// <summary>
		/// Releases the unmanaged resources used by the <see cref="NotifyIcon"/> and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources;
		/// false to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			//remove the icon
			this.Visible = false;

			base.Dispose(disposing);
		}

		#endregion

		#region Properties

		#region Context Menu
		/*
		private ContextMenu mMenu;
		/// <summary>
		/// Gets or sets the shortcut menu for the icon.
		/// <para><b>New in v1.3</b></para>
		/// </summary>
		public ContextMenu ContextMenu
		{
			get
			{
				return mMenu;
			}
			set
			{
				mMenu = value;
			}
		}*/
		#endregion

		#region Icon
		// <summary>
		// The Icon displayed by the NotifyIcon component.
		// </summary>
		/*public Icon Icon
		{
			get
			{
				return m_icon;
			}
			set
			{
				m_icon = value;
				m_flags = m_flags | NotifyIconFlags.Icon;
				Console.WriteLine("Flags " + m_flags + " Icon " + System.Drawing.Icon.GetHicnFromIcon(m_icon));
				BitConverter.GetBytes((int)System.Drawing.Icon.GetHicnFromIcon(m_icon)).CopyTo(m_data, 20);
				BitConverter.GetBytes((uint)m_flags).CopyTo(m_data, 12);
				if(m_visible)
				{
					Shell_NotifyIcon(NotifyIconMessage.Modify, m_data);
				}
			}
		}*/

		/// <summary>
		/// Get or set the native Icon handle used to display the NotifyIcon.
		/// <para><b>New in v1.3</b></para>
		/// </summary>
		/// <remarks>Ensure that the value you set to this property is a valid HIcon.
		/// By default the applications icon is assigned to the NotifyIcon control.</remarks>
		public IntPtr IconHandle
		{
			get
			{
				return (IntPtr)BitConverter.ToInt32(m_data, 20);
			}
			set
			{
				//copy handle to byte array
				BitConverter.GetBytes(value.ToInt32()).CopyTo(m_data, 20);
			}
		}
		#endregion

		#region Text
		/// <summary>
		/// Gets or sets the ToolTip text displayed when the mouse hovers over a status area icon.
		/// </summary>
		/// <remarks>The Pocket PC interface does not display tooptips.</remarks>
		/// <value>The ToolTip text displayed when the mouse hovers over a status area icon.</value>
		/// <exception cref="ArgumentException">ToolTip text must be less than 64 characters long.</exception>
		public string Text
		{
			get
			{
				//return string extracted from array
				string text = System.Text.Encoding.Unicode.GetString(m_data, 24, 128);
				return text.Substring(0, text.IndexOf('\0'));
			}
			set
			{
				if(value.Length < 64)
				{
					//get bytes and copy to array
					System.Text.Encoding.Unicode.GetBytes(value).CopyTo(m_data, 24);
					//set tooltip valid flag
					m_flags = m_flags | NotifyIconFlags.ToolTip;
					BitConverter.GetBytes((uint)m_flags).CopyTo(m_data, 12);
				}
				else
				{
					throw new ArgumentException("NotifyIcon.Text must be less than 64 characters long.");
				}
			}
		}
		#endregion

		#region Visible
		/// <summary>
		/// Gets or sets a value indicating whether the icon is visible in the status notification area of the taskbar.
		/// </summary>
		/// <value>true if the icon is visible in the status area; otherwise, false. The default value is false.</value>
		/// <remarks>Since the default value is false, in order for the icon to show up in the status area, you must set the Visible property to true.</remarks>
		[System.ComponentModel.DefaultValue(false)]
		public bool Visible
		{
			get
			{
				return m_visible;
			}
			set
			{
				if(m_visible != value)
				{
					if(value)
					{
						if(Shell_NotifyIcon(NotifyIconMessage.Add, m_data))
						{
							//set state to visible
							m_visible=true;
						}
						else
						{
							//add failed
							throw new ExternalException("Error adding NotifyIcon");
						}
					}
					else
					{
						if(Shell_NotifyIcon(NotifyIconMessage.Delete, m_data))
						{
							//set state to invisible
							m_visible=false;
						}
						else
						{
							//delete failed
							throw new ExternalException("Error deleting NotifyIcon");
						}
					}

				}
			}
		}
		#endregion

		#endregion


		#region Events

		#region Click
		/// <summary>
		/// Occurs when the user clicks the icon in the status area.
		/// </summary>
		public event EventHandler Click;

		private void OnClick(EventArgs e)
		{
			if(Control.MouseButtons == MouseButtons.Right)
			{
				

			}
			if(this.Click != null)
			{
				//fire Click event
				this.Click(this, e);
			}
		}
		#endregion

		#region Double Click
		/// <summary>
		/// Occurs when the user double-clicks the icon in the status notification area of the taskbar.
		/// </summary>
		public event EventHandler DoubleClick;

		private void OnDoubleClick(EventArgs e)
		{
			if(this.DoubleClick != null)
			{
				//fire DoubleClick event
				this.DoubleClick(this, e);
			}
		}
		#endregion

		#region Mouse Up
		/// <summary>
		/// Occurs when the user releases the mouse button while the pointer is over the icon in the status notification area of the taskbar.
		/// </summary>
		public event MouseEventHandler MouseUp;

		private void OnMouseUp(MouseEventArgs e)
		{
			if(this.MouseUp != null)
			{
				//fire Mouse Up event
				this.MouseUp(this, e);
			}
			//only fire click if this is not the second click of a doubleclick sequence
			if(!this.m_doubleclick)
			{
				this.OnClick(new EventArgs());
			}

			//reset doubleclick flag
			this.m_doubleclick = false;
		}
		#endregion

		#region Mouse Down
		/// <summary>
		/// Occurs when the user presses the mouse button while the pointer is over the icon in the status notification area of the taskbar.
		/// </summary>
		public event MouseEventHandler MouseDown;

		private void OnMouseDown(MouseEventArgs e)
		{
			if(this.MouseDown != null)
			{
				//fire Mouse Down event
				this.MouseDown(this, e);
			}
		}
		#endregion

		#endregion

		#region NotifyIcon MessageWindow
#if !DESIGN
#if !NDOC
		
		private sealed class NotifyIconMessageWindow : MessageWindow
		{	
			//reference to parent control
			private NotifyIcon m_parent;

			/// <summary>
			/// Creates a new instance of the NotifyIconMessageWindow class.
			/// </summary>
			/// <param name="parent">NotifyIcon for which this MessageWindow is operating.</param>
			public NotifyIconMessageWindow(NotifyIcon parent)
			{
				m_parent = parent;
			}

			/// <summary>
			/// Handles incoming windows Messages and acts accordingly
			/// </summary>
			/// <param name="m">Windows Message</param>
			protected override void WndProc(ref Microsoft.WindowsCE.Forms.Message m)
			{
				//notification message received
				if(m.Msg==(int)WM.NOTIFY)
				{
					//switch on action
					switch((int)m.LParam)
					{
						case 0x0201:
							//left button mouse down
							m_parent.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
							break;
						case 0x0202:
							//left button mouse up
							m_parent.OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
							//only fire click if this is not the second click of a doubleclick sequence
							break;
						case 0x0203:
							//left button doubleclick
							m_parent.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0));
							m_parent.m_doubleclick = true;
							m_parent.OnDoubleClick(new EventArgs());
							break;
						case 0x204:
							//r button down
							m_parent.OnMouseDown(new MouseEventArgs(MouseButtons.Right, 1, 0, 0, 0));
							break;
						case 0x0205:
							//r button up
							m_parent.OnMouseUp(new MouseEventArgs(MouseButtons.Right, 1, 0, 0, 0));
							break;
					}
				}

				//let windows handle any other messages
				base.WndProc (ref m);
			}
		}
		
#endif
#endif
#endregion

		#region NotifyIcon Message
		/// <summary>
		/// Specifies the action to take with a NotifyIcon.
		/// </summary>
		private enum NotifyIconMessage : uint
		{
			/// <summary>
			/// Add the Icon to the tray.
			/// </summary>
			Add    = 0x00000000,
			/// <summary>
			/// Change the Icon properties.
			/// </summary>
			Modify = 0x00000001,
			/// <summary>
			/// Delete the Icon from the tray.
			/// </summary>
			Delete = 0x00000002,
		}
		#endregion

		#region NotifyIcon Flags
		/// <summary>
		/// Flags that indicate which of the other members contain valid data.
		/// </summary>
		[Flags]
		private enum NotifyIconFlags : uint
		{
			/// <summary>
			/// The uCallbackMessage member is valid.
			/// </summary>
			Message = 0x00000001,
			/// <summary>
			/// The hIcon member is valid.
			/// </summary>
			Icon    = 0x00000002,
			/// <summary>
			/// The szTip member is valid.
			/// </summary>
			ToolTip = 0x00000004,
		}
		#endregion

		#region API P/Invokes

#if !DESIGN
		[DllImport("coredll.dll", EntryPoint="Shell_NotifyIcon", SetLastError=true)]
		private extern static bool Shell_NotifyIcon(
			NotifyIconMessage dwMessage,
			byte[] lpData);
		
		[DllImport("coredll.dll", EntryPoint="ExtractIconEx", SetLastError=true)]
		private extern static IntPtr ExtractIconEx(
			string lpszFile, 
			int hIconIndex, 
			int hiconLarge, 
			ref IntPtr hIcon, 
			uint nIcons); 
#else
        private static bool Shell_NotifyIcon(
			NotifyIconMessage dwMessage,
			byte[] lpData) { return true; }
			
		private static IntPtr ExtractIconEx(
			string lpszFile, 
			int hIconIndex, 
			int hiconLarge, 
			ref IntPtr hIcon, 
			uint nIcons) { return IntPtr.Zero; }
#endif
		#endregion

	}
}
