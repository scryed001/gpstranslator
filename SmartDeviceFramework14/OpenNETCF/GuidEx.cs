//==========================================================================================
//
//		OpenNETCF.GuidEx
//		Copyright (C) 2003-2005, OpenNETCF.org
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
//		!!! A HUGE thank-you goes out to Casey Chesnut for supplying parts of this code !!!
//      !!! You can contact Casey at http://www.brains-n-brawn.com   
//
//==========================================================================================

using System;
using System.Runtime.InteropServices;
using OpenNETCF.Security.Cryptography;

// New for v1.3 - "The Guid to end all Guids" - Peter Foot

namespace OpenNETCF
{
	/// <summary>
	/// Helper class for generating a globally unique identifier (GUID).
	/// <para><b>Revised in v1.3</b></para>
	/// </summary>
	/// <seealso cref="System.Guid"/>
	public sealed class GuidEx
	{
		public static Guid Empty = Guid.Empty;

		private Guid _guid; 

		private GuidEx(){}

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the System.Guid class using the value represented by the 
		/// specified string.
		/// </summary>
		/// <param name="guid">A System.String that contains a GUID in one of the following formats 
		/// ('d' represents a hexadecimal digit whose case is ignored): 32 contiguous digits: 
		/// dddddddddddddddddddddddddddddddd  -or- Groups of 8, 4, 4, 4, and 12 digits with hyphens 
		/// between the groups. The entire GUID can optionally be enclosed in matching braces or 
		/// parentheses: dddddddd-dddd-dddd-dddd-dddddddddddd -or- {dddddddd-dddd-dddd-dddd-dddddddddddd}
		/// -or- (dddddddd-dddd-dddd-dddd-dddddddddddd)  -or- Groups of 8, 4, and 4 digits, and a 
		/// subset of eight groups of 2 digits, with each group prefixed by "0x" or "0X", and 
		/// separated by commas. The entire GUID, as well as the subset, is enclosed in matching 
		/// braces: {0xdddddddd, 0xdddd, 0xdddd,{0xdd,0xdd,0xdd,0xdd,0xdd,0xdd,0xdd,0xdd}}  All
		/// braces, commas, and "0x" prefixes are required. All embedded spaces are ignored. 
		/// All leading zeroes in a group are ignored.   The digits shown in a group are the 
		/// maximum number of meaningful digits that can appear in that group. You can specify 
		/// from 1 to the number of digits shown for a group. The specified digits are assumed 
		/// to be the low order digits of the group. If you specify more digits for a group than 
		/// shown, the high-order digits are ignored.</param>
		public GuidEx(string guid)
		{
			_guid = new Guid(guid);
		}

		/// <summary>
		/// Initializes a new instance of the System.Guid class using the specified integers and bytes.  
		/// </summary>
		/// <param name="a">The first 4 bytes of the GUID.</param>
		/// <param name="b">The next 2 bytes of the GUID.</param>
		/// <param name="c">The next 2 bytes of the GUID.</param>
		/// <param name="d">The next byte of the GUID.</param>
		/// <param name="e">The next byte of the GUID.</param>
		/// <param name="f">The next byte of the GUID.</param>
		/// <param name="g">The next byte of the GUID.</param>
		/// <param name="h">The next byte of the GUID.</param>
		/// <param name="i">The next byte of the GUID.</param>
		/// <param name="j">The next byte of the GUID.</param>
		/// <param name="k">The next byte of the GUID.</param>
		public GuidEx( System.Int32 a , System.Int16 b , System.Int16 c , System.Byte d , System.Byte e , System.Byte f , System.Byte g , System.Byte h , System.Byte i , System.Byte j , System.Byte k)
		{
			_guid = new Guid(a, b, c, d, e, f, g, h, i, j, k);
		}

		/// <summary>
		/// Initializes a new instance of the System.Guid class using the specified integers and byte array. 
		/// </summary>
		/// <param name="a">The first 4 bytes of the GUID</param>
		/// <param name="b">The next 2 bytes of the GUID.</param>
		/// <param name="c">The next 2 bytes of the GUID.</param>
		/// <param name="d">The remaining 8 bytes of the GUID.</param>
		public GuidEx(System.Int32 a , System.Int16 b , System.Int16 c , byte[] d)
		{
			_guid = new Guid(a, b, c, d);
		}

		/// <summary>
		/// Initializes a new instance of the GuidEx class using the specified array of bytes. 
		/// </summary>
		/// <param name="b">A 16 element byte array containing values with which to initialize the GUID.</param>
		public GuidEx(byte[] b)
		{
			_guid = new Guid(b);
		}

		#endregion

        #region New Guid
		/// <summary>
		/// Initializes a new instance of the <see cref="System.Guid"/> class.
		/// </summary>
		/// <returns>A new <see cref="System.Guid"/> object.</returns>
		/// <remarks>On CE.NET 4.2 and higher this method uses the CoCreateGuid API call.
		/// On earlier versions it uses the OpenNETCF.Security.Cryptography classes to generate a random Guid.</remarks>
		public static System.Guid NewGuid()
		{
			//cocreateguid supported on CE.NET 4.2 and above
			if(System.Environment.OSVersion.Version > new Version(4,1))
			{
				return NewOleGuid();
			}
			else
			{
				//check if target has crypto API support
				if(OpenNETCF.Security.Cryptography.NativeMethods.Context.IsCryptoApi)
				{
					return NewCryptoGuid();
				}
				else
				{
					//if not use random generator
					return NewRandomGuid();
				}
			}
		}
		#endregion

		#region Constants
		// constants that are used in the class
		private class Const
		{
			// guid variant types
			public enum GuidVariant
			{
				ReservedNCS = 0x00,
				Standard = 0x02,
				ReservedMicrosoft = 0x06,
				ReservedFuture = 0x07
			}

			// guid version types
			public enum GuidVersion
			{
				TimeBased = 0x01,
				Reserved = 0x02,
				NameBased = 0x03,
				Random = 0x04
			}

			// multiplex variant info
			public const int VariantByte = 8;
			public const int VariantByteMask = 0x3f;
			public const int VariantByteShift = 6;

			// multiplex version info
			public const int VersionByte = 7;
			public const int VersionByteMask = 0x0f;
			public const int VersionByteShift = 4;
		}
		#endregion

		#region Crypto
		/// <summary>
		/// Create a new Random Guid using Crypto APIs
		/// </summary>
		/// <returns></returns>
		public static System.Guid NewCryptoGuid()
		{
			//create guid manually
			byte[] guidbytes = new byte[16];

			//use crypto apis to generate random bytes
			OpenNETCF.Security.Cryptography.RNGCryptoServiceProvider rng = new OpenNETCF.Security.Cryptography.RNGCryptoServiceProvider();
			rng.GetBytes(guidbytes);
			
			//set version etc	
			MakeValidRandomGuid(guidbytes);

			// create the new System.Guid object
			return new System.Guid(guidbytes);
		}
		#endregion

		#region Random
		/// <summary>
		/// Create a new Random Guid (For platforms without Crypto support).
		/// </summary>
		/// <returns></returns>
		public static System.Guid NewRandomGuid()
		{
			byte[] guidbytes = OpenNETCF.Security.Cryptography.NativeMethods.Rand.CeGenRandom(16);

			//set version etc
			MakeValidRandomGuid(guidbytes);
			
         
			// create the new System.Guid object
			return new System.Guid(guidbytes);
		}
		#endregion

		#region Helper Methods
		private static void MakeValidRandomGuid(byte[] guidbytes)
		{
			// set the variant
			guidbytes[Const.VariantByte] &= Const.VariantByteMask;
			guidbytes[Const.VariantByte] |= 
				((int)Const.GuidVariant.Standard << Const.VariantByteShift);

			// set the version
			guidbytes[Const.VersionByte] &= Const.VersionByteMask;
			guidbytes[Const.VersionByte] |= 
				((int)Const.GuidVersion.Random << Const.VersionByteShift);
		}
		#endregion

		#region Ticks
		/// <summary>
		/// Create a new <see cref="Guid"/> only using TickCount and bit shifting.
		/// </summary>
		public static System.Guid NewTicksGuid()
		{
			// Create a unique GUID
			long fileTime = DateTime.Now.ToUniversalTime().ToFileTime();
			UInt32 high32 = (UInt32)(fileTime >> 32)+0x146BF4;
			int tick = System.Environment.TickCount;
			byte[] guidBytes = new byte[8];

			// load the byte array with random bits
			Random rand = new Random((int)fileTime);
			rand.NextBytes(guidBytes);

			// use tick info in the middle of the array
			guidBytes[2] = (byte)(tick >> 24);
			guidBytes[3] = (byte)(tick >> 16);
			guidBytes[4] = (byte)(tick >> 8);
			guidBytes[5] = (byte)tick;

			// Construct a Guid with our data
			System.Guid guid = new System.Guid((int)fileTime, (short)high32, (short)((high32 | 0x10000000) >> 16), guidBytes);
			return guid;
		}
		#endregion

		#region Ole
		/// <summary>
		/// Create a new <see cref="Guid"/> using COM APIs
		/// </summary>
		/// <returns></returns>
		/// <remarks>Requires Windows CE.NET 4.1 or higher.</remarks>
		public static System.Guid NewOleGuid()
		{
			System.Guid val = System.Guid.Empty;

			int hresult = 0;
			hresult = CoCreateGuid(ref val);

			if(hresult != 0)
			{
				throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error(), "Error creating new Guid");
			}

			return val;
		}
		[DllImport("ole32.dll", SetLastError=true)]
		private static extern int CoCreateGuid(ref System.Guid pguid);
		#endregion

		#region ToString

		/// <summary>
		/// Returns a String representation of the value of this instance in registry format.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _guid.ToString ();
		}

		/// <summary>
		/// Returns a String representation of the value of this Guid instance, according 
		/// to the provided format specifier. 
		/// </summary>
		/// <param name="format">A single format specifier that indicates how to format 
		/// the value of this System.Guid. The format parameter can be "N", "D", "B", or 
		/// "P". If format is null or the empty string (""), "D" is used.</param>
		/// <returns></returns>
		public string ToString(string format)
		{
			return _guid.ToString(format);
		}

		/// <summary>
		/// Returns a System.String representation of the value of this instance of the 
		/// GuidEx class, according to the provided format specifier and culture-specific 
		/// format information.
		/// </summary>
		/// <param name="format"> A single format specifier that indicates how to format the 
		/// value of this System.Guid. The format parameter can be "N", "D", "B", or "P". 
		/// If format is null or the empty string (""), "D" is used.</param>
		/// <param name="provider">(Reserved) An IFormatProvider reference that supplies 
		/// culture-specific formatting services.</param>
		/// <returns></returns>
		public string ToString(string format, IFormatProvider provider)
		{
			return _guid.ToString(format, provider);
		}

		#endregion

		#region ToGuid

		/// <summary>
		/// Converts a OpenNETCF.GuidEx into a System.Guid for framework compatibility.
		/// </summary>
		/// <returns></returns>
		public Guid ToGuid()
		{
			return _guid;
		}

		#endregion
	}
}

