/*=======================================================================================

	OpenNETCF.Windows.Forms.PictureBoxEx
	Copyright © 2005, OpenNETCF.org

	This library is free software; you can redistribute it and/or modify it under 
	the terms of the OpenNETCF.org Shared Source License.

	This library is distributed in the hope that it will be useful, but 
	WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
	FITNESS FOR A PARTICULAR PURPOSE. See the OpenNETCF.org Shared Source License 
	for more details.

	You should have received a copy of the OpenNETCF.org Shared Source License 
	along with this library; if not, email licensing@opennetcf.org to request a copy.

	If you wish to contact the OpenNETCF Advisory Board to discuss licensing, please 
	email licensing@opennetcf.org.

	For general enquiries, email enquiries@opennetcf.org or visit our website at:
	http://www.opennetcf.org

=======================================================================================*/

// 
// Author: 	Sergey Bogdanov <sergey.bogdanov@gmail.com>
// Url:		http://www.sergeybogdanov.com
//
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.ComponentModel;
#if !DESIGN
using OpenNETCF.Win32;
#endif

namespace OpenNETCF.Windows.Forms
{
	/// <summary>
	/// Represents an enhanced PictureBox.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
#if DESIGN
	[DefaultEvent("Click")]
	public class PictureBoxEx : System.Windows.Forms.PictureBox
#else	
	public class PictureBoxEx : System.Windows.Forms.PictureBox, OpenNETCF.Windows.Forms.IWin32Window
#endif
	{


		public PictureBoxEx()
		{
			_transparentColor = Color.FromArgb(255, 0, 255);
		}

		#region Events
#if DESIGN

		[Browsable(true),
		Category("Action"),
		Description("Occurs when the control is clicked.")]
		public new event EventHandler Click;
	
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

		#region Name
		/// <summary>
		/// Gets the name of the control.
		/// </summary>
		public string Name
		{
			get
			{
				return ControlEx.GetName(this);
			}
		}
		#endregion

		#region Tag
		private object mTag;
		/// <summary>
		/// Gets or sets the object that contains data about the control.
		/// </summary>
#if DESIGN
		[Bindable(true),
		DefaultValue((string) null),
		TypeConverter(typeof(StringConverter))]
		public new object Tag
#else
		public object Tag
#endif
		{
			get
			{
				return mTag;
			}
			set
			{
				mTag = value;
			}
		}
		#endregion

		#region Transparent Color
		Color _transparentColor;
		/// <summary>
		/// Sets or gets transparent color for an Image. The default color is 255, 0, 255.
		/// </summary>
#if DESIGN
		[Category("Appearance"),
		Description("Specifies the transparent color.")]
#endif
		public Color TransparentColor
		{
			get
			{
				return _transparentColor;
			}
			set
			{
				_transparentColor = value;
			}
		}
		#endregion
	
		protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs e)
		{
			SolidBrush sb = new SolidBrush(Parent.BackColor);
			e.Graphics.FillRectangle(sb, this.ClientRectangle);
		}
		
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			if (Image != null)
			{
				ImageAttributes attr = new ImageAttributes();
				attr.SetColorKey(_transparentColor, _transparentColor);
				e.Graphics.DrawImage(this.Image, this.ClientRectangle, 0, 0, this.Image.Width, this.Image.Height, GraphicsUnit.Pixel, attr);
			}
		}

		protected override void OnResize(EventArgs e)
		{
		      base.OnResize(e);
		      base.Invalidate();
		}

#if !DESIGN	
		
		#region Handle
		private IntPtr _hwnd = IntPtr.Zero;

		/// <summary>
		/// Gets the window handle that the control is bound to.
		/// </summary>
		/// <value>An IntPtr that contains the window handle (HWND) of the control.</value>
		public IntPtr Handle
		{
			get { return _hwnd; }
		}
		#endregion

		protected override void OnParentChanged(EventArgs e)
		{
			base.OnParentChanged (e);
			_hwnd = ControlEx.GetHandle(this);
		}
#endif
	
	}
}
