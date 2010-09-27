//==========================================================================================
//
//		OpenNETCF.WindowsCE.Forms.InputPanelEx
//		Copyright (c) 2004-2005, OpenNETCF.org
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

// 05/12/2004 Peter Foot

using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
#if !NDOC
using Microsoft.WindowsCE.Forms;
#endif


namespace OpenNETCF.WindowsCE.Forms
{
	/// <summary>
	/// Enhances the <see cref="InputPanel"/> class for controlling the soft input panel (SIP) for entering data on Pocket PCs and other Windows CE.NET devices
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	#if NDOC
	public class InputPanelEx
	#else
	public class InputPanelEx : InputPanel
#endif
	{
		private InputMethod mCurrentMethod;
		private InputMethodCollection mMethods;

		/// <summary>
		/// Initializes a new instance of the <see cref="InputPanelEx"/> class for enabling and disabling the soft input panel (SIP).
		/// </summary>
		public InputPanelEx()
		{
			mMethods = new InputMethodCollection();
		}
		

		#region Current InputMethod Property
#if !DESIGN		
		/// <summary>
		/// Gets or sets the input method for the Pocket PC.
		/// <para><b>New in v1.3</b></para>
		/// </summary>

		public InputMethod CurrentInputMethod
		{
			get
			{
				Guid clsid = Guid.Empty;
#if !NDOC
				bool success = SipGetCurrentIM(ref clsid);

				if(!success)
				{
					throw new Win32Exception(Marshal.GetLastWin32Error(), "Error retrieving current SIP");
				}
#endif
				mCurrentMethod = InputMethod.FromClsid(clsid);

				return mCurrentMethod;
			}
			set
			{
				//get guid
				Guid clsid = value.Clsid;
#if !NDOC
				//try set new sip
				bool success = SipSetCurrentIM(ref clsid);

				if(!success)
				{
					throw new Win32Exception(Marshal.GetLastWin32Error(), "Error setting current SIP");
				}
#endif
				//store new value
				mCurrentMethod = value;
			}
		}
		
		[DllImport("coredll.dll", EntryPoint="SipGetCurrentIM", SetLastError=true)]
		private static extern bool SipGetCurrentIM(ref Guid pClsid);

		[DllImport("coredll.dll", EntryPoint="SipSetCurrentIM", SetLastError=true)]
		private static extern bool SipSetCurrentIM(ref Guid pClsid);
#endif
		#endregion


		

		#region Unused
		/*
		
		// Not currently used
		
		[DllImport("aygshell.dll", EntryPoint="SHSipInfo", SetLastError=true)]
		private static extern bool SHSipInfo (
						SPI uiAction, 
						int uiParam, 
						byte[] pvParam, 
						int fWinIni );

		private enum SPI : int
		{
			SETSIPINFO = 224,
			GETSIPINFO = 225,
			SETCURRENTIM = 226,
			GETCURRENTIM = 227,
			SETCOMPLETIONINFO = 223,
			APPBUTTONCHANGE = 228,
			RESERVED = 229,
			SYNCSETTINGSCHANGE = 230,
			SIPMOVE = 250,
		}

		private class SIPINFO
		{
			public int cbSize;
			public int fdwFlags;

			public int rcVisibleDesktopLeft;
			public int rcVisibleDesktopTop;
			public int rcVisibleDesktopRight;
			public int rcVisibleDesktopBottom;

			public int rcSipRectLeft;
			public int rcSipRectTop; 
			public int rcSipRectRight; 
			public int rcSipRectBottom; 

			public int dwImDataSize;
			IntPtr pvImData;

			internal SIPINFO()
			{
				cbSize = 44;
			}
		}

		[Flags()]
		private enum SPIF : int
		{
			OFF	= 0x00000000,
			ON = 0x00000001,
			DOCKED =0x00000002,
			LOCKED = 0x00000004,
			DISABLECOMPLETION = 0x08,
		}
		*/
		#endregion

		#region Input Methods
#if !DESIGN
		/// <summary>
		/// Gets a collection of the input methods available on a Pocket PC.
		/// </summary>
		public InputMethodCollection InputMethods
		{
			get
			{
				return mMethods;
			}
		}
#endif
		#endregion

		#region Input Method Collection
		/// <summary>
		/// Provides access to all input method software installed on a Pocket PC.
		/// </summary>
		public class InputMethodCollection : IList, ICollection, IEnumerable
		{
			private ArrayList methods;

			internal InputMethodCollection()
			{
				methods = new ArrayList();

				//for v1 just add the known methods
				methods.Add(new InputMethod(new Guid("42429667-ae04-11d0-a4f8-00aa00a749b9"), "Keyboard"));
				methods.Add(new InputMethod(new Guid("42429690-ae04-11d0-a4f8-00aa00a749b9"), "LetterRecognizer"));
				methods.Add(new InputMethod(new Guid("42429691-ae04-11d0-a4f8-00aa00a749b9"), "BlockRecognizer"));

			}

			#region IList Members

			/// <summary>
			/// Gets a value that indicates whether the collection is read-only.
			/// </summary>
			public bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			/// <summary>
			/// Gets the item at the specified index.
			/// </summary>
			public InputMethod this[int index]
			{
				get
				{
					return (InputMethod)methods[index];
				}
			}

			object IList.this[int index]
			{
				get
				{
					return this[index];
				}
				set
				{
					// TODO:  Add InputMethodCollection.this setter implementation
				}
			}

			/// <summary>
			/// Removes the <see cref="InputMethod"/> at the specified index.
			/// </summary>
			/// <param name="index"></param>
			public void RemoveAt(int index)
			{
				methods.RemoveAt(index);
			}

			void IList.Insert(int index, object value)
			{
				// TODO:  Add InputMethodCollection.Insert implementation
			}

			void IList.Remove(object value)
			{
				// TODO:  Add InputMethodCollection.Remove implementation
			}
			/// <summary>
			/// Determines whether the <see cref="InputMethodCollection"/> class contains a specified value.
			/// </summary>
			/// <param name="method">An <see cref="InputMethod"/> to locate in the collection.</param>
			/// <returns>true if present, else false.</returns>
			public bool Contains(InputMethod method)
			{
				return methods.Contains(method);
			}

			bool IList.Contains(object value)
			{
				return Contains((InputMethod)value);
			}

			void IList.Clear()
			{
				// TODO:  Add InputMethodCollection.Clear implementation
			}

			/// <summary>
			/// Determines the index of a specific item in the <see cref="InputMethodCollection"/> class.
			/// </summary>
			/// <param name="method">The input method to locate in the collection.</param>
			/// <returns>Index in the array.</returns>
			public int IndexOf(InputMethod method)
			{
				return methods.IndexOf(method);
			}

			int IList.IndexOf(object value)
			{
				return IndexOf((InputMethod)value);
			}

			int IList.Add(object value)
			{
				// TODO:  Add InputMethodCollection.Add implementation
				return 0;
			}

			bool IList.IsFixedSize
			{
				get
				{
					return true;
				}
			}

			#endregion

			#region ICollection Members

			bool ICollection.IsSynchronized
			{
				get
				{
					// TODO:  Add InputMethodCollection.IsSynchronized getter implementation
					return false;
				}
			}

			/// <summary>
			/// Gets the number of elements in the <see cref="InputMethodCollection"/> class.
			/// </summary>
			public int Count
			{
				get
				{
					return methods.Count;
				}
			}

			/// <summary>
			/// Copies the collection values to a one-dimensional <see cref="Array"/> instance at the specified index.
			/// </summary>
			/// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the values copied from <see cref="InputMethodCollection"/>.</param>
			/// <param name="index">The index in the array where copying to begins.</param>
			public void CopyTo(Array array, int index)
			{
				methods.CopyTo(array, index);
			}

			object ICollection.SyncRoot
			{
				get
				{
					return null;
				}
			}

			#endregion

			#region IEnumerable Members

			/// <summary>
			/// Returns an enumerator that can iterate through the <see cref="InputMethodCollection"/> class.
			/// </summary>
			/// <returns></returns>
			public IEnumerator GetEnumerator()
			{
				return methods.GetEnumerator();
			}

			#endregion
		}
		#endregion

	}
}
