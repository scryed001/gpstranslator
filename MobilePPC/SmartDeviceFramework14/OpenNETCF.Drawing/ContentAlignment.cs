//==========================================================================================
//
//		OpenNETCF.Drawing.ContentAlignment
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
using System;

namespace OpenNETCF.Drawing
{
	/// <summary>
	/// Specifies alignment of content on the drawing surface.
	/// </summary>
	public enum ContentAlignment : int
	{
		/// <summary>
		/// Content is vertically aligned at the bottom, and horizontally aligned at the center.
		/// </summary>
		BottomCenter	= 0x200,
		/// <summary>
		/// Content is vertically aligned at the bottom, and horizontally aligned on the left.
		/// </summary>
		BottomLeft		= 0x100,
		/// <summary>
		/// Content is vertically aligned at the bottom, and horizontally aligned on the right.
		/// </summary>
		BottomRight		= 0x400,
		/// <summary>
		/// Content is vertically aligned in the middle, and horizontally aligned at the center.
		/// </summary>
		MiddleCenter	= 0x020,
		/// <summary>
		/// Content is vertically aligned in the middle, and horizontally aligned on the left.
		/// </summary>
		MiddleLeft		= 0x010,
		/// <summary>
		/// Content is vertically aligned in the middle, and horizontally aligned on the right.
		/// </summary>
		MiddleRight		= 0x040,
		/// <summary>
		/// Content is vertically aligned at the top, and horizontally aligned at the center.
		/// </summary>
		TopCenter		= 0x002,
		/// <summary>
		/// Content is vertically aligned at the top, and horizontally aligned on the left.
		/// </summary>
		TopLeft			= 0x001,
		/// <summary>
		/// Content is vertically aligned at the top, and horizontally aligned on the right.
		/// </summary>
		TopRight		= 0x004
	}
}
