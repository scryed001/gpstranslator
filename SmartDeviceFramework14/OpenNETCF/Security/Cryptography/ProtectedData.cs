//==========================================================================================
//
//		OpenNETCF.Security.Cryptography.ProtectedData
//		Copyright (c) 2003-2004, OpenNETCF.org
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

namespace OpenNETCF.Security.Cryptography
{
	/// <summary>
	/// Specifies the scope of the data protection to be applied by 
	/// the <see cref="OpenNETCF.Security.Cryptography.ProtectedData.Protect"/> method
	/// </summary>
	/// <remarks>
	/// This enumeration is used with the <see cref="OpenNETCF.Security.Cryptography.ProtectedData.Protect"/> and <see cref="OpenNETCF.Security.Cryptography.ProtectedData.Unprotect"/> methods to protect 
	/// data through encryption.
	/// </remarks>
	public enum DataProtectionScope : int
	{
		/// <summary>
		/// Specifies that the protected data is associated with the current user. 
		/// Only threads running under the current user context can unprotect the data.
		/// </summary>
		CurrentUser = 0,
		/// <summary>
		/// Specifies that the protected data is associated with the machine context. 
		/// Any process running under Admin credentials on the computer can unprotect data. 
		/// </summary>
		LocalMachine = 1,
	}

	/// <summary>
	/// Contains methods for protecting and unprotecting data. This class cannot be inherited.
	/// </summary>
	/// <remarks>
	/// The class consists of two wrappers for the unmanaged Data Protection API (DPAPI) methods, 
	/// <see cref="Protect"/> and <see cref="Unprotect"/>. 
	/// These two methods can be used to protect and unprotect data such as passwords, keys, 
	/// and connection strings.
	/// </remarks>
	public sealed class ProtectedData
	{
		private const int CRYPTPROTECT_LOCAL_MACHINE = 0x4;

		/// <summary>
		/// Protects the <c>userData</c> parameter and returns a byte array.
		/// </summary>
		/// <param name="userData">Byte array containing data to be protected.</param>
		/// <param name="optionalEntropy">Additional byte array used to encrypt the data.</param>
		/// <param name="scope">Value from the <see cref="DataProtectionScope"/> enumeration.</param>
		/// <returns>A byte array representing the encrypted data.</returns>
		/// <remarks>
		/// This method can be used to protect data such as passwords, keys, or connection strings. 
		/// The <c>optionalEntropy</c> parameter enables you to use additional information to protect the data.
		/// This information must also be used when unprotecting the data using the <see cref="Unprotect"/> method.
		/// </remarks>
		public static byte[] Protect(byte[] userData, byte[] optionalEntropy, DataProtectionScope scope)
		{
			byte[] buffer = null;

			CRYPTOAPI_BLOB protectedBlob = new CRYPTOAPI_BLOB();
			GCHandle gchUserData = new GCHandle();
			GCHandle gchEntropy = new GCHandle();

			if(userData == null)
			{
				throw new ArgumentNullException("userData");
			}

			try
			{
				gchUserData = GCHandle.Alloc(userData, GCHandleType.Pinned);
				CRYPTOAPI_BLOB userDataBlob = new CRYPTOAPI_BLOB();
				userDataBlob.cbData = (uint)userData.Length;
				
				userDataBlob.pbData = new IntPtr((int)gchUserData.AddrOfPinnedObject() + 4);

				CRYPTOAPI_BLOB entropyBlob = new CRYPTOAPI_BLOB();
				if(optionalEntropy != null)
				{
					gchEntropy = GCHandle.Alloc(optionalEntropy, GCHandleType.Pinned);
					entropyBlob.cbData = (uint)optionalEntropy.Length;
					entropyBlob.pbData = gchEntropy.AddrOfPinnedObject();
				}

				uint flags = 1;
				if(scope == DataProtectionScope.LocalMachine)
				{
					flags = (uint)(flags | CRYPTPROTECT_LOCAL_MACHINE);
				}

				if(!CryptProtectData(ref userDataBlob,string.Empty, ref entropyBlob, IntPtr.Zero,IntPtr.Zero,flags,ref protectedBlob) )
				{
					throw new CryptographicException(Marshal.GetLastWin32Error());
				}
				if(protectedBlob.pbData == IntPtr.Zero)
				{
					throw new OutOfMemoryException();
				}
				
				buffer = new byte[protectedBlob.cbData];
				Marshal.Copy(protectedBlob.pbData, buffer, 0, buffer.Length);
			}	
			finally
			{
				if(gchUserData.IsAllocated)
				{
					gchUserData.Free();
				}

				if(gchEntropy.IsAllocated)
				{
					gchEntropy.Free();
				}

				if(protectedBlob.pbData != IntPtr.Zero)
				{
					ZeroMemory(protectedBlob.pbData, protectedBlob.cbData);
					LocalFree(protectedBlob.pbData);
				}
			}

			return buffer;
		}

		/// <summary>
		/// Unprotects the <c>encryptedData</c> parameter and returns a byte array.
		/// </summary>
		/// <param name="encryptedData">Byte array containing data encrypted with the <see cref="Protect"/> method</param>
		/// <param name="optionalEntropy">Additional byte array that was used to encrypt the data.</param>
		/// <param name="scope">Value from the <see cref="DataProtectionScope"/> enumeration</param>
		/// <returns>A byte array representing the unprotected data.</returns>
		/// <remarks>
		/// This method can be used to unprotect data that was encrypted using the Protect method. 
		/// The <c>optionalEntropy</c> parameter, if used during encryption, must be supplied to unencrypt the data.
		/// </remarks>
		public static byte[] Unprotect(byte[] encryptedData, byte[] optionalEntropy, DataProtectionScope scope)
		{
			byte[] buffer = null;

			CRYPTOAPI_BLOB unprotectedBlob = new CRYPTOAPI_BLOB();
			GCHandle gchProtectedData = new GCHandle();
			GCHandle gchEntropy = new GCHandle();

			try
			{
				gchProtectedData = GCHandle.Alloc(encryptedData, GCHandleType.Pinned);
				CRYPTOAPI_BLOB protectedBlob = new CRYPTOAPI_BLOB();
				protectedBlob.cbData = (uint)encryptedData.Length;
				
				protectedBlob.pbData = new IntPtr((int)gchProtectedData.AddrOfPinnedObject()+4);
				
				CRYPTOAPI_BLOB entropyBlob = new CRYPTOAPI_BLOB();
				if(optionalEntropy != null)
				{
					gchEntropy = GCHandle.Alloc(optionalEntropy, GCHandleType.Pinned);
					entropyBlob.cbData = (uint)optionalEntropy.Length;
					entropyBlob.pbData = gchEntropy.AddrOfPinnedObject();
				}

				uint flags = 1;
				if(scope == DataProtectionScope.LocalMachine)
				{
					flags = (uint)(flags | CRYPTPROTECT_LOCAL_MACHINE);
				}

				if(!CryptUnprotectData(ref protectedBlob,IntPtr.Zero,ref entropyBlob,IntPtr.Zero,IntPtr.Zero,flags,ref unprotectedBlob))
				{
					throw new CryptographicException(Marshal.GetLastWin32Error());
				}
				if(unprotectedBlob.pbData == IntPtr.Zero)
				{
					throw new OutOfMemoryException();
				}

				buffer = new byte[unprotectedBlob.cbData];
				Marshal.Copy(unprotectedBlob.pbData, buffer, 0, buffer.Length);
			}
			finally
			{
				if(gchProtectedData.IsAllocated)
				{
					gchProtectedData.Free();
				}

				if(gchEntropy.IsAllocated)
				{
					gchEntropy.Free();
				}

				if(unprotectedBlob.pbData != IntPtr.Zero)
				{
					ZeroMemory(unprotectedBlob.pbData,unprotectedBlob.cbData);
					LocalFree(unprotectedBlob.pbData);
				}
			}

			return buffer;
		}

		#region Native Methods

		[DllImport("Coredll.dll",SetLastError=true)]
		static extern bool CryptProtectData
			(
			ref CRYPTOAPI_BLOB pDataIn,
			string szDataDescr,
			ref CRYPTOAPI_BLOB pOptionalEntropy,
			IntPtr pvReserved,
			IntPtr pPromptStruct,
			uint dwFlags,
			ref CRYPTOAPI_BLOB pDataOut
			);

		[DllImport("Coredll.dll",SetLastError=true)]
		static extern bool CryptUnprotectData
			(
			ref CRYPTOAPI_BLOB pDataIn,
			IntPtr ppszDataDescr,
			ref CRYPTOAPI_BLOB Entropy,
			IntPtr pvReserved,
			IntPtr pPromptStruct,
			uint dwFlags,
			ref CRYPTOAPI_BLOB pDataOut
			);

		[DllImport("Coredll.dll")]
		static extern void memset(IntPtr pbData, uint offset, uint cbData);

		[DllImport("Coredll.dll")]
		static extern void LocalFree(IntPtr pbData);

		static void ZeroMemory(IntPtr pbData, uint cbData)
		{
			memset(pbData,0,cbData);
		}

		#endregion
		
		struct CRYPTOAPI_BLOB
		{
			public uint cbData;
			public IntPtr pbData;
		}
	}
}
