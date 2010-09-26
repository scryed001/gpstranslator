//=================================================================================================
//
//		OpenNETCF.Windows.Forms.GroupBox
//		Copyright (C) 2005, OpenNETCF.org
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
//=================================================================================================

using System;
using System.Drawing;
using System.Windows.Forms;

#region Designer ==================================================================================

#if DESIGN
using System.ComponentModel;
using System.Windows.Forms.Design;
#endif

#if DESIGN && STANDALONE
[assembly: System.CF.Design.RuntimeAssemblyAttribute("OpenNETCF.Windows.Forms.GroupBox, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
#endif

#endregion ========================================================================================

namespace OpenNETCF.Windows.Forms
{
	/// <summary>
	/// Represents a Windows group box.
	/// </summary>
	/// <remarks>
	/// The <b>GroupBox</b> displays a frame around a group of controls with or without a caption. 
	/// Use a <b>GroupBox</b> to logically group a collection of controls on a form. The group box is a 
	/// container control that can be used to define groups of controls.
	/// </remarks>
	/// <seealso cref="T:System.Windows.Forms.GroupBox">System.Windows.Forms.GroupBox Control</seealso>
	#region Designer ==================================================================
#if DESIGN
	[DesignerAttribute(typeof(ParentControlDesigner))]
#endif
	#endregion ========================================================================
	public class GroupBox : System.Windows.Forms.Panel, OpenNETCF.Windows.Forms.IWin32Window
	{
		#region Fields ==============================================================================
		
		#region Defaults ============================================================================

		// Note: Changing the default values may have an impact on the behavior of this class. Before 
		// changing any default value(s), ensure that the intended behavior will remain intact, and 
		// after changing any default value(s), ensure that all comments accurately reflect the new 
		// value(s).

		private const BorderStyle DefaultBorderStyle = BorderStyle.FixedSingle;
		private static readonly Color DefaultAmbienceColor = Color.Empty;
		private static readonly Font DefaultAmbienceFont = null;
		private static readonly Font DefaultFontValue = new Font("Tahoma", 8.25F, FontStyle.Bold);
		private const object DefaultTag = null;

		#endregion ==================================================================================

		private const int Offset = 8;
		private const int Space = 4;

		// Public Property Correspondents
		private Color backColor = DefaultAmbienceColor;
		private BorderStyle borderStyle = DefaultBorderStyle;
		private Font font = DefaultAmbienceFont;
		private Color foreColor = DefaultAmbienceColor;
		private IntPtr handle = IntPtr.Zero;
		private object tag = DefaultTag;

		#endregion ==================================================================================

		#region Properties ==========================================================================

		/// <summary>
		/// Gets or sets the background color for the control.
		/// </summary>
		/// <value>A <see cref="T:System.Drawing.Color"/> that represents the background color of the control.</value>
		/// <remarks>
		/// This is an "ambient" property. In other words, if this property value is not explicitly set then 
		/// the BackColor property value of the parent will be returned. If this property value is not explicitly 
		/// set and the control does not have an associated parent, then the DefaultBackColor is returned.
		/// </remarks>
		#region Designer ============================================================================
#if DESIGN
		[
			BrowsableAttribute(true),
			DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible),
			EditorBrowsableAttribute(EditorBrowsableState.Always),
			DesignOnlyAttribute(false),
			CategoryAttribute("Appearance"),
			DescriptionAttribute("The background color of the control."),
			AmbientValueAttribute(typeof(Color), "Empty")
		]
#endif
		#endregion ==================================================================================
		public override Color BackColor
		{
			get
			{
				if (backColor == DefaultAmbienceColor)
				{
					if (this.Parent != null)
					{
						return this.Parent.BackColor;
					}
					else
					{
						return DefaultBackColor;
					}
				}
				return backColor;
			}
			set
			{
				if (backColor != value)
				{
					// OnBackColorChanging(...)
					backColor = value;
					this.Invalidate();
					// OnBackColorChanged(...)
				}
			}
		}
		#region Designer ============================================================================
#if DESIGN
		/// <summary>
		/// Indicates whether the BackColor property should be persisted. This method is intended to be used by a design environment and should only be called directly by an inheriting class from inside the override (Overrides in Visual Basic).
		/// </summary>
		/// <returns>True if the property value has changed from its default; otherwise, False.</returns>
		protected virtual bool ShouldSerializeBackColor()
		{
			return (backColor != DefaultAmbienceColor);
		}
#endif
		#endregion ==================================================================================

		/// <summary>
		/// Indicates the border style for the control.
		/// </summary>
		/// <value>One of the <see cref="T:System.Windows.Forms.BorderStyle"/> values. The default is BorderStyle.FixedSingle.</value>
		/// <remarks>
		/// On Windows CE devices where the intrinsic controls are rendered with a 3D look, the BorderStyle 
		/// should be set to BorderStyle.Fixed3D. For Pocket PC, and similar operating systems where the user 
		/// interface has a "flat" rendering, the BorderStyle should be set to BorderStyle.FixedSingle.
		/// </remarks>
#if DESIGN
		#region Designer ============================================================================
		[
			BrowsableAttribute(true),
			DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible),
			EditorBrowsableAttribute(EditorBrowsableState.Always),
			DesignOnlyAttribute(false),
			CategoryAttribute("Appearance"),
			DescriptionAttribute("Gets or sets the border style for the control."),
			DefaultValueAttribute(DefaultBorderStyle)
		]
		public new BorderStyle BorderStyle
		#endregion ==================================================================================
#else
		public BorderStyle BorderStyle
#endif
		{
			get
			{
				return borderStyle;
			}
			set
			{
				if (borderStyle != value)
				{
					// OnBorderStyleChanging(...)
					borderStyle = value;
					this.Invalidate();
					// OnBorderStyleChanged(...)
				}
			}
		}

		/// <summary>
		/// Gets the default background color of the control.
		/// </summary>
		/// <value>The default background <see cref="T:System.Drawing.Color"/> of the control. The default is SystemColors.Control.</value>
#if DESIGN
		#region Designer ============================================================================
		public static new Color DefaultBackColor
		#endregion ==================================================================================
#else
		public static Color DefaultBackColor
#endif
		{
			get
			{
				return SystemColors.Control;
			}
		}

		/// <summary>
		/// Gets the default font of the control.
		/// </summary>
		/// <value>The default <see cref="T:System.Drawing.Font"/> of the control. The default is set to Tahoma (8 point, bold) in accordance with the comparative text sub-header recommendation in the pocket pc user interface guidelines.</value>
#if DESIGN
		#region Designer ============================================================================
		public static new Font DefaultFont
		#endregion ==================================================================================
#else
		public static Font DefaultFont
#endif
		{
			get
			{
				return DefaultFontValue;
			}
		}

		/// <summary>
		/// Gets the default foreground color of the control.
		/// </summary>
		/// <value>The default foreground <see cref="T:System.Drawing.Color"/> of the control. The default is SystemColors.ControlText.</value>
#if DESIGN
		#region Designer ============================================================================
		public static new Color DefaultForeColor
		#endregion ==================================================================================
#else
		public static Color DefaultForeColor
#endif
		{
			get
			{
				return SystemColors.ControlText;
			}
		}

		/// <summary>
		/// Gets or sets the font of the text displayed by the control.
		/// </summary>
		/// <value>The <see cref="T:System.Drawing.Font"/> object to apply to the text displayed by the control.</value>
		#region Designer ============================================================================
#if DESIGN
		[
			BrowsableAttribute(true),
			DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible),
			EditorBrowsableAttribute(EditorBrowsableState.Always),
			DesignOnlyAttribute(false),
			CategoryAttribute("Appearance"),
			DescriptionAttribute("The font used to display text in the control."),
			AmbientValueAttribute(null)
		]
#endif
		#endregion ==================================================================================
		public override Font Font
		{
			get
			{
				if (font == DefaultAmbienceFont)
				{
					return DefaultFont;
				}
				return font;
			}
			set
			{
				if (font != value)
				{
					// OnFontChanging(...)
					font = value;
					this.Invalidate();
					// OnFontChanged(...)
				}
			}
		}
		#region Designer ============================================================================
#if DESIGN
		/// <summary>
		/// Indicates whether the Font property should be persisted. This method is intended to be used by a design environment and should only be called directly by an inheriting class from inside the override (Overrides in Visual Basic).
		/// </summary>
		/// <returns>True if the property value has changed from its default; otherwise, False.</returns>
		protected virtual bool ShouldSerializeFont()
		{
			return (!Font.Equals(DefaultFontValue));
		}
#endif
		#endregion ==================================================================================

		/// <summary>
		/// Gets or sets the foreground color of the control.
		/// </summary>
		/// <value>A <see cref="T:System.Drawing.Color"/> that represents the foreground color of the control.</value>
		/// <remarks>
		/// This is an "ambient" property. In other words, if this property value is not explicitly set then 
		/// the ForeColor property value of the parent will be returned. If this property value is not explicitly 
		/// set and the control does not have an associated parent, then the DefaultForeColor is returned.
		/// </remarks>
		#region Designer ============================================================================
#if DESIGN
		[
			BrowsableAttribute(true),
			DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible),
			EditorBrowsableAttribute(EditorBrowsableState.Always),
			DesignOnlyAttribute(false),
			CategoryAttribute("Appearance"),
			DescriptionAttribute("The foreground color of the control."),
			AmbientValueAttribute(typeof(Color), "Empty")
		]
#endif
		#endregion ==================================================================================
		public override Color ForeColor
		{
			get
			{
				if (foreColor == DefaultAmbienceColor)
				{
					if (this.Parent != null)
					{
						return this.Parent.ForeColor;
					}
					else
					{
						return DefaultForeColor;
					}
				}
				return foreColor;
			}
			set
			{
				if (foreColor != value)
				{
					// OnForeColorChanging(...)
					foreColor = value;
					this.Invalidate();
					// OnForeColorChanged(...)
				}
			}
		}
		#region Designer ============================================================================
#if DESIGN
		/// <summary>
		/// Indicates whether the ForeColor property should be persisted. This method is intended to be used by a design environment and should only be called directly by an inheriting class from inside the override (Overrides in Visual Basic).
		/// </summary>
		/// <returns>True if the property value has changed from its default; otherwise, False.</returns>
		protected virtual bool ShouldSerializeForeColor()
		{
			return (foreColor != DefaultAmbienceColor);
		}
#endif
		#endregion ==================================================================================

#if !DESIGN
		/// <summary>
		/// Gets the window handle that the control is bound to.
		/// </summary>
		/// <value>An <see cref="IntPtr"/> that contains the window handle (HWND) of the control.</value>
		public virtual IntPtr Handle
		{
			get
			{
				return handle;
			}
		}

		/// <summary>
		/// Gets the name of the control.
		/// </summary>
		/// <value>The name of the control. The default is an empty string ("").</value>
		public string Name
		{
			get
			{
				return ControlEx.GetName(this);
			}
		}
#endif

		/// <summary>
		/// Gets or sets the object that contains data about the control.
		/// </summary>
		/// <value>A <see cref="T:System.Object"/> that contains data about the control. The default is a null reference (Nothing in Visual Basic).</value>
		/// <remarks>
		/// Any type derived from the <see cref="T:System.Object"/> class can be assigned to this property. If the 
		/// Tag property is set through the Windows Forms designer, only text can be assigned.
		/// </remarks>
#if DESIGN
		#region Designer ============================================================================
		[
			BrowsableAttribute(true),
			DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible),
			EditorBrowsableAttribute(EditorBrowsableState.Always),
			DesignOnlyAttribute(false),
			BindableAttribute(true),
			TypeConverterAttribute(typeof(StringConverter)),
			CategoryAttribute("Data"),
			DescriptionAttribute("User defined data associated with the control.")
		]
		public new object Tag
		#endregion ==================================================================================
#else
		public object Tag
#endif
		{
			get
			{
				return tag;
			}
			set
			{
				if (tag != value)
				{
					// OnTagChanging(...)
					tag = value;
					// OnTagChanged(...)
				}
			}
		}
		#region Designer ============================================================================
#if DESIGN
		/// <summary>
		/// Indicates whether the Tag property should be persisted. This method is intended to be used by a design environment and should only be called directly by an inheriting class from inside the override (Overrides in Visual Basic).
		/// </summary>
		/// <returns>True if the property value has changed from its default; otherwise, False.</returns>
		protected virtual bool ShouldSerializeTag()
		{
			return (Tag != DefaultTag);
		}
		/// <summary>
		/// Resets the Tag property to its default value. This method is intended to be used by a design environment and should only be called directly by an inheriting class from inside the override (Overrides in Visual Basic).
		/// </summary>
		protected virtual void ResetTag()
		{
			Tag = DefaultTag;
		}
#endif
		#endregion ==================================================================================

		#region Designer ============================================================================
#if DESIGN
		/// <summary>
		/// Gets or sets the text associated with this control.
		/// </summary>
		/// <value>The text associated with this control.</value>
		[
			BrowsableAttribute(true),
			DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible),
			EditorBrowsableAttribute(EditorBrowsableState.Always),
			DesignOnlyAttribute(false),
			BindableAttribute(true)
		]
		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
			}
		}
#endif
		#endregion ==================================================================================

		#endregion ==================================================================================

		#region Methods =============================================================================

		/// <summary>
		/// Initializes a new instance of the GroupBox class.
		/// </summary>
		public GroupBox()
		{
			#region Designer =========================================================================
#if DESIGN
			this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
#endif
			#endregion ===============================================================================
		}

		/// <summary>
		/// Raises the EnabledChanged event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected override void OnEnabledChanged(System.EventArgs e)
		{
			this.Invalidate();
			base.OnEnabledChanged(e);
		}

		/// <summary>
		/// Raises the Paint event.
		/// </summary>
		/// <param name="e">A PaintEventArgs that contains the event data.</param>
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			bool textAvailable = false;
			string textValue = this.Text;
			Font fontValue = this.Font;
			SizeF textSize = SizeF.Empty;
			int topOffset = 0;
			int lineOffset = 0;
			Rectangle textRect = Rectangle.Empty;

			if ((textValue != null) && (fontValue != null))
			{
				if (textValue.Length > 0)
				{
					textAvailable = true;
					textSize = e.Graphics.MeasureString(textValue, fontValue);
					topOffset = Convert.ToInt32(textSize.Height / 2);
					textRect = new Rectangle((Offset + Space), 0, (this.Width - ((Offset + Space) * 2)), this.Height);
					lineOffset = Math.Min((textRect.X + textRect.Width), (Offset + Convert.ToInt32(textSize.Width) + (Space * 2)));
				}
			}

			#region Background =======================================================================

			SolidBrush backGroundBrush = new SolidBrush(this.BackColor);
			e.Graphics.FillRectangle(backGroundBrush, 0, 0, this.Width, this.Height);
			backGroundBrush.Dispose();

			#endregion ===============================================================================

			#region Text =============================================================================

			if (textAvailable)
			{
				SolidBrush textBrush;

				if (this.Enabled)
				{
					textBrush = new SolidBrush(this.ForeColor);
				}
				else
				{
					textBrush = new SolidBrush(SystemColors.GrayText);
				}

				e.Graphics.DrawString(textValue, fontValue, textBrush, textRect);

				textBrush.Dispose();
			}

			#endregion ===============================================================================

			#region Border ===========================================================================

			switch (this.BorderStyle)
			{
				case BorderStyle.None:
				{	
					// Do nothing.
					break;
				}
				case BorderStyle.FixedSingle:
				{
					Pen borderPen = new Pen(SystemColors.ControlDarkDark);

					// Top border.
					if (textAvailable)
					{
						e.Graphics.DrawLine(borderPen, 0, topOffset, Offset, topOffset);
						e.Graphics.DrawLine(borderPen, lineOffset, topOffset, (this.Width - 1), topOffset);
					}
					else
					{
						e.Graphics.DrawLine(borderPen, 0, 0, (this.Width - 1), 0);
					}

					// Right border.
					e.Graphics.DrawLine(borderPen, (this.Width - 1), topOffset, (this.Width - 1), (this.Height - 1));

					// Bottom border.
					e.Graphics.DrawLine(borderPen, 0, (this.Height - 1), (this.Width - 1), (this.Height - 1));

					// Left border.
					e.Graphics.DrawLine(borderPen, 0, topOffset, 0, (this.Height - 1));

					borderPen.Dispose();

					break;
				}
				case BorderStyle.Fixed3D:
				{
					Pen border3DDarkPen = new Pen(SystemColors.ControlDark);
					Pen border3DLightPen = new Pen(SystemColors.ControlLightLight);

					// Top border.
					if (textAvailable)
					{
						// Dark line.
						e.Graphics.DrawLine(border3DDarkPen, 0, topOffset, Offset, topOffset);
						e.Graphics.DrawLine(border3DDarkPen, lineOffset, topOffset, (this.Width - 2), topOffset);
						// Light line.
						e.Graphics.DrawLine(border3DLightPen, 1, (topOffset + 1), Offset, (topOffset + 1));
						e.Graphics.DrawLine(border3DLightPen, lineOffset, (topOffset + 1), (this.Width - 3), (topOffset + 1));
					}
					else
					{
						// Dark line.
						e.Graphics.DrawLine(border3DDarkPen, 0, 0, (this.Width - 2), 0);
						// Light line.
						e.Graphics.DrawLine(border3DLightPen, 1, 1, (this.Width - 3), 1);
					}

					// Right border.
					e.Graphics.DrawLine(border3DDarkPen, (this.Width - 2), topOffset, (this.Width - 2), (this.Height - 2));
					e.Graphics.DrawLine(border3DLightPen, (this.Width - 1), topOffset, (this.Width - 1), (this.Height - 1));

					// Bottom border.
					e.Graphics.DrawLine(border3DDarkPen, 0, (this.Height - 2), (this.Width - 2), (this.Height - 2));
					e.Graphics.DrawLine(border3DLightPen, 0, (this.Height - 1), (this.Width - 1), (this.Height - 1));

					// Left border.
					e.Graphics.DrawLine(border3DDarkPen, 0, topOffset, 0, (this.Height - 2));
					e.Graphics.DrawLine(border3DLightPen, 1, (topOffset + 1), 1, (this.Height - 3));

					border3DDarkPen.Dispose();
					border3DLightPen.Dispose();

					break;
				}
			}

			#endregion ===============================================================================

			base.OnPaint(e);
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
		/// Raises the ParentChanged event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected override void OnParentChanged(EventArgs e)
		{
#if !DESIGN
			this.handle = ControlEx.GetHandle(this);
#endif
			base.OnParentChanged(e);
		}

		/// <summary>
		/// Raises the Resize event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected override void OnResize(System.EventArgs e)
		{
			this.Invalidate();
			base.OnResize(e);
		}

		/// <summary>
		/// Raises the TextChanged event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected override void OnTextChanged(System.EventArgs e)
		{
			this.Invalidate();
			base.OnTextChanged(e);
		}

		#endregion ==================================================================================
	}
}