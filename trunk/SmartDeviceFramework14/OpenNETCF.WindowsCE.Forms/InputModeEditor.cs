//==========================================================================================
//
//		OpenNETCF.WindowsCE.Forms.InputModeEditor
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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using OpenNETCF.Win32;
using OpenNETCF.Runtime.InteropServices;

namespace OpenNETCF.WindowsCE.Forms
{
	/// <summary>
	/// Provides access to Smartphone input methods for entering text.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	public class InputModeEditor
	{
		private InputModeEditor(){}

		//messages
		private const int EM_SETINPUTMODE = 0x00DE;
		private const int LB_SETINPUTMODE = 0x01C1;

		private const int EM_SETSYMBOLS = 0x00DF;

		/// <summary>
		/// Specifies the input mode on a Smartphone.
		/// </summary>
		public static void SetInputMode(Control control, InputMode mode)
		{
			int msg;

			//only certain controls are supported
			if(control is TextBox)
			{
				msg = EM_SETINPUTMODE; 
			}
			else if(control is ComboBox)
			{
				msg = LB_SETINPUTMODE;
			}
			else
			{
				return;
			}

			//get the handle to the child (native) control
			control.Capture = true;
			IntPtr hwnd = Win32Window.GetWindow(Win32Window.GetCapture(), GW.CHILD);
			control.Capture = false;			

			//send message
			IntPtr result = Win32Window.SendMessage(hwnd, msg, IntPtr.Zero, (int)mode);
		}

		/// <summary>
		/// Sets the symbols to use with the control on Smartphone.
		/// </summary>
		/// <param name="control">Control for which Symbol collection is to be used.</param>
		/// <param name="symbols">Collection of characters to be used as symbols.</param>
		public void SetSymbols(Control control, string symbols)
		{
			if(control is TextBox)
			{
				//get the handle to the child (native) control
				control.Capture = true;
				IntPtr hwnd = Win32Window.GetWindow(Win32Window.GetCapture(), GW.CHILD);
				control.Capture = false;

				IntPtr pSymbols = MarshalEx.StringToHGlobalUni(symbols);
				IntPtr result = Win32Window.SendMessage(hwnd, EM_SETSYMBOLS, IntPtr.Zero, pSymbols.ToInt32());
				MarshalEx.FreeHGlobal(pSymbols);
			}
		}

		[DllImport("aygshell.dll", SetLastError=true)]
		private static extern int SHSetInputContext(IntPtr hwnd, int dwFeature, IntPtr lpValue );
	}
}
