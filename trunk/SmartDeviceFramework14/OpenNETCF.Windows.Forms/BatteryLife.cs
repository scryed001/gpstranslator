//===========================================================================
//
//  	OpenNETCF.Windows.Forms.BatteryLife.dll
//		Copyright (C) 2003, Tim Wilson, OpenNETCF.org
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
#if DESIGN
using System.ComponentModel;
#endif


namespace OpenNETCF.Windows.Forms
{
	/*
	/// <summary>
	/// Specifies valid values used in representing the status of AC power to the device.
	/// </summary>
	public enum ACPowerStatusEnum : byte
	{
		/// <summary>
		/// Disconnected from AC power.
		/// </summary>
		Offline = 0,
		/// <summary>
		/// Connected to AC power.
		/// </summary>
		Online = 1,
		/// <summary>
		/// Unknown whether connected/disconnected to/from AC power.
		/// </summary>
		Unknown = 255,
	}

	/// <summary>
	/// Specifies valid values used in representing the status of the battery charge.
	/// </summary>
	[Flags]
	public enum BatteryChargeStatusEnum : byte
	{
		/// <summary>
		/// Battery charge is in the high range.
		/// </summary>
		High = 1,
		/// <summary>
		/// Battery charge is in the low range.
		/// </summary>
		Low = 2,
		/// <summary>
		/// Battery charge is in the critical range.
		/// </summary>
		Critical = 4,
		/// <summary>
		/// Battery is charging.
		/// </summary>
		Charging = 8,
		/// <summary>
		/// No system battery exists.
		/// </summary>
		NoSystemBattery = 128,
		/// <summary>
		/// Battery charge status is unknown.
		/// </summary>
		Unknown = 255
	}
	*/

	/// <summary>
	/// Creates a simple custom progress bar to display the percentage of battery 
	/// life left on a device.
	/// </summary>
	/// <remarks>This control will not work as expected on the emulator.</remarks>
	public class BatteryLife : System.Windows.Forms.Control
	{
		/*
		[StructLayout(LayoutKind.Sequential)]
		internal struct SYSTEM_POWER_STATUS_EX
		{
			public byte ACLineStatus;
			public byte BatteryFlag;
			public byte BatteryLifePercent;
			public byte Reserved1;
			public uint BatteryLifeTime;
			public uint BatteryFullLifeTime;
			public byte Reserved2;
			public byte BackupBatteryFlag;
			public byte BackupBatteryLifePercent;
			public byte Reserved3;
			public uint BackupBatteryLifeTime;
			public uint BackupBatteryFullLifeTime;
		}

		// This function returns TRUE if successful; otherwise, it returns FALSE.
		[DllImport("Coredll.dll")]
		internal static extern bool GetSystemPowerStatusEx(ref SYSTEM_POWER_STATUS_EX pstatus, bool fUpdate);

		*/

		private static bool onDesktop = !(System.Environment.OSVersion.Platform == PlatformID.WinCE);

		private PowerStatus system_power_status_ex;
		private Bitmap bitmap = null;
		private Graphics graphics = null;
		private Pen penBorderColor = null;
		private SolidBrush brushStatusBarColor = null;
		private SolidBrush brushPercentage = null;
		private bool disposed = false;
		private byte _borderSize = 1;

		/// <summary>
		/// Initializes a new instance of the BatteryLife class.
		/// </summary>
		public BatteryLife()
		{
			brushPercentage = new SolidBrush(Color.LightSlateGray);
			penBorderColor = new Pen(Color.Black);
			brushStatusBarColor = new SolidBrush(SystemColors.Highlight);
			if (!onDesktop)
			{
				system_power_status_ex = new PowerStatus();
			}
#if !DESIGN
			UpdateBatteryLife();
#else
//			this.SetStyle(ControlStyles.ResizeRedraw, true);
//			this.UpdateStyles();

			//system_power_status_ex.BatteryLifePercent = 75;
#endif
		}

		/// <summary>
		/// Destroys this BatteryLife object.
		/// </summary>
		~BatteryLife()
		{
			Dispose(false);
		}

		/// <summary>
		/// Releases all resources used by this BatteryLife object.
		/// </summary>
		public new void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases the unmanaged resources used by this BatteryLife object and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing"><b>true</b> to release both managed and unmanaged resources; <b>false</b> to release only unmanaged resources.</param>
		/// <remarks>This method is called by the public Dispose() method and the Finalize method. <b>Dispose()</b> invokes the protected <b>Dispose(Boolean)</b> method with the disposing parameter set to <b>true</b>. <b>Finalize</b> invokes <b>Dispose</b> with <i>disposing</i> set to <b>false</b>.</remarks>
		protected override void Dispose(bool disposing)
		{
			if(!this.disposed)
			{
				try
				{
					if(disposing)
					{
						// Dispose managed resources.
						bitmap.Dispose();
						graphics.Dispose();
						penBorderColor.Dispose();
						brushStatusBarColor.Dispose();
						brushPercentage.Dispose();
					}
#if !DESIGN
					// Dispose unmanaged resources.
#endif
					disposed = true;
				}
				finally
				{
					base.Dispose(disposing);
				}
			}
		}

		/// <summary>
		/// Paints the background of the control.
		/// </summary>
		/// <param name="e">A PaintEventArgs that contains information about the control to paint.</param>
		protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs e)
		{
			//			base.OnPaintBackground(e);
		}

		/// <summary>
		/// Raises the Paint event.
		/// </summary>
		/// <param name="e">A PaintEventArgs that contains the event data.</param>
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			//hard code value when shown on desktop
			int bat = 0;

			if (onDesktop)
			{
				bat = 75;
			}
			else
			{
				bat = system_power_status_ex.BatteryLifePercent;
			}

			string text = bat.ToString() + " %";

			SizeF size = e.Graphics.MeasureString(text, this.Font);

			// Paint the background.
			SolidBrush backColorBrush = new SolidBrush(this.BackColor);
			graphics.FillRectangle(backColorBrush, 0, 0, this.Width, this.Height);
			backColorBrush.Dispose();

			graphics.FillRectangle(brushStatusBarColor, this.ClientRectangle.X + (_borderSize + 1), this.ClientRectangle.Y + (_borderSize + 1), Convert.ToInt32((this.ClientRectangle.Width - ((_borderSize + 1) * 2)) * (Convert.ToSingle(bat) / 100)), (this.ClientRectangle.Height - (_borderSize + 1) * 2));
			graphics.DrawRectangle(penBorderColor, this.ClientRectangle.X, this.ClientRectangle.Y, this.ClientRectangle.Width - 1, this.ClientRectangle.Height - 1);
			graphics.DrawString(text, this.Font, brushPercentage, ((this.ClientRectangle.Width / 2) - (size.Width / 2)), ((this.ClientRectangle.Height / 2) - (size.Height / 2)));

			e.Graphics.DrawImage(bitmap, 0, 0);

			base.OnPaint(e);
		}

		/// <summary>
		/// Raises the Resize event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected override void OnResize(System.EventArgs e)
		{
			if (bitmap != null)
			{
				bitmap.Dispose();
			}
			if (graphics != null)
			{
				graphics.Dispose();
			}
			bitmap = new Bitmap(this.ClientRectangle.Width, this.ClientRectangle.Height);
			graphics = Graphics.FromImage(bitmap);

			this.Invalidate();

			base.OnResize(e);
		}

		/// <summary>
		/// Forces an update of the progress bar that represents the battery life percentage. 
		/// </summary>
		/// <returns><b>true</b> if successful, otherwise <b>false</b></returns>
		public bool UpdateBatteryLife()
		{
#if !DESIGN
			//bool success = GetSystemPowerStatusEx(ref system_power_status_ex, true);
			this.Invalidate();
			return true;
			//return success;
#else
			return false;
#endif
		}

		// Hide these properties
#if DESIGN
		[Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text { get{return base.Text;} set{base.Text = value;} }
#endif

		/// <summary>
		/// The color used to display the battery life percentage.
		/// </summary>
#if DESIGN
		[
			Category("Appearance"),
			Description("The color used to display the battery life percentage.")
		]
#endif
		public override Color ForeColor
		{
			get
			{
				if (brushPercentage != null)
				{
					return brushPercentage.Color;
				}
				else
				{
					return Color.Empty;
				}
			}
			set
			{
				if (brushPercentage != null)
				{
					brushPercentage.Color = value;
				}
				else
				{
					brushPercentage = new SolidBrush(value);
				}
				base.ForeColor = value;
				this.Invalidate();
			}
		}
#if DESIGN
		public bool ShouldSerializeForeColor()
		{
			return ForeColor != Color.LightSlateGray;
		} 
		public override void ResetForeColor()
		{
			ForeColor = Color.LightSlateGray;
		}
#endif

		/// <summary>
		/// The color used to display the 1 pixel border around the control.
		/// </summary>
#if DESIGN
		[
			Category("Appearance"),
			Description("The color used to display the 1 pixel border around the control.")
		]
#endif
		public Color BorderColor
		{
			get
			{
				if (penBorderColor != null)
				{
					return penBorderColor.Color;
				}
				else
				{
					return Color.Empty;
				}
			}
			set
			{
				if (penBorderColor != null)
				{
					penBorderColor.Color = value;
				}
				else
				{
					penBorderColor = new Pen(value);
				}
				this.Invalidate();
			}
		}
#if DESIGN
		public bool ShouldSerializeBorderColor()
		{
			return BorderColor != Color.Black;
		} 
		public void ResetBorderColor()
		{
			BorderColor = Color.Black;
		}
#endif

		/// <summary>
		/// The color used to display the status bar representing the percentage of battery life.
		/// </summary>
#if DESIGN
		[
			Category("Appearance"),
			Description("The color used to display the status bar representing the percentage of battery life.")
		]
#endif
		public Color StatusBarColor
		{
			get
			{
				if (brushStatusBarColor != null)
				{
					return brushStatusBarColor.Color;
				}
				else
				{
					return Color.Empty;
				}
			}
			set
			{
				if (brushStatusBarColor != null)
				{
					brushStatusBarColor.Color = value;
				}
				else
				{
					brushStatusBarColor = new SolidBrush(value);
				}
				this.Invalidate();
			}
		}
#if DESIGN
		public bool ShouldSerializeStatusBarColor()
		{
			return StatusBarColor != SystemColors.Highlight;
		} 
		public void ResetStatusBarColor()
		{
			StatusBarColor = SystemColors.Highlight;
		}
#endif

		/// <summary>
		/// Gets a value that represents the status of AC power to the device.
		/// </summary>
#if DESIGN
		[
			Browsable(false)
		]
#endif
		public PowerLineStatus ACPowerStatus
		{
			get
			{
				if (onDesktop)
				{
					return PowerLineStatus.Unknown;
				}

				return (PowerLineStatus)system_power_status_ex.PowerLineStatus;
			}
		}

		/// <summary>
		/// Gets a value that represents the status of the battery charge.
		/// </summary>
#if DESIGN
		[
			Browsable(false)
		]
#endif
		public BatteryChargeStatus BatteryChargeStatus
		{
			get
			{
				if (onDesktop)
				{
					return BatteryChargeStatus.Unknown;
				}

				return (BatteryChargeStatus)system_power_status_ex.BatteryChargeStatus;
			}
		}

		/// <summary>
		/// Gets a value that represents the percentage of battery life left in the device.
		/// </summary>
#if DESIGN
		[
			Browsable(false)
		]
#endif
		public byte BatteryLifePercent
		{
			get
			{
				if (onDesktop)
				{
					return 0xFF;
				}

				return system_power_status_ex.BatteryLifePercent;
			}
		}

		/// <summary>
		/// Gets a value that represents the number of seconds of battery life remaining, or -1 if remaining seconds are unknown.
		/// </summary>
#if DESIGN
		[
			Browsable(false)
		]
#endif
		public int BatteryLifeTime
		{
			get
			{
				if (onDesktop)
				{
					return -1;
				}
				
				return system_power_status_ex.BackupBatteryLifeRemaining;
			}
		}

		/// <summary>
		/// Gets a value that represents the number of seconds of battery life when at full charge, or -1 if full lifetime is unknown.
		/// </summary>
#if DESIGN
		[
			Browsable(false)
		]
#endif
		public int BatteryFullLifeTime
		{
			get
			{
				if (onDesktop)
				{
					return -1;
				}
				
				return system_power_status_ex.BatteryFullLifeTime;
			}
		}
	}
}