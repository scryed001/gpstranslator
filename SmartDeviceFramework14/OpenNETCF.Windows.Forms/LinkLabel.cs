//==========================================================================================
//
//		OpenNETCF.Windows.Forms.LinkLabel
//		Copyright (c) 2003, OpenNETCF.org
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

// ================================================================================================
// Dependencies
//	- OpenNETCF.Windows.Forms.IWin32Window (IWin32Window.cs)
// - OpenNETCF.Win32.Win32Window (WindowHelper.cs)
// ================================================================================================

using System;
using System.Drawing;
using System.Windows.Forms;
#if DESIGN
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.ComponentModel.Design;
#endif

namespace OpenNETCF.Windows.Forms
{
	#region Enums ==================================================================================

	/// <summary>
	/// Specifies the behavior of a link in a <see cref="LinkLabel"/>.
	/// </summary>
	public enum LinkBehavior
	{
//		/// <summary>
//		/// The behavior of this setting depends on the options set using the Internet Options dialog box in Control Panel or Internet Explorer.
//		/// </summary>
//		SystemDefault = 0,

		/// <summary>
		/// The link always displays with underlined text.
		/// </summary>
		AlwaysUnderline = 1,

//		/// <summary>
//		/// The link displays underlined text only when the mouse is hovered over the link text.
//		/// </summary>
//		HoverUnderline = 2,

		/// <summary>
		/// The link text is never underlined. The link can still be distinguished from other text by use of the LinkColor property of the LinkLabel control.
		/// </summary>
		NeverUnderline = 3
	}

	#endregion =====================================================================================


	#region LinkLabelLinkClickedEventArgs ==========================================================

	/// <summary>
	/// Provides data for the OnLinkClicked event.
	/// </summary>
	public class LinkLabelLinkClickedEventArgs : EventArgs
	{
		private object linkData;

		/// <summary>
		/// Initializes a new instance of the LinkLabelLinkClickedEventArgs class, given the link data.
		/// </summary>
		/// <param name="linkData">The LinkData of the LinkLabel instance.</param>
		public LinkLabelLinkClickedEventArgs(object linkData)
		{
			this.linkData = linkData;
		}

		/// <summary>
		/// Gets the data associated with the link.
		/// </summary>
		public object LinkData
		{
			get
			{
				return linkData;
			}
		}
	}

	#endregion =====================================================================================


	#region LinkLabelLinkClickedEventHandler Delegate ==============================================

	/// <summary>
	/// Represents the method that will handle the LinkClicked event of a LinkLabel.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">A LinkLabelLinkClickedEventArgs that contains the event data.</param>
	public delegate void LinkLabelLinkClickedEventHandler(object sender, LinkLabelLinkClickedEventArgs e);

	#endregion ====================================================================================


	#region LinkLabel ==============================================================================

	/// <summary>
	/// Represents a Windows label control that can display hyperlinks.
	/// </summary>
#if DESIGN
	[DefaultProperty("Text")]
	[DefaultEvent("LinkClicked")]
	[DesignerAttribute(typeof(LinkLabelDesigner))]
#endif
	public class LinkLabel : System.Windows.Forms.Control, OpenNETCF.Windows.Forms.IWin32Window
	{
		#region Events ==============================================================================

		/// <summary>
		/// Occurs when a link is clicked within the control.
		/// </summary>
#if DESIGN
		[
			Category("Action"),
			Description("Occurs when the link is clicked.")
		]
#endif
		public event LinkLabelLinkClickedEventHandler LinkClicked;

		#endregion ==================================================================================
		
		#region Fields ==============================================================================

		private Color _linkColor;

		private bool autoSize = false;
		private Color backColor = Color.Empty;
		private Color activeLinkColor = Color.Red;
		private Font font = new Font("Tahoma", 9F, FontStyle.Regular | FontStyle.Underline);
		private Color linkColor = Color.Blue;
		private Color disabledLinkColor = Color.Gray;
		private Color visitedLinkColor = Color.Purple;
		private LinkBehavior linkBehavior = LinkBehavior.AlwaysUnderline;
		private object linkData = null;
		private bool linkVisited = false;

		#endregion ==================================================================================

		#region Properties ==========================================================================

		// Hide the ForeColor property in the designer
#if DESIGN
		[Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color ForeColor{ get{return base.ForeColor;} set{base.ForeColor = value;} }
#endif

		/// <summary>
		/// Gets or sets a value indicating whether the control is automatically resized to display its entire contents.
		/// </summary>
#if DESIGN
		[
			Category("Behavior"),
			Description("Enables automatic resizing based on font size. Note that this is only valid for label controls that don't wrap text."),
			DefaultValue(false)
		]
#endif
		public bool AutoSize
		{
			get
			{
				return autoSize;
			}
			set
			{
				if (autoSize != value)
				{
					SizeF size = CreateGraphics().MeasureString(this.Text, this.Font);
					this.Width = (int)size.Width;
					this.Height = (int)size.Height;
					autoSize = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the background color for the control.
		/// </summary>
#if DESIGN
		[
			Category("Appearance"),
			Description("The background color used to display text in the control.")
		]
#endif
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
		/// Gets or sets the font of the text displayed by the control.
		/// </summary>
#if DESIGN
		[
			Category("Appearance"),
			Description("The font used to display text in the control.")
		]
#endif
		public override System.Drawing.Font Font
		{
			get
			{
				return font;
			}
			set
			{
				font = value;
				InvalidateLinkFont();
			}
		}
#if DESIGN
		public bool ShouldSerializeFont()
		{
			return ((Font.Name != "Tahoma") || (Font.Size != 9F) || (Font.Style != (FontStyle.Regular | FontStyle.Underline)));
		}
		public override void ResetFont()
		{
			this.Font = new Font("Tahoma", 9F, FontStyle.Regular | FontStyle.Underline);
		}
#endif

		/// <summary>
		/// Gets or sets the color used to display an active link.
		/// </summary>
#if DESIGN
		[
			Category("Appearance"),
			Description("Determines the color of the hyperlink when the user is clicking on the link."),
			DefaultValue(typeof(Color), "Red")
		]
#endif
		public Color ActiveLinkColor
		{
			get
			{
				return activeLinkColor;
			}
			set
			{
				activeLinkColor = value;
			}
		}

		/// <summary>
		/// Gets or sets the color used when displaying a normal link.
		/// </summary>
#if DESIGN
		[
			Category("Appearance"),
			Description("Determines the color of the hyperlink in its default state."),
			DefaultValue(typeof(Color), "Blue")
		]
#endif
		public Color LinkColor
		{
			get
			{
				return linkColor;
			}
			set
			{
				linkColor = value;
				if ((this.Enabled == true) && (linkVisited == false))
				{
					this._linkColor = linkColor;
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets or sets the color used when displaying a disabled link.
		/// </summary>
#if DESIGN
		[
			Category("Appearance"),
			Description("Determines the color of the hyperlink when disabled."),
			DefaultValue(typeof(Color), "Gray")
		]
#endif
		public Color DisabledLinkColor
		{
			get
			{
				return disabledLinkColor;
			}
			set
			{
				disabledLinkColor = value;
				if (this.Enabled == false)
				{
					this._linkColor = disabledLinkColor;
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets or sets the color used when displaying a link that that has been previously visited.
		/// </summary>
#if DESIGN
		[
			Category("Appearance"),
			Description("Determines the color of the hyperlink when the LinkVisited property is set to true."),
			DefaultValue(typeof(Color), "Purple")
		]
#endif
		public Color VisitedLinkColor
		{
			get
			{
				return visitedLinkColor;
			}
			set
			{
				visitedLinkColor = value;
				if (linkVisited == true)
				{
					this._linkColor = visitedLinkColor;
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets or sets a value that represents the behavior of a link.
		/// </summary>
#if DESIGN
		[
			Category("Behavior"),
			Description("Determines the underline behavior of the hyperlink."),
			DefaultValue(LinkBehavior.AlwaysUnderline)
		]
#endif
		public LinkBehavior LinkBehavior
		{
			get
			{
				return linkBehavior;
			}
			set
			{
				linkBehavior = value;
				InvalidateLinkFont();
			}
		}

		/// <summary>
		/// Gets or sets the data associated with the link.
		/// </summary>
#if DESIGN
		[
			Category("Data"),
			Description("Allows data to be associated with the link."),
			DefaultValue(null),
			TypeConverter(typeof(LinkDataConverter))
		]
#endif
		public object LinkData
		{
			get
			{
				return linkData;
			}
			set
			{
				linkData = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether a link should be displayed as though it were visited.
		/// </summary>
		/// <remarks>
		/// A LinkLabel control does not automatically denote that a link is a visited link. 
		/// To display the link as a visited link, you can set the value of this property to true 
		/// in an event handler for the LinkClicked event of a LinkLabel.
		/// </remarks>
#if DESIGN
		[
			Category("Appearance"),
			Description("Determines if the hyperlink should be rendered as visited."),
			DefaultValue(false)
		]
#endif
		public bool LinkVisited
		{
			get
			{
				return linkVisited;
			}
			set
			{
				linkVisited = value;
				if (linkVisited == true)
				{
					this._linkColor = visitedLinkColor;
				}
				else
				{
					if (this.Enabled == true)
					{
						this._linkColor = linkColor;
					}
					else
					{
						this._linkColor = disabledLinkColor;
					}
				}
				this.Invalidate();
			}
		}

		/// <summary>
		/// Gets the window handle that the control is bound to.
		/// </summary>
		/// <value>An IntPtr that contains the window handle (HWND) of the control.</value>
		/// <remarks>The value of the Handle property is a Windows HWND.</remarks>
#if DESIGN
		[
			Browsable(false),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public new IntPtr Handle
#else
		public IntPtr Handle
#endif
		{
			get
			{
				IntPtr hWnd;
				this.Capture = true;
				hWnd = OpenNETCF.Win32.Win32Window.GetCapture();
				this.Capture = false;
				return hWnd;
			}
		}

		#endregion ==================================================================================

		#region Methods =============================================================================

		/// <summary>
		/// Initializes a new default instance of the LinkLabel class.
		/// </summary>
		public LinkLabel()
		{
#if DESIGN
			SetStyle(ControlStyles.ResizeRedraw, true);
#endif
			this._linkColor = linkColor;
		}

		private void InvalidateLinkFont()
		{
			FontStyle fontStyle = font.Style;
			switch (linkBehavior)
			{
				case LinkBehavior.AlwaysUnderline:
				{
					fontStyle |= FontStyle.Underline;
					break;
				}
				case LinkBehavior.NeverUnderline:
				{
					fontStyle &= ~FontStyle.Underline;
					break;
				}
			}
			font = new Font(font.Name, font.Size, fontStyle);
			Invalidate();
			ResizeControl();
		}

		private void ResizeControl()
		{
			if (autoSize == true)
			{
				SizeF size = CreateGraphics().MeasureString(this.Text, this.Font);
				this.Width = (int)size.Width;
				this.Height = (int)size.Height;
			}
		}

		/// <summary>
		/// Raises the EnabledChanged event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected override void OnEnabledChanged(System.EventArgs e)
		{
			if (this.Enabled == true)
			{
				if (linkVisited == false)
				{
					this._linkColor = linkColor;
				}
				else
				{
					this._linkColor = visitedLinkColor;
				}
			}
			else
			{
				this._linkColor = disabledLinkColor;
			}
			this.Invalidate();
			base.OnEnabledChanged(e);
		}

		/// <summary>
		/// Raises the TextChanged event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected override void OnTextChanged(System.EventArgs e)
		{
			ResizeControl();
			this.Invalidate();
			base.OnTextChanged(e);
		}

		/// <summary>
		/// Raises the Resize event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected override void OnResize(System.EventArgs e)
		{
			if (autoSize)
			{
				ResizeControl();
			}
			this.Invalidate();
			base.OnResize(e);
		}

		/// <summary>
		/// Paints the background of the control.
		/// </summary>
		/// <param name="e">A PaintEventArgs that contains information about the control to paint.</param>
#if DESIGN
		[
			Browsable(false),
			EditorBrowsable(EditorBrowsableState.Never)
		]
#endif
		protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs e)
		{
			// base.OnPaintBackground(e);
		}

		/// <summary>
		/// Raises the Paint event.
		/// </summary>
		/// <param name="e">A PaintEventArgs that contains the event data.</param>
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			SolidBrush backColorBrush = new SolidBrush(this.BackColor);
			SolidBrush textBrush = new SolidBrush(_linkColor);

			// Paint the background.
			e.Graphics.FillRectangle(backColorBrush, 0, 0, this.Width, this.Height);

			// Draw the label text.
			e.Graphics.DrawString(this.Text, this.Font, textBrush, this.ClientRectangle);

			// Cleanup.
			backColorBrush.Dispose();
			textBrush.Dispose();

			base.OnPaint(e);
		}

		/// <summary>
		/// Raises the MouseDown event.
		/// </summary>
		/// <param name="e">A MouseEventArgs that contains the event data.</param>
		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			this._linkColor = activeLinkColor;
			this.Invalidate();
			base.OnMouseDown(e);
		}

		/// <summary>
		/// Raises the MouseUp event.
		/// </summary>
		/// <param name="e">A MouseEventArgs that contains the event data.</param>
		protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
		{
			if (linkVisited == false)
			{
				this._linkColor = linkColor;
			}
			else
			{
				this._linkColor = visitedLinkColor;
			}
			this.Invalidate();
			base.OnMouseUp(e);
		}

		/// <summary>
		/// Raises the Click event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected override void OnClick(System.EventArgs e)
		{
			LinkLabelLinkClickedEventArgs le = new LinkLabelLinkClickedEventArgs(linkData);
			OnLinkClicked(le);
			base.OnClick(e);
		}

		/// <summary>
		/// Raises the LinkClicked event.
		/// </summary>
		/// <param name="e">A LinkLabelLinkClickedEventArgs that contains the event data.</param>
		protected virtual void OnLinkClicked(LinkLabelLinkClickedEventArgs e)
		{
			if (LinkClicked != null)
			{
				LinkClicked(this, e);
			}
		}

		#endregion ==================================================================================
	}

	#endregion =====================================================================================


	#region LinkLabelDesigner ======================================================================

#if DESIGN
	internal class LinkLabelDesigner : System.Windows.Forms.Design.ControlDesigner
	{
		public LinkLabelDesigner()
		{ /* Constructor */ }

		public override SelectionRules SelectionRules
		{
			get
			{
				SelectionRules selectionRules = base.SelectionRules;
				PropertyDescriptor propDescriptor = TypeDescriptor.GetProperties(this.Component)["AutoSize"];
				if (propDescriptor != null)
				{
					bool autoSize = (bool)propDescriptor.GetValue(this.Component);
					if (autoSize)
					{
						selectionRules = SelectionRules.Visible | SelectionRules.Moveable;
					}
					else
					{
						selectionRules = SelectionRules.Visible | SelectionRules.AllSizeable | SelectionRules.Moveable;
					}
				}
				return selectionRules;
			}
		}
	}
#endif

	#endregion =====================================================================================


	#region LinkDataConverter ======================================================================

#if DESIGN
	/// <summary>
	/// This class is used, via a TypeConverter attribute [TypeConverter(typeof(LinkDataConverter))], 
	/// to inform the design-time environment that the LinkData property can accept a string at 
	/// design-time through the property editor. Since the LinkData property expects an object, the 
	/// designer will disable access to this property unless we tell it what to do - this 
	/// TypeConverter is what tells the design-time environment what to do with the value.
	/// </summary>
	internal class LinkDataConverter : System.ComponentModel.TypeConverter
	{
		/// <summary>
		/// Returns whether this converter can convert an object of one type to the type of this converter.
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
		/// <param name="sourceType">A Type that represents the type you want to convert from.</param>
		/// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		/// <summary>
		/// Converts the given value to the type of this converter.
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
		/// <param name="culture">The CultureInfo to use as the current culture.</param>
		/// <param name="value">The Object to convert.</param>
		/// <returns>An Object that represents the converted value.</returns>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				return value;
			}
			return base.ConvertFrom(context, culture, value);
		}

		/// <summary>
		/// Converts the given value object to the specified type, using the specified context and culture information.
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
		/// <param name="culture">A CultureInfo object.</param>
		/// <param name="value">The Object to convert.</param>
		/// <param name="destinationType">The Type to convert the value parameter to.</param>
		/// <returns>An Object that represents the converted value.</returns>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				return value;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
#endif

	#endregion =====================================================================================
}