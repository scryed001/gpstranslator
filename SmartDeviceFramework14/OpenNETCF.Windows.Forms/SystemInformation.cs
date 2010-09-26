//==========================================================================================
//
//		OpenNETCF.Windows.Forms.SystemInformationEx
//		Copyright (c) 2005, OpenNETCF.org
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
using System.Runtime.InteropServices;
using System.Windows.Forms;

//Retrieve power and other system metrics. New for v1.3 Peter Foot

namespace OpenNETCF.Windows.Forms
{
	/// <summary>
	/// Provides information about the current system environment.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	/// <remarks>Enhances functionality available in <see cref="SystemInformation"/>.</remarks>
	public class SystemInformationEx
	{
		private SystemInformationEx(){}

		private static PowerStatus powerStatus;
		
		

		

		/// <summary>
		/// Gets the height, in pixels, of the standard title bar area of a window.
		/// </summary>
		public static int CaptionHeight
		{
			get
			{
				return GetSystemMetrics(SM.CYCAPTION);
			}
		}

		/// <summary>
		/// Gets the maximum size, in pixels, that a cursor can occupy.
		/// </summary>
		public static Size CursorSize
		{
			get
			{
				return new Size(GetSystemMetrics(SM.CXCURSOR), GetSystemMetrics(SM.CYCURSOR));
			}
		}

		/// <summary>
		/// Gets a value indicating whether the debug version of Windows CE is installed.
		/// </summary>
		public static bool DebugOS
		{
			get
			{
				return (GetSystemMetrics(SM.DEBUG) != 0);
			}
		}

		/// <summary>
		/// Gets the dimensions, in pixels, of the area within which the user must click twice for the operating system to consider the two clicks a double-click.
		/// </summary>
		public static Size DoubleClickSize
		{
			get
			{
				return new Size(GetSystemMetrics(SM.CXDOUBLECLK), GetSystemMetrics(SM.CYDOUBLECLK));
			}
		}

		//for completeness wrap the framework property
		/// <summary>
		/// Gets the maximum number of milliseconds allowed between mouse clicks for a double-click to be valid.
		/// </summary>
		public static int DoubleClickTime
		{
			get
			{
				return SystemInformation.DoubleClickTime;
			}
		}
		/// <summary>
		/// Gets the thickness, in pixels, of the frame border of a window that has a caption and is not resizable.
		/// </summary>
		public static Size FixedFrameBorderSize
		{
			get
			{
				return new Size(GetSystemMetrics(SM.CXDLGFRAME), GetSystemMetrics(SM.CYDLGFRAME));
			}
		}

		/// <summary>
		/// Gets the width, in pixels, of the arrow bitmap on the horizontal scroll bar.
		/// </summary>
		public static int HorizontalScrollBarArrowWidth
		{
			get
			{
				return GetSystemMetrics(SM.CXHSCROLL);
			}
		}

		/// <summary>
		/// Gets the default height, in pixels, of the horizontal scroll bar.
		/// </summary>
		public static int HorizontalScrollBarHeight
		{
			get
			{
				return GetSystemMetrics(SM.CYHSCROLL);
			}
		}

		/// <summary>
		/// Gets the dimensions, in pixels, of the Windows default program icon size.
		/// </summary>
		public static Size IconSize
		{
			get
			{
				return new Size(GetSystemMetrics(SM.CXICON), GetSystemMetrics(SM.CYICON));
			}
		}

		/// <summary>
		/// Gets the size, in pixels, of the grid square used to arrange icons in a large-icon view.
		/// </summary>
		public static Size IconSpacingSize
		{
			get
			{
				return new Size(GetSystemMetrics(SM.CXICONSPACING), GetSystemMetrics(SM.CXICONSPACING));
			}
		}

		/// <summary>
		/// Gets the height, in pixels, of one line of a menu.
		/// </summary>
		public static int MenuHeight
		{
			get
			{
				return GetSystemMetrics(SM.CYMENU);
			}
		}

		/// <summary>
		/// Gets the number of display monitors.
		/// </summary>
		public static int MonitorCount
		{
			get
			{
				int cmons = GetSystemMetrics(SM.CMONITORS);
				if(cmons == 0)
				{
					return 1;
				}

				return cmons;
			}
		}

		/// <summary>
		/// Gets a value indicating whether all the display monitors are using the same pixel color format.
		/// </summary>
		public static bool MonitorsSameDisplayFormat
		{
			get
			{
				if(MonitorCount > 1)
				{
					return (GetSystemMetrics(SM.SAMEDISPLAYFORMAT) != 0);
				}
				return true;
			}
		}

		/// <summary>
		/// Gets a value indicating whether a mouse is installed.
		/// </summary>
		public static bool MousePresent
		{
			get
			{
				return (GetSystemMetrics(SM.MOUSEPRESENT) != 0);
			}
		}

		#region Power Status
		/// <summary>
		/// Gets the current system power status.
		/// </summary>
		public static PowerStatus PowerStatus
		{
			get
			{
				if(powerStatus==null)
				{
					powerStatus = new PowerStatus();
				}

				return powerStatus;

			}
		} 
		#endregion

		/// <summary>
		/// Gets the dimensions, in pixels, of the current video mode of the primary display.
		/// </summary>
		public static Size PrimaryMonitorSize
		{
			get
			{
				return new Size(GetSystemMetrics(SM.CXSCREEN), GetSystemMetrics(SM.CYSCREEN));
			}
		}

		/// <summary>
		/// Gets the dimensions, in pixels, of a small icon.
		/// </summary>
		public static Size SmallIconSize
		{
			get
			{
				return new Size(GetSystemMetrics(SM.CXSMICON), GetSystemMetrics(SM.CYSMICON));
			}
		}

		/// <summary>
		/// Gets the height, in pixels, of the arrow bitmap on the vertical scroll bar.
		/// </summary>
		public static int VerticalScrollBarArrowHeight
		{
			get
			{
				return GetSystemMetrics(SM.CYVSCROLL);
			}
		}

		/// <summary>
		/// Gets the default width, in pixels, of the vertical scroll bar.
		/// </summary>
		public static int VerticalScrollBarWidth
		{
			get
			{
				return GetSystemMetrics(SM.CXVSCROLL);
			}
		}

		/// <summary>
		/// Gets the bounds of the virtual screen.
		/// </summary>
		public static Rectangle VirtualScreen
		{
			get
			{
				return new Rectangle(GetSystemMetrics(SM.XVIRTUALSCREEN),GetSystemMetrics(SM.YVIRTUALSCREEN),GetSystemMetrics(SM.CXVIRTUALSCREEN),GetSystemMetrics(SM.CYVIRTUALSCREEN));
			}
		}

		#region Working Area
		/// <summary>
		/// Gets the size, in pixels, of the working area of the screen.
		/// </summary>
		public static Rectangle WorkingArea
		{
			get
			{
				return Screen.PrimaryScreen.WorkingArea;
			}
		}
		#endregion

		#region Native Methods
		[DllImport("coredll.dll", SetLastError=true)]
		private static extern int GetSystemMetrics(SM nIndex); 
		#endregion
	}

	#region System Metrics
	internal enum SM : int
	{
		CXSCREEN             =0,
		CYSCREEN             =1,
		CXVSCROLL            =2,
		CYHSCROLL            =3,
		CYCAPTION            =4,
		CXBORDER             =5,
		CYBORDER             =6,
		CXDLGFRAME           =7,
		CYDLGFRAME           =8,
		CXICON               =11,
		CYICON               =12,
		// @CESYSGEN IF GWES_ICONCURS
		CXCURSOR             =13,
		CYCURSOR             =14,
		// @CESYSGEN ENDIF
		CYMENU               =15,
		CXFULLSCREEN 		=16,
		CYFULLSCREEN         =17,
		MOUSEPRESENT         =19,
		CYVSCROLL            =20,
		CXHSCROLL            =21,
		DEBUG                =22,
		CXDOUBLECLK          =36,
		CYDOUBLECLK          =37,
		CXICONSPACING        =38,
		CYICONSPACING        =39,
		CXEDGE               =45,
		CYEDGE               =46,
		CXSMICON             =49,
		CYSMICON             =50,

		XVIRTUALSCREEN       =76,
		YVIRTUALSCREEN       =77,
		CXVIRTUALSCREEN      =78,
		CYVIRTUALSCREEN      =79,
		CMONITORS            =80,
		SAMEDISPLAYFORMAT    =81,
	}
	#endregion
}
