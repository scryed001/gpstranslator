//==========================================================================================
//
//		OpenNETCF.Windows.Forms.DateTimePicker
//		Copyright (c) 2003-2005, OpenNETCF.org
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

using OpenNETCF.Drawing;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

#if DESIGN
using System.ComponentModel;
#else
using OpenNETCF;
using OpenNETCF.Win32;
#endif

namespace OpenNETCF.Windows.Forms
{
#if DESIGN
	public class DateTimePicker : System.Windows.Forms.DateTimePicker
	{

		#region Format
		OpenNETCF.Windows.Forms.DateTimePickerFormat format = OpenNETCF.Windows.Forms.DateTimePickerFormat.Short;

		[Category("Appearance"),
		DefaultValue(OpenNETCF.Windows.Forms.DateTimePickerFormat.Short),
		Description("Gets or sets the format of the date and time displayed in the control.")]
		/// <summary>
		/// Gets or sets the format of the date and time displayed in the control.
		/// </summary>
		public new OpenNETCF.Windows.Forms.DateTimePickerFormat Format
		{
			get
			{
				return format;
			}
			set
			{
				format = value;

				//convert value so designer updates
				switch(value)
				{
					case OpenNETCF.Windows.Forms.DateTimePickerFormat.Custom:
						base.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
						return;
					case OpenNETCF.Windows.Forms.DateTimePickerFormat.Long:
						base.Format = System.Windows.Forms.DateTimePickerFormat.Long;
						return;
					case OpenNETCF.Windows.Forms.DateTimePickerFormat.Short:
						base.Format = System.Windows.Forms.DateTimePickerFormat.Short;
						return;
					case OpenNETCF.Windows.Forms.DateTimePickerFormat.Time:
						base.Format = System.Windows.Forms.DateTimePickerFormat.Time;
						return;
				}
			}
		}
		#endregion

	}
#else

	/// <summary>
	/// Represents a Windows date/time picker control.
	/// <para><b>Revised in v1.3</b></para>
	/// </summary>
	public class DateTimePicker : ControlEx
	{
		//dtp style
		private int m_style;

		#region Constructor

		static DateTimePicker()
		{
			// initialize the common controls
			Win32Window.InitCommonControlsEx(new byte[]{8,0,0,0,0,1,0,0});
		}

		public DateTimePicker() : base(true)
		{}
		#endregion

		#region Overrides

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ClassName = "SysDateTimePick32";
				cp.ClassStyle |= m_style;
				return cp;
			}
		}

#if !NDOC
		protected override void OnNotifyMessage(ref Microsoft.WindowsCE.Forms.Message m)
		{
			if(m.Msg == 0x004E)
			{
				NMHDR nh = (NMHDR)Marshal.PtrToStructure(m.LParam, typeof(OpenNETCF.Win32.NMHDR));

				switch(nh.code)
				{
					case (int)DTN.DATETIMECHANGE:
						OnValueChanged(new EventArgs());
						break;
					case (int)DTN.CLOSEUP:
						OnCloseUp(new EventArgs());
						break;
					case (int)DTN.DROPDOWN:
						OnDropDown(new EventArgs());
						break;
					default:
						MessageBox.Show(nh.code.ToString("X"));
						break;
				}
			}

			base.OnNotifyMessage (ref m);
		}
#endif

		
		#endregion

		/// <summary>
		/// Sets the current style of the DateTimePicker with all the values specified in this wrapper.
		/// </summary>
		private void SetDTPStyle()
		{
			DTS style = 0;
			if(showCheckBox)
				style |= DTS.SHOWNONE;

			if(showUpDown)
				style |= DTS.UPDOWN;

			switch(format)
			{
				case DateTimePickerFormat.Long:
					style |= DTS.LONGDATEFORMAT;
					break;

				case DateTimePickerFormat.Short:
					style |= DTS.SHORTDATEFORMAT;
					break;

				case DateTimePickerFormat.Time:
					style |= DTS.TIMEFORMAT;
					break;

				case DateTimePickerFormat.Custom:
					Win32Window.SendMessage(ChildHandle, (int)DTM.SETFORMATW, 0, CustomFormat);
					break;
			}

			m_style = (int)style;            			
		}

		#region Public Fields
		/// <summary>
		/// Specifies the maximum date value of the date/time picker control.
		/// </summary>
		public static readonly DateTime MaxDateTime = new DateTime(9990, 12, 31);
		/// <summary>
		/// Specifies the minimum date value of the date/time picker control.
		/// </summary>
		public static readonly DateTime MinDateTime = new DateTime(1753, 1, 1);
		#endregion

		#region Properties

		DateTime dateValue = DateTime.Now;

		/// <summary>
		/// Gets or sets the date/time value assigned to the control.
		/// </summary>
		public DateTime Value
		{
			get
			{
				byte[] data = new byte[16];								
				Win32Window.SendMessage(ChildHandle, (int)DTM.GETSYSTEMTIME, (int)GDT.VALID, data);
				SystemTime st = new SystemTime(data);
				return st.ToDateTime();			
			}
			set
			{
				dateValue = value;

				SystemTime date = SystemTime.FromDateTime(value);
				Win32Window.SendMessage(ChildHandle, (int)DTM.SETSYSTEMTIME, (int)GDT.VALID, date.ToByteArray());
			}
			
		}

		DateTime minDate = MinDateTime;

		/// <summary>
		/// Gets or sets the minimum date and time that can be selected in the control.
		/// </summary>
		public DateTime MinDate
		{
			get
			{
				return minDate;
			}
			set
			{
				minDate = value;

				byte[] maxTimeData = SystemTime.FromDateTime(maxDate).ToByteArray();
				byte[] minTimeData = SystemTime.FromDateTime(minDate).ToByteArray();
				byte[] data = new byte[32];
				Array.Copy(minTimeData, 0, data, 0, 16);
				Array.Copy(maxTimeData, 0, data, 16, 16);
				Win32Window.SendMessage(ChildHandle, (int)DTM.SETRANGE, (int)(GDTR.MIN | GDTR.MAX), data);
			}
		}

		DateTime maxDate = MaxDateTime;
	
		/// <summary>
		/// Gets or sets the maximum date and time that can be selected in the control.
		/// </summary>
		public DateTime MaxDate
		{
			get
			{
				return maxDate;
			}
			set
			{				
				maxDate = value;

				byte[] maxTimeData = SystemTime.FromDateTime(maxDate).ToByteArray();
				byte[] minTimeData = SystemTime.FromDateTime(minDate).ToByteArray();
				byte[] data = new byte[32];
				Array.Copy(minTimeData, 0, data, 0, 16);
				Array.Copy(maxTimeData, 0, data, 16, 16);
				Win32Window.SendMessage(ChildHandle, (int)DTM.SETRANGE, (int)(GDTR.MIN | GDTR.MAX), data);
			}
		}

		private string customFormat;

		/// <summary>
		/// Gets or sets the custom date/time format string.
		/// </summary>
		public string CustomFormat
		{
			get
			{
				return customFormat;
			}
			set
			{
				customFormat = value;
				
				SetDTPStyle();
			}
		}

		bool showCheckBox = false;
	
		/// <summary>
		/// Determines whether a check box is displayed in the control.  
		/// When the check box is unchecked, no value is selected.
		/// </summary>
		public bool ShowCheckBox
		{
			get
			{
				return showCheckBox;
			}
			set
			{
				showCheckBox = value;

				SetDTPStyle();
			}
		}

		bool showUpDown = false;

		/// <summary>
		/// Controls whether an up-Down(spin) button is used to modify dates instead of a drop-down calendar.
		/// </summary>
		public bool ShowUpDown
		{
			get
			{
				return showUpDown;
			}
			set
			{
				showUpDown = value;

				SetDTPStyle();

			}
		}

		DateTimePickerFormat format = DateTimePickerFormat.Short;

		/// <summary>
		/// Gets or sets the format of the date and time displayed in the control.
		/// </summary>
		public DateTimePickerFormat Format
		{
			get
			{
				return format;
			}
			set
			{
				format = value;

				SetDTPStyle();			

			}
		}

		#region Checked
		/// <summary>
		/// Gets or sets a value indicating whether the <see cref="Value"/> property has been set with a valid date/time value and the displayed value is able to be updated.
		/// <para><b>New in v1.4</b></para></summary>
		public bool Checked
		{
			get
			{
				byte[] buff = new byte[16];
				int validity = (int)Win32Window.SendMessage(ChildHandle, (int)DTM.GETSYSTEMTIME, 0, buff);
				switch((GDT)validity)
				{
					case GDT.VALID:
						return true;
					case GDT.ERROR:
					case GDT.NONE:
						return false;
					default:
						return false;
				}
			}
			set
			{
				if(value)
				{
					//set last valid value
					Win32Window.SendMessage(ChildHandle, (int)DTM.SETSYSTEMTIME, (int)GDT.VALID, SystemTime.FromDateTime(this.dateValue));
				}
				else
				{
					Win32Window.SendMessage(ChildHandle, (int)DTM.SETSYSTEMTIME, (int)GDT.NONE, 0);
					
				}
			}
		}
		#endregion

		#region Calendar Appearance Properties


		private Color calendarForeColor = SystemColors.ControlText;

		/// <summary>
		/// The color used to display text within a month.
		/// </summary>
		public Color CalendarForeColor
		{
			get
			{
				return calendarForeColor;
			}
			set
			{
				calendarForeColor = value;

				Win32Window.SendMessage(this.ChildHandle, (int)DTM.SETMCCOLOR, 
					(int)MCSC.TEXT, 
					ColorTranslator.ToWin32(value));				
			}
		}

		private Color calendarMonthBackground = SystemColors.Window;

		/// <summary>
		/// The background color displayed within the month.
		/// This is not supported in Windows CE?
		/// </summary>
		public Color CalendarMonthBackground
		{
			get
			{
				return calendarMonthBackground;
			}
			set
			{
				calendarMonthBackground = value;

				Win32Window.SendMessage(this.ChildHandle, (int)DTM.SETMCCOLOR, 
					(int)MCSC.BACKGROUND, 
					ColorTranslator.ToWin32(value));				

			}
		}

		private Color calendarTitleBackColor = SystemColors.ActiveCaption;

		/// <summary>
		/// The background color displayed in the calendar's title.
		/// </summary>
		public Color CalendarTitleBackColor
		{
			get
			{
				return calendarTitleBackColor;
			}
			set
			{
				calendarTitleBackColor = value;

				Win32Window.SendMessage(this.ChildHandle, (int)DTM.SETMCCOLOR, 
					(int)MCSC.TITLEBK, 
					ColorTranslator.ToWin32(value));				
			}
		}

		private Color calendarTitleForeColor = SystemColors.ActiveCaptionText;


		/// <summary>
		/// The color used to display text within the calendar's title.
		/// </summary>
		public Color CalendarTitleForeColor
		{
			get
			{
				return calendarTitleForeColor;
			}
			set
			{
				calendarTitleForeColor = value;

				Win32Window.SendMessage(this.ChildHandle,(int)DTM.SETMCCOLOR, 
					(int)MCSC.TITLETEXT, 
					ColorTranslator.ToWin32(value));				

			}
		}

		private Color calendarTrailingForeColor = SystemColors.GrayText;

		/// <summary>
		/// The color used to display the header and trailing day text.
		/// Header and trailing days are the days from the previous and following months that appear on the current month calendar.
		/// </summary>
		public Color CalendarTrailingForeColor
		{
			get
			{
				return calendarTrailingForeColor;
			}
			set
			{
				calendarTrailingForeColor = value;

				Win32Window.SendMessage(this.ChildHandle, (int)DTM.SETMCCOLOR, 
					(int)MCSC.TRAILINGTEXT, 
					ColorTranslator.ToWin32(value));				
			}
		}
		#endregion

		#endregion

		#region Events
		/// <summary>
		/// Occurs when the S<see cref="Value"/> property changes.
		/// </summary>
		public event EventHandler ValueChanged;

		protected void OnValueChanged(EventArgs e)
		{
			if(this.ValueChanged!=null)
			{
				this.ValueChanged(this, e);
			}
		}

		/// <summary>
		/// Occurs when the drop-down calendar is shown.
		/// </summary>
		public event EventHandler DropDown;

		protected void OnDropDown(EventArgs e)
		{
			if(this.DropDown!=null)
			{
				this.DropDown(this, e);
			}
		}

		/// <summary>
		/// Occurs when the drop-down calendar is dismissed and disappears.
		/// </summary>
		public event EventHandler CloseUp;

		protected void OnCloseUp(EventArgs e)
		{
			if(this.CloseUp!=null)
			{
				this.CloseUp(this, e);
			}
		}
		#endregion

	}
	#endif

	/*
	/// <summary>
	/// Represents a Windows date/time picker control.
	/// </summary>
	public class OldDateTimePicker : Control
	{
		#region Overrides

		/// <summary>
		/// Resizes the underlying DateTimePicker control.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnResize(EventArgs e)
		{	
#if !DESIGN
			Win32Window.SetWindowPos(dtpWnd, Win32Window.SetWindowPosZOrder.HWND_TOP, 0, 0, this.Width, this.Height, Win32Window.SetWindowPosFlags.SWP_NOMOVE);
#else
			dtp.Width = this.Width;
			dtp.Height = this.Height;
#endif
		}

#if !DESIGN
		/// <summary>
		/// Releases the unmanaged resources used by the DateTimePicker and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources;
		/// false to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				Win32Window.DestroyWindow(dtpWnd);
			}
			base.Dispose (disposing);
		}
#endif

		#endregion

		#region Public Fields
		/// <summary>
		/// Specifies the maximum date value of the date/time picker control.
		/// </summary>
		public static readonly DateTime MaxDateTime = new DateTime(9990, 12, 31);
		/// <summary>
		/// Specifies the minimum date value of the date/time picker control.
		/// </summary>
		public static readonly DateTime MinDateTime = new DateTime(1753, 1, 1);
		#endregion
		
		#region Private fields and helper methods

#if DESIGN
		System.Windows.Forms.DateTimePicker dtp;
#endif

		#region "API Declares"

		[Flags]
		enum SetRangeMessage
		{
			GDTR_MIN = 0x0001,
			GDTR_MAX = 0x0002
		}

		enum MonthCalendarColorMessage
		{
			MCSC_BACKGROUND = 0,
			MCSC_TEXT,
			MCSC_TITLEBK,
			MCSC_TITLETEXT,
			MCSC_MONTHBK,
			MCSC_TRAILINGTEXT
		}

		enum DTPMessage : int
		{
			DTM_GETSYSTEMTIME = 0x1001,
			DTM_SETSYSTEMTIME = 0x1002,
			DTM_GETRANGE = 0x1003,
			DTM_SETRANGE = 0x1004,
			DTM_SETFORMATA = 0x1005,
			DTM_SETFORMATW = 0x1032,
			DTM_SETMCCOLOR = 0x1006,
			DTM_GETMCCOLOR = 0x1007,
			DTM_GETMONTHCAL = 0x1008,
			DTM_SETMCFONT = 0x1009,
			DTM_GETMCFONT = 0x100A,
		}

		[Flags]
		enum DTPStyle : uint
		{
			DTS_UPDOWN = 0x0001, // use UPDOWN instead of MONTHCAL
			DTS_SHOWNONE = 0x0002, // allow a NONE or checkbox selection
			DTS_SHORTDATEFORMAT = 0x0000, // use the short date format (app must forward WM_WININICHANGE messages)
			DTS_LONGDATEFORMAT = 0x0004, // use the long date format (app must forward WM_WININICHANGE messages)
			DTS_TIMEFORMAT = 0x0009, // use the time format (app must forward WM_WININICHANGE messages)
			DTS_APPCANPARSE = 0x0010, // allow user entered strings (app MUST respond to DTN_USERSTRING)
			DTS_RIGHTALIGN = 0x0020, // right-align popup instead of left-align it
			DTS_NONEBUTTON = 0x0080, // use NONE button instead of checkbox
		}

		[DllImport("Commctrl.dll")]
		private static extern bool InitCommonControlsEx(byte[] data);

		#endregion

#if !DESIGN
		IntPtr parentWnd;
		IntPtr dtpWnd;
		      
		// the base style for the DateTimePicker window
		int baseStyle = (int)(Win32Window.WindowStyle.WS_BORDER | Win32Window.WindowStyle.WS_VISIBLE | Win32Window.WindowStyle.WS_CHILD);

		/// <summary>
		/// Sets the current style of the DateTimePicker with all the values specified in this wrapper.
		/// </summary>
		private void SetDTPStyle()
		{
			DTPStyle style = 0;
			if(showCheckBox)
				style |= DTPStyle.DTS_SHOWNONE;

			if(showUpDown)
				style |= DTPStyle.DTS_UPDOWN;

			switch(format)
			{
				case DateTimePickerFormat.Long:
					style |= DTPStyle.DTS_LONGDATEFORMAT;
					break;

				case DateTimePickerFormat.Short:
					style |= DTPStyle.DTS_SHORTDATEFORMAT;
					break;

				case DateTimePickerFormat.Time:
					style |= DTPStyle.DTS_TIMEFORMAT;
					break;

				case DateTimePickerFormat.Custom:
					Win32Window.SendMessage(dtpWnd, (int)DTPMessage.DTM_SETFORMATW, 0, CustomFormat);
					break;
			}

			// call a simple SetWindowStyle for the dtp window
			Win32Window.SetWindowStyle(dtpWnd, baseStyle | (int)style);            			
		}
#endif

		#endregion

		#region Properties

		DateTime dateValue = DateTime.Now;
#if DESIGN
		[
		Category("Behavior"),
		Description("Gets or sets the date/time value assigned to the control.")
		]
#endif	
		/// <summary>
		/// Gets or sets the date/time value assigned to the control.
		/// </summary>
		public DateTime Value
		{
			set
			{
				dateValue = value;
#if !DESIGN
				SystemTime date = SystemTime.FromDateTime(value);
				Win32Window.SendMessage(dtpWnd, (int)DTPMessage.DTM_SETSYSTEMTIME, 0, date.ToByteArray());
#else
				dtp.Value = value;
#endif
			}
			get
			{
#if !DESIGN
				byte[] data = new byte[16];								
				Win32Window.SendMessage(dtpWnd, (int)DTPMessage.DTM_GETSYSTEMTIME, 0, data);
				SystemTime st = new SystemTime(data);
				return st.ToDateTime();			
#else
				return dateValue;
#endif

			}
		}

		DateTime minDate = MinDateTime;
#if DESIGN
		[
		Category("Behavior"),
		Description("Gets or sets the minimum date and time that can be selected in the control.")
		]
#endif	
		/// <summary>
		/// Gets or sets the minimum date and time that can be selected in the control.
		/// </summary>
		public DateTime MinDate
		{
			get
			{
				return minDate;
			}
			set
			{
				minDate = value;

#if !DESIGN
				byte[] maxTimeData = SystemTime.FromDateTime(maxDate).ToByteArray();
				byte[] minTimeData = SystemTime.FromDateTime(minDate).ToByteArray();
				byte[] data = new byte[32];
				Array.Copy(minTimeData, 0, data, 0, 16);
				Array.Copy(maxTimeData, 0, data, 16, 16);
				Win32Window.SendMessage(dtpWnd, (int)DTPMessage.DTM_SETRANGE, (int)(SetRangeMessage.GDTR_MIN | SetRangeMessage.GDTR_MAX), data);
#endif
			}
		}

		DateTime maxDate = MaxDateTime;
#if DESIGN
		[
		Category("Behavior"),
		Description("Gets or sets the maximum date and time that can be selected in the control.")
		]
#endif	
		/// <summary>
		/// Gets or sets the maximum date and time that can be selected in the control.
		/// </summary>
		public DateTime MaxDate
		{
			get
			{
				return maxDate;
			}
			set
			{				
				maxDate = value;

#if !DESIGN
				byte[] maxTimeData = SystemTime.FromDateTime(maxDate).ToByteArray();
				byte[] minTimeData = SystemTime.FromDateTime(minDate).ToByteArray();
				byte[] data = new byte[32];
				Array.Copy(minTimeData, 0, data, 0, 16);
				Array.Copy(maxTimeData, 0, data, 16, 16);
				Win32Window.SendMessage(dtpWnd, (int)DTPMessage.DTM_SETRANGE, (int)(SetRangeMessage.GDTR_MIN | SetRangeMessage.GDTR_MAX), data);
#endif
			}
		}

		private string customFormat;
#if DESIGN
		[
		Category("Behavior"),
		Description("Gets or sets the custom date/time format string.")
		]
#endif	
		/// <summary>
		/// Gets or sets the custom date/time format string.
		/// </summary>
		public string CustomFormat
		{
			get
			{
				return customFormat;
			}
			set
			{
				customFormat = value;
#if !DESIGN				
				SetDTPStyle();
#else
				dtp.CustomFormat = value;
#endif
			}
		}

		bool showCheckBox = false;
#if DESIGN
		[
		Category("Appearance"),
		Description("Determines whether a check box is displayed in the control.  " + 
			"When the check box is unchecked, no value is selected.")
		]
#endif		
		/// <summary>
		/// Determines whether a check box is displayed in the control.  
		/// When the check box is unchecked, no value is selected.
		/// </summary>
		public bool ShowCheckBox
		{
			get
			{
				return showCheckBox;
			}
			set
			{
				showCheckBox = value;
#if !DESIGN
				SetDTPStyle();
#else
				dtp.ShowCheckBox = value;
#endif
			}
		}

		bool showUpDown = false;
#if DESIGN
		[
		Category("Appearance"),
		Description("Controls whether an up-Down(spin) button is used to modify dates instead of " + 
			"a drop-down calendar.")
		]
#endif
		/// <summary>
		/// Controls whether an up-Down(spin) button is used to modify dates instead of a drop-down calendar.
		/// </summary>
		public bool ShowUpDown
		{
			get
			{
				return showUpDown;
			}
			set
			{
				showUpDown = value;
#if !DESIGN
				SetDTPStyle();
#else
				dtp.ShowUpDown = value;
#endif
			}
		}

		DateTimePickerFormat format = DateTimePickerFormat.Short;
#if DESIGN
		[
		Category("Appearance"),
		Description("Gets or sets the format of the date and time displayed in the control.")
		]
#endif
		/// <summary>
		/// Gets or sets the format of the date and time displayed in the control.
		/// </summary>
		public DateTimePickerFormat Format
		{
			get
			{
				return format;
			}
			set
			{
				format = value;
#if !DESIGN
				SetDTPStyle();
#else
				switch(value)
				{
					case DateTimePickerFormat.Long:
						dtp.Format = System.Windows.Forms.DateTimePickerFormat.Long;
						break;

					case DateTimePickerFormat.Short:
						dtp.Format = System.Windows.Forms.DateTimePickerFormat.Short;
						break;

					case DateTimePickerFormat.Time:
						dtp.Format = System.Windows.Forms.DateTimePickerFormat.Time;
						break;

					case DateTimePickerFormat.Custom:
						dtp.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
						break;

					default:
						throw new Exception("Not a valid format conversion.");
				}				
#endif
			}
		}

		#region Calendar Appearance Properties
		/*
				struct LOGFONT
				{
					public int height;
					public int width;
					public int escapement;
					public int orientation;
					public int weight;
					public byte italic;
					public byte underline;
					public byte strikeout;
					public byte charset;
					public byte outprecision;
					public byte clipprecision;
					public byte quality;
					public byte pitchandfamily;
					public string facename;
				}

				[DllImport("coredll.dll")]
				static extern int CreateFontIndirect(ref LOGFONT lf);

				int GetHFONTFromFont(Font f)
				{
					LOGFONT lf = new LOGFONT();
			
				}


				private Font calendarFont = new Font(FontFamily.GenericSansSerif, 8.25F, FontStyle.Regular);
#if DESIGN
				[
				System.ComponentModel.Category("Appearance"),
				System.ComponentModel.Description("The font used to display the calendar.")
				]
#endif
				public Font CalendarFont
				{
					get
					{
						return calendarFont;
					}
					set
					{
						if(calendarFont == value)
							return;

						calendarFont = value;
#if !DESIGN
						if(dtpWnd == IntPtr.Zero)
							return;	

	
					//	Win32Window.SendMessage(dtpWnd, (uint)DTPMessage.DTM_SETMCFONT, 
#endif
					}
				}
		

		

		private Color calendarForeColor = SystemColors.ControlText;
#if DESIGN
		[
		System.ComponentModel.Category("Appearance"),
		System.ComponentModel.Description("The color used to display text within a month.")
		]
#endif
		/// <summary>
		/// The color used to display text within a month.
		/// </summary>
		public Color CalendarForeColor
		{
			get
			{
				return calendarForeColor;
			}
			set
			{
				calendarForeColor = value;
#if !DESIGN
				Win32Window.SendMessage(dtpWnd, (int)DTM.SETMCCOLOR, 
					(int)MCSC.TEXT, 
					ColorTranslator.ToWin32(value));				
#endif
			}
		}

		private Color calendarMonthBackground = SystemColors.Window;
#if DESIGN
		[
		System.ComponentModel.Category("Appearance"),
		System.ComponentModel.Description("The background color displayed within the month.")
		]
#endif
		/// <summary>
		/// The background color displayed within the month.
		/// This is not supported in Windows CE?
		/// </summary>
		public Color CalendarMonthBackground
		{
			get
			{
				return calendarMonthBackground;
			}
			set
			{
				calendarMonthBackground = value;
#if !DESIGN
				Win32Window.SendMessage(dtpWnd, (int)DTM.SETMCCOLOR, 
					(int)MCSC.BACKGROUND, 
					ColorTranslator.ToWin32(value));				
#endif
			}
		}

		private Color calendarTitleBackColor = SystemColors.ActiveCaption;
#if DESIGN
		[
		System.ComponentModel.Category("Appearance"),
		System.ComponentModel.Description("The background color displayed in the calendar's title.")
		]
#endif
		/// <summary>
		/// The background color displayed in the calendar's title.
		/// </summary>
		public Color CalendarTitleBackColor
		{
			get
			{
				return calendarTitleBackColor;
			}
			set
			{
				calendarTitleBackColor = value;
#if !DESIGN
				Win32Window.SendMessage(dtpWnd, (int)DTM.SETMCCOLOR, 
					(int)MCSC.TITLEBK, 
					ColorTranslator.ToWin32(value));				
#endif
			}
		}

		private Color calendarTitleForeColor = SystemColors.ActiveCaptionText;
#if DESIGN
		[
		System.ComponentModel.Category("Appearance"),
		System.ComponentModel.Description("The color used to display text within the calendar's title.")
		]
#endif
		/// <summary>
		/// The color used to display text within the calendar's title.
		/// </summary>
		public Color CalendarTitleForeColor
		{
			get
			{
				return calendarTitleForeColor;
			}
			set
			{
				calendarTitleForeColor = value;
#if !DESIGN
				Win32Window.SendMessage(dtpWnd,(int)DTM.SETMCCOLOR, 
					(int)MCSC.TITLETEXT, 
					ColorTranslator.ToWin32(value));				
#endif
			}
		}

		private Color calendarTrailingForeColor = SystemColors.GrayText;
#if DESIGN
		[
		System.ComponentModel.Category("Appearance"),
		System.ComponentModel.Description("The color used to display the header and trailing day text.  " + 
			"Header and trailing days are the days from the previous and following months that appear " +
			"on the current month calendar.")
		]
#endif
		/// <summary>
		/// The color used to display the header and trailing day text.
		/// Header and trailing days are the days from the previous and following months that appear on the current month calendar.
		/// </summary>
		public Color CalendarTrailingForeColor
		{
			get
			{
				return calendarTrailingForeColor;
			}
			set
			{
				calendarTrailingForeColor = value;
#if !DESIGN
				Win32Window.SendMessage(dtpWnd, (int)DTM.SETMCCOLOR, 
					(int)MCSC.TRAILINGTEXT, 
					ColorTranslator.ToWin32(value));				
#endif
			}
		}
		#endregion

		#endregion

		/// <summary>
		/// Initializes a new instance of the System.Windows.Forms.DateTimePicker class.  
		/// </summary>
		public OldDateTimePicker()// : base()
		{           
#if !DESIGN
			// initialize the common controls
			InitCommonControlsEx(new byte[]{8,0,0,0,0,1,0,0});						

			// get the handle to the control window
			this.Capture = true;
			parentWnd = Win32Window.GetCapture();
			this.Capture = false;	
			            		
			// create our DateTimePicker control and add it to the wrapper control
			dtpWnd = Win32Window.CreateWindowExW(0, "SysDateTimePick32", "", baseStyle, 0, 0,
				Size.Width, Size.Height, parentWnd, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

			// initialize the DateTimePicker's style
			SetDTPStyle();
#else
			dtp = new System.Windows.Forms.DateTimePicker();
			dtp.Visible = true;
			dtp.Location = new Point(0, 0);
			dtp.Width = 100;
			dtp.Height = 20;
			dtp.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.Controls.Add(dtp);
#endif
		}
	}*/

	#region DateTimePickerFormat Enumeration
	/// <summary>
	/// Specifies the date and time format the <see cref="T:OpenNETCF.Windows.Forms.DateTimePicker"/> control displays.
	/// </summary>
	public enum DateTimePickerFormat
	{
		/// <summary>
		/// The <see cref="T:OpenNETCF.Windows.Forms.DateTimePicker"/> control displays the date/time value in the time format set by the user's operating system. 
		/// </summary>
		Time = 4,
		/// <summary>
		/// The <see cref="T:OpenNETCF.Windows.Forms.DateTimePicker"/> control displays the date/time value in the short date format set by the user's operating system.
		/// </summary>
		Short = 2,
		/// <summary>
		/// The <see cref="T:OpenNETCF.Windows.Forms.DateTimePicker"/> control displays the date/time value in the long date format set by the user's operating system.
		/// </summary>
		Long = 1,
		/// <summary>
		/// The <see cref="T:OpenNETCF.Windows.Forms.DateTimePicker"/> control displays the date/time value in a custom format.
		/// </summary>
		Custom = 8, 
	}
	#endregion
}
