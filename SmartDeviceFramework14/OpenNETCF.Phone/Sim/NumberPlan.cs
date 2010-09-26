//==========================================================================================
//
//		OpenNETCF.Phone.Sim.NumberPlan
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

namespace OpenNETCF.Phone.Sim
{
	/// <summary>
	/// Specifies the numbering plan used in the <see cref="PhonebookEntry.Address"/>.
	/// </summary>
	public enum NumberPlan : int
	{
		/// <summary>
		/// Unknown numbering.
		/// </summary>
		Unknown             =(0x00000000),
		/// <summary>
		/// ISDN/telephone numbering plan (E.164/E.163)
		/// </summary>
		Telephone           =(0x00000001),
		/// <summary>
		/// Data numbering plan (X.121)
		/// </summary>
		Data                =(0x00000002),
		/// <summary>
		/// Telex numbering plan
		/// </summary>
		Telex               =(0x00000003),
		/// <summary>
		/// National numbering plan
		/// </summary>
		National            =(0x00000004),
		/// <summary>
		/// Private numbering plan
		/// </summary>
		Private             =(0x00000005),
		/// <summary>
		/// ERMES numbering plan (ETSI DE/PS 3 01-3)
		/// </summary>
		Ermes               =(0x00000006),
	}
}
