/*=======================================================================================

	OpenNETCF.Windows.Forms.TextBoxEx
	Copyright © 2003-2005, OpenNETCF.org

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

// Change log
//
// ----------------------------
// July 17, 2003 - Neil Cowburn
// ----------------------------
// - Base source submitted
//
// ---------------------------
// August 5, 2003 - Mark Stega
// ---------------------------
// - Added designer support
//
// ----------------------------
// August 12, 2003 - Mark Stega
// ----------------------------
// - Changed class name to TextBoxEx
// - Changed output to be OpenNETCF.Windows.Forms.TextBoxEx
//
// -------------------------------
// October 28, 2003 - Neil Cowburn
// -------------------------------
// - Changed preprocessor directives to use DESIGN
// - Added conditional preprocessor to RuntimeAssemblyAttribute
//
// -------------------------------
// February 03, 2004 - Peter Foot
// -------------------------------
// - Renamed HWnd property to Handle implementing IWin32Window interface

//

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;
#if !DESIGN
using OpenNETCF.Win32;
#endif

#if DESIGN && STANDALONE
[assembly: System.CF.Design.RuntimeAssemblyAttribute("OpenNETCF.Windows.Forms.TextBox, Version=1.0.5000.4, Culture=neutral, PublicKeyToken=null")]
#endif

namespace OpenNETCF.Windows.Forms
{
	/// <summary>
	/// Specifies the style of the <see cref="TextBoxEx"/>.
	/// </summary>
    public enum TextBoxStyle
    {
		/// <summary>
		/// Default.
		/// </summary>
        Default = 0,
		/// <summary>
		/// Text is all numeric.
		/// </summary>
        Numeric = 0x2000,
		/// <summary>
		/// Text is all upper-case.
		/// </summary>
        UpperCase = 0x8,
		/// <summary>
		/// Text is all lower-case.
		/// </summary>
        LowerCase = 0x10
    }

	/// <summary>
	/// Represents an enhanced TextBox.
	/// </summary>
	public class TextBoxEx : System.Windows.Forms.TextBox, IWin32Window
	{

#if DESIGN
        /// <summary>
        /// Constructor
        /// </summary>
        public TextBoxEx()
		{
            BorderStyle = BorderStyle.FixedSingle;
		}

		[Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new System.Drawing.Color ForeColor
		{
			get
			{
				return base.ForeColor;
			}
			set
			{
				base.ForeColor = value;
			}
		}

		[Browsable(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new System.Drawing.Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
			}
		}

		[DefaultValue(0x0)]
		public TextBoxStyle Style
        {
			get
			{
				return style;
			}
			set
			{
				style = value;
			}
		}
        #region ------------ Design time painting ------------

		/// <summary>
		/// Paints the background of the control.
		/// </summary>
		/// <param name="e">A PaintEventArgs that contains information about the control to paint.</param>
		protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs e)
		{
//			base.OnPaintBackground(e);
		}

        /// <summary>
        /// OnPaint override
        /// </summary>
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs pea)
        {
            Pen pb = new Pen(Color.Black);

            SolidBrush bw = new SolidBrush(Color.White);
            SolidBrush bb = new SolidBrush(Color.Black);
            SolidBrush bg = new SolidBrush(Color.Gray);

            if (this.ReadOnly)
                pea.Graphics.FillRectangle(bg,this.ClientRectangle);
            else
                pea.Graphics.FillRectangle(bw,this.ClientRectangle);

            pea.Graphics.DrawString(
                this.Text,
                this.Font,
                bb,
                2,
                2);

            pb.Dispose();

            bb.Dispose();
            bw.Dispose();
            bg.Dispose();

//            base.OnPaint(pea);
        }

        /// Raises the Resize event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected override void OnResize(System.EventArgs e)
		{
			this.Invalidate();

			base.OnResize(e);
		}
        #endregion
#endif

		private TextBoxStyle style;

		#region Tag
		private object mTag;

		/// <summary>
		/// Gets or sets the object that contains data about the control.
		/// <para><b>New in v1.3</b></para>
		/// </summary>
#if DESIGN
		[Bindable(true), DefaultValue((string) null), TypeConverter(typeof(StringConverter))]
#endif
		public object Tag
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

#if !DESIGN
       
        private int defaultStyle = 0;
        

        protected virtual event EventHandler OnStyleChanged;

		/// <summary>
		/// Create a new instance of TextBoxEx.
		/// </summary>
        public TextBoxEx()
        {
            defaultStyle = Win32Window.GetWindowLong(hwnd, (int)GWL.STYLE);
        }

		#region Handle
 
		private IntPtr hwnd = IntPtr.Zero;

		/// <summary>
		/// Gets the window handle that the control is bound to.
		/// </summary>
		/// <value>An IntPtr that contains the window handle (HWND) of the control.</value>
        public IntPtr Handle
        {
            get
			{
				if(hwnd == IntPtr.Zero)
				{
					hwnd = ControlEx.GetHandle(this);
				}

				return hwnd;
			}
        }
		#endregion

		#region Name
		/// <summary>
		/// Gets the name of the control.
		/// <para><b>New in v1.3</b></para>
		/// </summary>
		public string Name
		{
			get
			{
				return ControlEx.GetName(this);
			}
		}
		#endregion


		/// <summary>
		/// Sets the TextBoxStyle.
		/// </summary>
		/// <remarks>Similar to the CharacterCasing property in the full framework.</remarks>
        public TextBoxStyle Style
        {
            get { return style; }
            set 
            { 
                style = value;
                OnTextBoxStyleChanged(null);
            }
        }

		/// <summary>
		/// Raises the OnStyleChanged event.
		/// </summary>
		/// <param name="e"></param>
        private void OnTextBoxStyleChanged(EventArgs e)
        {
            switch(style)
            {					
                case TextBoxStyle.Default:
                    Win32Window.SetWindowLong(hwnd, (int)GWL.STYLE, defaultStyle);
                    break;
                case TextBoxStyle.Numeric:
                    Win32Window.SetWindowLong(hwnd, (int)GWL.STYLE, defaultStyle|(int)ES.NUMBER);
                    break;
                case TextBoxStyle.UpperCase:
                    Win32Window.SetWindowLong(hwnd, (int)GWL.STYLE, defaultStyle|(int)ES.UPPERCASE);
                    break;
                case TextBoxStyle.LowerCase:
                    Win32Window.SetWindowLong(hwnd, (int)GWL.STYLE, defaultStyle|(int)ES.LOWERCASE);
                    break;
                default:
                    Win32Window.SetWindowLong(hwnd, (int)GWL.STYLE, defaultStyle);
                    break;
            }

            if(OnStyleChanged != null)
                OnStyleChanged(this, e);
        }

		protected override void OnParentChanged(System.EventArgs e)
		{
			//reset handle
			//next attempt to retrieve it will capture again
			hwnd = IntPtr.Zero;
		}


		#region Clipboard Support
		/// <summary>
		/// Moves the current selection in the <see cref="TextBoxEx"/> to the Clipboard.
		/// <para><b>New in v1.1</b></para>
		/// </summary>
		public void Cut()
		{
			//send Cut message
			Win32Window.SendMessage(this.Handle, (int)WM.CUT, 0, 0);
		}

		/// <summary>
		/// Copies the current selection in the <see cref="TextBoxEx"/> to the Clipboard.
		/// <para><b>New in v1.1</b></para>
		/// </summary>
		public void Copy()
		{
			//send Copy message
			Win32Window.SendMessage(this.Handle, (int)WM.COPY, 0, 0);
		}

		/// <summary>
		/// Replaces the current selection in the <see cref="TextBoxEx"/> with the contents of the Clipboard.
		/// <para><b>New in v1.1</b></para>
		/// </summary>
		public void Paste()
		{
			//send Paste message
			Win32Window.SendMessage(this.Handle, (int)WM.PASTE, 0, 0);
		}

		/// <summary>
		/// Clears all text from the <see cref="TextBoxEx"/> control.
		/// <para><b>New in v1.1</b></para>
		/// </summary>
		public void Clear()
		{
			//send Clear message
			Win32Window.SendMessage(this.Handle, (int)WM.CLEAR, 0, 0);
		}

		/// <summary>
		/// Undoes the last edit operation in the <see cref="TextBoxEx"/>.
		/// <para><b>New in v1.1</b></para>
		/// </summary>
		public void Undo()
		{
			//send Undo message
			Win32Window.SendMessage(this.Handle, (int)WM.UNDO, 0, 0);
		}
		#endregion


#endif
	}
}
