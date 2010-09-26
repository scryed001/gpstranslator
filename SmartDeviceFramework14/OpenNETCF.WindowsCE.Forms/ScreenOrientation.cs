//==========================================================================================
//
//		OpenNETCF.WindowsCE.Forms.ScreenOrientation
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

// Tested with PPC2003SE Emulator 27/08/2004 Peter Foot 

using System;

namespace OpenNETCF.WindowsCE.Forms
{
	/// <summary>
	/// Specifies the angle of the device screen that can be accessed by the <see cref="SystemSettings.ScreenOrientation"/> property.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	/// <remarks>The default portrait orientation is at angle 0.
	/// Requires Pocket PC 2003 Second Edition.</remarks>
	public enum ScreenOrientation : short
	{
		/// <summary>
		/// Specifies a portrait orientation at 0 degrees.
		/// </summary>
		Angle0	= 0,
		/// <summary>
		/// Specifies a landscape orientation at 90 degrees.
		/// </summary>
		Angle90	= 1,
		/// <summary>
		/// Specifies an orientation at 180 degrees.
		/// </summary>
		Angle180 = 2,
		/// <summary>
		/// Specifies an orientation at 270 degrees.
		/// </summary>
		Angle270 = 4,
	}
}
