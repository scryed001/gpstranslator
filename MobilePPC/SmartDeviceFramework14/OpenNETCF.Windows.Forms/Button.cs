//=================================================================================================
//
//		OpenNETCF.Windows.Forms.ButtonEx
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
using System.Drawing.Imaging;
using System.Reflection;
using System.Windows.Forms;
using OpenNETCF.Drawing;

namespace OpenNETCF.Windows.Forms
{
	/// <summary>
	/// Represents a Windows button control.
	/// </summary>
	#region Designer ===============================================================================
#if DESIGN
	[
		DefaultPropertyAttribute("Text"),
		DefaultEventAttribute("Click")
	]
#endif
	#endregion =====================================================================================
	public class ButtonEx : OpenNETCF.Windows.Forms.ButtonBase, OpenNETCF.Windows.Forms.IButtonControl
	{
		#region Fields ==============================================================================

		#region Defaults ============================================================================

		// Note: Changing the default values may have an impact on the behavior of this class. Before 
		// changing any default value(s), ensure that the intended behavior will remain intact, and 
		// after changing any default value(s), ensure that all comments accurately reflect the new 
		// value(s).

		private const DialogResult DefaultDialogResult = DialogResult.None;
		private static readonly Color DefaultAmbienceColor = Color.Empty;
		private static readonly Image DefaultActiveBackgroundImage = null;
		private static readonly Color DefaultBorderColor = Color.Black;
		private static readonly Image DefaultDisabledBackgroundImage = null;
		private const bool DefaultAutoSize = false;
		private const BorderStyle DefaultBorderStyle = BorderStyle.FixedSingle;
		private static readonly Color DefaultDisabledColor = SystemColors.GrayText;
		private const bool DefaultTransparentImage = true;
		private static readonly Font DefaultFontValue = new Font("Tahoma", 8.25F, FontStyle.Bold);
		private static readonly Size DefaultSizeValue = new Size(72, 20);

		#endregion ==================================================================================

		private readonly int SequentialPaintingToken = Int32.MinValue;
		private bool active = false;

		// Public Property Correspondents
		private DialogResult dialogResult = DefaultDialogResult;
		private Color activeBackColor = DefaultAmbienceColor;
		private Image activeBackgroundImage = DefaultActiveBackgroundImage;
		private Color activeBorderColor = DefaultBorderColor;
		private Color activeForeColor = DefaultAmbienceColor;
		private bool autoSize = DefaultAutoSize;
		private Color borderColor = DefaultBorderColor;
		private BorderStyle borderStyle = DefaultBorderStyle;
		private Color disabledBackColor = DefaultAmbienceColor;
		private Image disabledBackgroundImage = DefaultDisabledBackgroundImage;
		private Color disabledBorderColor = DefaultDisabledColor;
		private Color disabledForeColor = DefaultDisabledColor;
		private bool transparentImage = DefaultTransparentImage;

		#endregion ==================================================================================

		#region Properties ==========================================================================

		#region IButtonControl Properties ===========================================================

		/// <summary>
		/// Gets or sets a value that is returned to the parent form when the button is clicked.
		/// </summary>
		/// <value>One of the <see cref="T:System.Windows.Forms.DialogResult"/> values. The default value is None.</value>
		#region Designer ============================================================================
#if DESIGN
		[
			BrowsableAttribute(true),
			DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible),
			DesignOnlyAttribute(false),
			CategoryAttribute("Behavior"),
			DescriptionAttribute("The dialog result produced in a modal form by clicking the button."),
			DefaultValueAttribute(DefaultDialogResult)
		]
#endif
		#endregion ==================================================================================
		#region Attributes ==========================================================================
		[
			EditorBrowsableAttribute(EditorBrowsableState.Always)
		]
		#endregion ==================================================================================
		public virtual DialogResult DialogResult
		{
			get
			{
				return dialogResult;
			}
			set
			{
				if (Enum.IsDefined(typeof(System.Windows.Forms.DialogResult), value))
				{
					if (dialogResult != value)
					{
						// OnDialogResultChanging(...)
						dialogResult = value;
						// OnDialogResultChanged(...)
					}
				}
				else
				{
					throw new OpenNETCF.ComponentModel.InvalidEnumArgumentException("The DialogResult property must be one of the System.Windows.Forms.DialogResult values.");
				}
			}
		}

		#endregion ==================================================================================

		#region Extended Properties =================================================================

		#region Obsolete Properties =================================================================

		/// <summary>
		/// Gets or sets the background image displayed for the control in a disabled state.
		/// </summary>
		/// <value>A <see cref="T:System.Drawing.Image"/> that represents the disabled image to display in the background of the control.</value>
		/// <remarks>
		/// <b>Note:</b> This property does not exist for the <see cref="T:System.Windows.Forms.Button"/> control in the Full Framework (1.0, 1.1) or Compact Framework (1.0).
		/// </remarks>
		#region Designer ============================================================================
#if DESIGN
		[
			BrowsableAttribute(false),
			DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)
		]
#endif
		#endregion ==================================================================================
		#region Attributes ==========================================================================
		[
			ObsoleteAttribute("The DisabledBackgroundImage property should be used as a replacement."),
			EditorBrowsableAttribute(EditorBrowsableState.Never)
		]
		#endregion ==================================================================================
		public virtual Image BackgroundImageDisabled
		{
			get
			{
				return this.DisabledBackgroundImage;
			}
			set
			{
				this.DisabledBackgroundImage = value;
			}
		}

		/// <summary>
		/// Gets or sets the background image displayed for the control in an active state.
		/// </summary>
		/// <value>A <see cref="T:System.Drawing.Image"/> that represents the active image to display in the background of the control.</value>
		/// <remarks>
		/// <b>Note:</b> This property does not exist for the <see cref="T:System.Windows.Forms.Button"/> control in the Full Framework (1.0, 1.1) or Compact Framework (1.0).
		/// </remarks>
		#region Designer ============================================================================
#if DESIGN
		[
			BrowsableAttribute(false),
			DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)
		]
#endif
		#endregion ==================================================================================
		#region Attributes ==========================================================================
		[
			ObsoleteAttribute("The ActiveBackgroundImage property should be used as a replacement."),
			EditorBrowsableAttribute(EditorBrowsableState.Never)
		]
		#endregion ==================================================================================
		public virtual Image BackgroundImagePressed
		{
			get
			{
				return this.ActiveBackgroundImage;
			}
			set
			{
				this.ActiveBackgroundImage = value;
			}
		}

		#endregion ==================================================================================

		/// <summary>
		/// Gets or sets the background color for the control in an active state.
		/// </summary>
		/// <value>A <see cref="T:System.Drawing.Color"/> that represents the active background color of the control.</value>
		/// <remarks>
		/// If this property value is not explicitly set then the ForeColor property value will be returned.
		/// <br /><br />
		/// <b>Note:</b> This property does not exist for the <see cref="T:System.Windows.Forms.Button"/> control in the Full Framework (1.0, 1.1) or Compact Framework (1.0).
		/// </remarks>
		#region Designer ============================================================================
#if DESIGN
		[
			BrowsableAttribute(true),
			DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible),
			DesignOnlyAttribute(false),
			CategoryAttribute("Appearance"),
			DescriptionAttribute("The background color of the control in an active state.")
		]
#endif
		#endregion ==================================================================================
		#region Attributes ==========================================================================
		[
			EditorBrowsableAttribute(EditorBrowsableState.Always)
		]
		#endregion ==================================================================================
		public virtual Color ActiveBackColor
		{
			get
			{
				if (activeBackColor == DefaultAmbienceColor)
				{
					return this.ForeColor;
				}
				return activeBackColor;
			}
			set
			{
				if (activeBackColor != value)
				{
					// OnActiveBackColorChanging(...)
					activeBackColor = value;
					if (this.active)
					{
						this.Invalidate();
					}
					// OnActiveBackColorChanged(...)
				}
			}
		}
		#region Designer ============================================================================
#if DESIGN
		/// <summary>
		/// Indicates whether the ActiveBackColor property should be persisted. This method is intended to be used by a design environment and should only be called directly by an inheriting class from inside the override (Overrides in Visual Basic).
		/// </summary>
		/// <returns>True if the property value has changed from its default; otherwise, False.</returns>
		protected virtual bool ShouldSerializeActiveBackColor()
		{
			return (activeBackColor != DefaultAmbienceColor);
		}
		/// <summary>
		/// Resets the ActiveBackColor property to its default value. This method is intended to be used by a design environment and should only be called directly by an inheriting class from inside the override (Overrides in Visual Basic).
		/// </summary>
		protected virtual void ResetActiveBackColor()
		{
			ActiveBackColor = DefaultAmbienceColor;
		}
#endif
		#endregion ==================================================================================

		/// <summary>
		/// Gets or sets the background image displayed for the control in an active state.
		/// </summary>
		/// <value>A <see cref="T:System.Drawing.Image"/> that represents the active image to display in the background of the control.</value>
		/// <remarks>
		/// <b>Note:</b> This property does not exist for the <see cref="T:System.Windows.Forms.Button"/> control in the Full Framework (1.0, 1.1) or Compact Framework (1.0).
		/// </remarks>
		#region Designer ============================================================================
#if DESIGN
		[
			BrowsableAttribute(true),
			DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible),
			DesignOnlyAttribute(false),
			CategoryAttribute("Appearance"),
			DescriptionAttribute("The background image displayed for the control in an active state.")
		]
#endif
		#endregion ==================================================================================
		#region Attributes ==========================================================================
		[
			EditorBrowsableAttribute(EditorBrowsableState.Always)
		]
		#endregion ==================================================================================
		public virtual Image ActiveBackgroundImage
		{
			get
			{
				return activeBackgroundImage;
			}
			set
			{
				if (activeBackgroundImage != value)
				{
					// OnActiveBackgroundImageChanging(...)
					activeBackgroundImage = value;
					if (this.active)
					{
						this.Invalidate();
					}
					// OnActiveBackgroundImageChanged(...)
				}
			}
		}
		#region Designer ============================================================================
#if DESIGN
		/// <summary>
		/// Indicates whether the ActiveBackgroundImage property should be persisted. This method is intended to be used by a design environment and should only be called directly by an inheriting class from inside the override (Overrides in Visual Basic).
		/// </summary>
		/// <returns>True if the property value has changed from its default; otherwise, False.</returns>
		protected virtual bool ShouldSerializeActiveBackgroundImage()
		{
			return (ActiveBackgroundImage != DefaultActiveBackgroundImage);
		}
		/// <summary>
		/// Resets the ActiveBackgroundImage property to its default value. This method is intended to be used by a design environment and should only be called directly by an inheriting class from inside the override (Overrides in Visual Basic).
		/// </summary>
		protected virtual void ResetActiveBackgroundImage()
		{
			ActiveBackgroundImage = DefaultActiveBackgroundImage;
		}
#endif
		#endregion ==================================================================================

		/// <summary>
		/// Gets or sets the color of the border for the control in an active state.
		/// </summary>
		/// <value>A <see cref="T:System.Drawing.Color"/> that represents the border color of the control. The default is Color.Black.</value>
		/// <remarks>
		/// This property is only valid when the BorderStyle property is set to FixedSingle.
		/// <br /><br />
		/// <b>Note:</b> This property does not exist for the <see cref="T:System.Windows.Forms.Button"/> control in the Full Framework (1.0, 1.1) or Compact Framework (1.0).
		/// </remarks>
		#region Designer ============================================================================
#if DESIGN
		[
			BrowsableAttribute(true),
			DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible),
			DesignOnlyAttribute(false),
			CategoryAttribute("Appearance"),
			DescriptionAttribute("The border color of the control in an active state.")
		]
#endif
		#endregion ==================================================================================
		#region Attributes ==========================================================================
		[
			EditorBrowsableAttribute(EditorBrowsableState.Always)
		]
		#endregion ==================================================================================
		public virtual Color ActiveBorderColor
		{
			get
			{
				return activeBorderColor;
			}
			set
			{
				if (activeBorderColor != value)
				{
					// OnActiveBorderColorChanging(...)
					activeBorderColor = value;
					if ((this.active) && (this.BorderStyle == BorderStyle.FixedSingle))
					{
						this.Invalidate();
					}
					// OnActiveBorderColorChanged(...)
				}
			}
		}
		#region Designer ============================================================================
#if DESIGN
		/// <summary>
		/// Indicates whether the ActiveBorderColor property should be persisted. This method is intended to be used by a design environment and should only be called directly by an inheriting class from inside the override (Overrides in Visual Basic).
		/// </summary>
		/// <returns>True if the property value has changed from its default; otherwise, False.</returns>
		protected virtual bool ShouldSerializeActiveBorderColor()
		{
			return (ActiveBorderColor != DefaultBorderColor);
		}
		/// <summary>
		/// Resets the ActiveBorderColor property to its default value. This method is intended to be used by a design environment and should only be called directly by an inheriting class from inside the override (Overrides in Visual Basic).
		/// </summary>
		protected virtual void ResetActiveBorderColor()
		{
			ActiveBorderColor = DefaultBorderColor;
		}
#endif
		#endregion ==================================================================================

		/// <summary>
		/// Gets or sets the foreground color for the control in an active state.
		/// </summary>
		/// <value>A <see cref="T:System.Drawing.Color"/> that represents the active foreground color of the control.</value>
		/// <remarks>
		/// If this property value is not explicitly set then the BackColor property value will be returned.
		/// <br /><br />
		/// <b>Note:</b> This property does not exist for the <see cref="T:System.Windows.Forms.Button"/> control in the Full Framework (1.0, 1.1) or Compact Framework (1.0).
		/// </remarks>
		#region Designer ============================================================================
#if DESIGN
		[
			BrowsableAttribute(true),
			DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible),
			DesignOnlyAttribute(false),
			CategoryAttribute("Appearance"),
			DescriptionAttribute("The foreground color of the control in an active state.")
		]
#endif
		#endregion ==================================================================================
		#region Attributes ==========================================================================
		[
			EditorBrowsableAttribute(EditorBrowsableState.Always)
		]
		#endregion ==================================================================================
		public virtual Color ActiveForeColor
		{
			get
			{
				if (activeForeColor == DefaultAmbienceColor)
				{
					return this.BackColor;
				}
				return activeForeColor;
			}
			set
			{
				if (activeForeColor != value)
				{
					// OnActiveForeColorChanging(...)
					activeForeColor = value;
					if (this.active)
					{
						this.Invalidate();
					}
					// OnActiveForeColorChanged(...)
				}
			}
		}
		#region Designer ============================================================================
#if DESIGN
		/// <summary>
		/// Indicates whether the ActiveForeColor property should be persisted. This method is intended to be used by a design environment and should only be called directly by an inheriting class from inside the override (Overrides in Visual Basic).
		/// </summary>
		/// <returns>True if the property value has changed from its default; otherwise, False.</returns>
		protected virtual bool ShouldSerializeActiveForeColor()
		{
			return (activeForeColor != DefaultAmbienceColor);
		}
		/// <summary>
		/// Resets the ActiveForeColor property to its default value. This method is intended to be used by a design environment and should only be called directly by an inheriting class from inside the override (Overrides in Visual Basic).
		/// </summary>
		protected virtual void ResetActiveForeColor()
		{
			ActiveForeColor = DefaultAmbienceColor;
		}
#endif
		#endregion ==================================================================================

		/// <summary>
		/// Gets or sets a value indicating whether the control is automatically resized to display its contents.
		/// </summary>
		/// <value>A <see cref="T:System.Boolean"/> that is set to True if the control is automatically resized; otherwise, False.</value>
		/// <remarks>
		/// <b>Note:</b> This property does not exist for the <see cref="T:System.Windows.Forms.Button"/> control in the Full Framework (1.0, 1.1) or Compact Framework (1.0).
		/// </remarks>
		#region Designer ============================================================================
#if DESIGN
		[
			BrowsableAttribute(true),
			DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible),
			DesignOnlyAttribute(false),
			CategoryAttribute("Behavior"),
			DescriptionAttribute("Indicates whether the control is automatically resized."),
			DefaultValueAttribute(DefaultAutoSize)
		]
#endif
		#endregion ==================================================================================
		#region Attributes ==========================================================================
		[
			EditorBrowsableAttribute(EditorBrowsableState.Always)
		]
		#endregion ==================================================================================
		public virtual bool AutoSize
		{
			get
			{
				return autoSize;
			}
			set
			{
				if (autoSize != value)
				{
					// OnAutoSizeChanging(...)
					autoSize = value;
					// TODO: Provide an implementation, and all other necessary infrastructure, to support this property.
					// OnAutoSizeChanged(...)
				}
			}
		}

		/// <summary>
		/// Gets or sets the color of the border for the control in an enabled state.
		/// </summary>
		/// <value>A <see cref="T:System.Drawing.Color"/> that represents the border color of the control. The default is Color.Black.</value>
		/// <remarks>
		/// This property is only valid when the BorderStyle property is set to FixedSingle.
		/// <br /><br />
		/// <b>Note:</b> This property does not exist for the <see cref="T:System.Windows.Forms.Button"/> control in the Full Framework (1.0, 1.1) or Compact Framework (1.0).
		/// </remarks>
		#region Designer ============================================================================
#if DESIGN
		[
			BrowsableAttribute(true),
			DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible),
			DesignOnlyAttribute(false),
			CategoryAttribute("Appearance"),
			DescriptionAttribute("The border color of the control in an enabled state.")
		]
#endif
		#endregion ==================================================================================
		#region Attributes ==========================================================================
		[
			EditorBrowsableAttribute(EditorBrowsableState.Always)
		]
		#endregion ==================================================================================
		public virtual Color BorderColor
		{
			get
			{
				return borderColor;
			}
			set
			{
				if (borderColor != value)
				{
					// OnBorderColorChanging(...)
					borderColor = value;
					if ((this.Enabled) && (this.BorderStyle == BorderStyle.FixedSingle))
					{
						this.Invalidate();
					}
					// OnBorderColorChanged(...)
				}
			}
		}
		#region Designer ============================================================================
#if DESIGN
		/// <summary>
		/// Indicates whether the BorderColor property should be persisted. This method is intended to be used by a design environment and should only be called directly by an inheriting class from inside the override (Overrides in Visual Basic).
		/// </summary>
		/// <returns>True if the property value has changed from its default; otherwise, False.</returns>
		protected virtual bool ShouldSerializeBorderColor()
		{
			return (BorderColor != DefaultBorderColor);
		}
		/// <summary>
		/// Resets the BorderColor property to its default value. This method is intended to be used by a design environment and should only be called directly by an inheriting class from inside the override (Overrides in Visual Basic).
		/// </summary>
		protected virtual void ResetBorderColor()
		{
			BorderColor = DefaultBorderColor;
		}
#endif
		#endregion ==================================================================================

		/// <summary>
		/// Gets or sets the style of the border for the control.
		/// </summary>
		/// <value>One of the <see cref="T:System.Windows.Forms.BorderStyle"/> values. The default is FixedSingle.</value>
		/// <remarks>
		/// It is recommended that applications targeting the broad Windows platform use the Fixed3D value, and applications targeting either the Pocket PC or Smartphone platform use the default, FixedSingle.
		/// <br /><br />
		/// <b>Note:</b> This property does not exist for the <see cref="T:System.Windows.Forms.Button"/> control in the Full Framework (1.0, 1.1) or Compact Framework (1.0).
		/// </remarks>
		#region Designer ============================================================================
#if DESIGN
		[
			BrowsableAttribute(true),
			DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible),
			DesignOnlyAttribute(false),
			CategoryAttribute("Appearance"),
			DescriptionAttribute("The style of the border for the control."),
			DefaultValueAttribute(DefaultBorderStyle)
		]
#endif
		#endregion ==================================================================================
		#region Attributes ==========================================================================
		[
			EditorBrowsableAttribute(EditorBrowsableState.Always)
		]
		#endregion ==================================================================================
		public BorderStyle BorderStyle
		{
			get
			{
				return borderStyle;
			}
			set
			{
				if (Enum.IsDefined(typeof(System.Windows.Forms.BorderStyle), value))
				{
					if (borderStyle != value)
					{
						// OnBorderStyleChanging(...)
						borderStyle = value;
						this.Invalidate();
						// OnBorderStyleChanged(...)
					}
				}
				else
				{
					throw new OpenNETCF.ComponentModel.InvalidEnumArgumentException("The BorderStyle property must be one of the System.Windows.Forms.BorderStyle values.");
				}
			}
		}

		/// <summary>
		/// Gets or sets the background color for the control in a disabled state.
		/// </summary>
		/// <value>A <see cref="T:System.Drawing.Color"/> that represents the disabled background color of the control.</value>
		/// <remarks>
		/// If this property value is not explicitly set then the BackColor property value will be returned.
		/// <br /><br />
		/// <b>Note:</b> This property does not exist for the <see cref="T:System.Windows.Forms.Button"/> control in the Full Framework (1.0, 1.1) or Compact Framework (1.0).
		/// </remarks>
		#region Designer ============================================================================
#if DESIGN
		[
			BrowsableAttribute(true),
			DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible),
			DesignOnlyAttribute(false),
			CategoryAttribute("Appearance"),
			DescriptionAttribute("The background color of the control in a disabled state.")
		]
#endif
		#endregion ==================================================================================
		#region Attributes ==========================================================================
		[
			EditorBrowsableAttribute(EditorBrowsableState.Always)
		]
		#endregion ==================================================================================
		public virtual Color DisabledBackColor
		{
			get
			{
				if (disabledBackColor == DefaultAmbienceColor)
				{
					return this.BackColor;
				}
				return disabledBackColor;
			}
			set
			{
				if (disabledBackColor != value)
				{
					// OnDisabledBackColorChanging(...)
					disabledBackColor = value;
					if (!this.Enabled)
					{
						this.Invalidate();
					}
					// OnDisabledBackColorChanged(...)
				}
			}
		}
		#region Designer ============================================================================
#if DESIGN
		/// <summary>
		/// Indicates whether the DisabledBackColor property should be persisted. This method is intended to be used by a design environment and should only be called directly by an inheriting class from inside the override (Overrides in Visual Basic).
		/// </summary>
		/// <returns>True if the property value has changed from its default; otherwise, False.</returns>
		protected virtual bool ShouldSerializeDisabledBackColor()
		{
			return (disabledBackColor != DefaultAmbienceColor);
		}
		/// <summary>
		/// Resets the DisabledBackColor property to its default value. This method is intended to be used by a design environment and should only be called directly by an inheriting class from inside the override (Overrides in Visual Basic).
		/// </summary>
		protected virtual void ResetDisabledBackColor()
		{
			DisabledBackColor = DefaultAmbienceColor;
		}
#endif
		#endregion ==================================================================================

		/// <summary>
		/// Gets or sets the background image displayed for the control in a disabled state.
		/// </summary>
		/// <value>A <see cref="T:System.Drawing.Image"/> that represents the disabled image to display in the background of the control.</value>
		/// <remarks>
		/// <b>Note:</b> This property does not exist for the <see cref="T:System.Windows.Forms.Button"/> control in the Full Framework (1.0, 1.1) or Compact Framework (1.0).
		/// </remarks>
		#region Designer ============================================================================
#if DESIGN
		[
			BrowsableAttribute(true),
			DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible),
			DesignOnlyAttribute(false),
			CategoryAttribute("Appearance"),
			DescriptionAttribute("The background image displayed for the control in a disabled state.")
		]
#endif
		#endregion ==================================================================================
		#region Attributes ==========================================================================
		[
			EditorBrowsableAttribute(EditorBrowsableState.Always)
		]
		#endregion ==================================================================================
		public virtual Image DisabledBackgroundImage
		{
			get
			{
				return disabledBackgroundImage;
			}
			set
			{
				if (disabledBackgroundImage != value)
				{
					// OnDisabledBackgroundImageChanging(...)
					disabledBackgroundImage = value;
					if (!this.Enabled)
					{
						this.Invalidate();
					}
					// OnDisabledBackgroundImageChanged(...)
				}
			}
		}
		#region Designer ============================================================================
#if DESIGN
		/// <summary>
		/// Indicates whether the DisabledBackgroundImage property should be persisted. This method is intended to be used by a design environment and should only be called directly by an inheriting class from inside the override (Overrides in Visual Basic).
		/// </summary>
		/// <returns>True if the property value has changed from its default; otherwise, False.</returns>
		protected virtual bool ShouldSerializeDisabledBackgroundImage()
		{
			return (DisabledBackgroundImage != DefaultDisabledBackgroundImage);
		}
		/// <summary>
		/// Resets the DisabledBackgroundImage property to its default value. This method is intended to be used by a design environment and should only be called directly by an inheriting class from inside the override (Overrides in Visual Basic).
		/// </summary>
		protected virtual void ResetDisabledBackgroundImage()
		{
			DisabledBackgroundImage = DefaultDisabledBackgroundImage;
		}
#endif
		#endregion ==================================================================================

		/// <summary>
		/// Gets or sets the color of the border for the control in a disabled state.
		/// </summary>
		/// <value>A <see cref="T:System.Drawing.Color"/> that represents the disabled border color of the control.</value>
		/// <remarks>
		/// This property is only valid when the BorderStyle property is set to FixedSingle.
		/// <br /><br />
		/// <b>Note:</b> This property does not exist for the <see cref="T:System.Windows.Forms.Button"/> control in the Full Framework (1.0, 1.1) or Compact Framework (1.0).
		/// </remarks>
		#region Designer ============================================================================
#if DESIGN
		[
			BrowsableAttribute(true),
			DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible),
			DesignOnlyAttribute(false),
			CategoryAttribute("Appearance"),
			DescriptionAttribute("The border color of the control in a disabled state.")
		]
#endif
		#endregion ==================================================================================
		#region Attributes ==========================================================================
		[
			EditorBrowsableAttribute(EditorBrowsableState.Always)
		]
		#endregion ==================================================================================
		public virtual Color DisabledBorderColor
		{
			get
			{
				return disabledBorderColor;
			}
			set
			{
				if (disabledBorderColor != value)
				{
					// OnDisabledBorderColorChanging(...)
					disabledBorderColor = value;
					if ((!this.Enabled) && (this.BorderStyle == BorderStyle.FixedSingle))
					{
						this.Invalidate();
					}
					// OnDisabledBorderColorChanged(...)
				}
			}
		}
		#region Designer ============================================================================
#if DESIGN
		/// <summary>
		/// Indicates whether the DisabledBorderColor property should be persisted. This method is intended to be used by a design environment and should only be called directly by an inheriting class from inside the override (Overrides in Visual Basic).
		/// </summary>
		/// <returns>True if the property value has changed from its default; otherwise, False.</returns>
		protected virtual bool ShouldSerializeDisabledBorderColor()
		{
			return (DisabledBorderColor != DefaultDisabledColor);
		}
		/// <summary>
		/// Resets the DisabledBorderColor property to its default value. This method is intended to be used by a design environment and should only be called directly by an inheriting class from inside the override (Overrides in Visual Basic).
		/// </summary>
		protected virtual void ResetDisabledBorderColor()
		{
			DisabledBorderColor = DefaultDisabledColor;
		}
#endif
		#endregion ==================================================================================

		/// <summary>
		/// Gets or sets the foreground color for the control in a disabled state.
		/// </summary>
		/// <value>A <see cref="T:System.Drawing.Color"/> that represents the disabled foreground color of the control.</value>
		/// <remarks>
		/// <b>Note:</b> This property does not exist for the <see cref="T:System.Windows.Forms.Button"/> control in the Full Framework (1.0, 1.1) or Compact Framework (1.0).
		/// </remarks>
		#region Designer ============================================================================
#if DESIGN
		[
			BrowsableAttribute(true),
			DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible),
			DesignOnlyAttribute(false),
			CategoryAttribute("Appearance"),
			DescriptionAttribute("The foreground color of the control in a disabled state.")
		]
#endif
		#endregion ==================================================================================
		#region Attributes ==========================================================================
		[
			EditorBrowsableAttribute(EditorBrowsableState.Always)
		]
		#endregion ==================================================================================
		public virtual Color DisabledForeColor
		{
			get
			{
				return disabledForeColor;
			}
			set
			{
				if (disabledForeColor != value)
				{
					// OnDisabledForeColorChanging(...)
					disabledForeColor = value;
					if (!this.Enabled)
					{
						this.Invalidate();
					}
					// OnDisabledForeColorChanged(...)
				}
			}
		}
		#region Designer ============================================================================
#if DESIGN
		/// <summary>
		/// Indicates whether the DisabledForeColor property should be persisted. This method is intended to be used by a design environment and should only be called directly by an inheriting class from inside the override (Overrides in Visual Basic).
		/// </summary>
		/// <returns>True if the property value has changed from its default; otherwise, False.</returns>
		protected virtual bool ShouldSerializeDisabledForeColor()
		{
			return (DisabledForeColor != DefaultDisabledColor);
		}
		/// <summary>
		/// Resets the DisabledForeColor property to its default value. This method is intended to be used by a design environment and should only be called directly by an inheriting class from inside the override (Overrides in Visual Basic).
		/// </summary>
		protected virtual void ResetDisabledForeColor()
		{
			DisabledForeColor = DefaultDisabledColor;
		}
#endif
		#endregion ==================================================================================

		/// <summary>
		/// Gets or sets a value indicating whether the foreground image for the control contains transparency.
		/// </summary>
		/// <value>A <see cref="T:System.Boolean"/> that is set to True if the foreground image of the control contains transparency; otherwise, False. The default is True.</value>
		/// <remarks>
		/// The color of the top-left pixel in the foreground image is used as the transparency key.
		/// <br /><br />
		/// <b>Note:</b> This property does not exist for the <see cref="T:System.Windows.Forms.Button"/> control in the Full Framework (1.0, 1.1) or Compact Framework (1.0).
		/// </remarks>
		#region Designer ============================================================================
#if DESIGN
		[
			BrowsableAttribute(true),
			DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible),
			DesignOnlyAttribute(false),
			CategoryAttribute("Behavior"),
			DescriptionAttribute("Indicates whether the foreground image for the control contains transparency."),
			DefaultValueAttribute(DefaultTransparentImage)
		]
#endif
		#endregion ==================================================================================
		#region Attributes ==========================================================================
		[
			EditorBrowsableAttribute(EditorBrowsableState.Always)
		]
		#endregion ==================================================================================
		public bool TransparentImage
		{
			get
			{
				return transparentImage;
			}
			set
			{
				if (transparentImage != value)
				{
					// OnTransparentImageChanging(...)
					transparentImage = value;
					if (this.Image != null)
					{
						this.Invalidate();
					}
					// OnTransparentImageChanged(...)
				}
			}
		}

		#endregion ==================================================================================

		/// <summary>
		/// Gets the default font of the control.
		/// </summary>
		/// <value>The default <see cref="T:System.Drawing.Font"/> of the control. The default is set to Tahoma (8 point, bold) in accordance with the "command button" font recommendation in the Pocket PC user interface guidelines.</value>
		public static new Font DefaultFont
		{
			get
			{
				return DefaultFontValue;
			}
		}

		/// <summary>
		/// Gets or sets the font of the text displayed by the control.
		/// </summary>
		/// <value>The <see cref="T:System.Drawing.Font"/> object to apply to the text displayed by the control.</value>
		public override Font Font
		{
			get
			{
				if (!base.IsFontSet)
				{
					return DefaultFont;
				}
				return base.Font;
			}
			set
			{
				base.Font = value;
			}
		}

		/// <summary>
		/// Gets the default size of the control.
		/// </summary>
		/// <value>The default <see cref="T:System.Drawing.Size"/> of the control. The default is set to a height of 20 pixels in accordance with the "command button" size recommendation in the Pocket PC user interface guidelines.</value>
		protected override Size DefaultSize
		{
			get
			{
				return DefaultSizeValue;
			}
		}

		#endregion ==================================================================================

		#region Methods =============================================================================

		#region IButtonControl Methods ==============================================================

		/// <summary>
		/// Notifies the button whether it is the default button so that it can adjust its appearance accordingly.
		/// </summary>
		/// <param name="value">True if the button is to have the appearance of the default button; otherwise, False.</param>
		public virtual void NotifyDefault(bool value)
		{
			if (this.IsDefault != value)
			{
				this.IsDefault = value;
			}
		}

		/// <summary>
		/// Generates a Click event for a button.
		/// </summary>
		public virtual void PerformClick()
		{
			if ((this.Enabled) && (this.Visible))
			{
				this.OnClick(EventArgs.Empty);
			}
		}

		#endregion ==================================================================================

		/// <summary>
		/// Initializes a new instance of the ButtonEx class.
		/// </summary>
		public ButtonEx()
		{
			// Register in the sequential painting process so that, primarily, if this class is the most 
			// derived class in the hierarchy that needs to update the presentation of the control, the 
			// Paint event may be delayed until all classes in the hierarchy have had a chance to update 
			// the presentation.
			this.SequentialPaintingToken = base.RegisterSequentialPainting();

			// Request that the presentation be buffered before being drawn to the control.
			base.DoubleBufferEnabled = true;
		}

		/// <summary>
		/// Raises the Click event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected override void OnClick(System.EventArgs e)
		{
			// If the user has selected the control, by indication of the left mouse button, then ensure 
			// that the presentation of the control is updated accordingly.
			if (this.active)
			{
				this.active = false;
				this.Invalidate();
			}
			// If applicable, indicate to the Form, through the Forms DialogResult property, that the 
			// control was selected.
			if ((this.DialogResult != DialogResult.None) && (this.TopLevelControl is System.Windows.Forms.Form))
			{
				((System.Windows.Forms.Form)this.TopLevelControl).DialogResult = this.DialogResult;
			}
			base.OnClick(e);
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
		/// Raises the GotFocus event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected override void OnGotFocus(System.EventArgs e)
		{
			this.Invalidate();
			base.OnGotFocus(e);
		}

		/// <summary>
		/// Raises the KeyPress event.
		/// </summary>
		/// <param name="e">A KeyPressEventArgs that contains the event data.</param>
		protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
		{
			base.OnKeyPress(e);
			// If the end-developer hasn't explicitly handled the key, then simulate a click if the key 
			// represents either the Enter or Space key.
			if (!e.Handled)
			{
				switch (Convert.ToInt32(e.KeyChar))
				{
					case (int)Keys.Enter: case (int)Keys.Space:
					{
						this.PerformClick();
						break;
					}
				}
			}
		}

		/// <summary>
		/// Raises the LostFocus event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected override void OnLostFocus(System.EventArgs e)
		{
			this.Invalidate();
			base.OnLostFocus(e);
		}

		/// <summary>
		/// Raises the MouseDown event.
		/// </summary>
		/// <param name="e">A MouseEventArgs that contains the event data.</param>
		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			// If the user has selected the control, by indication of the left mouse button, then ensure 
			// that the control is focused and update the presentation.
			if (e.Button == MouseButtons.Left)
			{
				this.active = true;
				if (!this.Focused)
				{
					this.Focus();
				}
				else
				{
					this.Invalidate();
				}
			}
			base.OnMouseDown(e);
		}

		/// <summary>
		/// Raises the MouseMove event.
		/// </summary>
		/// <param name="e">A MouseEventArgs that contains the event data.</param>
		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
		{
			// If the user has selected the control, by indication of the left mouse button, and then 
			// moves the mouse position, ensure that the presentation of the control is updated accordingly.
			if (e.Button == MouseButtons.Left)
			{
				Rectangle controlRect = new Rectangle(0, 0, this.Width, this.Height);
				if ((this.active) && (!controlRect.Contains(e.X, e.Y)))
				{
					this.active = false;
					this.Invalidate();
				}
				else if ((!this.active) && (controlRect.Contains(e.X, e.Y)))
				{
					this.active = true;
					this.Invalidate();
				}
			}
			base.OnMouseMove(e);
		}

		/// <summary>
		/// Raises the Paint event.
		/// </summary>
		/// <param name="e">A PaintEventArgs that contains the event data.</param>
		/// <remarks>
		/// <b>Notes to Inheritors:</b> See the example section, in the <see cref="T:OpenNETCF.Windows.Forms.ButtonBase"/> control documentation, for the recommended pattern to ensure that double buffering and structured paint sequencing are both accommodated.
		/// </remarks>
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			// Call the base classes OnPaint method to ensure that all appropriate base painting has been 
			// done prior to performing the presentation contribution of this class.
			base.OnPaint(e);

			// Get a reference to the proper Graphics object used to update the presentation of the control.
			Graphics presentation = base.GetPresentationMedium(e.Graphics);

			#region Presentation Logic ===============================================================

			Size focusBorderSize = new Size(1, 1);

			#region Background =======================================================================

			Image backgroundImage = this.BackgroundImage;

			// Determine the image to be used as the background for the control, if applicable.
			if ((this.active) && (this.ActiveBackgroundImage != null))
			{
				backgroundImage = this.ActiveBackgroundImage;
			}
			else if ((!this.Enabled) && (this.DisabledBackgroundImage != null))
			{
				backgroundImage = this.DisabledBackgroundImage;
			}

			// If there is no background image for the control, paint using the appropriate background 
			// color; otherwise, paint the background image.
			if (backgroundImage == null)
			{
				SolidBrush backColorBrush = new SolidBrush(this.BackColor);
				if (this.active)
				{
					backColorBrush.Color = this.ActiveBackColor;
				}
				else if (!this.Enabled)
				{
					backColorBrush.Color = this.DisabledBackColor;
				}
				presentation.FillRectangle(backColorBrush, 0, 0, this.Width, this.Height);
				backColorBrush.Dispose();
			}
			else
			{
				Rectangle destinationRect = new Rectangle(0, 0, this.Width, this.Height);
				Rectangle sourceRect = new Rectangle(0, 0, backgroundImage.Width, backgroundImage.Height);
				presentation.DrawImage(backgroundImage, destinationRect, sourceRect, GraphicsUnit.Pixel);
			}

			#endregion ===============================================================================

			#region Content ==========================================================================

			int minimumContentOffset = 1;
			Rectangle minimumContentRect = Rectangle.Empty;
			Size maximumBorderSize = Size.Empty;

			// Determine the maximum size of the border based on the specified border style.
			switch (this.BorderStyle)
			{
				case BorderStyle.None:
				{
					maximumBorderSize = new Size(0, 0);
					break;
				}
				case BorderStyle.FixedSingle:
				{
					maximumBorderSize = new Size(1, 1);
					break;
				}
				case BorderStyle.Fixed3D:
				{
					maximumBorderSize = new Size(2, 2);
					break;
				}
			}

			// Add the focus border size to the maximum border size value.
			maximumBorderSize.Width += focusBorderSize.Width;
			maximumBorderSize.Height += focusBorderSize.Height;

			// Determine the worst case scenario for the content rectangle.
			minimumContentRect = new Rectangle((maximumBorderSize.Width + minimumContentOffset), (maximumBorderSize.Height + minimumContentOffset), (this.ClientRectangle.Width - ((maximumBorderSize.Width + minimumContentOffset) * 2)), (this.ClientRectangle.Height - ((maximumBorderSize.Height + minimumContentOffset) * 2)));

			#region Image ============================================================================

			Image foregroundImage = this.Image;

			// Paint the image to be used as part of the content for the control, if applicable.
			if (foregroundImage != null)
			{
				ImageAttributes attributes;
				Point contentPoint;
				Rectangle destinationRect;
				SizeF contentSize;

				// Create an ImageAttributes object to set the transparency key, if applicable.
				attributes = new ImageAttributes();

				// Determine the location at which to draw the content image.
				contentSize = new SizeF(foregroundImage.Size.Width, foregroundImage.Size.Height);
				contentPoint = GetLocationFromContentAlignment(this.ImageAlign, contentSize, minimumContentRect);

				// Determine the rectangle in which the content image may be painted.
				destinationRect = new Rectangle(contentPoint.X, contentPoint.Y, foregroundImage.Width, foregroundImage.Height);

				// Set the transparency color to be used when painting the content image, if applicable.
				if (this.TransparentImage)
				{
					Bitmap foregroundBitmap;
					bool bitmapCreated;
					Color foregroundColor;
					// Get a reference to the content image as a Bitmap object.
					bitmapCreated = false;
					foregroundBitmap = foregroundImage as Bitmap;
					if (foregroundBitmap == null)
					{
						foregroundBitmap = new Bitmap(foregroundImage);
						bitmapCreated = true;
					}
					// Get the color of the top-left pixel in the content image.
					foregroundColor = foregroundBitmap.GetPixel(0, 0);
					// Set the transparency key on the ImageAttributes object.
					attributes.SetColorKey(foregroundColor, foregroundColor);
					// If a Bitmap object was created, release the resources.
					if (bitmapCreated)
					{
						foregroundBitmap.Dispose();
					}
				}

				// Paint the content image for the control.
				presentation.DrawImage(foregroundImage, destinationRect, 0, 0, foregroundImage.Width, foregroundImage.Height, GraphicsUnit.Pixel, attributes);
			}

			#endregion ===============================================================================

			#region Text =============================================================================

			// TODO: If the control is disabled and the BorderStyle property is set to Fixed3D, the content 
			// text should be painted in the resting state using the dark border color and then the content 
			// text should again be painted at the same location as it would be in the active state using 
			// the lightest border color.

			string foregroundText = this.Text;

			// Paint the text to be used as part of the content for the control, if applicable.
			if ((foregroundText != null) && (this.Font != null))
			{
				bool activeAction;
				Point contentPoint;
				Rectangle destinationRect;
				SizeF contentSize;
				SolidBrush foreColorBrush;

				// Determine the location at which to draw the content text.
				contentSize = presentation.MeasureString(foregroundText, this.Font);
				contentPoint = GetLocationFromContentAlignment(this.TextAlign, contentSize, minimumContentRect);

				// If the control is active and rendered with a three-dimensional appearance, offset the 
				// location of the content text.
				activeAction = false;
				if ((this.BorderStyle == BorderStyle.Fixed3D) && (this.active))
				{
					activeAction = true;
					contentPoint.X += 1;
					contentPoint.Y += 1;
				}

				// Determine the rectangle in which the content text may be painted.
				destinationRect = new Rectangle(contentPoint.X, contentPoint.Y, (minimumContentRect.X + minimumContentRect.Width - contentPoint.X + ((activeAction) ? 1 : 0)), (minimumContentRect.Y + minimumContentRect.Height - contentPoint.Y + ((activeAction) ? 1 : 0)));

				// Determine the color to be used as the foreground for the control.
				foreColorBrush = new SolidBrush(this.ForeColor);
				if (this.active)
				{
					foreColorBrush.Color = this.ActiveForeColor;
				}
				else if (!this.Enabled)
				{
					foreColorBrush.Color = this.DisabledForeColor;
				}

				// Paint the content text for the control.
				presentation.DrawString(foregroundText, this.Font, foreColorBrush, destinationRect);

				// Release any resources.
				foreColorBrush.Dispose();
			}

			#endregion ===============================================================================

			#endregion ===============================================================================

			#region Border ===========================================================================

			bool paintFocusBorder = false;
			Pen borderPen = new Pen(this.BorderColor);
			Size borderOffsetSize = new Size(0, 0);

			// Determine the color to be used as the border for the control.
			if (this.active)
			{
				borderPen.Color = this.ActiveBorderColor;
			}
			else if (!this.Enabled)
			{
				borderPen.Color = this.DisabledBorderColor;
			}

			// If the control is focused or specified as the default, indicate that the focus border 
			// should be painted.
			if ((this.Focused) || (this.IsDefault))
			{
				paintFocusBorder = true;
			}

			// Modify the border offset size if the focus border is to be painted.
			if (paintFocusBorder)
			{
				borderOffsetSize = focusBorderSize;
			}

			// Paint the border for the control.
			switch (this.BorderStyle)
			{
				case BorderStyle.FixedSingle:
				{
					// Paint the border for the control with a single-line appearance.
					presentation.DrawRectangle(borderPen, borderOffsetSize.Width, borderOffsetSize.Height, (this.ClientRectangle.Width - (borderOffsetSize.Width * 2) - 1), (this.ClientRectangle.Height - (borderOffsetSize.Height * 2) - 1));
					break;
				}
				case BorderStyle.Fixed3D:
				{
					// TODO: If the control is focused and the BorderStyle property is set to Fixed3D, a dotted 
					// rectangle should be painted around the outside of the content rectangle.

					// TODO: Calculate the light and dark border colors based on the painted background color.

					Color darkBorderColor = SystemColors.ControlDark;
					Color darkestBorderColor = Color.Black;
					Color lightBorderColor = SystemColors.Control;
					Color lightestBorderColor = SystemColors.ControlLightLight;
					Pen borderPenFixed3D = new Pen(darkBorderColor);

					// Paint the border for the control with a three-dimensional appearance. The border 
					// will look different depending on whether the control is in an active or inactive 
					// state.
					if (!this.active)
					{
						// Paint the dark lines.
						presentation.DrawLine(borderPenFixed3D, (this.ClientRectangle.Width - borderOffsetSize.Width - 2), (borderOffsetSize.Height + 1), (this.ClientRectangle.Width - borderOffsetSize.Width - 2), (this.ClientRectangle.Height - borderOffsetSize.Height - 2));
						presentation.DrawLine(borderPenFixed3D, (borderOffsetSize.Width + 1), (this.ClientRectangle.Height - borderOffsetSize.Height - 2), (this.ClientRectangle.Width - borderOffsetSize.Width - 2), (this.ClientRectangle.Height - borderOffsetSize.Height - 2));
						// Paint the darkest lines.
						borderPenFixed3D.Color = darkestBorderColor;
						presentation.DrawLine(borderPenFixed3D, (this.ClientRectangle.Width - borderOffsetSize.Width - 1), borderOffsetSize.Height, (this.ClientRectangle.Width - borderOffsetSize.Width - 1), (this.ClientRectangle.Height - borderOffsetSize.Height - 1));
						presentation.DrawLine(borderPenFixed3D, borderOffsetSize.Width, (this.ClientRectangle.Height - borderOffsetSize.Height - 1), (this.ClientRectangle.Width - borderOffsetSize.Width - 1), (this.ClientRectangle.Height - borderOffsetSize.Height - 1));
						// Paint the light lines.
						borderPenFixed3D.Color = lightBorderColor;
						presentation.DrawLine(borderPenFixed3D, (borderOffsetSize.Width + 1), (borderOffsetSize.Height + 1), (this.ClientRectangle.Width - borderOffsetSize.Width - 3), (borderOffsetSize.Height + 1));
						presentation.DrawLine(borderPenFixed3D, (borderOffsetSize.Width + 1), (borderOffsetSize.Height + 1), (borderOffsetSize.Width + 1), (this.ClientRectangle.Height - borderOffsetSize.Height - 3));
						// Paint the lightest lines.
						borderPenFixed3D.Color = lightestBorderColor;
						presentation.DrawLine(borderPenFixed3D, borderOffsetSize.Width, borderOffsetSize.Height, (this.ClientRectangle.Width - borderOffsetSize.Width - 2), borderOffsetSize.Height);
						presentation.DrawLine(borderPenFixed3D, borderOffsetSize.Width, borderOffsetSize.Height, borderOffsetSize.Width, (this.ClientRectangle.Height - borderOffsetSize.Height - 2));
					}
					else
					{
						presentation.DrawRectangle(borderPenFixed3D, borderOffsetSize.Width, borderOffsetSize.Height, (this.ClientRectangle.Width - (borderOffsetSize.Width * 2) - 1), (this.ClientRectangle.Height - (borderOffsetSize.Height * 2) - 1));
					}

					// Release any resources.
					borderPenFixed3D.Dispose();

					break;
				}
			}

			// If the control is focused or specified as the default, paint a focus border.
			if (paintFocusBorder)
			{
				presentation.DrawRectangle(borderPen, 0, 0, (this.ClientRectangle.Width - 1), (this.ClientRectangle.Height - 1));
			}

			// Release any resources.
			borderPen.Dispose();

			#endregion ===============================================================================

			#endregion ===============================================================================

			// Indicate that this class is done updating the presentation. If the sequential paint token 
			// for this class was the last one registered, then the Paint event will be triggered, and, 
			// if the presentation was double buffered, the contents of the buffer will be drawn to the 
			// control.
			base.NotifyPaintingComplete(this.SequentialPaintingToken, e);
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

		/// <summary>
		/// Determines the location at which the content should be displayed, on the control, based on the indicated alignment value.
		/// </summary>
		/// <param name="alignment">The <see cref="T:OpenNETCF.Drawing.ContentAlignment"/> that represents how the content should be positioned on the control.</param>
		/// <param name="contentSize">The <see cref="T:System.Drawing.SizeF"/> containing the width and height of the content to display on the control.</param>
		/// <param name="clipRect">The <see cref="T:System.Drawing.Rectangle"/> that represents the allotted area in which the content may be displayed.</param>
		/// <returns>A <see cref="T:System.Drawing.Point"/> containing the x and y coordinates at which the content should be displayed.</returns>
		private Point GetLocationFromContentAlignment(OpenNETCF.Drawing.ContentAlignment alignment, SizeF contentSize, Rectangle clipRect)
		{
			byte x = 0;
			Point point = Point.Empty;

			// Vertical Alignment
			if ((((int)alignment) & 0xF00) > 0)			// Bottom
			{
				point.Y = (int)(clipRect.Height - contentSize.Height);
				x = (byte)((int)alignment >> 8);
			}
			else if ((((int)alignment) & 0x0F0) > 0)	// Middle
			{
				point.Y = (int)(((float)clipRect.Height / 2) - (contentSize.Height / 2));
				x = (byte)((int)alignment >> 4);
			}
			else if ((((int)alignment) & 0x00F) > 0)	// Top
			{
				point.Y = 0;
				x = (byte)((int)alignment);
			}

			// Horizontal Alignment
			if ((x & 0x2) > 0)		// Center
			{
				point.X = (int)(((float)clipRect.Width / 2) - (contentSize.Width / 2));
			}
			else if ((x & 0x1) > 0)	// Left
			{
				point.X = 0;
			}
			else if ((x & 0x4) > 0)	// Right
			{
				point.X = (int)(clipRect.Width - contentSize.Width);
			}

			// Ensure that the x coordinate is greater than or equal to zero.
			if (point.X < 0)
			{
				point.X = 0;
			}

			// Ensure that the y coordinate is greater than or equal to zero.
			if (point.Y < 0)
			{
				point.Y = 0;
			}

			// Offset the x and y coordinates based on the clip rectangle.
			point.X += clipRect.X;
			point.Y += clipRect.Y;

			return point;
		}

		#endregion ==================================================================================
	}
}