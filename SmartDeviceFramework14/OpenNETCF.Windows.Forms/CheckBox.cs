/*=======================================================================================

	OpenNETCF.Windows.Forms.CheckBox
	Copyright © 2003, OpenNETCF.org

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
using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Reflection;
#if DESIGN
using System.ComponentModel;
#endif

#if DESIGN && STANDALONE
	[assembly: System.CF.Design.RuntimeAssemblyAttribute("OpenNETCF.Windows.Forms.CheckBoxEx, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
#endif
namespace OpenNETCF.Windows.Forms
{
	/// <summary>
	/// Represents a check box.  
	/// </summary>

#if DESIGN
	[DefaultProperty("Text")]
	[DefaultEvent("CheckStateChanged")]
	[Designer(typeof(System.Windows.Forms.Design.ControlDesigner))]
#endif
	public class CheckBoxEx : Control, IWin32Window
	{
		#region private members
		private Bitmap m_bmpOffscreen;
		//private string text;
		private bool bChecked;
		private Brush textBrush;
		private Rectangle hotClickArea;
		private Graphics gxOff;
		private Color backColor;
		private Color foreColor;
		private System.Windows.Forms.CheckState checkState;
		private Pen forePen;
		private Color textColor;
		private bool autoCheck;
		private bool raiseEvent;
		private System.Windows.Forms.BorderStyle borderStyle;
		#endregion

		#region public events declare
		/// <summary>
		/// Occurs when the value of the CheckBox.CheckState property changes.  
		/// </summary>
		public event System.EventHandler CheckStateChanged;

		public new event System.EventHandler GotFocus;
		public new event System.EventHandler LostFocus;

		#endregion

		public CheckBoxEx()
		{
			//text = "";
			bChecked = false;
			foreColor = SystemColors.ControlText;
			backColor = Color.Empty;
			textColor = SystemColors.ControlText;
			textBrush = new SolidBrush(textColor);
			forePen = new Pen(this.ForeColor);
			borderStyle = BorderStyle.None;
			checkState = CheckState.Unchecked;
			autoCheck = true;
			raiseEvent = false;
			hotClickArea = new Rectangle(2, 2, 14, 14);
			this.Font = new Font("Tahoma", 9F, FontStyle.Regular);
			this.Width = 70;
			this.Height = 20;
		}

		#region public properties
		/// <summary>
		/// Gets or sets the border style color of the control. 
		/// </summary>
#if DESIGN
		[Category("Appearance")]
#endif
		public System.Windows.Forms.BorderStyle BorderStyle
		{
			get
			{
				return borderStyle;
			}
			set
			{
				if (borderStyle != value)
				{
					borderStyle = value;	
					this.Invalidate();
				}
			}

		}
#if DESIGN
		public bool ShouldSerializeForeColor()
		{
			return (foreColor != Color.Empty);
		} 
		public override void ResetForeColor()
		{
			ForeColor = Color.Empty;
		}
#endif

		/// <summary>
		/// Gets or sets the foreground color of the control. 
		/// </summary>
		public override Color ForeColor
		{
			get
			{
				return foreColor;
			}
			set
			{
				if (foreColor != value)
				{
					foreColor = value;	
					//textBrush = new SolidBrush(foreColor);
					forePen = new Pen(foreColor);
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets or sets the color of the text of the control. 
		/// </summary>
#if DESIGN
		[Category("Appearance")]
#endif
		public Color TextColor
		{
			get
			{
				return textColor;
			}
			set
			{
				if (textColor != value)
				{
					textColor = value;	
					textBrush = new SolidBrush(textColor);
					this.Invalidate();
				}
			}
		}


		/// <summary>
		///  Gets or set a value indicating whether the Checked or CheckState values and the check box's appearance are automatically changed when the check box is clicked.  
		/// </summary>
#if DESIGN
		[Category("Behavior"),
		Description("Causes the check box to automatically change state when clicked")]
#endif
		public bool AutoCheck 
		{
			get
			{
				return autoCheck;
			}
			set
			{
				if (autoCheck != value)
				{
					autoCheck = value;	
				}
			}
		}

#if DESIGN
		public bool ShouldSerializeBackColor()
		{
			return (backColor != Color.Empty);
		} 
		public override void ResetBackColor()
		{
			BackColor = Color.Empty;
		}
#endif

		/// <summary>
		/// Gets or sets the background color for the control.  
		/// </summary>
		public override Color BackColor
		{
			get
			{
				if ((backColor == Color.Empty) && (this.Parent != null))
				{
					return this.Parent.BackColor;
				}
				return backColor;
			}
			set
			{
				backColor = value;
				this.Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets the state of the check box.  
		/// </summary>
#if DESIGN
		[Category("Appearance"),
		Description("Indicates the check state of the check box")]
#endif
		public System.Windows.Forms.CheckState CheckState
		{
			get
			{
				return checkState;
			}
			set
			{
				if (checkState != value)
				{
					checkState = value;	
					if (checkState == CheckState.Checked)
						bChecked = true;
					else if (checkState == CheckState.Unchecked)
						bChecked = false;
					else
						bChecked = false;

					this.Invalidate();
					OnCheckStateChanged(null);
				}
			}
		}

		/// <summary>
		/// Gets or set a value indicating whether the check box is in the checked state.  
		/// </summary>
#if DESIGN
		[Category("Appearance"),
		Description("Indicates whether the checkbox is checked or unchecked")]
#endif
		public bool Checked
		{
			get
			{
				return bChecked;
			}
			set
			{
				if (bChecked != value)
				{
					bChecked = value;	
					if (bChecked)
						checkState = CheckState.Checked;
					else
						checkState = CheckState.Unchecked;

					this.Invalidate();
				}
			}
		}

		//		/// <summary>
		//		/// Gets or sets the text associated with this control.  
		//		/// </summary>
		//		public new string Text
		//		{
		//			get
		//			{
		//				return text;
		//			}
		//			set
		//			{
		//				if (text != value)
		//				{
		//					text = value;	
		//					this.Invalidate();
		//				}
		//			}
		//		}

		#endregion

		#region events and overrides
		/// <summary>
		/// Raises the System.Windows.Forms.CheckBox.CheckStateChanged event.  
		/// </summary>
		/// <param name="e">An System.EventArgs that contains the event data.</param>
		protected virtual void OnCheckStateChanged ( System.EventArgs e )
		{
			//Raise event
			if (CheckStateChanged!=null)
				CheckStateChanged(this, e);
		}

		protected override void OnTextChanged(EventArgs e)
		{
			this.Invalidate();
			base.OnTextChanged (e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			

			Rectangle textRect = this.ClientRectangle;
			
			textRect.Inflate(-2, -2);
			textRect.X += 20;

			Rectangle checkRect = new Rectangle(2, 2, 14, 14);

			//Paint background.
			SolidBrush backColorBrush = new SolidBrush(this.BackColor);
			gxOff.FillRectangle(backColorBrush, 0, 0, this.Width, this.Height);
			backColorBrush.Dispose();

			gxOff.DrawRectangle(forePen, checkRect);

			Point pt = new Point(5, 6);
			//Draw checkbox image
			if (checkState == CheckState.Checked)
			{
				DrawCheck(gxOff, forePen, pt);
			}
			else if (checkState == CheckState.Indeterminate)
			{
				//checkRect.X++;
				//checkRect.Y++;

				checkRect.Inflate(-1, -1);

				gxOff.FillRectangle(new SolidBrush(Color.LightGray), checkRect);
				
			}

			//Draw string
			gxOff.DrawString(this.Text, this.Font, textBrush, textRect);
			
			if (borderStyle == BorderStyle.FixedSingle)
			{
				Rectangle rc = this.ClientRectangle;
				rc.Height--;
				rc.Width--;
				//Draw border
				gxOff.DrawRectangle(new Pen(Color.Black), rc);
			}

			//gxOff.ResetClip();
			//Blit on the control's Graphics
			e.Graphics.DrawImage(m_bmpOffscreen, 0, 0);
			
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			//do nothing
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			this.Focus();

			raiseEvent = false;
			if ((hotClickArea.Contains(e.X, e.Y)) && (autoCheck == true))
			{
				if (bChecked) 
					checkState = CheckState.Unchecked;
				else
					checkState = CheckState.Checked;

				bChecked = !bChecked;
				raiseEvent = true;
				this.Invalidate();
			}

		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (raiseEvent)
				OnCheckStateChanged(null);

			raiseEvent = false;
			base.OnMouseUp (e);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize (e);
			if ((this.Width > 0) && (this.Height > 0))
			{
				m_bmpOffscreen = new Bitmap(this.Width, this.Height);
				gxOff = Graphics.FromImage(m_bmpOffscreen);
			}
			
		}

		protected override void OnParentChanged(EventArgs e)
		{
			base.OnParentChanged (e);
			
			#if DESIGN
				if (this.Parent!=null)
				this.BackColor = Parent.BackColor;
			#endif
		}

		protected override void OnGotFocus(EventArgs e)
		{
			if (this.GotFocus!=null)
				this.GotFocus(this, null);
		}

		protected override void OnLostFocus(EventArgs e)
		{
			if (this.LostFocus!=null)
				this.LostFocus(this, null);
		}


		#endregion

		private void DrawCheck(Graphics g, Pen pen, Point pt) 
		{
			g.DrawLine(pen, pt.X, pt.Y + 2, pt.X + 3, pt.Y + 5);
			g.DrawLine(pen, pt.X, pt.Y + 3, pt.X + 3, pt.Y + 6);
			g.DrawLine(pen, pt.X, pt.Y + 4, pt.X + 3, pt.Y + 7);
			g.DrawLine(pen, pt.X + 4, pt.Y + 4, pt.X + 8, pt.Y);
			g.DrawLine(pen, pt.X + 4, pt.Y + 5, pt.X + 8, pt.Y + 1);
			g.DrawLine(pen, pt.X + 4, pt.Y + 6, pt.X + 8, pt.Y + 2);
		}

		#region IWin32Window Members

		private IntPtr mHwnd = IntPtr.Zero;

		/// <summary>
		/// Gets the window handle that the control is bound to.
		/// <para><b>New in v1.3</b></para>
		/// </summary>
		public IntPtr Handle
		{
			get
			{
				if(mHwnd==IntPtr.Zero)
				{
					mHwnd = ControlEx.GetHandle(this);
				}
				return mHwnd;
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

		#region Tag
		private object mTag;

		/// <summary>
		/// Gets or sets the object that contains data about the control.
		/// <para><b>New in v1.3</b></para>
		/// </summary>
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
	}
}
