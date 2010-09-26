//==========================================================================================
//
//		OpenNETCF.Windows.Forms.WebBrowser
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

// This class was previously released as HTMLViewer, please see the documentation for changes to the object model
// Peter Foot 18th Sept 2004

using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
#if !DESIGN
using OpenNETCF.Win32;
using OpenNETCF.Runtime.InteropServices;
#endif


namespace OpenNETCF.Windows.Forms
{
	/// <summary>
	/// Represents a controls that provides Web browsing capabilities.
	/// </summary>
#if DESIGN
	[System.ComponentModel.DefaultEvent("DocumentCompleted"),
	System.ComponentModel.DefaultProperty("Url")]
#endif
	public class WebBrowser : ControlEx
	{
		//property backers
		private bool m_cleartype = false;
		private bool m_contextmenu = false;
		private bool m_script = true;
		private bool m_shrink = true;
		private string m_title;
		private string m_url;
		
		//stores urls previously visited
		private Stack m_history;
		private bool m_addtohistory = true;

		#region Constructor

		static WebBrowser()
		{
#if !DESIGN
			//load htmlview module
			IntPtr module = Core.LoadLibrary("htmlview.dll");

			//init htmlcontrol
			int result = InitHTMLControl(Instance);		
#endif
		}

		/// <summary>
		/// Create a new instance of <see cref="WebBrowser"/>.
		/// </summary>
		public WebBrowser() : base(true)
		{
			//new stack (for history urls)
			m_history = new Stack();
		}
		#endregion

#if !DESIGN
		//set control specific createparams
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ClassName = "DISPLAYCLASS";
				return cp;
			}
		}
#endif

		#region WndProc
#if !DESIGN
#if !NDOC
		/// <summary>
		/// Handles notification messages from the native control
		/// </summary>
		/// <param name="m">Message</param>
		protected override void OnNotifyMessage(ref Microsoft.WindowsCE.Forms.Message m)
		{
			//check message is a notification
			if(m.Msg == (int)WM.NOTIFY)
			{
				//marshal notification data into NMHDR struct
				NMHDR hdr = (NMHDR)Marshal.PtrToStructure(m.LParam, typeof(NMHDR));

								
				//context menu event is a special case
				if(hdr.code == (int)NM.CONTEXTMENU)
				{
					//if the ContextMenu property is set show the user's context menu
					//if no ContextMenu is set we ignore this notification and use the built in Context Menu.
					if(this.ContextMenu != null)
					{
						NM_HTMLCONTEXT nmhc = (NM_HTMLCONTEXT)Marshal.PtrToStructure(m.LParam, typeof(NM_HTMLCONTEXT));
					
						//cancel default context menu
						m.Result = new IntPtr(-1);
						//set position
						System.Drawing.Point pos = new System.Drawing.Point(nmhc.x,nmhc.y);
						//show popup
						this.ContextMenu.Show(this,pos);
					}
				}
				else
				{
					//get html viwer specific data from the notification message
					NM_HTMLVIEW nmhtml = (NM_HTMLVIEW)Marshal.PtrToStructure(m.LParam, typeof(NM_HTMLVIEW));
					
					//marshal the Target string
					string target = MarshalEx.PtrToStringAuto(nmhtml.szTarget);

					//check the incoming message code and process as required
					switch(nmhtml.code)
					{
							//hotspot click
						case (int)NM.HOTSPOT:
						case (int)NM.BEFORENAVIGATE:
							OnNavigating(new WebBrowserNavigatingEventArgs(target));
							break;

							//navigate complete
						case (int)NM.NAVIGATECOMPLETE:
							OnNavigated(new WebBrowserNavigatedEventArgs(target));
							break;

							//document complete
						case (int)NM.DOCUMENTCOMPLETE:
							OnDocumentCompleted(new WebBrowserDocumentCompletedEventArgs(target));
							break;

							//title notification
						case (int)NM.TITLECHANGE:
						case (int)NM.TITLE:
							m_title = target;
							OnDocumentTitleChanged(new EventArgs());
							break;
					}
				}
			}

			base.OnNotifyMessage (ref m);
		}
#endif
#endif
		#endregion

		#region Methods

		#region Add Text Method
		/// <summary>
		/// Adds HTML text to the WebBrowser control.
		/// </summary>
		/// <param name="text">HTML text to be added.</param>
		public void AddText(string text)
		{
			AddText(text, false);
		}
		/// <summary>
		/// Adds text to the WebBrowser control.
		/// </summary>
		/// <param name="text">Text to be added.</param>
		/// <param name="plain">If TRUE text is treated as plain text, else treated as HTML Source.</param>
		public void AddText(string text, bool plain)
		{
#if !DESIGN
			//allocate temporary native buffer
			IntPtr stringptr = MarshalEx.StringToHGlobalUni(text + '\0');
				
			int iplain = 0;

			if(plain)
				iplain = -1;

			//send message to native control adding to current text
			Win32Window.SendMessage(ChildHandle,(int)DTM.ADDTEXTW, iplain, (int)stringptr);

			//free native memory
			MarshalEx.FreeHGlobal(stringptr);
#endif
		}
		#endregion

		#region Clear Method
		/// <summary>
		/// Clears the contents of the WebBrowser.
		/// </summary>
		public void Clear()
		{
			//clear local text
			this.Text = "";
#if !DESIGN
			Win32Window.SendMessage(ChildHandle,(int)DTM.CLEAR, 0, 0);
#endif
		}
		#endregion

		#region End Of Source Method
		/// <summary>
		/// Inform the WebBrowser that the current document is complete.
		/// </summary>
		public void EndOfSource()
		{
#if !DESIGN
			//send message to native control indicating end of source text
			Win32Window.SendMessage(ChildHandle,(int)DTM.ENDOFSOURCE, 0, 0);
#endif
		}
		#endregion


		#region Go Home
		/// <summary>
		/// Navigates to the default home page.
		/// </summary>
		public void GoHome()
		{
			Navigate("about:home");
		}
		#endregion

		#region Go Back
		/// <summary>
		/// Navigates the <see cref="WebBrowser"/> control to the previous page in the navigation history, if one is available.
		/// </summary>
		public void GoBack()
		{
			if(m_history.Count > 0)
			{
				//don't add url to history stack
				m_addtohistory = false;
				//pop the last url off of the history stack
				Navigate(m_history.Pop().ToString());
			}
		}
		#endregion


		#region Jump To Anchor
		/// <summary>
		/// Instruct the WebBrowser to jump to the indicated anchor.
		/// <para><b>New in v1.3</b></para>
		/// </summary>
		/// <param name="anchor">The name of the anchor to which the HTML control is to jump.</param>
		/// <remarks>If the anchor is already in the document, the jump occurs immediately.
		/// If the specified anchor is not in the document, the request will be remembered such that, if the anchor is added, the jump will occur as soon as the anchor is available.
		/// While there is a pending anchor, any navigation commands from the user, such as keyboard scrolling or using the scroll bar, aborts the pending anchor jump.</remarks>
		public void JumpToAnchor(string anchor)
		{
#if !DESIGN
			//allocate temporary native buffer
			IntPtr stringptr = MarshalEx.StringToHGlobalUni(anchor + '\0');

			//send message to native control adding to current text
			Win32Window.SendMessage(ChildHandle,(int)DTM.ANCHORW, 0, (int)stringptr);

			//free native memory
			MarshalEx.FreeHGlobal(stringptr);
#endif
		}
		#endregion

		#region Navigate
		/// <summary>
		/// Loads the document at the specified URL into the <see cref="WebBrowser"/> control, replacing the previous document.
		/// </summary>
		/// <param name="url">The URL of the document to load.</param>
		public void Navigate(string url)
		{
#if !DESIGN
			//allocate temporary native buffer
			IntPtr stringptr = MarshalEx.StringToHGlobalUni(url + '\0');
				
			//send message to native control
			Win32Window.SendMessage(ChildHandle,(int)DTM.NAVIGATE, 0, (int)stringptr);

			//free native memory
			MarshalEx.FreeHGlobal(stringptr);
#endif
		}
		#endregion

		#region Refresh
		/// <summary>
		/// Reloads the document currently displayed in the WebBrowser control by checking the server for an updated version.
		/// </summary>
		public override void Refresh()
		{
			//only refresh if an address has been set
			if(m_url != null)
			{
#if !DESIGN
				//allocate temporary native buffer
				IntPtr stringptr = MarshalEx.StringToHGlobalUni(m_url + '\0');
				
				//send message to native control
				Win32Window.SendMessage(ChildHandle,(int)DTM.NAVIGATE, (int)NAVIGATEFLAG.REFRESH, (int)stringptr);

				//free native memory
				MarshalEx.FreeHGlobal(stringptr);
#endif
			}
		}
		#endregion

		#region Stop
		/// <summary>
		/// Cancels any pending navigation and stops any dynamic page elements, such as background sounds and animations.
		/// </summary>
		public void Stop()
		{
#if !DESIGN
			//send message to native control
			Win32Window.SendMessage(ChildHandle,(int)DTM.STOP, 0, 0);
#endif
		}
		#endregion


		#region Select All
		/// <summary>
		/// Selects all the text in the current HTML page.
		/// </summary>
		public void SelectAll()
		{
#if !DESIGN
			//send message
			Win32Window.SendMessage(ChildHandle,(int)DTM.SELECTALL, 0, 0);
#endif
		}

		#endregion
	
		#endregion

		#region Properties

		#region Can Go Back
		/// <summary>
		/// Gets a value indicating whether a previous page in navigation history is available.
		/// </summary>
#if DESIGN
		[Browsable(false)]
#endif
		public bool CanGoBack
		{
			get
			{
				return m_history.Count > 0 ?  true : false;
			}
		}
		#endregion

		#region Document Text
		/// <summary>
		/// Gets or sets the HTML contents of the page displayed in the WebBrowser control.
		/// </summary>
#if DESIGN
		[Category("Misc"),
		Description("Document Text")]
#endif
		public string DocumentText
		{
			/*get
			{
				//get handle of inner browser control
				//IntPtr htmlHwnd = Win32Window.GetWindow(ChildHandle, Win32Window.GetWindowParam.GW_CHILD);
				int textLen = (int)Win32Window.SendMessage(ChildHandle, (int)DTM.WM_GETTEXTLENGTH, 0,0);

				if(textLen > 0)
				{
					//allocate buffer to receive text
					IntPtr textBuffer = MarshalEx.AllocHGlobal((textLen + 1) * 2);

					//send get_text message
					Win32Window.SendMessage(ChildHandle, (int)DTM.WM_GETTEXT, textLen + 1, textBuffer);

					//marshal to a string
					string text = Marshal.PtrToStringUni(textBuffer);

					//free native buffer
					MarshalEx.FreeHGlobal(textBuffer);

					return text;

				}
				//no text present
				return "";
			}*/
			set
			{

#if !DESIGN
				//add text
				AddText(value);
				//mark as end of source
				EndOfSource();
#endif
			}
		}
		#endregion

		#region Document Title
		/// <summary>
		/// Gets the title of the document currently displayed in the WebBrowser control.
		/// </summary>
#if DESIGN
		[Browsable(false)]
#endif
		public string DocumentTitle
		{
			get
			{
				return m_title;
			}
		}
		#endregion


		#region Enable ClearType Property
		/// <summary>
		/// Gets or sets a value indicating whether the control renders with ClearType text
		/// </summary>
		/// <remarks>By default ClearType is disabled.</remarks>
#if DESIGN
		[Category("Appearance"),
		Description("Indicates whether to render the page with Cleartype smoothing."),
		DefaultValue(false)]
#endif
		public bool EnableClearType
		{
			get
			{
				return m_cleartype;
			}
			set
			{
				m_cleartype = value;

#if !DESIGN
				if(Created)
				{
					int intval = value ? -1 : 0;
				
					Win32Window.SendMessage(ChildHandle,(int)DTM.ENABLECLEARTYPE, 0, intval);
				}

				if(value)
				{
					this.CreateParams.ClassStyle = this.CreateParams.ClassStyle | (int)HS.CLEARTYPE;
				}
				else
				{
					this.CreateParams.ClassStyle = this.CreateParams.ClassStyle ^ (int)HS.CLEARTYPE;
				}
#endif
			}
		}
		#endregion

		#region Enable ContextMenu Property
		/// <summary>
		/// Gets or sets a value which determines whether a ContextMenu is available for this control.
		/// </summary>
		/// <remarks>Setting this property to true will allow the default context menu to display.
		/// Note that if you supply a custom context menu using the ContextMenu property there is currently no way to disable the default menu - so it is not recommended you try to use a custom context menu.</remarks>
#if DESIGN
		[Category("Behavior"),
		Description("Show the default context menu."),
		DefaultValue(false)]
#endif
		public bool EnableContextMenu
		{
			get
			{
				return m_contextmenu;
			}
			set
			{
				m_contextmenu = value;
#if !DESIGN
				if(Created)
				{
					//send to control
					int intval = value ? -1 : 0;

					Win32Window.SendMessage(ChildHandle,(int)DTM.ENABLECONTEXTMENU, 0, intval);
				}

				if(value)
				{
					this.CreateParams.ClassStyle = this.CreateParams.ClassStyle | (int)HS.CONTEXTMENU;
				}
				else
				{
					this.CreateParams.ClassStyle = this.CreateParams.ClassStyle ^ (int)HS.CONTEXTMENU;
				}
#endif
			}
		}
		#endregion

		#region Enable Scripting Property
		/// <summary>
		/// Gets or sets a value indicating whether the control supports Scripting.
		/// </summary>
		/// <remarks>By default Scripting is enabled.</remarks>
#if DESIGN
		[Category("Behavior"),
		Description("Indicates whether to enable or disable J/Script support."),
		DefaultValue(true)]
#endif
		public bool EnableScripting
		{
			get
			{
				return m_script;
			}
			set
			{
				m_script = value;

#if !DESIGN
				if(Created)
				{
					int intval = value ? -1 : 0;
				
					Win32Window.SendMessage(ChildHandle,(int)DTM.ENABLESCRIPTING, 0, intval);
				}

				if(value)
				{
					this.CreateParams.ClassStyle = this.CreateParams.ClassStyle ^ (int)HS.NOSCRIPTING;
				}
				else
				{
					this.CreateParams.ClassStyle = this.CreateParams.ClassStyle | (int)HS.NOSCRIPTING;
				}
#endif
			}
		}
		#endregion

		#region Enable Shrink Property
		/// <summary>
		/// Gets or sets a value indicating whether the control renders with Shrink mode.
		/// </summary>
		/// <remarks>If shrink mode is enabled, the HTML control will shrink images as required to make the page fit the window.
		/// By default Shrink mode is enabled.</remarks>
#if DESIGN
		[Category("Behavior"),
		Description("Indicates whether to try to fit the page to the screen."),
		DefaultValue(true)]
#endif
		public bool EnableShrink
		{
			get
			{
				return m_shrink;
			}
			set
			{
				m_shrink = value;

#if !DESIGN
				if(Created)
				{
					int intval = value ? -1 : 0;
				
					Win32Window.SendMessage(ChildHandle,(int)DTM.ENABLESHRINK, 0, intval);
				}

				if(value)
				{
					this.CreateParams.ClassStyle = this.CreateParams.ClassStyle | (int)HS.NOFITTOWINDOW;
				}
				else
				{
					this.CreateParams.ClassStyle = this.CreateParams.ClassStyle ^ (int)HS.NOFITTOWINDOW;
				}
#endif
			}
		}
		#endregion


		#region Layout Height Property
		/// <summary>
		/// Gets the height in pixels of the content in the WebBrowser.
		/// </summary>
#if DESIGN
		[Browsable(false)]
#endif
		public int LayoutHeight
		{
			get
			{
#if !DESIGN
				return (int)Win32Window.SendMessage(ChildHandle,(int)DTM.LAYOUTHEIGHT, 0, 0);
#else
				return 0;
#endif
			}
		}
		#endregion

		#region Layout Width Property
		/// <summary>
		/// Gets the width in pixels of the content in the WebBrowser.
		/// </summary>
#if DESIGN
		[Browsable(false)]
#endif
		public int LayoutWidth
		{
			get
			{
#if !DESIGN
				return (int)Win32Window.SendMessage(ChildHandle,(int)DTM.LAYOUTWIDTH, 0, 0);
#else
				return 0;
#endif
			}
		}
		#endregion


		#region Url
		/// <summary>
		/// Gets or sets the URL of the current document.
		/// </summary>
#if DESIGN
		[Category("Behavior"),
		Description("Specifies the url then Web browser control has navigated to.")]
#endif
		public string Url
		{
			get
			{
				return m_url;
			}
			set
			{
				m_url = value;
					
				//navigate to the new url
				Navigate(m_url);
			}
		}
		#endregion

		#endregion

		#region Events
		/// <summary>
		/// Occurs when the <see cref="WebBrowser"/> control finishes loading a document.
		/// </summary>
#if DESIGN
		[Category("Behavior"),
		Description("Occurs when the documented hosted in the Web browser control is fully loaded.")]

#endif
		public event WebBrowserDocumentCompletedEventHandler DocumentCompleted;

		/// <summary>
		/// Raises the <see cref="DocumentCompleted"/> event.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnDocumentCompleted(WebBrowserDocumentCompletedEventArgs e)
		{
			if(DocumentCompleted!=null)
			{
				DocumentCompleted(this,e);
			}
		}

		/// <summary>
		/// Occurs when the <see cref="DocumentTitle"/> property value changes.
		/// </summary>
#if DESIGN
		[Category("Behavior"),
		Description("Occurs when the title of the document changes.")]

#endif
		public event EventHandler DocumentTitleChanged;

		/// <summary>
		/// Raises the <see cref="DocumentTitleChanged"/> event.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnDocumentTitleChanged(EventArgs e)
		{
			if(DocumentTitleChanged!=null)
			{
				DocumentTitleChanged(this,e);
			}
		}

		/// <summary>
		/// Occurs when the <see cref="WebBrowser"/> control has navigated to a new document and has begun loading it.
		/// </summary>
#if DESIGN
		[Category("Action"),
		Description("Occurs after browser control navigation occurs.")]

#endif
		public event WebBrowserNavigatedEventHandler Navigated;

		/// <summary>
		/// Raises the <see cref="Navigated"/> event.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnNavigated(WebBrowserNavigatedEventArgs e)
		{
			if(m_url != null & m_addtohistory)
			{
				//push the url to the history stack
				m_history.Push(m_url);
				//reset add to history flag
				m_addtohistory = true;
			}

			//set the new url
			m_url = e.Url;

			if(Navigated!=null)
			{
				Navigated(this,e);
			}
		}

		/// <summary>
		/// Occurs before the <see cref="WebBrowser"/> control navigates to a new document.
		/// </summary>
#if DESIGN
		[Category("Action"),
		Description("Occurs before browser control navigation occurs.")]

#endif
		public event WebBrowserNavigatingEventHandler Navigating;

		/// <summary>
		/// Raises the <see cref="Navigating"/> event.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnNavigating(WebBrowserNavigatingEventArgs e)
		{
			if(Navigating!=null)
			{
				Navigating(this,e);
			}
		}
		#endregion

#if !DESIGN
		#region Event Overrides
		/// <summary>
		/// Handles the control receiving input focus.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnGotFocus(EventArgs e)
		{
			//set focus to inner HTML control
			IntPtr hwnd = Win32Window.GetWindow(ChildHandle, GW.CHILD);
			Win32Window.SetFocus(hwnd);
		}

		#endregion



		#region NM_HTMLVIEW
		//define your control specific notification message header (or use NMHDR from OpenNETCF.Win32.Win32Window)
		//see SDK documentation for full description of NM_HTMLVIEW
		private struct NM_HTMLVIEW
		{
			public IntPtr hwndFrom; 
			public int idFrom; 
			public int code; 
			public IntPtr szTarget;
			public IntPtr szData;
			public IntPtr dwCookie;
			public IntPtr szExInfo;
		}
		#endregion

		#region NM_HTMLCONTEXT

		/// <summary>
		/// Structure passed with a Context Menu notification
		/// </summary>
		private struct NM_HTMLCONTEXT
		{
			public IntPtr hwndFrom; 
			public int idFrom; 
			public int code; 
			public int x;
			public int y;
			public uint uTypeFlags;
			public IntPtr szLinkHREF;
			public IntPtr szLinkName;
			int dwUnused;
			public int dwImageCooki;
			int dwUnused2;
		}
		#endregion

		#region P/Invokes
		/// <summary>
		/// Initialise the native HTML control.
		/// </summary>
		/// <param name="hinst">Instance Handle.</param>
		/// <returns></returns>
		[DllImport("htmlview.dll", EntryPoint="InitHTMLControl", SetLastError=true)] 
		private static extern int InitHTMLControl(IntPtr hinst);
		#endregion

#endif

		#region NAVIGATEFLAG Enumeration
		//additional flags sent on navigation
		[Flags()]
		private enum NAVIGATEFLAG : int
		{
			/// <summary>
			/// Forced refresh of the URL content from the server, without checking expiration time or last-modified time. 
			/// </summary>
			REFRESH = 0x0020,
			/// <summary>
			/// The navigation is relative to the current page. 
			/// </summary>
			RELATIVE = 0x0040,
			/// <summary>
			/// The user entered the URL for the navigation. 
			/// </summary>
			ENTERED = 0x0080,
			/// <summary>
			/// The navigation is ignoring the TARGET attribute (if navigated via an &lt;A HREF&gt; tag. 
			/// </summary>
			IGNORETARGET = 0x0200,
			/// <summary>
			/// Load the content from the cache, without checking expiration time. Go online only if no cache. 
			/// </summary>
			GETFROMCACHE = 0x0400,
			/// <summary>
			/// Do not cache the content downloaded from the URL.
			/// </summary>
			NOCACHE = 0x1000,
			/// <summary>
			/// Check the server to see that the most recent content is available -- this option will allow the HTML control to go online without checking the expiration time. Then, the underlying code will compare the "last-modified" time with the server, and download the more recent content, if necessary.
			/// </summary>
			RESYNCHRONIZE = 0x2000, 
		}
		#endregion

		#region DTM Enumeration
		/// <summary>
		/// Messages which can be sent to the native Html control.
		/// </summary>
		private enum DTM : int
		{
			/// <summary>
			/// Base message
			/// </summary>
			WM_USER = 1024,

			WM_SETTEXT = 0x000C,
			
			WM_GETTEXT = 0x000D,
			WM_GETTEXTLENGTH = 0x000E,

			/// <summary>
			/// Sent by an application to the HTML viewer control to request that default stylesheet rules be applied to the current HTML window. 
			/// </summary>
			ADDSTYLE = (WM_USER + 126),

			/// <summary>
			/// Sent by an application to add text to the HTML Control. 
			/// </summary>
			ADDTEXT = (WM_USER + 101),

			/// <summary>
			/// Sent by an application to add Unicode text to the HTML control. 
			/// </summary>
			ADDTEXTW = (WM_USER + 102),

			/// <summary>
			/// Sent by an application to tell the HTML control to jump to the indicated anchor. 
			/// </summary>
			ANCHOR = (WM_USER + 105),

			/// <summary>
			/// Sent by an application to tell the HTML control to jump to the indicated anchor. 
			/// </summary>
			ANCHORW = (WM_USER + 106),
		
			/// <summary>
			///  Sent by an application to the HTML viewer control to request a reference to its IDispatch interface. 	
			/// </summary>
			BROWSERDISPATCH = (WM_USER + 124),

			/// <summary>
			///  Sent by an application to clear the contents of the HTML control. 
			/// </summary>
			CLEAR = (WM_USER + 113),
		
			/// <summary>
			///  Sent by an application to enable or disable ClearType for HTML text rendering. 
			/// </summary>
			ENABLECLEARTYPE = (WM_USER + 114),

			/// <summary>
			///  Sent by an application to enable or disable the context-sensitive menu for the HTML control. 
			///  </summary>
			ENABLECONTEXTMENU = (WM_USER + 110),

			/// <summary>
			///  Sent by an application to enable or disable scripting for the HTML control. 
			/// </summary>
			ENABLESCRIPTING = (WM_USER + 115),
		
			/// <summary>
			///  Sent by an application to command the HTML control to toggle the image shrink enable mode, or shrink mode. 
			/// </summary>
			ENABLESHRINK = (WM_USER + 107),
		
			/// <summary>
			///  Sent by an application to inform the HTML control that the document is complete. 
			/// </summary>
			ENDOFSOURCE = (WM_USER + 104),

			/// <summary>
			/// Sent by an application to inform the HTML control that the image indicated by the cookie could not be loaded. 
			/// </summary>
			IMAGEFAIL = (WM_USER + 109),
		
			/// <summary>
			///  Sent by an application to determine if some text was selected in the HTML control. 
			/// </summary>
			ISSELECTION = (WM_USER + 112),
	
			/// <summary>
			///  Sent by an application to determine the layout height (in pixels) of the content in the HTML control. 
			/// </summary>
			LAYOUTHEIGHT = (WM_USER + 118),

			/// <summary>
			///  Sent by an application to determine the layout width (in pixels) of the content in the HTML control. 
			/// </summary>
			LAYOUTWIDTH = (WM_USER + 117),
		
			/// <summary>
			///  Sent by an application to navigate to a particular URL in the HTML control. 
			/// </summary>
			NAVIGATE = (WM_USER + 120),
		
			/// <summary>
			///  Sent by an application to tell an HTML viewer control to select all the text in the current HTML page. 
			/// </summary>
			SELECTALL = (WM_USER + 111),
		
			/// <summary>
			///  Sent by an application to associate a bitmap with an image. 
			/// </summary>
			SETIMAGE = (WM_USER + 103),

			/// <summary>
			///  Sent by an application to STOP a navigation started in the HTML control. 
			/// </summary>
			STOP = (WM_USER + 125),

			/// <summary>
			/// Sent by an application to specify the zoom level (0-4). 
			/// </summary>
			ZOOMLEVEL = (WM_USER + 116),
		}
		#endregion

		#region NM Enumeration
		/// <summary>
		/// Notifications received from the native control
		/// </summary>
		private enum NM : int
		{
			WM_USER = 1024,
			/// <summary>
			/// Sent by the HTML viewer control if the user taps on a link or submits a form.
			/// </summary>
			HOTSPOT =             (WM_USER + 101),
		
			/// <summary>
			/// Sent by the HTML viewer control to notify the application to load the image.
			/// </summary>
			INLINE_IMAGE =        (WM_USER + 102),
		
			/// <summary>
			/// Sent by the HTML viewer control to notify the application to load the sound.
			/// </summary>
			INLINE_SOUND =        (WM_USER + 103),

			/// <summary>
			/// Sent by the HTML viewer control to notify the application and provide the title of the HTML document.
			/// </summary>
			TITLE =               (WM_USER + 104),

			/// <summary>
			/// Sent by the HTML viewer control to notify the application and provide the HTTP-EQUIV and CONTENT parameters of the META tag in the HTML document.
			/// </summary>
			META =                (WM_USER + 105),

			/// <summary>
			/// Sent by the HTML viewer control when it encounters a BASE tag in an HTML document.
			/// </summary>
			BASE =                (WM_USER + 106),

			/// <summary>
			/// Sent by the HTML viewer control when a user holds the stylus down in the viewer window before a context menu appears.
			/// </summary>
			CONTEXTMENU =         (WM_USER + 107),

			/// <summary>
			/// Sent by the HTML viewer control to notify the application that XML was loaded.
			/// </summary>
			INLINEXML =          (WM_USER + 108),

			/// <summary>
			/// Sent by the HTML viewer control before a navigation request to a URL begins.
			/// </summary>
			BEFORENAVIGATE =      (WM_USER + 109),

			/// <summary>
			/// Sent by the HTML viewer control when the document navigation is complete.
			/// </summary>
			DOCUMENTCOMPLETE =    (WM_USER + 110),

			/// <summary>
			/// Sent by the HTML viewer control when a navigation request has completed.
			/// </summary>
			NAVIGATECOMPLETE =    (WM_USER + 111),
	
			/// <summary>
			/// Sent by the HTML viewer control when the document title changes.
			/// </summary>
			TITLECHANGE =         (WM_USER + 112),

			/// <summary>
			/// *DOCUMENTATION UNAVAILABLE*
			/// </summary>
			INLINESTYLE =        (WM_USER + 113),
		}
		#endregion

	}

	/// <summary>
	/// Represents the method that will handle the DocumentCompleted event of a <see cref="WebBrowser"/> control.
	/// </summary>
	public delegate void WebBrowserDocumentCompletedEventHandler(object sender, WebBrowserDocumentCompletedEventArgs e);

	/// <summary>
	/// Provides data for the DocumentCompleted event.
	/// </summary>
	public class WebBrowserDocumentCompletedEventArgs : EventArgs
	{
		private string m_url;

		/// <summary>
		/// Initializes a new instance of the WebBrowserDocumentCompletedEventArgs class.
		/// </summary>
		/// <param name="url">The URL of the newly loaded document.</param>
		public WebBrowserDocumentCompletedEventArgs(string url)
		{
			m_url = url;
		}

		/// <summary>
		/// Gets the URL of the document to which the WebBrowser control has navigated.
		/// </summary>
		public string Url
		{
			get
			{
				return m_url;
			}
		}
	}

	/// <summary>
	/// Represents the method that will handle the Navigated event of a <see cref="WebBrowser"/> control.
	/// </summary>
	public delegate void WebBrowserNavigatedEventHandler(object sender, WebBrowserNavigatedEventArgs e);

	/// <summary>
	/// Provides data for the Navigated event.
	/// </summary>
	public class WebBrowserNavigatedEventArgs : EventArgs
	{
		private string m_url;

		/// <summary>
		/// Initializes a new instance of the WebBrowserNavigatedEventArgs class.
		/// </summary>
		/// <param name="url">The URL of the document to which the WebBrowser control has navigated.</param>
		public WebBrowserNavigatedEventArgs(string url)
		{
			m_url = url;
		}

		/// <summary>
		/// Gets the URL of the document to which the WebBrowser control has navigated.
		/// </summary>
		public string Url
		{
			get
			{
				return m_url;
			}
		}
	}

	/// <summary>
	/// Represents the method that will handle the Navigating event of a <see cref="WebBrowser"/> control.
	/// </summary>
	public delegate void WebBrowserNavigatingEventHandler(object sender, WebBrowserNavigatingEventArgs e);

	/// <summary>
	/// Provides data for the Navigating event.
	/// </summary>
	public class WebBrowserNavigatingEventArgs : CancelEventArgs
	{
		private string m_url;

		/// <summary>
		/// Initializes a new instance of the WebBrowserNavigatingEventArgs class.
		/// </summary>
		/// <param name="url">The URL of the newly loaded document.</param>
		public WebBrowserNavigatingEventArgs(string url)
		{
			m_url = url;
		}

		/// <summary>
		/// Gets the URL of the document to which the WebBrowser control is navigating.
		/// </summary>
		public string Url
		{
			get
			{
				return m_url;
			}
		}
	}
}
