//==========================================================================================
//
//		OpenNETCF.Windows.Forms.ControlEx
//		Copyright (c) 2004, OpenNETCF.org
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
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#if !DESIGN
#if !NDOC
using OpenNETCF.Win32;
using Microsoft.WindowsCE.Forms;
#endif
#endif

// Intended as a base class for all OpenNETCF controls
// Don't inherit from this class yet since it is still undergoing testing
// Peter Foot 2004/07/17

namespace OpenNETCF.Windows.Forms
{
	/// <summary>
	/// Extends the standard <see cref="System.Windows.Forms.Control"/> class.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	/// <seealso cref="System.Windows.Forms.Control"/>
#if DESIGN
	[DesignTimeVisible(false),
	Browsable(false)]
	public abstract class ControlEx : Control
#else
	public abstract class ControlEx : Control, IWin32Window
#endif
	{
		#region Fields

				
		//are we hosting a native control
		private bool m_native;
		//handle to child control
		private IntPtr m_childhwnd = IntPtr.Zero;

#if !DESIGN
#if !NDOC
		//message window
		private ControlMessageWindow m_msgwnd;
#endif
#endif
		//instance handle
		private static IntPtr m_instance;

		#endregion

		#region Constructor
#if !DESIGN
#if !NDOC
		static ControlEx()
		{
			//get module handle
			m_instance = Core.GetModuleHandle(null);	
		}
#endif
#endif
		/// <summary>
		/// Creates a new ControlEx object.
		/// </summary>
		/// <param name="native">If true initialises members for hosting a native control.</param>
		protected ControlEx(bool native)
		{

			m_native = native;
#if !DESIGN
#if !NDOC
			if(native)
			{
				//create a messagewindow to intercept native events
				m_msgwnd = new ControlMessageWindow(this);
			}
#endif
#endif
		}
		#endregion


		#region Border Style

		//border style
		private BorderStyle m_border = BorderStyle.None;

		/// <summary>
		/// Gets or sets the border style for the control.
		/// </summary>
		/// <value>One of the <see cref="BorderStyle"/> values.
		/// Fixed3D is interpreted the same as FixedSingle.</value>
#if DESIGN
		[Category("Appearance"),
		Description("The style of the border."),
		DefaultValue(BorderStyle.None)]
#endif
		public BorderStyle BorderStyle
		{
			get
			{
				return m_border;
			}
			set
			{
				m_border = value;
#if !DESIGN
#if !NDOC
				if(value==BorderStyle.None)
				{
					//remove WS_BORDER style
					Win32Window.UpdateWindowStyle(Handle, (int)WS.BORDER, 0);
				}
				else
				{
					//set WS_Border style
					Win32Window.UpdateWindowStyle(Handle, 0,(int)WS.BORDER);
				}
#endif
#else
				Invalidate();
#endif
			}
		}
		#endregion

		#region Child Handle
		/// <summary>
		/// Window Handle of child native control (if present)
		/// </summary>
		protected internal IntPtr ChildHandle
		{
			get
			{
				return m_childhwnd;
			}
			set
			{
				m_childhwnd = value;
			}
		}
		#endregion

		#region Host Handle
#if !NDOC
		internal IntPtr HostHandle
		{
			get
			{
#if DESIGN
				return IntPtr.Zero;
#else
				return m_msgwnd.Hwnd;
#endif
			}
		}
#endif
		#endregion

		#region Created
#if !DESIGN
		/// <summary>
		/// Gets a value indicating whether the control has been created.
		/// </summary>
		public bool Created
		{
			get
			{
				return (m_childhwnd != IntPtr.Zero);
			}
		}
#endif
		#endregion

		#region Create Params
#if !DESIGN
		/// <summary>
		/// Gets the required creation parameters when the control handle is created.
		/// </summary>
		protected virtual CreateParams CreateParams 
		{
			get
			{
				//set defaults in createparams
				CreateParams cp = new CreateParams();
#if !NDOC
				cp.Parent = m_msgwnd.Hwnd;

				cp.ClassStyle = (int)(WS.VISIBLE | WS.TABSTOP | WS.CHILD);
				cp.Width = this.Width;
				cp.Height = this.Height;
				cp.X = 0;
				cp.Y = 0;
#endif
				return cp;
			}
		}
#endif
		#endregion

		#region Handle
#if !DESIGN

		private IntPtr m_hwnd = IntPtr.Zero;

		/// <summary>
		/// Gets the window handle that the control is bound to.
		/// </summary>
		public IntPtr Handle
		{
			get
			{
#if !NDOC
				//if not set get the handle
				if(m_hwnd==IntPtr.Zero)
				{
					this.Capture = true;
					m_hwnd = Win32Window.GetCapture();
					this.Capture = false;
				}
#endif
				//return handle
				return m_hwnd;
			}
		}
#endif
		#endregion

		#region Instance
		/// <summary>
		/// Returns the native instance handle of the calling process.
		/// </summary>
		/// <remarks>Required for creating some native controls.</remarks>
		protected static IntPtr Instance
		{
			get
			{
				return m_instance;
			}
		}
		#endregion

		#region Name

		//name of control
		private string m_name = String.Empty;

		/// <summary>
		/// Gets the name of the control.
		/// </summary>
#if DESIGN 
		public new string Name
#else
		public string Name
#endif
		{
			get
			{
				//if not got the name yet do some reflection
				if(m_name == string.Empty)
				{
					m_name = GetName(this);
				}

				//return the name
				return m_name;
			}
		}
		#endregion

		#region Recreating Handle
#if !DESIGN
		private bool m_recreating = false;
		/// <summary>
		/// Gets a value indicating whether the control is currently re-creating its handle.
		/// </summary>
		/// <remarks>true if the control is currently re-creating its handle; otherwise, false.</remarks>
		public bool RecreatingHandle
		{
			get
			{
				return m_recreating;
			}
		}
#endif
		#endregion

		#region Tag
		/// <summary>
		/// Gets or sets the object that contains data about the control.
		/// </summary>
		/// <value>An <see cref="System.Object"/> that contains data about the control.
		/// The default is null.</value>
#if DESIGN
		[Bindable(true), DefaultValue((string) null), TypeConverter(typeof(StringConverter))]
		public new object Tag
#else
		public object Tag
#endif
		{
			get
			{
				return m_tag;
			}
			set
			{
				m_tag = value;
			}
		}

		private object m_tag;

		#endregion

		#region Key Events
#if DESIGN

		[Browsable(true),
		Category("Key"),
		Description("Occurs when a key is first pressed.")]
		public new event KeyEventHandler KeyDown;

		[Browsable(true),
		Category("Key"),
		Description("Occurs after a user is finished pressing a key.")]
		public new event KeyPressEventHandler KeyPress;

		[Browsable(true),
		Category("Key"),
		Description("Occurs when a key is released.")]
		public new event KeyEventHandler KeyUp;
#endif	
		#endregion

		#region Create Control
#if !DESIGN
		/// <summary>
		/// Forces the creation of the control, including the creation of the handle and any child controls.
		/// </summary>
		public void CreateControl()
		{
#if !NDOC
			//if hosting a native control
			if(m_native)
			{
				//check if a control is already created
				if(m_childhwnd != IntPtr.Zero)
				{
					m_recreating = true;

					//destroy the existing control
					Win32Window.DestroyWindow(m_childhwnd);

					m_childhwnd = IntPtr.Zero;
				}

				CreateParams cp = this.CreateParams;

				//dont create null window
				if(cp.ClassName!=null)
				{
					//create the control
					m_childhwnd = Win32Window.CreateWindowEx(cp.ExStyle, cp.ClassName, cp.Caption,cp.ClassStyle, cp.X, cp.Y, cp.Width, cp.Height, cp.Parent, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
				}

				m_recreating = false;

			}
#endif
		}
#endif
		#endregion

		#region On Got Focus
#if !DESIGN
		/// <summary>
		/// Passes the focus to a hosted native control, or does the default behaviour.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnGotFocus(EventArgs e)
		{
#if !NDOC
			//if hosting a native control
			if(m_native)
			{
				//if child control is created
				if(ChildHandle != IntPtr.Zero)
				{
					//pass focus to native control
					if ( Win32Window.GetFocus() != ChildHandle )
						Win32Window.SetFocus(ChildHandle);
				}
			}
			else
			{
				//default behaviour
				base.OnGotFocus (e);
			}
#endif
		}
#endif
		#endregion

		#region Designer OnPaint Support
#if DESIGN
		protected override void OnPaint(PaintEventArgs e)
		{
			//fill background
			e.Graphics.FillRectangle(new SolidBrush(this.BackColor),0,0,this.Width,this.Height);

			if(m_border != BorderStyle.None)
			{
				//draw a border
				e.Graphics.DrawRectangle(new Pen(Color.Black), new Rectangle(0,0,this.Width-1, this.Height-1));
			}

			base.OnPaint (e);
		}
#endif
		#endregion

		#region On Parent Changed
#if !DESIGN
		/// <summary>
		/// Occurs when the control is associated with a new Parent.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnParentChanged(EventArgs e)
		{
			//reset handle
			m_hwnd = IntPtr.Zero;

			//(re)create the native control once this control is placed on a form.
			CreateControl();

			base.OnParentChanged (e);
		}
#endif
		#endregion

		#region On Resize
		/// <summary>
		/// Handles changes in the controls size.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnResize(EventArgs e)
		{
			if(m_native)
			{
#if !DESIGN
#if !NDOC
				//resize messagewindow container and then inner control
				Win32Window.SetWindowPos(m_msgwnd.Hwnd, 0, 0, 0,this.Width, this.Height, 0);

				//if control is created resize it to fit
				if(ChildHandle != IntPtr.Zero)
				{
					Win32Window.SetWindowPos(ChildHandle, 0, 0, 0, this.Width, this.Height, 0);
				}
#endif
#else	
			//force designer to update
			Invalidate();
#endif
			}

			base.OnResize (e);
		}
		#endregion		

		#region Set Bounds
#if !DESIGN
		/// <summary>
		/// Sets the bounds of the control to the specified location and size.
		/// </summary>
		/// <param name="x">The new <see cref="Control.Left"/> property value of the control.</param>
		/// <param name="y">The new <see cref="Control.Top"/> property value of the control.</param>
		/// <param name="width">The new <see cref="Control.Width"/> property value of the control.</param>
		/// <param name="height"> The new <see cref="Control.Height"/> property value of the control.</param>
		public void SetBounds(int x, int y, int width, int height)
		{
			this.Bounds = new Rectangle(x, y, width, height);
		}
#endif
		#endregion		


		#region Message Processing Members
#if !DESIGN
#if !NDOC
		//used internally by the ControlMessageWindow to pass through messages
		internal void OnMessage(ref Microsoft.WindowsCE.Forms.Message m)
		{
			//call the controls OnNotifyMessage method
			OnNotifyMessage(ref m);
		}

		//used internally by the ControlMessageWindow to pass through messages
		internal virtual void OnEraseBkgndMessage(ref Microsoft.WindowsCE.Forms.Message m)
		{
		}

		/// <summary>
		/// Processes incoming notification messages.
		/// </summary>
		/// <param name="m"></param>
		protected virtual void OnNotifyMessage(ref Microsoft.WindowsCE.Forms.Message m)
		{
			//don't handle any messages by default - leave this up to inheritors
		}
#endif
#endif
		#region Message Window

#if !DESIGN
#if !NDOC
		/// <summary>
		/// Generic MessageWindow implementation for hosting native controls.
		/// </summary>
		internal class ControlMessageWindow : MessageWindow
		{
			//parent control
			private ControlEx m_parent;

			//message received from native control as notification
			private const int WM_NOTIFY = 0x004E;
			// used to detect that VoiceRecorder window lost focus
			private const int WM_ERASEBKGND = 0x0014;

			#region Constructor
			//creates a new messagewindow associated with the specific control.
			internal ControlMessageWindow(ControlEx parent)
			{
				//set parent
				m_parent = parent;

				//make this window a child of parent control
				Win32Window.SetParent(this.Hwnd, m_parent.Handle);

				//set window style - make visible
				Win32Window.SetWindowLong(this.Hwnd, (int)GWL.STYLE, 0x46000000);
				//set ex style top-most
				Win32Window.SetWindowLong(this.Hwnd, (int)GWL.EXSTYLE, 0x00000008);

				//set visible style full size of parent
				Win32Window.SetWindowPos(this.Hwnd, 0, 0, 0, m_parent.Width, m_parent.Height, SWP.SHOWWINDOW |SWP.NOSIZE |SWP.NOZORDER);
			}
			#endregion

			#region WndProc
			/// <summary>
			/// Handles incoming windows messages from the native control.
			/// </summary>
			/// <param name="m">Message</param>
			protected override void WndProc(ref Message m)
			{
				switch(m.Msg)
				{
					case WM_ERASEBKGND:
						m_parent.OnEraseBkgndMessage(ref m);
						break;

					case WM_NOTIFY:
						//pass message to parent control
						m_parent.OnMessage(ref m);
						break;

					case (int)WM.COMMAND:
						//pass message to parent control
						m_parent.OnMessage(ref m);
						break;
				}

				//let the system catch other messages here
				base.WndProc (ref m);
			}
			#endregion

		}
#endif
#endif
		#endregion

		#endregion

		#region IDisposable Members
		/// <summary>
		/// Releases the unmanaged resources used by the <see cref="ControlEx"/> and its child controls and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
#if !DESIGN
#if !NDOC
			if(m_childhwnd != IntPtr.Zero)
			{
				//destroy native control
				Win32Window.DestroyWindow(m_childhwnd);
				m_childhwnd = IntPtr.Zero;
			}

			if(m_msgwnd != null)
			{
				//dispose message window
				m_msgwnd.Dispose();
			}
#endif
#endif

			base.Dispose (disposing);
		}
		#endregion


		#region Static Handle Method
		/// <summary>
		/// Return the native window handle for a control.
		/// </summary>
		/// <param name="c"></param>
		/// <returns>HWnd of selected control or IntPtr.Zero on failure.</returns>
		public static IntPtr GetHandle(Control c)
		{
#if DESIGN
			return IntPtr.Zero;
#else
#if !NDOC
			c.Capture = true;
			IntPtr handle = Win32Window.GetCapture();
			c.Capture = false;

			return handle;
#else
			return IntPtr.Zero;
#endif
#endif
		}
		#endregion

		#region Static Name Methods

		// Thanks to Chris Tacke for these methods for working with Control Names
		// More details on Chris's blog:-
		// http://blog.opennetcf.org/ctacke/PermaLink,guid,64c28b10-d3a8-4c6b-b11a-2ae2de4bdaf1.aspx

		/// <summary>
		/// Gets the control on the specified Form or container with the supplied name.
		/// </summary>
		/// <param name="parent">Parent control, e.g. Form</param>
		/// <param name="name">Control name.</param>
		/// <returns>Control or null (Nothing in VB) on failure.</returns>
		public static Control GetControlByName(Control parent, string name)
		{
			FieldInfo info = parent.GetType().GetField(name,
				BindingFlags.NonPublic | BindingFlags.Instance |
				BindingFlags.Public | BindingFlags.IgnoreCase);

			if(info == null) return null;

			object o = info.GetValue(parent);

			if(o == null) return null;

			return (Control)o;
		}

		/// <summary>
		/// Returns the name of the control specified.
		/// </summary>
		/// <param name="control">Control.</param>
		/// <returns>Name as defined when control was created.</returns>
		public static string GetName(Control control)
		{
			FieldInfo[] fi = control.Parent.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance |
				BindingFlags.Public | BindingFlags.IgnoreCase);

			foreach(FieldInfo f in fi)
			{
				if(f.GetValue(control.Parent).Equals(control))
					return f.Name;
			}

			return string.Empty;
		}

		#endregion

	}
}
