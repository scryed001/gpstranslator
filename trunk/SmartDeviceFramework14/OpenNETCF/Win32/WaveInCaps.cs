//==========================================================================================
//
//		OpenNETCF.Multimedia.Audio.WaveInCaps
//		Copyright (c) 2005, OpenNETCF.org
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
using System.Runtime.InteropServices;
using System.Text;

namespace OpenNETCF.Win32
{
	/// <summary>
	/// Class for getting audio device capabilities.
	/// </summary>
	public class WaveInCaps
	{
		private const int MAXPNAMELEN = 32;
			
		private const int wMIDOffset			= 0;
		private const int wPIDOffset			= wMIDOffset + 2;
		private const int vDriverVersionOffset	= wPIDOffset + 2;
		private const int szPnameOffset			= vDriverVersionOffset + 4;
		private const int dwFormatsOffset		= szPnameOffset + MAXPNAMELEN * 2;
		private const int wChannelsOffset		= dwFormatsOffset + 4;
		private const int wReserved1Offset		= wChannelsOffset + 2;

		private byte[] flatStruct = new byte[2 + 2 + 4 + MAXPNAMELEN * 2 + 4 + 2 + 2];

		public byte[] ToByteArray()
		{
			return flatStruct;
		}

		public static implicit operator byte[]( WaveInCaps wic )
		{
			return wic.flatStruct;
		}

		public WaveInCaps()
		{
			Array.Clear(flatStruct, 0, flatStruct.Length);
		}

		public WaveInCaps( byte[] bytes ) : this( bytes, 0 )
		{
		}

		public WaveInCaps( byte[] bytes, int offset )
		{
			Buffer.BlockCopy( bytes, offset, flatStruct, 0, flatStruct.Length );
		}
			
		public short MID
		{
			get
			{
				return BitConverter.ToInt16(flatStruct, wMIDOffset);
			}
			set
			{
				byte[]	bytes = BitConverter.GetBytes( value );
				Buffer.BlockCopy( bytes, 0, flatStruct, wMIDOffset, Marshal.SizeOf(value));
			}
		}

		public short PID
		{
			get
			{
				return BitConverter.ToInt16(flatStruct, wPIDOffset);
			}
			set
			{
				byte[]	bytes = BitConverter.GetBytes( value );
				Buffer.BlockCopy( bytes, 0, flatStruct, wPIDOffset, Marshal.SizeOf(value));
			}
		}

		public int DriverVersion
		{
			get
			{
				return BitConverter.ToInt32(flatStruct, vDriverVersionOffset);
			}
			set
			{
				byte[]	bytes = BitConverter.GetBytes( value );
				Buffer.BlockCopy( bytes, 0, flatStruct, vDriverVersionOffset, Marshal.SizeOf(value));
			}
		}

		public string szPname
		{
			get
			{
				return Encoding.Unicode.GetString(flatStruct, szPnameOffset, MAXPNAMELEN * 2).Trim('\0');
			}
		}
		
		public int Formats
		{
			get
			{
				return BitConverter.ToInt32(flatStruct, dwFormatsOffset);
			}
			set
			{
				byte[]	bytes = BitConverter.GetBytes( value );
				Buffer.BlockCopy( bytes, 0, flatStruct, dwFormatsOffset, Marshal.SizeOf(value));
			}
		}

		public short Channels
		{
			get
			{
				return BitConverter.ToInt16(flatStruct, wChannelsOffset);
			}
			set
			{
				byte[]	bytes = BitConverter.GetBytes( value );
				Buffer.BlockCopy( bytes, 0, flatStruct, wChannelsOffset, Marshal.SizeOf(value));
			}
		}

		public short wReserved1
		{
			get
			{
				return BitConverter.ToInt16(flatStruct, wReserved1Offset);
			}
			set
			{
				byte[]	bytes = BitConverter.GetBytes( value );
				Buffer.BlockCopy( bytes, 0, flatStruct, wReserved1Offset, Marshal.SizeOf(value));
			}
		}
	}
}
