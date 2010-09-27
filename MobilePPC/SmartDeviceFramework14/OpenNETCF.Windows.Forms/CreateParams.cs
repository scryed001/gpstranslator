//==========================================================================================
//
//		OpenNETCF.Windows.Forms.CreateParams
//		Copyright (c) 2004, OpenNETCF.org
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

namespace OpenNETCF.Windows.Forms
{
	/// <summary>
	/// Encapsulates the information needed when creating a control.
	/// </summary>
	public class CreateParams
	{
		#region Fields

		private string caption;
		private string className;
		private int classStyle;
		private int exStyle;
		private int height;
		//private object param;
		private IntPtr parent;
		private int width;
		private int x;
		private int y;

		#endregion

		/// <summary>
		/// Initializes a new instance of the CreateParams class.
		/// </summary>
		public CreateParams()
		{
		}

		/// <summary>
		/// Gets or sets the control's initial text.
		/// </summary>
		public string Caption
		{
			get
			{
				return caption;
			}
			set
			{
				caption = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of the Windows class to derive the control from.
		/// </summary>
		public string ClassName
		{
			get
			{
				return className;
			}
			set
			{
				className = value;
			}
		}

		/// <summary>
		/// Gets or sets a bitwise combination of class style values.
		/// </summary>
		public int ClassStyle
		{
			get
			{
				return classStyle;
			}
			set
			{
				classStyle = value;
			}
		}

		/// <summary>
		/// Gets or sets a bitwise combination of extended window style values.
		/// </summary>
		public int ExStyle
		{
			get
			{
				return exStyle;
			}
			set
			{
				exStyle = value;
			}
		}

		/// <summary>
		/// Gets or sets the initial height of the control.
		/// </summary>
		public int Height
		{
			get
			{
				return height;
			}
			set
			{
				height = value;
			}
		}

		/// <summary>
		/// Gets or sets the control's parent.
		/// </summary>
		public IntPtr Parent
		{
			get
			{
				return parent;
			}
			set
			{
				parent = value;
			}
		}

		/// <summary>
		/// Gets or sets the initial width of the control.
		/// </summary>
		public int Width
		{
			get
			{
				return width;
			}
			set
			{
				width = value;
			}
		}

		/// <summary>
		/// Gets or sets the initial left position of the control.
		/// </summary>
		public int X
		{
			get
			{
				return x;
			}
			set
			{
				x = value;
			}
		}

		/// <summary>
		/// Gets or sets the initial top position of the control.
		/// </summary>
		public int Y
		{
			get
			{
				return y;
			}
			set
			{
				y = value;
			}
		}
	}
}
