//==========================================================================================
//
//		OpenNETCF.Win32.Core
//		Copyright (c) 2003-2005, OpenNETCF.org
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
using System.Reflection;

namespace OpenNETCF.Win32
{
	/// <summary>
	/// OpenNETCF Win32 API Wrapper Class for CoreDLL.dll
	/// </summary>
	/// <remarks>For Audio API calls see <see cref="OpenNETCF.Win32.Wave"/>. For GDI API calls see <see cref="OpenNETCF.Win32.GDI"/> in the OpenNETCF.Drawing.dll assembly.</remarks>
	public sealed class Core
	{
		#region --------------- Library Constants and enums ---------------

		internal const int	INFINITE				= -1;
		/// <summary>
		/// The returned handle is not valid.
		/// </summary>
		public const int	INVALID_HANDLE_VALUE	= -1;

		/// <summary>
		/// The function returns if input exists for the queue, even if the input has been seen (but not removed) using a call to another function, such as PeekMessage.
		/// </summary>
		public const int MWMO_INPUTAVAILABLE	= 0x0004;

		#region Wait Enumeration
		/// <summary>
		/// Responses from <see cref="WaitForSingleObject"/> function.
		/// </summary>
		public enum Wait : int
		{
			/// <summary>
			/// The state of the specified object is signaled.
			/// </summary>
			Object		= 0x00000000,
			/// <summary>
			/// Wait abandoned.
			/// </summary>
			Abandoned	= 0x00000080,
			/// <summary>
			/// Wait failed.
			/// </summary>
			Failed		= -1,
		}
		#endregion

		#region WakeMask
		/// <summary>
		/// Flags to use for <see cref="MsgWaitForMultipleObjects"/> function.
		/// <para><b>New in v1.3</b></para>
		/// </summary>
		[Flags]
		public enum WakeMask: int
		{
			/// <summary>
			/// A WM_KEYUP, WM_KEYDOWN, WM_SYSKEYUP, or WM_SYSKEYDOWN message is in the queue
			/// </summary>
			Key          = 0x0001,
			/// <summary>
			/// A WM_MOUSEMOVE message is in the queue
			/// </summary>
			MouseMove    = 0x0002,
			/// <summary>
			/// A mouse-button message ( WM_LBUTTONUP, WM_RBUTTONDOWN, and so on) is in the queue
			/// </summary>
			MouseButton  = 0x0004,
			/// <summary>
			/// A posted message (other than those just listed) is in the queue
			/// </summary>
			PostMessage  = 0x0008,
			/// <summary>
			/// A WM_TIMER message is in the queue
			/// </summary>
			Timer        = 0x0010,
			/// <summary>
			/// A WM_PAINT message is in the queue
			/// </summary>
			Paint        = 0x0020,
			/// <summary>
			/// A message sent by another thread or application is in the queue
			/// </summary>
			SendMessage  = 0x0040,
            
			/// <summary>
			/// A WM_MOUSEMOVE message or mouse-button message ( WM_LBUTTONUP, WM_RBUTTONDOWN, and so on) is in the queue
			/// </summary>
			Mouse        = MouseMove | MouseButton,
			/// <summary>
			/// An input message is in the queue
			/// </summary>
			Input        = Mouse | Key,
			/// <summary>
			/// An input, WM_TIMER, WM_PAINT, WM_HOTKEY, or posted message is in the queue
			/// </summary>
			AllEvents    = Input | PostMessage | Timer | Paint,
			/// <summary>
			/// Any message is in the queue
			/// </summary>
			AllInput     = Input | PostMessage | Timer | Paint | SendMessage
		}
		#endregion

		#region Format Message Flags Enumeration
		/// <summary>
		/// Specifies aspects of the formatting process and how to interpret the lpSource parameter.
		/// </summary>
		/// <remarks>The low-order byte of dwFlags specifies how the function handles line breaks in the output buffer.
		/// The low-order byte can also specify the maximum width of a formatted output line.</remarks>
		[Flags]
		public enum FormatMessageFlags : int
		{
			/// <summary>
			/// The function allocates a buffer large enough to hold the formatted message, and places a pointer to the allocated buffer at the address specified by lpBuffer.
			/// </summary>
			AllocateBuffer = 0x00000100,
			/// <summary>
			/// Insert sequences in the message definition are to be ignored and passed through to the output buffer unchanged.
			/// </summary>
			IgnoreInserts  = 0x00000200,
			/// <summary>
			/// Specifies that lpSource is a pointer to a null-terminated message definition.
			/// </summary>
			FromString     = 0x00000400,
			/// <summary>
			/// Specifies that lpSource is a module handle containing the message-table resource(s) to search.
			/// </summary>
			FromHModule    = 0x00000800,
			/// <summary>
			/// Specifies that the function should search the system message-table resource(s) for the requested message.
			/// </summary>
			FromSystem     = 0x00001000,
			/// <summary>
			/// Specifies that the Arguments parameter is not a va_list structure, but instead is just a pointer to an array of 32-bit values that represent the arguments.
			/// </summary>
			ArgumentArray  = 0x00002000,
			/// <summary>
			/// Use the <b>MaxWidthMask</b> constant and bitwise Boolean operations to set and retrieve this maximum width value.
			/// </summary>
			MaxWidthMask  = 0x000000FF,
		}
		#endregion

		internal const uint	CLR_INVALID				= 0xFFFFFFFF;
		
		internal const Int32 METHOD_BUFFERED = 0;
		internal const Int32 FILE_ANY_ACCESS = 0;
		internal const Int32 FILE_DEVICE_HAL = 0x00000101;
		internal static Int32 IOCTL_HAL_GET_DEVICEID = ((FILE_DEVICE_HAL) << 16) |
			((FILE_ANY_ACCESS) << 14) | ((21) << 2) | (METHOD_BUFFERED);
    
		/// <summary>
		/// Region type
		/// </summary>
		public enum GDIRegion : int
		{
			/// <summary>
			/// 
			/// </summary>
			NULLREGION = 1,
			/// <summary>
			/// 
			/// </summary>
			SIMPLEREGION = 2,
			/// <summary>
			/// 
			/// </summary>
			COMPLEXREGION = 3
		}

		/// <summary>
		/// Background mode
		/// </summary>
		public enum BackMode : int
		{
			/// <summary>
			/// Background is Transparent.
			/// </summary>
			TRANSPARENT = 1,
			/// <summary>
			/// Background is Opaque.
			/// </summary>
			OPAQUE= 2
		}

		private enum KeyEvents
		{
			ExtendedKey = 0x0001,
			KeyUp       = 0x0002,
			Silent      = 0x0004
		}

		/// <summary>
		/// Specifies an action, or state, that may occur, or should 
		/// occur, in relation to a keyboard key.
		/// </summary>
		public enum KeyActionState : int
		{
			/// <summary>
			/// The key is in the down state.
			/// </summary>
			Down	= 0x01,
			/// <summary>
			/// The key is in the up state.
			/// </summary>
			Up		= 0x02,
			/// <summary>
			/// The key has been pressed down and then released.
			/// </summary>
			Press	= 0x03
		}

		#region Key State Flags
		/// <summary>
		/// KeyStateFlags for Keyboard methods
		/// </summary>
		[Flags()]
		public enum KeyStateFlags : int
		{
			/// <summary>
			/// Key is toggled.
			/// </summary>
			Toggled		= 0x0001,
			/// <summary>
			/// 
			/// </summary>
			AsyncDown	= 0x0002,		//	 went down since last GetAsync call.
			/// <summary>
			/// Key was previously down.
			/// </summary>
			PrevDown	= 0x0040,
			/// <summary>
			/// Key is currently down.
			/// </summary>
			Down		= 0x0080,
			/// <summary>
			/// Left or right CTRL key is down.
			/// </summary>
			AnyCtrl		= 0x40000000,
			/// <summary>
			/// Left or right SHIFT key is down.
			/// </summary>
			AnyShift	= 0x20000000,
			/// <summary>
			/// Left or right ALT key is down.
			/// </summary>
			AnyAlt		= 0x10000000,
			/// <summary>
			/// VK_CAPITAL is toggled.
			/// </summary>
			Capital		= 0x08000000,
			/// <summary>
			/// Left CTRL key is down.
			/// </summary>
			LeftCtrl	= 0x04000000,
			/// <summary>
			/// Left SHIFT key is down.
			/// </summary>
			LeftShift	= 0x02000000,
			/// <summary>
			/// Left ALT key is down.
			/// </summary>
			LeftAlt		= 0x01000000,
			/// <summary>
			/// Left Windows logo key is down.
			/// </summary>
			LeftWin		= 0x00800000,
			/// <summary>
			/// Right CTRL key is down.
			/// </summary>
			RightCtrl	= 0x00400000,
			/// <summary>
			/// Right SHIFT key is down
			/// </summary>
			RightShift	= 0x00200000,
			/// <summary>
			/// Right ALT key is down
			/// </summary>
			RightAlt	= 0x00100000,
			/// <summary>
			/// Right Windows logo key is down.
			/// </summary>
			RightWin	= 0x00080000,
			/// <summary>
			/// Corresponding character is dead character.
			/// </summary>
			Dead		= 0x00020000,
			/// <summary>
			/// No characters in pCharacterBuffer to translate.
			/// </summary>
			NoCharacter	= 0x00010000,
			/// <summary>
			/// Use for language specific shifts.
			/// </summary>
			Language1	= 0x00008000,
			/// <summary>
			/// NumLock toggled state.
			/// </summary>
			NumLock		= 0x00001000,
		}
		#endregion



		#region ProcessorArchitecture
		/// <summary>
		/// Processor Architecture values (GetSystemInfo)
		/// </summary>
		/// <seealso cref="M:OpenNETCF.WinAPI.Core.GetSystemInfo(OpenNETCF.WinAPI.Core.SYSTEM_INFO)"/>
		public enum ProcessorArchitecture : short
		{
			/// <summary>
			/// Processor is Intel x86 based.
			/// </summary>
			Intel	= 0,
			/// <summary>
			/// Processor is MIPS based.
			/// </summary>
			MIPS	= 1,
			/// <summary>
			/// Processor is Alpha based.
			/// </summary>
			Alpha	= 2,
			/// <summary>
			/// Processor is Power PC based.
			/// </summary>
			PPC		= 3,
			/// <summary>
			/// Processor is SH3, SH4 etc.
			/// </summary>
			SHX		= 4,
			/// <summary>
			/// Processor is ARM based.
			/// </summary>
			ARM		= 5,
			/// <summary>
			/// Processor is Intel 64bit.
			/// </summary>
			IA64	= 6,
			/// <summary>
			/// Processor is Alpha 64bit.
			/// </summary>
			Alpha64 = 7,
			/// <summary>
			/// Unknown processor architecture.
			/// </summary>
			Unknown = -1,
		}
		#endregion

		#region Processor Type
		/// <summary>
		/// Processor type values (GetSystemInfo)
		/// </summary>
		/// <seealso cref="M:OpenNETCF.Win32.Core.GetSystemInfo(OpenNETCF.Win32.Core.SYSTEM_INFO)"/>
		public enum ProcessorType : int
		{
			/// <summary>
			/// Processor is Intel 80386.
			/// </summary>
			Intel_386			= 386,
			/// <summary>
			/// Processor is Intel 80486.
			/// </summary>
			Intel_486			= 486,
			/// <summary>
			/// Processor is Intel Pentium (80586).
			/// </summary>
			Intel_Pentium		= 586,
			/// <summary>
			/// Processor is Intel Pentium II (80686).
			/// </summary>
			Intel_PentiumII		= 686,
			/// <summary>
			/// Processor is Intel 64bit (IA64).
			/// </summary>
			Intel_IA64		= 2200,
			/// <summary>
			/// Processor is MIPS R4000.
			/// </summary>
			MIPS_R4000        = 4000,
			/// <summary>
			/// Processor is Alpha 21064.
			/// </summary>
			Alpha_21064       = 21064,
			/// <summary>
			/// Processor is Power PC 403.
			/// </summary>
			PPC_403           = 403,
			/// <summary>
			/// Processor is Power PC 601.
			/// </summary>
			PPC_601           = 601,
			/// <summary>
			/// Processor is Power PC 603.
			/// </summary>
			PPC_603           = 603,
			/// <summary>
			/// Processor is Power PC 604.
			/// </summary>
			PPC_604           = 604,
			/// <summary>
			/// Processor is Power PC 620.
			/// </summary>
			PPC_620           = 620,
			/// <summary>
			/// Processor is Hitachi SH3.
			/// </summary>
			Hitachi_SH3       = 10003,
			/// <summary>
			/// Processor is Hitachi SH3E.
			/// </summary>
			Hitachi_SH3E      = 10004,
			/// <summary>
			/// Processor is Hitachi SH4.
			/// </summary>
			Hitachi_SH4       = 10005,
			/// <summary>
			/// Processor is Motorola 821.
			/// </summary>
			Motorola_821      = 821,
			/// <summary>
			/// Processor is SH3.
			/// </summary>
			SHx_SH3           = 103,
			/// <summary>
			/// Processor is SH4.
			/// </summary>
			SHx_SH4           = 104,
			/// <summary>
			/// Processor is StrongARM.
			/// </summary>
			StrongARM         = 2577,
			/// <summary>
			/// Processor is ARM 720.
			/// </summary>
			ARM720            = 1824,
			/// <summary>
			/// Processor is ARM 820.
			/// </summary>
			ARM820            = 2080,
			/// <summary>
			/// Processor is ARM 920.
			/// </summary>
			ARM920            = 2336,
			/// <summary>
			/// Processor is ARM 7 TDMI.
			/// </summary>
			ARM_7TDMI         = 70001
		}
		#endregion

		#region SystemInfo
		/// <summary>
		/// This structure contains information about the current computer system. This includes the processor type, page size, memory addresses, and OEM identifier.
		/// </summary>
		/// <seealso cref="GetSystemInfo"/>
		public struct SystemInfo
		{
			/// <summary>
			/// The system's processor architecture.
			/// </summary>
			public ProcessorArchitecture ProcessorArchitecture;

			internal ushort wReserved;
			/// <summary>
			/// The page size and the granularity of page protection and commitment.
			/// </summary>
			public int PageSize;
			/// <summary>
			/// Pointer to the lowest memory address accessible to applications and dynamic-link libraries (DLLs). 
			/// </summary>
			public int MinimumApplicationAddress;
			/// <summary>
			/// Pointer to the highest memory address accessible to applications and DLLs.
			/// </summary>
			public int MaximumApplicationAddress;
			/// <summary>
			/// Specifies a mask representing the set of processors configured into the system. Bit 0 is processor 0; bit 31 is processor 31. 
			/// </summary>
			public int ActiveProcessorMask;
			/// <summary>
			/// Specifies the number of processors in the system.
			/// </summary>
			public int NumberOfProcessors;
			/// <summary>
			/// Specifies the type of processor in the system.
			/// </summary>
			public ProcessorType ProcessorType;
			/// <summary>
			/// Specifies the granularity with which virtual memory is allocated.
			/// </summary>
			public int AllocationGranularity;
			/// <summary>
			/// Specifies the system’s architecture-dependent processor level.
			/// </summary>
			public short ProcessorLevel;
			/// <summary>
			/// Specifies an architecture-dependent processor revision.
			/// </summary>
			public short ProcessorRevision;
		}
		#endregion

		#region SystemParametersInfoAction Enumeration
		/// <summary>
		/// Specifies the system-wide parameter to query or set.
		/// </summary>
		public enum SystemParametersInfoAction : int
		{
			/// <summary>
			/// Retrieves the two mouse threshold values and the mouse speed.
			/// </summary>
			GetMouse = 3,
			/// <summary>
			/// Sets the two mouse threshold values and the mouse speed.
			/// </summary>
			SetMouse = 4,

			/// <summary>
			/// For Windows CE 2.12 and later, sets the desktop wallpaper.
			/// </summary>
			SetDeskWallpaper = 20,
			/// <summary>
			/// 
			/// </summary>
			SetDeskPattern = 21,

			/// <summary>
			/// Sets the size of the work area — the portion of the screen not obscured by the system taskbar or by toolbars displayed on the desktop by applications.
			/// </summary>
			SetWorkArea = 47,
			/// <summary>
			/// Retrieves the size of the work area on the primary screen.
			/// </summary>
			GetWorkArea = 48,

			/// <summary>
			/// Retrieves whether the show sounds option is on or off.
			/// </summary>
			GetShowSounds = 56,
			/// <summary>
			/// Turns the show sounds accessibility option on or off.
			/// </summary>
			SetShowSounds = 57,

			/// <summary>
			/// Gets the number of lines to scroll when the mouse wheel is rotated.
			/// </summary>
			GetWheelScrollLines = 104,
			/// <summary>
			/// Sets the number of lines to scroll when the mouse wheel is rotated.
			/// </summary>
			SetWheelScrollLines = 105,

			/// <summary>
			/// Retrieves a contrast value that is used in smoothing text displayed using Microsoft® ClearType®.
			/// </summary>
			GetFontSmoothingContrast = 0x200C,
			/// <summary>
			/// Sets the contrast value used when displaying text in a ClearType font.
			/// </summary>
			SetFontSmoothingContrast = 0x200D,

			/// <summary>
			/// Retrieves the screen saver time-out value, in seconds.
			/// </summary>
			GetScreenSaveTimeout = 14,
			/// <summary>
			/// Sets the screen saver time-out value to the value of the uiParam parameter.
			/// </summary>
			SetScreenSaveTimeout = 15,

			/// <summary>
			/// Sets the amount of time that Windows CE will stay on with battery power before it suspends due to user inaction.
			/// </summary>
			SetBatteryIdleTimeout = 251,
			/// <summary>
			/// Retrieves the amount of time that Windows CE will stay on with battery power before it suspends due to user inaction.
			/// </summary>
			GetBatteryIdleTimeout = 252,

			/// <summary>
			/// Sets the amount of time that Windows CE will stay on with AC power before it suspends due to user inaction.
			/// </summary>
			SetExternalIdleTimeout = 253,
			/// <summary>
			/// Retrieves the amount of time that Windows CE will stay on with AC power before it suspends due to user inaction.
			/// </summary>
			GetExternalIdleTimeout = 254,

			/// <summary>
			/// Sets the amount of time that Windows CE will stay on after a user notification that reactivates the suspended device.
			/// </summary>
			SetWakeupIdleTimeout = 255,
			/// <summary>
			/// Retrieves the amount of time that Windows CE will stay on after a user notification that reactivates a suspended device.
			/// </summary>
			GetWakeupIdleTimeout = 256,

			// @CESYSGEN ENDIF

			//The following flags also used with WM_SETTINGCHANGE
			// so don't use the values for future SPI_*

			//#define SETTINGCHANGE_START  		0x3001
			//#define SETTINGCHANGE_RESET  		0x3002
			//#define SETTINGCHANGE_END    		0x3003

			/// <summary>
			/// Get the platform name e.g. PocketPC, Smartphone etc.
			/// </summary>
			GetPlatformType = 257,
			/// <summary>
			/// Get OEM specific information.
			/// </summary>
			GetOemInfo = 258,

		}
		#endregion

		#region SystemParametersInfoFlags Enumeration
		/// <summary>
		/// Specifies whether the user profile is to be updated, and if so, whether the WM_SETTINGCHANGE message is to be broadcast to all top-level windows to notify them of the change.
		/// </summary>
		public enum SystemParametersInfoFlags : int
		{
			/// <summary>
			/// No notifications are sent on settings changed.
			/// </summary>
			None = 0,
			/// <summary>
			/// Writes the new system-wide parameter setting to the user profile.
			/// </summary>
			UpdateIniFile = 0x0001,
			/// <summary>
			/// Broadcasts the WM_SETTINGCHANGE message after updating the user profile.
			/// </summary>
			SendChange = 0x0002,
		}
		#endregion

		#region System Power Status

		/// <summary>
		/// AC power status.
		/// </summary>
		/// <remarks>Used by <see cref="T:OpenNETCF.Win32.Core.SystemPowerStatus"/> Structure.</remarks>
		[Obsolete("Use the .NET compliant OpenNETCF.Windows.Forms.SystemInformationEx.PowerStatus and related functionality.", false)]
		public enum ACLineStatus : byte
		{
			/// <summary>
			/// AC power is offline.
			/// </summary>
			Offline         = 0x00,
			/// <summary>
			/// AC power is online.
			/// </summary>
			Online			= 0x01,
			/// <summary>
			/// Unit is on backup power.
			/// </summary>
			BackupPower		= 0x02,
			/// <summary>
			/// AC line status is unknown.
			/// </summary>
			Unknown			= 0xFF,
		}

		/// <summary>
		/// Battery charge status.
		/// </summary>
		[Flags(), Obsolete("Use the .NET compliant OpenNETCF.Windows.Forms.SystemInformationEx.PowerStatus and related functionality.", false)]
		public enum BatteryFlag : byte
		{
			/// <summary>
			/// Battery is high.
			/// </summary>
			High		= 0x01,
			/// <summary>
			/// Battery is low.
			/// </summary>
			Low			= 0x02,
			/// <summary>
			/// Battery is critically low.
			/// </summary>
			Critical	= 0x04,
			/// <summary>
			/// Battery is currently charging.
			/// </summary>
			Charging	= 0x08,
			/// <summary>
			/// No battery is attached.
			/// </summary>
			NoBattery	= 0x80,
			/// <summary>
			/// Battery charge status is unknown.
			/// </summary>
			Unknown		= 0xFF,

		}

		/// <summary>
		/// The remaining battery power is unknown.
		/// </summary>
		[Obsolete("Use the .NET compliant OpenNETCF.Windows.Forms.SystemInformationEx.PowerStatus and related functionality.", false)]
		public const byte BatteryPercentageUnknown = 0xFF;

		[Obsolete("Use the .NET compliant OpenNETCF.Windows.Forms.SystemInformationEx.PowerStatus and related functionality.", false)]
		internal const uint BatteryLifeUnknown = 0xFFFFFFFF;

		
		/// <summary>
		/// This structure contains information about the power status of the system.
		/// </summary>
		[Obsolete("Use the .NET compliant OpenNETCF.Windows.Forms.SystemInformationEx.PowerStatus and related functionality.", false)]
		public struct SystemPowerStatus 
		{
			/// <summary>
			/// AC power status.
			/// </summary>
			public ACLineStatus ACLineStatus;
			/// <summary>
			/// Battery charge status.
			/// </summary>
			public BatteryFlag BatteryFlag;
			/// <summary>
			/// Percentage of full battery charge remaining.
			/// This member can be a value in the range 0 to 100, or 255 if status is unknown.
			/// All other values are reserved.
			/// </summary>
			public byte BatteryLifePercent;
			internal byte Reserved1;
			/// <summary>
			/// Number of seconds of battery life remaining, or 0xFFFFFFFF if remaining seconds are unknown.
			/// </summary>
			public int BatteryLifeTime;
			/// <summary>
			/// Number of seconds of battery life when at full charge, or 0xFFFFFFFF if full lifetime is unknown.
			/// </summary>
			private uint dwBatteryFullLifeTime;
			/// <summary>
			/// Number of seconds of battery life when at full charge, or 0 if full lifetime is unknown.
			/// </summary>
			public int BatteryFullLifeTime
			{
				get
				{
					if(dwBatteryFullLifeTime==BatteryLifeUnknown)
					{
						return 0;
					}
					else
					{
						return Convert.ToInt32(dwBatteryFullLifeTime);
					}
				}
			}
			internal byte Reserved2;
			/// <summary>
			/// Battery charge status.
			/// </summary>
			public BatteryFlag BackupBatteryFlag;
			/// <summary>
			/// Percentage of full backup battery charge remaining. Must be in the range 0 to 100.
			/// </summary>
			public byte BackupBatteryLifePercent;
			internal byte Reserved3;
			/// <summary>
			/// Number of seconds of backup battery life remaining.
			/// </summary>
			private uint dwBackupBatteryLifeTime;
			/// <summary>
			/// Number of seconds of backup battery life remaining.
			/// </summary>
			public int BackupBatteryLifeTime
			{
				get
				{
					if(dwBackupBatteryLifeTime==BatteryLifeUnknown)
					{
						return 0;
					}
					else
					{
						return Convert.ToInt32(dwBackupBatteryLifeTime);
					}
				}
			}
			/// <summary>
			/// Number of seconds of backup battery life when at full charge.
			/// </summary>
			private uint dwBackupBatteryFullLifeTime;
			/// <summary>
			/// Number of seconds of backup battery life when at full charge.
			/// </summary>
			public int BackupBatteryFullLifeTime
			{
				get
				{
					if(dwBackupBatteryFullLifeTime==BatteryLifeUnknown)
					{
						return 0;
					}
					else
					{
						return Convert.ToInt32(dwBackupBatteryFullLifeTime);
					}
				}
			}
		}
		#endregion

		#region Memory Status
		/// <summary>
		/// This structure contains information about current memory availability. The GlobalMemoryStatus function uses this structure.
		/// </summary>
		public struct MemoryStatus
		{
			internal uint dwLength;
			/// <summary>
			/// Specifies a number between 0 and 100 that gives a general idea of current memory utilization, in which 0 indicates no memory use and 100 indicates full memory use.
			/// </summary>
			public int MemoryLoad;
			/// <summary>
			/// Indicates the total number of bytes of physical memory.
			/// </summary>
			public int TotalPhysical;
			/// <summary>
			/// Indicates the number of bytes of physical memory available.
			/// </summary>
			public int AvailablePhysical;
			/// <summary>
			/// Indicates the total number of bytes that can be stored in the paging file. Note that this number does not represent the actual physical size of the paging file on disk.
			/// </summary>
			public int TotalPageFile; 
			/// <summary>
			/// Indicates the number of bytes available in the paging file.
			/// </summary>
			public int AvailablePageFile; 
			/// <summary>
			/// Indicates the total number of bytes that can be described in the user mode portion of the virtual address space of the calling process.
			/// </summary>
			public int TotalVirtual; 
			/// <summary>
			/// Indicates the number of bytes of unreserved and uncommitted memory in the user mode portion of the virtual address space of the calling process.
			/// </summary>
			public int AvailableVirtual;
		}
		#endregion

		#endregion ---------------------------------------------
    
		
		#region --------------- Multimedia API Calls ---------------
		

		/// <summary>
		/// Set the volume for the default waveOut device (device ID = 0)
		/// </summary>
		/// <param name="Volume"></param>
		public void SetVolume(int Volume)
		{
			WaveFormatEx	format		= new WaveFormatEx();
			IntPtr			hWaveOut	= IntPtr.Zero;

			Wave.waveOutOpen(out hWaveOut, 0, format, IntPtr.Zero, 0, 0);
			Wave.waveOutSetVolume(hWaveOut, Volume);
			Wave.waveOutClose(hWaveOut);
		}

		/// <summary>
		/// Set the volume for an already-open waveOut device
		/// </summary>
		/// <param name="hWaveOut"></param>
		/// <param name="Volume"></param>
		public void SetVolume(IntPtr hWaveOut, int Volume)
		{
			Wave.waveOutSetVolume(hWaveOut, Volume);
		}

		/// <summary>
		/// Get the current volume setting for the default waveOut device (device ID = 0)
		/// </summary>
		/// <returns></returns>
		public int GetVolume()
		{
			WaveFormatEx	format		= new WaveFormatEx();
			IntPtr			hWaveOut	= IntPtr.Zero;
			int			volume		= 0;

			Wave.waveOutOpen(out hWaveOut, 0, format, IntPtr.Zero, 0, 0);
			Wave.waveOutGetVolume(hWaveOut, ref volume);
			Wave.waveOutClose(hWaveOut);

			return volume;
		}

		/// <summary>
		/// Set the current volume setting for an already-open waveOut device
		/// </summary>
		/// <param name="hWaveOut"></param>
		/// <returns></returns>
		public int GetVolume(IntPtr hWaveOut)
		{
			int			volume		= 0;

			Wave.waveOutGetVolume(hWaveOut, ref volume);

			return volume;
		}
		#endregion ---------------------------------------------

		#region --------------- Memory API Calls ---------------
	
		/// <summary>
		/// Retrieves the memory status of the device
		/// </summary>
		public static MemoryStatus GlobalMemoryStatus()
		{
			MemoryStatus ms = new MemoryStatus();
			
			GlobalMemoryStatusCE( out ms );
			
			return ms;
		}

		#endregion ---------------------------------------------

		#region --------------- GDI API Calls ---------------

		/// <summary>
		/// Find a Window
		/// </summary>
		/// <param name="lpClassName">Can be empty</param>
		/// <param name="lpWindowName">Caption or text of Window to find</param>
		/// <returns>Handle to specified window.</returns>
		[Obsolete("Use OpenNETCF.Win32.Win32Window.FindWindow found in the OpenNETCF.Windows.Forms assembly.", false)]
		public static IntPtr FindWindow(string lpClassName, string lpWindowName)
		{
			IntPtr ptr = IntPtr.Zero;

			ptr = FindWindowCE(lpClassName, lpWindowName);

			if(ptr == IntPtr.Zero)
			{
				throw new WinAPIException("Failed to find Window");
			}

			return ptr;
		}

		/// <summary>
		/// Set the forecolor of text in the selected DC
		/// </summary>
		/// <param name="hdc"></param>
		/// <param name="crColor"></param>
		/// <returns></returns>
		public static int SetTextColor(IntPtr hdc, int crColor)
		{
			uint prevColor;
			
			CheckHandle(hdc);

			prevColor = SetTextColorCE(hdc, crColor);

			if(prevColor == CLR_INVALID)
			{
				throw new WinAPIException("Failed to set color");
			}

			return Convert.ToInt32(prevColor);
		}

		/// <summary>
		/// Get the forecolor of text in the selected DC
		/// </summary>
		/// <param name="hdc"></param>
		/// <returns></returns>
		public static int GetTextColor(IntPtr hdc)
		{
			CheckHandle(hdc);

			return GetTextColorCE(hdc);
		}

		/// <summary>
		/// Set the backcolor in the selected DC
		/// </summary>
		/// <param name="hdc"></param>
		/// <param name="crColor"></param>
		/// <returns></returns>
		public static int SetBkColor(IntPtr hdc, int crColor)
		{
			uint prevColor;
			
			CheckHandle(hdc);

			prevColor = SetBkColorCE(hdc, crColor);

			if(prevColor == CLR_INVALID)
			{
				throw new WinAPIException("Failed to set color");
			}

			return Convert.ToInt32(prevColor);
		}
		
		/// <summary>
		/// Set the backmode in the selected DC
		/// </summary>
		/// <param name="hdc"></param>
		/// <param name="iBkMode"></param>
		/// <returns></returns>
		public static BackMode SetBkMode(IntPtr hdc, BackMode iBkMode)
		{
			int prevMode;
			
			CheckHandle(hdc);

			prevMode = SetBkModeCE(hdc, (int)iBkMode);

			if(prevMode == 0)
			{
				throw new WinAPIException("Failed to set BkMode");
			}

			return (BackMode)prevMode;
		}

		/// <summary>
		/// Select a system object (FONT, DC, etc.)
		/// </summary>
		/// <param name="hdc"></param>
		/// <param name="hgdiobj"></param>
		/// <returns></returns>
		public static IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj)
		{
			IntPtr ptr = IntPtr.Zero;

			CheckHandle(hdc);
			CheckHandle(hgdiobj);

			ptr = SelectObjectCE(hdc, hgdiobj);

			if(ptr == IntPtr.Zero)
			{
				throw new WinAPIException("Failed. Selected object is not a region.");
			}

			return ptr;
		}

		/// <summary>
		/// Release a Device Context
		/// </summary>
		/// <param name="hWnd"></param>
		/// <param name="hDC"></param>
		public static void ReleaseDC(IntPtr hWnd, IntPtr hDC)
		{
			CheckHandle(hDC);

			if(ReleaseDCCE(hWnd, hDC) == 0)
			{
				throw new WinAPIException("Failed to release DC.");
			}
		}

		/// <summary>
		/// Get the DC for the specified window
		/// </summary>
		/// <param name="hWnd">Native window handle of the window.</param>
		/// <returns>Device Context Handle for specified window.</returns>
		public static IntPtr GetWindowDC(IntPtr hWnd)
		{
			IntPtr ptr = IntPtr.Zero;

			ptr = GetWindowDCCE(hWnd);

			if(ptr == IntPtr.Zero)
			{
				throw new WinAPIException("Failed to get DC.");
			}

			return ptr;
		}

		/// <summary>
		/// Get the DC for the specified window handle
		/// </summary>
		/// <param name="hWnd"></param>
		/// <returns></returns>
		public static IntPtr GetDC(IntPtr hWnd)
		{
			IntPtr ptr = IntPtr.Zero;

			ptr = GetDCCE(hWnd);

			if(ptr == IntPtr.Zero)
			{
				throw new WinAPIException("Failed to get DC.");
			}

			return ptr;
		}

		/// <summary>
		/// Draw a rectangle in a DC
		/// </summary>
		/// <param name="hdc"></param>
		/// <param name="nLeftRect"></param>
		/// <param name="nTopRect"></param>
		/// <param name="nRightRect"></param>
		/// <param name="nBottomRect"></param>
		public static void Rectangle(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect)
		{
			CheckHandle(hdc);

			if(Convert.ToBoolean(RectangleCE(hdc, nLeftRect, nTopRect, nRightRect, nBottomRect)) == false)
			{
				throw new WinAPIException("Failed to draw rectangle.");
			}
		}
		#endregion ---------------------------------------------

		#region --------------- Synchronization API Calls ---------------

		/// <summary>
		/// Create a system event (*not* a managed event)
		/// </summary>
		/// <param name="bManualReset"></param>
		/// <param name="bInitialState"></param>
		/// <param name="lpName"></param>
		/// <returns></returns>
		[Obsolete("Use OpenNETCF.Threading.EventWaitHandle to work with event handles", false)]
		public static IntPtr CreateEvent(bool bManualReset, bool bInitialState, string lpName)
		{
			return OpenNETCF.Threading.NativeMethods.CreateEvent(IntPtr.Zero, bManualReset, bInitialState, lpName);
		}

		/// <summary>
		/// This function sets the state of the specified event object to signaled
		/// </summary>
		/// <param name="hEvent"></param>
		/// <returns></returns>
		[Obsolete("Use OpenNETCF.Threading.EventWaitHandle to work with event handles", false)]
		public static bool SetEvent(IntPtr hEvent)
		{
			return OpenNETCF.Threading.NativeMethods.EventModify(hEvent,OpenNETCF.Threading.NativeMethods.EVENT.SET);
		}

		/// <summary>
		/// This function sets the state of the specified event object to nonsignaled
		/// </summary>
		/// <param name="hEvent"></param>
		/// <returns></returns>
		[Obsolete("Use OpenNETCF.Threading.EventWaitHandle to work with event handles", false)]
		public static bool ResetEvent(IntPtr hEvent)
		{
			return OpenNETCF.Threading.NativeMethods.EventModify(hEvent, OpenNETCF.Threading.NativeMethods.EVENT.RESET);
		}

		/// <summary>
		/// This function provides a single operation that sets (to signaled) the state of the specified event object and then resets it (to nonsignaled) after releasing the appropriate number of waiting threads
		/// </summary>
		/// <param name="hEvent"></param>
		/// <returns></returns>
		[Obsolete("Use OpenNETCF.Threading.EventWaitHandle to work with event handles", false)]
		public static bool PulseEvent(IntPtr hEvent)
		{
			return OpenNETCF.Threading.NativeMethods.EventModify(hEvent, OpenNETCF.Threading.NativeMethods.EVENT.PULSE);
		}

		/// <summary>
		/// This function returns when the specified object is in the signaled state or when the time-out interval elapses
		/// </summary>
		/// <param name="hHandle"></param>
		/// <param name="dwMilliseconds"></param>
		/// <returns></returns>
		[Obsolete("Use OpenNETCF.Threading.EventWaitHandle to work with event handles", false),
		CLSCompliant(false)]
		public static Wait WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds)
		{
			return (Wait)OpenNETCF.Threading.NativeMethods.WaitForSingleObject(hHandle, Convert.ToInt32(dwMilliseconds));
		}

		/// <summary>
		/// This function returns when one of the following occurs:
		/// <list type=""><item>Either any one of the specified objects are in the signaled state.</item><item>The time-out interval elapses.</item></list>
		/// <i>New in SDF version 1.1</i>
		/// </summary>
		/// <param name="Handles">An array of handles to wait on</param>
		/// <param name="WaitForAll">Wait for all handles before returning</param>
		/// <param name="Timeout">Timeout period in milliseconds</param>
		/// <returns>WAIT_FAILED indicates failure.</returns>
		[CLSCompliant(false)]
		public static Wait WaitForMultipleObjects(IntPtr[] Handles, bool WaitForAll, int Timeout)
		{
			return (Wait)OpenNETCF.Threading.NativeMethods.WaitForMultipleObjects((uint)Handles.Length, Handles, WaitForAll, (uint)Timeout);
		}

		/// <summary>
		/// This function returns when one of the following occurs:
		/// <list type=""><item>Either any one of the specified objects are in the signaled state.</item><item>The time-out interval elapses.</item></list>
		/// <para><b>New in v1.3</b></para>
		/// </summary>
		/// <param name="Handles">An array of handles to wait on</param>
		/// <param name="Timeout">Timeout period in milliseconds</param>
		/// <param name="Mask">Input types for which an input event object handle will be added to the array of object handles</param>
		/// <returns>WAIT_FAILED indicates failure.</returns>
		[CLSCompliant(false)]
		public static Wait MsgWaitForMultipleObjects(IntPtr[] Handles, int Timeout, WakeMask Mask)
		{
			return (Wait)OpenNETCF.Threading.NativeMethods.MsgWaitForMultipleObjectsEx((uint)Handles.Length, Handles, (uint)Timeout, (uint) Mask, 0);
		}

		
		/// <summary>
		/// This function creates a named or unnamed mutex object
		/// </summary>
		/// <param name="lpMutexAttributes">Ignored. Must be null</param>
		/// <param name="bInitialOwner">Boolean that specifies the initial owner 
		/// of the mutex object. If this value is true and the caller created 
		/// the mutex, the calling thread obtains ownership of the mutex object. 
		/// Otherwise, the calling thread does not obtain ownership of the mutex.</param>
		/// <param name="lpName">String specifying the name of the mutex object.</param>
		/// <returns>A handle to the mutex object indicates success. If the 
		/// named mutex object existed before the function call, the function 
		/// returns a handle to the existing object. A return value of null 
		/// indicates failure. </returns>
		[Obsolete("Use OpenNETCF.Threading.MutexEx to create a named Mutex", false)]
		public static IntPtr CreateMutex(IntPtr lpMutexAttributes, bool bInitialOwner, string lpName)
		{
			IntPtr h;
			const int ERROR_ALREADY_EXISTS = 183;

			h = OpenNETCF.Threading.NativeMethods.CreateMutex(lpMutexAttributes, bInitialOwner, lpName);

			if(h != IntPtr.Zero)
			{
				if(Marshal.GetLastWin32Error() == ERROR_ALREADY_EXISTS)
				{
					throw new WinAPIException("Mutex already exists", (int)h);
				}
			}
			else
			{
				throw new WinAPIException("CreateMutex failed.",(int)h);
			}
			return h;
		}

		/// <summary>
		/// This function releases ownership of the specified mutex object
		/// </summary>
		/// <param name="hMutex"></param>
		/// <returns></returns>
		[Obsolete("Use OpenNETCF.Threading.MutexEx to create a named Mutex", false)]
		public static bool ReleaseMutex(IntPtr hMutex)
		{
			bool b;

			b = OpenNETCF.Threading.NativeMethods.ReleaseMutex(hMutex);

			if(!b)
			{
				throw new WinAPIException("Error releasing Mutex.", Marshal.GetLastWin32Error());
			}
			return b;
		}
		#endregion ---------------------------------------------

		#region --------------- Performance Monitoring functions ---------------

		/// <summary>
		/// This function retrieves the frequency of the high-resolution performance counter if one is provided by the OEM.
		/// </summary>
		/// <returns>The current performance-counter frequency. If the installed hardware does not support a high-resolution performance counter, this parameter can be to zero.</returns>
		public static long QueryPerformanceFrequency()
		{
			long lpFrequency = 0;

			int i;

			i = QueryPerformanceFrequencyCE(ref lpFrequency);

			if(i == 0)
			{
				throw new WinAPIException("The hardware does not support a high frequency counter.");
			}

			return lpFrequency;
		}

		/// <summary>
		/// This function retrieves the current value of the high-resolution performance counter if one is provided by the OEM
		/// </summary>
		/// <returns>The current performance-counter value. If the installed hardware does not support a high-resolution performance counter, this parameter can be set to zero.</returns>
		public static long QueryPerformanceCounter()
		{
			long lpPerformanceCount = 0;

			int i;

			i = QueryPerformanceCounterCE(ref lpPerformanceCount);

			if(i == 0)
			{
				throw new WinAPIException("The hardware does not support a high frequency counter.");
			}

			return lpPerformanceCount;
		}

		#endregion ---------------------------------------------

		#region --------------- System Info functions ---------------

		#region Get System Info
		/// <summary>
		/// This function returns information about the current system
		/// </summary>
		public static SystemInfo GetSystemInfo()
		{
			SystemInfo pSI = new SystemInfo();

			try 
			{
				GetSystemInfoCE(out pSI);
			}
			catch(Exception)
			{
				throw new WinAPIException("Error retrieving system info.");
			}

			return pSI;
		}
		#endregion

		#region GetSystemPowerStatus
		/// <summary>
		/// This function retrieves the power status of the system.
		/// </summary>
		/// <returns>A SystemPowerStatus structure containing power state information.</returns>
		/// <remarks>The status indicates whether the system is running on AC or DC power, whether or not the batteries are currently charging, and the remaining life of main and backup batteries.</remarks>
		/// <param name="update">If True retrieves latest state, otherwise retrieves cached information which may be out of date.</param>
		[Obsolete("Use the .NET compliant OpenNETCF.Windows.Forms.SystemInformationEx.PowerStatus property.", true)]
		public static SystemPowerStatus GetSystemPowerStatusEx(bool update)
		{
			SystemPowerStatus pStatus = new SystemPowerStatus();

			try
			{
				GetSystemPowerStatusExCE(out pStatus, update);
			}
			catch(Exception)
			{
				throw new WinAPIException("Error retrieving system power status.");
			}

			return pStatus;
		}
		[DllImport("coredll.dll", EntryPoint="GetSystemPowerStatusEx", SetLastError=true)]
		private static extern bool GetSystemPowerStatusExCE(out SystemPowerStatus pStatus, bool fUpdate);

		#endregion

		#region Get Device ID

		private const Int32 ERROR_NOT_SUPPORTED = 0x32;
		private const Int32 ERROR_INSUFFICIENT_BUFFER = 0x7A;

		private static byte[] GetRawDeviceID()
		{
			// Initialize the output buffer to the size of a Win32 DEVICE_ID structure
			Int32 dwOutBytes = 0;
			//set an initial buffer size
			Int32 nBuffSize = 256;
			byte[] outbuff = new byte[nBuffSize];
			
			bool done = false;

			// Set DEVICEID.dwSize to size of buffer.  Some platforms look at
			// this field rather than the nOutBufSize param of KernelIoControl
			// when determining if the buffer is large enough.
			//
			BitConverter.GetBytes(nBuffSize).CopyTo(outbuff, 0);

			// Loop until the device ID is retrieved or an error occurs
			while (!done)
			{
				if (KernelIoControl(IOCTL_HAL_GET_DEVICEID, null, 0, outbuff,
						nBuffSize, ref dwOutBytes))
				{
					done = true;
				}
				else
				{
					int error = Marshal.GetLastWin32Error();
					switch (error)
					{
						case ERROR_NOT_SUPPORTED :
							throw new NotSupportedException("IOCTL_HAL_GET_DEVICEID is not supported on this device", new Exception("" + error));

						case ERROR_INSUFFICIENT_BUFFER :
							// The buffer wasn't big enough for the data.  The
							// required size is in the first 4 bytes of the output
							// buffer (DEVICE_ID.dwSize).
							nBuffSize = BitConverter.ToInt32(outbuff, 0);
							outbuff = new byte[nBuffSize];

							// Set DEVICEID.dwSize to size of buffer.  Some
							// platforms look at this field rather than the
							// nOutBufSize param of KernelIoControl when
							// determining if the buffer is large enough.
							//
							BitConverter.GetBytes(nBuffSize).CopyTo(outbuff, 0);
							break;

						default:
							throw new Exception("Unexpected error: " + error);
					}
				}
			}

			//return the raw buffer - a DEVICE_ID structure
			return outbuff;
		}

		/// <summary>
		/// Returns a Guid representing the unique idenitifier of the device.
		/// <para><b>New in v1.3</b></para>
		/// </summary>
		/// <returns></returns>
		public static Guid GetDeviceGuid()
		{
			byte[] outbuff = GetRawDeviceID();

			Int32 dwPresetIDOffset = BitConverter.ToInt32(outbuff, 0x4); //	DEVICE_ID.dwPresetIDOffset
			Int32 dwPresetIDSize = BitConverter.ToInt32(outbuff, 0x8); // DEVICE_ID.dwPresetSize
			Int32 dwPlatformIDOffset = BitConverter.ToInt32(outbuff, 0xc); // DEVICE_ID.dwPlatformIDOffset
			Int32 dwPlatformIDSize = BitConverter.ToInt32(outbuff, 0x10); // DEVICE_ID.dwPlatformIDBytes
			
			byte[] guidbytes = new byte[16];

			Buffer.BlockCopy(outbuff, dwPresetIDOffset + dwPresetIDSize - 10, guidbytes, 0, 10);
			Buffer.BlockCopy(outbuff, dwPlatformIDOffset + dwPlatformIDSize - 6, guidbytes, 10, 6);

			return new Guid(guidbytes);	
		}

		/// <summary>
		/// Returns a string containing a unique identifier for the device.
		/// </summary>
		/// <returns>Devices unique ID.</returns>
		public static string GetDeviceID()
		{
			byte[] outbuff = GetRawDeviceID();

			Int32 dwPresetIDOffset = BitConverter.ToInt32(outbuff, 0x4); //	DEVICE_ID.dwPresetIDOffset
			Int32 dwPresetIDSize = BitConverter.ToInt32(outbuff, 0x8); // DEVICE_ID.dwPresetSize
			Int32 dwPlatformIDOffset = BitConverter.ToInt32(outbuff, 0xc); // DEVICE_ID.dwPlatformIDOffset
			Int32 dwPlatformIDSize = BitConverter.ToInt32(outbuff, 0x10); // DEVICE_ID.dwPlatformIDBytes
			StringBuilder sb = new StringBuilder();

			for (int i = dwPresetIDOffset; i < dwPresetIDOffset + dwPresetIDSize; i++)
				sb.Append(String.Format("{0:X2}", outbuff[i]));
				
			sb.Append("-");
			for (int i = dwPlatformIDOffset; i < dwPlatformIDOffset + dwPlatformIDSize; i++)
				sb.Append(String.Format("{0:X2}", outbuff[i]));
			
			return sb.ToString();

		}
		#endregion

		#endregion ---------------------------------------------

		#region --------------- System Functions ----------------			

		/// <summary>
		/// Wrapper around LoadLibrary Win32 function
		/// </summary>
		/// <param name="library">Library module name</param>
		/// <returns>hModule or error code</returns>
		public static IntPtr LoadLibrary(string library)
		{
			IntPtr ret = LoadLibraryCE(library);
			if ( ret.ToInt32() <= 31 & ret.ToInt32() >= 0)
				throw new WinAPIException("Failed to load library " + library, Marshal.GetLastWin32Error());
			return ret;
		}
		


		#endregion

		#region --------------- Internal helper functions ----------------
		
		public static void CheckHandle(IntPtr hPtr)
		{
			if((hPtr == IntPtr.Zero) || ((int)hPtr == INVALID_HANDLE_VALUE))
			{
				throw new WinAPIException("Invalid Handle Value", 0);
			}
		}

		#endregion ---------------------------------------------

		#region ----------------- Keyboard functions ------------------
		/// <summary>
		/// Send a string to the keyboard
		/// </summary>
		/// <param name="Keys"></param>
		public static void SendKeyboardString(string Keys)
		{
			SendKeyboardString(Keys, KeyStateFlags.Down, IntPtr.Zero);
		}

		/// <summary>
		/// Send a string to the keyboard
		/// </summary>
		/// <param name="Keys"></param>
		/// <param name="Flags"></param>
		public static void SendKeyboardString(string Keys, KeyStateFlags Flags)
		{
			SendKeyboardString(Keys, Flags, IntPtr.Zero);
		}

		/// <summary>
		/// Send a string to the keyboard
		/// </summary>
		/// <param name="Keys"></param>
		/// <param name="Flags"></param>
		/// <param name="hWnd"></param>
		public static void SendKeyboardString(string Keys, KeyStateFlags Flags, IntPtr hWnd)
		{
			uint[]			keys	= new uint[Keys.Length];
			KeyStateFlags[]	states	= new KeyStateFlags[Keys.Length];
			KeyStateFlags[]	dead	= {KeyStateFlags.Dead};

			for(int k = 0 ; k < Keys.Length ; k++)
			{
				states[k] = Flags;
				keys[k] = Convert.ToUInt32(Keys[k]);
			}

			PostKeybdMessage(hWnd, 0, Flags, (uint)keys.Length, states, keys);
			PostKeybdMessage(hWnd, 0, dead[0], 1, dead, keys); 
		}

		/// <summary>
		/// Send a key to the keyboard
		/// </summary>
		/// <param name="VirtualKey"></param>
		public static void SendKeyboardKey(byte VirtualKey)
		{
			SendKeyboardKey(VirtualKey, true);
		}

		/// <summary>
		/// Send a key to the keyboard
		/// </summary>
		/// <param name="VirtualKey"></param>
		/// <param name="Silent"></param>
		public static void SendKeyboardKey(byte VirtualKey, bool Silent)
		{
			SendKeyboardKey(VirtualKey, Silent, KeyActionState.Press);
		}

		/// <summary>
		/// Simulates a keystroke that the system can use to generate a WM_KEYUP or WM_KEYDOWN message.
		/// </summary>
		/// <param name="VirtualKey">A System.Byte structure that contains a virtual-key code representing the key with which to perform an action.</param>
		/// <param name="Silent">A System.Boolean structure specifying true if a sound should be generated when the keystroke is simulated; otherwise, false.</param>
		/// <param name="State">A KeyActionState enumeration value indicating the action that should be performed with the specified virtual-key code.</param>
		public static void SendKeyboardKey(byte VirtualKey, bool Silent, KeyActionState State)
		{
			int silent = Silent ? (int)KeyEvents.Silent : 0;

			// Note that both of the operations below will be performed if the caller 
			// requested a key press operation (KeyActionState.Press).

			// If requested by the caller, send the virtual-key code as part of a key down operation.
			if ((State & KeyActionState.Down) > 0)
			{
				keybd_event(VirtualKey, 0, 0, silent);
			}

			// If requested by the caller, send the virtual-key code as part of a key up operation.
			if ((State & KeyActionState.Up) > 0)
			{
				keybd_event(VirtualKey, 0, (int)KeyEvents.KeyUp, silent);
			}
		}
		#endregion



		#region --------------- P/Invoke declarations ---------------	

		#region Keyboard P/Invokes
		[DllImport("coredll.dll", EntryPoint="keybd_event", SetLastError=true)]
		internal static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

		[DllImport("coredll.dll", EntryPoint="PostKeybdMessage", SetLastError=true)]
		internal static extern bool PostKeybdMessage(IntPtr hwnd, uint vKey, KeyStateFlags flags, uint cCharacters, KeyStateFlags[] pShiftStateBuffer, uint[] pCharacterBuffer);

		#endregion
		
		#region memory P/Invokes

		[DllImport("coredll", EntryPoint="GlobalMemoryStatus", SetLastError=false)]
		internal static extern void GlobalMemoryStatusCE(out MemoryStatus msce);

		#endregion
		
		#region Performance Monitoring P/Invokes

		[DllImport("coredll.dll", EntryPoint="QueryPerformanceFrequency", SetLastError=true)]
		internal static extern int QueryPerformanceFrequencyCE(ref Int64 lpFrequency);

		[DllImport("coredll.dll", EntryPoint="QueryPerformanceCounter", SetLastError=true)]
		internal static extern int QueryPerformanceCounterCE(ref Int64 lpPerformanceCount);

		#endregion

		#region GDI P/Invokes

		[DllImport("coredll.dll", EntryPoint="FindWindowW", SetLastError=true)]
		private static extern IntPtr FindWindowCE(string lpClassName, string lpWindowName);
		
		[DllImport ("coredll.dll", EntryPoint="SetTextColor", SetLastError=true)]
		private static extern uint SetTextColorCE(IntPtr hdc, int crColor);

		[DllImport ("coredll.dll", EntryPoint="GetTextColor", SetLastError=true)]
		private static extern int GetTextColorCE(IntPtr hdc);

		[DllImport ("coredll.dll", EntryPoint="SetBkColor", SetLastError=true)]
		private static extern uint SetBkColorCE(IntPtr hdc, int crColor);

		[DllImport ("coredll.dll", EntryPoint="SetBkMode", SetLastError=true)]
		private static extern int SetBkModeCE(IntPtr hdc, int iBkMode);

		[DllImport ("coredll.dll", EntryPoint="SelectObject", SetLastError=true)]
		private static extern IntPtr SelectObjectCE(IntPtr hdc, IntPtr hgdiobj);

		[DllImport("coredll.dll", EntryPoint="ReleaseDC", SetLastError=true)]
		private static extern int ReleaseDCCE(IntPtr hWnd, IntPtr hDC);

		[DllImport("coredll.dll", EntryPoint="GetWindowDC", SetLastError=true)]
		private static extern IntPtr GetWindowDCCE(IntPtr hWnd);

		[DllImport("coredll", EntryPoint="GetDC", SetLastError=true)]
		private static extern IntPtr GetDCCE(IntPtr hWnd);

		[DllImport("coredll", EntryPoint="Rectangle", SetLastError=true)]
		private static extern uint RectangleCE(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

		/// <summary>
		/// This function obtains information about a specified graphics object.
		/// </summary>
		/// <param name="hObj">Handle to the graphics object of interest.</param>
		/// <param name="cb">Specifies the number of bytes of information to be written to the buffer.</param>
		/// <param name="objdata">a buffer that is to receive the information about the specified graphics object.</param>
		/// <returns>If the function succeeds, and lpvObject is a valid pointer, the return value is the number of bytes stored into the buffer.</returns>
		[DllImport("coredll", EntryPoint="GetObject", SetLastError=true)]
		extern static public int GetObject(IntPtr hObj, int cb, byte[] objdata);
		/// <summary>
		/// This function obtains information about a specified graphics object.
		/// </summary>
		/// <param name="hObj">Handle to the graphics object of interest.</param>
		/// <param name="cb">Specifies the number of bytes of information to be written to the buffer.</param>
		/// <param name="objdata">a buffer that is to receive the information about the specified graphics object.</param>
		/// <returns>If the function succeeds, and lpvObject is a valid pointer, the return value is the number of bytes stored into the buffer.</returns>
		//[DllImport("coredll", EntryPoint="GetObject", SetLastError=true)]
		//extern static int GetObject(IntPtr hObj, int cb, DibSection objdata);

		/// <summary>
		/// This function creates a logical brush that has the specified solid color.
		/// </summary>
		/// <param name="crColor">Specifies the color of the brush.</param>
		/// <returns>A handle that identifies a logical brush indicates success.</returns>
		[DllImport("coredll", EntryPoint="CreateSolidBrush", SetLastError=true)]
		public static extern IntPtr CreateSolidBrush( int crColor );

		#endregion

		#region System Info P/Invokes

		[DllImport("coredll", EntryPoint="GetSystemInfo",SetLastError=true)]
		internal static extern void GetSystemInfoCE(out SystemInfo pSI); 

		[DllImport("coredll", EntryPoint="FormatMessageW", SetLastError=false)]
		internal static extern int FormatMessage(FormatMessageFlags dwFlags, int lpSource, int dwMessageId, int dwLanguageId, out IntPtr lpBuffer, int nSize, int[] Arguments );

		
		[DllImport("coredll.dll", SetLastError=true)]
		internal static extern bool KernelIoControl(int dwIoControlCode, byte[] inBuf, int inBufSize, byte[] outBuf, int outBufSize, ref int bytesReturned);

		[DllImport("coredll.dll", EntryPoint="SystemParametersInfo", SetLastError=true)]
		internal static extern bool SystemParametersInfo(SystemParametersInfoAction action, int size, byte[] buffer, SystemParametersInfoFlags winini);

		#endregion
	
		#region System P/Invokes

		/// <summary>
		/// This function returns the address of the specified exported DLL function.
		/// </summary>
		/// <param name="hModule">Handle to the DLL module that contains the function.
		/// The <see cref="LoadLibrary"/> or <see cref="GetModuleHandle"/> function returns this handle.</param>
		/// <param name="procName">string containing the function name, or specifies the function's ordinal value.
		/// If this parameter is an ordinal value, it must be in the low-order word; the high-order word must be zero.</param>
		/// <returns></returns>
		[DllImport("coredll.dll", EntryPoint="GetProcAddressW", SetLastError=true)]
		public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
		
		/// <summary>
		/// This function returns a module handle for the specified module if the file is mapped into the address space of the calling process.
		/// </summary>
		/// <param name="moduleName">string that contains the name of the module, which must be a DLL file.</param>
		/// <returns>A handle to the specified module indicates success. IntPtr.Zero indicates failure.</returns>
		[DllImport("coredll.dll", EntryPoint="GetModuleHandleW", SetLastError=true)]
		public static extern IntPtr GetModuleHandle(string moduleName);
		
		/// <summary>
		/// This function determines whether the calling process has read access to the memory at the specified address.
		/// </summary>
		/// <param name="fn">Pointer to an address in memory.</param>
		/// <returns>Zero indicates that the calling process has read access to the specified memory.
		/// Nonzero indicates that the calling process does not have read access to the specified memory.</returns>
		[DllImport("coredll.dll", EntryPoint="IsBadCodePtr", SetLastError=true)]
		public static extern bool IsBadCodePtr(IntPtr fn);

		[DllImport("coredll.dll", EntryPoint="LoadLibraryW", SetLastError=true)]
		internal static extern IntPtr LoadLibraryCE( string lpszLib );
		
		#endregion

		#endregion ---------------------------------------------

		/// <summary>
		/// This function closes an open object handle.
		/// </summary>
		/// <param name="hObject">Handle to an open object.</param>
		/// <returns>true indicates success, false indicates failure.</returns>
		[DllImport("coredll.dll", SetLastError=true)]
		public static extern bool CloseHandle(IntPtr hObject);
	}

	//here temporarily because vault wont allow new files

	/// <summary>
	/// Key State Masks for Mouse Messages.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	[Flags()]
	public enum MK : int
	{
		/// <summary>
		/// The user pressed the left mouse button.
		/// </summary>
		LBUTTON		= 0x0001,
		/// <summary>
		/// The user pressed the right mouse button.
		/// </summary>
		RBUTTON		= 0x0002,
		/// <summary>
		/// The user pressed the SHIFT key.
		/// </summary>
		SHIFT		= 0x0004,
		/// <summary>
		/// The user pressed the CTRL key.
		/// </summary>
		CONTROL		= 0x0008,
		/// <summary>
		/// The user pressed the middle mouse button.
		/// </summary>
		MBUTTON		= 0x0010,
	}

}

/* ============================== Change Log ==============================
 * see Changelog.txt
 * ======================================================================== */
