//=================================================================================================
//
//		OpenNETCF.Windows.Forms.ButtonBase
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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using OpenNETCF.Win32;

namespace OpenNETCF.Windows.Forms
{
	/// <summary>
	/// Implements the basic functionality common to button controls.
	/// </summary>
	public abstract class ButtonBase : System.Windows.Forms.Control, OpenNETCF.Windows.Forms.IWin32Window
	{
		#region Fields ==============================================================================

		#region Defaults ============================================================================

		// Note: Changing the default values may have an impact on the behavior of this class. Before 
		// changing any default value(s), ensure that the intended behavior will remain intact, and 
		// after changing any default value(s), ensure that all comments accurately reflect the new 
		// value(s).

		private static readonly Color DefaultAmbienceColor = Color.Empty;
		private static readonly Image DefaultBackgroundImage = null;
		private static readonly Color DefaultBackColorValue = SystemColors.Control;
		private static readonly Font DefaultAmbienceFont = null;
		private static readonly Font DefaultFontValue = new Font("Tahoma", 8.25F, FontStyle.Regular);
		private static readonly Color DefaultForeColorValue = SystemColors.ControlText;
		private static readonly object DefaultTag = null;
		private static readonly Size DefaultSizeValue = new Size(104, 20);
		private static readonly Image DefaultImage = null;
		private const OpenNETCF.Drawing.ContentAlignment DefaultImageAlign = OpenNETCF.Drawing.ContentAlignment.MiddleCenter;
		private const int DefaultImageIndex = -1;
		private static readonly ImageList DefaultImageList = null;
		private const OpenNETCF.Drawing.ContentAlignment DefaultTextAlign = OpenNETCF.Drawing.ContentAlignment.MiddleCenter;

		#endregion ==================================================================================

		private const byte VK_TAB = 0x09;
		private const byte VK_SHIFT = 0x10;

		private const int SequentialPaintingToken = 0;

		private Bitmap bufferBitmap = null;
		private bool internalChange = false;
		private int registeredSequentialPaintingToken = SequentialPaintingToken;

		// Public Property Correspondents
		private Color backColor = DefaultAmbienceColor;
		private Image backgroundImage = DefaultBackgroundImage;
		private Font font = DefaultAmbienceFont;
		private Color foreColor = DefaultAmbienceColor;
		private bool isDisposed = false;
		private object tag = DefaultTag;
		private Image image = DefaultImage;
		private OpenNETCF.Drawing.ContentAlignment imageAlign = DefaultImageAlign;
		private int imageIndex = DefaultImageIndex;
		private ImageList imageList = DefaultImageList;
		private OpenNETCF.Drawing.ContentAlignment textAlign = DefaultTextAlign;

		// Protected Property Correspondents
		private Graphics bufferGraphics = null; // Exposed through DoubleBuffer.
		private bool doubleBufferEnabled = false;
		private bool isDefault = false;

		#endregion ==================================================================================

		#region Properties ==========================================================================

		#region Extended Properties =================================================================

		/// <summary>
		/// Gets a value indicating whether the Font property actually contains a <see cref="T:System.Drawing.Font"/> object.
		/// </summary>
		/// <value>True if the Font property contains a <see cref="T:System.Drawing.Font"/> object; otherwise, False.</value>
		/// <remarks>
		/// <b>Note:</b> This property does not exist for the <see cref="T:System.Windows.Forms.ButtonBase"/> control in the Full Framework (1.0, 1.1) or Compact Framework (1.0).
		/// </remarks>
		protected virtual bool IsFontSet
		{
			get
			{
				return (this.font != DefaultAmbienceFont);
			}
		}

		#endregion ==================================================================================

		#region Control Properties ==================================================================

		#region Extended Properties =================================================================

		/// <summary>
		/// Gets the object used to represent the double buffer for the presentation of the control.
		/// </summary>
		/// <value>A <see cref="T:System.Drawing.Graphics"/> object used to double buffer the presentation of the control.</value>
		/// <remarks>
		/// The EnableDoubleBuffer property must be set to True for this property to return a valid object. If the EnableDoubleBuffer property is set to False, the default, then this property will return a null reference (Nothing in Visual Basic).
		/// <br /><br />
		/// <b>Note:</b> This property does not exist for the <see cref="T:System.Windows.Forms.ButtonBase"/> control in the Full Framework (1.0, 1.1) or Compact Framework (1.0).
		/// </remarks>
		protected Graphics DoubleBuffer
		{
			get
			{
				return this.bufferGraphics;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the presentation of the control is double buffered.
		/// </summary>
		/// <value>A <see cref="T:System.Boolean"/> that is set to True if the presentation of the control is double buffered; otherwise, False. The default is False.</value>
		/// <remarks>
		/// See the example section of the OnPaint method for the recommended pattern to ensure that double buffering is accommodated.
		/// <br /><br />
		/// <b>Note:</b> This property does not exist for the <see cref="T:System.Windows.Forms.ButtonBase"/> control in the Full Framework (1.0, 1.1) or Compact Framework (1.0).
		/// </remarks>
		protected bool DoubleBufferEnabled
		{
			get
			{
				return doubleBufferEnabled;
			}
			set
			{
				if (doubleBufferEnabled != value)
				{
					doubleBufferEnabled = value;
					if (doubleBufferEnabled)
					{
						UpdateDoubleBuffer();
					}
					else
					{
						DisposeDoubleBuffer();
					}
				}
			}
		}

		#endregion ==================================================================================

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
			DesignOnlyAttribute(false),
			CategoryAttribute("Appearance"),
			DescriptionAttribute("The background color of the control."),
			AmbientValueAttribute(typeof(Color), "Empty")
		]
#endif
		#endregion ==================================================================================
		#region Attributes ==========================================================================
		[
			EditorBrowsableAttribute(EditorBrowsableState.Always)
		]
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
		/// Gets or sets the background image displayed in the control.
		/// </summary>
		/// <value>A <see cref="T:System.Drawing.Image"/> that represents the image to display in the background of the control.</value>
		#region Attributes ==========================================================================
		[
			EditorBrowsableAttribute(EditorBrowsableState.Always)
		]
		#endregion ==================================================================================
#if DESIGN
		#region Designer ============================================================================
		[
			BrowsableAttribute(true),
			DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible),
			DesignOnlyAttribute(false),
			CategoryAttribute("Appearance"),
			DescriptionAttribute("The background image displayed in the control.")
		]
		public override Image BackgroundImage
		#endregion ==================================================================================
#else
		public virtual Image BackgroundImage
#endif
		{
			get
			{
				return backgroundImage;
			}
			set
			{
				if (backgroundImage != value)
				{
					// OnBackgroundImageChanging(...)
					backgroundImage = value;
					this.Invalidate();
					// OnBackgroundImageChanged(...)
				}
			}
		}
		#region Designer ============================================================================
#if DESIGN
		/// <summary>
		/// Indicates whether the BackgroundImage property should be persisted. This method is intended to be used by a design environment and should only be called directly by an inheriting class from inside the override (Overrides in Visual Basic).
		/// </summary>
		/// <returns>True if the property value has changed from its default; otherwise, False.</returns>
		protected virtual bool ShouldSerializeBackgroundImage()
		{
			return (BackgroundImage != DefaultBackgroundImage);
		}
		/// <summary>
		/// Resets the BackgroundImage property to its default value. This method is intended to be used by a design environment and should only be called directly by an inheriting class from inside the override (Overrides in Visual Basic).
		/// </summary>
		protected virtual void ResetBackgroundImage()
		{
			BackgroundImage = DefaultBackgroundImage;
		}
#endif
		#endregion ==================================================================================

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
				return DefaultBackColorValue;
			}
		}

		/// <summary>
		/// Gets the default font of the control.
		/// </summary>
		/// <value>The default <see cref="T:System.Drawing.Font"/> of the control. The default is set to Tahoma (8 point, regular) in accordance with the basic font recommendation in the Pocket PC user interface guidelines.</value>
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
				return DefaultForeColorValue;
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
			DesignOnlyAttribute(false),
			CategoryAttribute("Appearance"),
			DescriptionAttribute("The font used to display text in the control."),
			AmbientValueAttribute(null)
		]
#endif
		#endregion ==================================================================================
		#region Attributes ==========================================================================
		[
			EditorBrowsableAttribute(EditorBrowsableState.Always)
		]
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
			return (font != DefaultAmbienceFont);
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
			DesignOnlyAttribute(false),
			CategoryAttribute("Appearance"),
			DescriptionAttribute("The foreground color of the control."),
			AmbientValueAttribute(typeof(Color), "Empty")
		]
#endif
		#endregion ==================================================================================
		#region Attributes ==========================================================================
		[
			EditorBrowsableAttribute(EditorBrowsableState.Always)
		]
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
		/// <value>A <see cref="System.IntPtr"/> that contains the window handle (HWND) of the control.</value>
		public virtual IntPtr Handle
		{
			get
			{
				return ControlEx.GetHandle(this);
			}
		}

		/// <summary>
		/// Gets a value indicating whether the control has been disposed.
		/// </summary>
		/// <value>True if the control has been disposed; otherwise, False.</value>
		public bool IsDisposed
		{
			get
			{
				return isDisposed;
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
		#region Attributes ==========================================================================
		[
			EditorBrowsableAttribute(EditorBrowsableState.Always)
		]
		#endregion ==================================================================================
#if DESIGN
		#region Designer ============================================================================
		[
			BrowsableAttribute(true),
			DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible),
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

		/// <summary>
		/// Gets the default size of the control.
		/// </summary>
		/// <value>The default <see cref="T:System.Drawing.Size"/> of the control.</value>
#if DESIGN
		#region Designer ============================================================================
		protected override Size DefaultSize
		#endregion ==================================================================================
#else
		protected virtual Size DefaultSize
#endif
		{
			get
			{
				return DefaultSizeValue;
			}
		}

		#endregion ==================================================================================

		/// <summary>
		/// Gets or sets the image that is displayed on a button control.
		/// </summary>
		/// <value>The <see cref="T:System.Drawing.Image"/> displayed on the button control. The default value is a null reference (Nothing in Visual Basic).</value>
		#region Designer ============================================================================
#if DESIGN
		[
			BrowsableAttribute(true),
			DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible),
			DesignOnlyAttribute(false),
			RefreshPropertiesAttribute(RefreshProperties.All),
			CategoryAttribute("Appearance"),
			DescriptionAttribute("The image that is displayed on the control.")
		]
#endif
		#endregion ==================================================================================
		#region Attributes ==========================================================================
		[
			EditorBrowsableAttribute(EditorBrowsableState.Always)
		]
		#endregion ==================================================================================
		public Image Image
		{
			get
			{
				int index = this.ImageIndex;
				if (index > DefaultImageIndex)
				{
					return this.ImageList.Images[index];
				}
				return image;
			}
			set
			{
				if (image != value)
				{
					// OnImageChanging(...)
					image = value;
					if (!this.internalChange)
					{
						this.internalChange = true;
						this.ImageIndex = DefaultImageIndex;
						this.ImageList = DefaultImageList;
						this.internalChange = false;
						this.Invalidate();
					}
					// OnImageChanged(...)
				}
			}
		}
		#region Designer ============================================================================
#if DESIGN
		/// <summary>
		/// Indicates whether the Image property should be persisted. This method is intended to be used by a design environment and should only be called directly by an inheriting class from inside the override (Overrides in Visual Basic).
		/// </summary>
		/// <returns>True if the property value has changed from its default; otherwise, False.</returns>
		protected virtual bool ShouldSerializeImage()
		{
			return (image != DefaultImage);
		}
		/// <summary>
		/// Resets the Image property to its default value. This method is intended to be used by a design environment and should only be called directly by an inheriting class from inside the override (Overrides in Visual Basic).
		/// </summary>
		protected virtual void ResetImage()
		{
			Image = DefaultImage;
		}
#endif
		#endregion ==================================================================================

		/// <summary>
		/// Gets or sets the alignment of the image on the button control.
		/// </summary>
		/// <value>One of the <see cref="T:OpenNETCF.Drawing.ContentAlignment"/> values. The default value is MiddleCenter.</value>
		#region Designer ============================================================================
#if DESIGN
		[
			BrowsableAttribute(true),
			DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible),
			DesignOnlyAttribute(false),
			CategoryAttribute("Appearance"),
			DescriptionAttribute("The alignment of the image on the control."),
			DefaultValueAttribute(DefaultImageAlign)
		]
#endif
		#endregion ==================================================================================
		#region Attributes ==========================================================================
		[
			EditorBrowsableAttribute(EditorBrowsableState.Always)
		]
		#endregion ==================================================================================
		public OpenNETCF.Drawing.ContentAlignment ImageAlign
		{
			get
			{
				return imageAlign;
			}
			set
			{
				if (Enum.IsDefined(typeof(OpenNETCF.Drawing.ContentAlignment), value))
				{
					if (imageAlign != value)
					{
						// OnImageAlignChanging(...)
						imageAlign = value;
						this.Invalidate();
						// OnImageAlignChanged(...)
					}
				}
				else
				{
					throw new OpenNETCF.ComponentModel.InvalidEnumArgumentException("The ImageAlign property must be one of the OpenNETCF.Drawing.ContentAlignment values.");
				}
			}
		}

		/// <summary>
		/// Gets or sets the image list index value of the image displayed on the button control.
		/// </summary>
		/// <value>A zero-based index, which represents the image position in a <see cref="T:System.Windows.Forms.ImageList"/>. The default is -1.</value>
		#region Designer ============================================================================
#if DESIGN
		[
			BrowsableAttribute(true),
			DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible),
			DesignOnlyAttribute(false),
			RefreshPropertiesAttribute(RefreshProperties.All),
			CategoryAttribute("Appearance"),
			DescriptionAttribute("The image list index of the image displayed on the control."),
			DefaultValueAttribute(DefaultImageIndex)
		]
#endif
		#endregion ==================================================================================
		#region Attributes ==========================================================================
		[
			EditorBrowsableAttribute(EditorBrowsableState.Always)
		]
		#endregion ==================================================================================
		public int ImageIndex
		{
			get
			{
				if (this.ImageList != null)
				{
					int lastIndex = this.ImageList.Images.Count - 1;
					if (imageIndex > lastIndex)
					{
						return lastIndex;
					}
					return imageIndex;
				}
				return DefaultImageIndex;
			}
			set
			{
				if (value >= DefaultImageIndex)
				{
					if (imageIndex != value)
					{
						// OnImageIndexChanging(...)
						imageIndex = value;
						if (!this.internalChange)
						{
							this.internalChange = true;
							this.Image = DefaultImage;
							this.internalChange = false;
							this.Invalidate();
						}
						// OnImageIndexChanged(...)
					}
				}
				else
				{
					throw new ArgumentException(String.Format("The ImageIndex property must be greater than, or equal to, {0}.", DefaultImageIndex));
				}
			}
		}

		/// <summary>
		/// Gets or sets the image list that contains the image displayed on a button control.
		/// </summary>
		/// <value>A <see cref="T:System.Windows.Forms.ImageList"/>. The default value is a null reference (Nothing in Visual Basic).</value>
		#region Designer ============================================================================
#if DESIGN
		[
			BrowsableAttribute(true),
			DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible),
			DesignOnlyAttribute(false),
			RefreshPropertiesAttribute(RefreshProperties.All),
			CategoryAttribute("Appearance"),
			DescriptionAttribute("The image list containing the image that is displayed on the control.")
		]
#endif
		#endregion ==================================================================================
		#region Attributes ==========================================================================
		[
			EditorBrowsableAttribute(EditorBrowsableState.Always)
		]
		#endregion ==================================================================================
		public ImageList ImageList
		{
			get
			{
				return imageList;
			}
			set
			{
				if (imageList != value)
				{
					// OnImageListChanging(...)
					imageList = value;
					if (!this.internalChange)
					{
						this.internalChange = true;
						this.Image = DefaultImage;
						this.internalChange = false;
						this.Invalidate();
					}
					// OnImageListChanged(...)
				}
			}
		}
		#region Designer ============================================================================
#if DESIGN
		/// <summary>
		/// Indicates whether the ImageList property should be persisted. This method is intended to be used by a design environment and should only be called directly by an inheriting class from inside the override (Overrides in Visual Basic).
		/// </summary>
		/// <returns>True if the property value has changed from its default; otherwise, False.</returns>
		protected virtual bool ShouldSerializeImageList()
		{
			return (ImageList != DefaultImageList);
		}
		/// <summary>
		/// Resets the ImageList property to its default value. This method is intended to be used by a design environment and should only be called directly by an inheriting class from inside the override (Overrides in Visual Basic).
		/// </summary>
		protected virtual void ResetImageList()
		{
			ImageList = DefaultImageList;
		}
#endif
		#endregion ==================================================================================

		/// <summary>
		/// Gets or sets the alignment of the text on the button control.
		/// </summary>
		/// <value>One of the <see cref="T:OpenNETCF.Drawing.ContentAlignment"/> values. The default value is MiddleCenter.</value>
		#region Designer ============================================================================
#if DESIGN
		[
			BrowsableAttribute(true),
			DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible),
			DesignOnlyAttribute(false),
			CategoryAttribute("Appearance"),
			DescriptionAttribute("The alignment of the text on the control."),
			DefaultValueAttribute(DefaultTextAlign)
		]
#endif
		#endregion ==================================================================================
		#region Attributes ==========================================================================
		[
			EditorBrowsableAttribute(EditorBrowsableState.Always)
		]
		#endregion ==================================================================================
		public virtual OpenNETCF.Drawing.ContentAlignment TextAlign
		{
			get
			{
				return textAlign;
			}
			set
			{
				if (Enum.IsDefined(typeof(OpenNETCF.Drawing.ContentAlignment), value))
				{
					if (textAlign != value)
					{
						// OnTextAlignChanging(...)
						textAlign = value;
						this.Invalidate();
						// OnTextAlignChanged(...)
					}
				}
				else
				{
					throw new OpenNETCF.ComponentModel.InvalidEnumArgumentException("The TextAlign property must be one of the OpenNETCF.Drawing.ContentAlignment values.");
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the button control is the default button.
		/// </summary>
		/// <value>True if the button control is the default button; otherwise, False.</value>
		protected bool IsDefault
		{
			get
			{
				return isDefault;
			}
			set
			{
				if (isDefault != value)
				{
					isDefault = value;
					this.Invalidate();
				}
			}
		}

		#endregion ==================================================================================

		#region Methods =============================================================================

		#region Control Methods =====================================================================

		/// <summary>
		/// Releases the resources used by the control.
		/// </summary>
		/// <param name="disposing">True to release both managed and unmanaged resources; False to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				try
				{
					if (disposing)
					{
						// If applicable, dispose managed resources here.
						DisposeDoubleBuffer();
					}
					// If applicable, dispose unmanaged resources here.
					this.isDisposed = true;
				}
				finally
				{
					base.Dispose(disposing);
				}
			}
		}

		/// <summary>
		/// Draws the contents of the buffer to the control if the presentation was double buffered.
		/// </summary>
		/// <param name="controlGraphics">A <see cref="T:System.Drawing.Graphics"/> object representing the drawing surface of the control in which to output the contents of the buffer.</param>
		protected void DrawDoubleBuffer(Graphics controlGraphics)
		{
			if (this.DoubleBufferEnabled)
			{
				controlGraphics.DrawImage(this.bufferBitmap, 0, 0);
			}
			else
			{
				throw (new InvalidOperationException("A call to this method is only valid when the DoubleBufferEnabled property is set to true."));
			}
		}

		/// <summary>
		/// Gets a reference to the proper drawing medium used to update the presentation of the control.
		/// </summary>
		/// <param name="controlGraphics">A <see cref="T:System.Drawing.Graphics"/> object representing the drawing surface of the control.</param>
		/// <returns>A <see cref="T:System.Drawing.Graphics"/> object that should be used to update the presentation of the control.</returns>
		/// <remarks>
		/// The <see cref="T:System.Drawing.Graphics"/> object returned from this method will either be a reference to the buffer, if the DoubleBufferEnabled property is set to True, or the reference to the drawing surface of the control that was provided as the argument.
		/// </remarks>
		protected Graphics GetPresentationMedium(Graphics controlGraphics)
		{
			if (this.DoubleBufferEnabled)
			{
				return this.DoubleBuffer;
			}
			else
			{
				return controlGraphics;
			}
		}

		/// <summary>
		/// Determines if the specified sequential painting token was the last one registered.
		/// </summary>
		/// <param name="sequentialPaintingToken">A <see cref="T:System.Int32"/> that specifies the sequential painting token to compare with the last registered token.</param>
		/// <returns>A <see cref="T:System.Boolean"/> that is set to True if the specified sequential painting token was the last one registered; otherwise, False.</returns>
		/// <remarks>
		/// A control can register for sequential painting by calling the RegisterSequentialPainting method.
		/// </remarks>
		protected bool IsSequentialPaintingComplete(int sequentialPaintingToken)
		{
			return (this.registeredSequentialPaintingToken == sequentialPaintingToken);
		}

		/// <summary>
		/// Indicates that a certain class (generation) in the control hierarchy has completed updating the presentation.
		/// </summary>
		/// <param name="sequentialPaintingToken">A <see cref="T:System.Int32"/> that specifies the sequential painting token of the class (generation) that has completed updating the presentation.</param>
		/// <param name="args">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that references the original paint argument passed to the OnPaint method.</param>
		/// <remarks>
		/// If the sequential painting token argument represents the last token registered, this method will raise the Paint event, using the double buffer <see cref="T:System.Drawing.Graphics"/> object if the DoubleBufferEnabled property is set to True, and will also draw the contents of the buffer to the control, if applicable.
		/// </remarks>
		protected void NotifyPaintingComplete(int sequentialPaintingToken, PaintEventArgs args)
		{
			if (IsSequentialPaintingComplete(sequentialPaintingToken))
			{
				// Raise the Paint event so that the end-developer will have a chance to update the presentation.
				RaisePaintEvent(args);
				// If the presentation was double buffered, draw the contents of the buffer to the control.
				if (this.DoubleBufferEnabled)
				{
					DrawDoubleBuffer(args.Graphics);
				}
			}
		}

		/// <summary>
		/// Raises the KeyDown event.
		/// </summary>
		/// <param name="e">A KeyEventArgs that contains the event data.</param>
		protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
		{
			base.OnKeyDown(e);
			// If the end-developer hasn't explicitly handled the key, then simulate a Tab, or Shift+Tab, 
			// if the key represents a navigational key (Up, Down, Left, or Right).
			if (!e.Handled)
			{
				switch (e.KeyCode)
				{
					// Simulate a Shift+Tab key combination.
					case Keys.Up: case Keys.Left:
					{
						Core.SendKeyboardKey(VK_SHIFT, true, Core.KeyActionState.Down);
						Core.SendKeyboardKey(VK_TAB, true, Core.KeyActionState.Press);
						Core.SendKeyboardKey(VK_SHIFT, true, Core.KeyActionState.Up);
						break;
					}
					// Simulate a Tab key.
					case Keys.Down: case Keys.Right:
					{
						Core.SendKeyboardKey(VK_TAB, true, Core.KeyActionState.Press);
						break;
					}
				}
			}
		}

		#region Documentation =======================================================================
		/// <summary>
		/// Raises the Paint event.
		/// </summary>
		/// <param name="e">A PaintEventArgs that contains the event data.</param>
		/// <remarks>
		/// <b>Notes to Inheritors:</b> See the example section for the recommended pattern to ensure that double buffering and structured paint sequencing are both accommodated.
		/// </remarks>
		/// <example>
		/// <code>
		/// [C#]
		/// namespace MyCompanyName.TechnologyName
		/// {
		///   public class Button : OpenNETCF.Windows.Forms.ButtonBase
		///   {
		///     private readonly int SequentialPaintingToken = Int32.MinValue;
		///
		///     public Button()
		///     {
		///       // Note: The RegisterSequentialPainting method should only be called if the OnPaint method, 
		///       // in this class, is to be overridden.
		///       // Register in the sequential painting process so that, primarily, if this class is the most 
		///       // derived class in the hierarchy that needs to update the presentation of the control, the 
		///       // Paint event may be delayed until all classes in the hierarchy have had a chance to update 
		///       // the presentation.
		///       this.SequentialPaintingToken = base.RegisterSequentialPainting();
		///
		///       // Note: Double buffering does not need to be enabled for sequential painting to take place.
		///       // Request that the presentation be buffered before being drawn to the control.
		///       base.DoubleBufferEnabled = true;
		///     }
		///
		///     protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		///     {
		///       // Call the base classes OnPaint method to ensure that all appropriate base painting has been 
		///       // done prior to performing the presentation contribution of this class.
		///       base.OnPaint(e);
		///
		///       // Get a reference to the proper Graphics object used to update the presentation of the control.
		///       Graphics presentation = base.GetPresentationMedium(e.Graphics);
		///
		///       // ...
		///       // presentation.FillRectangle(Brush, X, Y, Width, Height);
		///       // ...
		///
		///       // Indicate that this class is done updating the presentation. If the sequential paint token 
		///       // for this class was the last one registered, then the Paint event will be triggered, and, 
		///       // if the presentation was double buffered, the contents of the buffer will be drawn to the 
		///       // control.
		///       base.NotifyPaintingComplete(this.SequentialPaintingToken, e);
		///     }
		///   }
		/// }
		/// </code>
		/// </example>
		#endregion ==================================================================================
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			// Note: Do not call the base classes OnPaint method in this method.

			// Get a reference to the proper Graphics object used to update the presentation of the control.
			Graphics presentation = GetPresentationMedium(e.Graphics);

			#region Presentation Logic ===============================================================

			// Paint a background for the control using the specified BackColor.
			SolidBrush backColorBrush = new SolidBrush(this.BackColor);
			presentation.FillRectangle(backColorBrush, 0, 0, this.Width, this.Height);
			backColorBrush.Dispose();

			#endregion ===============================================================================

			// Indicate that this class is done updating the presentation. If the sequential paint token 
			// for this class was the last one registered, then the Paint event will be triggered, and, 
			// if the presentation was double buffered, the contents of the buffer will be drawn to the 
			// control.
			NotifyPaintingComplete(SequentialPaintingToken, e);
		}

		/// <summary>
		/// Paints the background of the control.
		/// </summary>
		/// <param name="e">A PaintEventArgs that contains information about the control to paint.</param>
		/// <remarks>
		/// <b>Notes to Inheritors:</b> With the the hope of preventing noticeable flicker, this method has be overridden to do nothing. Therefore, all painting should be done in the OnPaint method.
		/// </remarks>
		protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs e)
		{
			// Do nothing with the hope of preventing noticeable flicker.
		}

		/// <summary>
		/// Raises the Resize event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected override void OnResize(System.EventArgs e)
		{
			if (this.DoubleBufferEnabled)
			{
				UpdateDoubleBuffer();
			}
			this.Invalidate();
			base.OnResize(e);
		}

		/// <summary>
		/// Explicitly raises the Paint event with double buffer awareness.
		/// </summary>
		/// <param name="args">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that references the original paint argument passed to the OnPaint method.</param>
		/// <remarks>
		/// If the DoubleBufferEnabled property is set to True, the double buffer <see cref="T:System.Drawing.Graphics"/> object will be passed through the event.
		/// </remarks>
		protected void RaisePaintEvent(PaintEventArgs args)
		{
			PaintEventArgs pea;
			// If double buffering is enabled, create a new PaintEventArgs object that references the 
			// double buffer Graphics object; otherwise, use the PaintEventArgs object that was passed 
			// into this method.
			if (this.DoubleBufferEnabled)
			{
				pea = new PaintEventArgs(this.DoubleBuffer, args.ClipRectangle);
			}
			else
			{
				pea = args;
			}
			// Since this classes base class is System.Windows.Forms.Control, no painting is actually 
			// performed in the base classes OnPaint method. Instead, the base classes OnPaint method 
			// simply raises the Paint event, if necessary.
			base.OnPaint(pea);
		}

		/// <summary>
		/// Registers the caller in the sequential painting process by generating a token that the caller can use to identify its sequencing order.
		/// </summary>
		/// <returns>A <see cref="T:System.Int32"/> that represents a sequential painting token.</returns>
		/// <remarks>
		/// The token generated by this method should be stored by the caller and passed back to this class when calling methods such as NotifyPaintingComplete or IsSequentialPaintingComplete.
		/// </remarks>
		protected int RegisterSequentialPainting()
		{
			return (this.registeredSequentialPaintingToken += 1);
		}

		/// <summary>
		/// Clean up the objects used to represent the double buffer for the presentation of the control.
		/// </summary>
		private void DisposeDoubleBuffer()
		{
			if (this.bufferGraphics != null)
			{
				this.bufferGraphics.Dispose();
				this.bufferGraphics = null;
			}
			if (this.bufferBitmap != null)
			{
				this.bufferBitmap.Dispose();
				this.bufferBitmap = null;
			}
		}

		/// <summary>
		/// Creates, or recreates, the objects used to represent the double buffer for the presentation of the control.
		/// </summary>
		private void UpdateDoubleBuffer()
		{
			DisposeDoubleBuffer();
			this.bufferBitmap = new Bitmap(this.ClientRectangle.Width, this.ClientRectangle.Height);
			this.bufferGraphics = Graphics.FromImage(this.bufferBitmap);
		}

		#endregion ==================================================================================

		/// <summary>
		/// Initializes a new instance of the ButtonBase class.
		/// </summary>
		protected ButtonBase()
		{
			this.Size = this.DefaultSize;
		}

		#endregion ==================================================================================
	}
}