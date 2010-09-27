//==========================================================================================
//
//		OpenNETCF.Windows.Forms.PowerStatus
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

namespace OpenNETCF.Windows.Forms
{
	/// <summary>
	/// Indicates current system power status information.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
	public class PowerStatus 
	{
		private byte mACLineStatus;
		private byte mBatteryFlag;
		private byte mBatteryLifePercent;
		private byte mReserved1;
		private int mBatteryLifeTime;
		private int mBatteryFullLifeTime;
		private byte mReserved2;
		private byte mBackupBatteryFlag;
		private byte mBackupBatteryLifePercent;
		private byte mReserved3;
		private int mBackupBatteryLifeTime;
		private int mBackupBatteryFullLifeTime;

		internal PowerStatus(){}

		/// <summary>
		/// AC power status.
		/// </summary>
		public PowerLineStatus PowerLineStatus
		{
			get
			{
				Update();
				return (PowerLineStatus)mACLineStatus;
			}
		}

		/// <summary>
		/// Gets the current battery charge status.
		/// </summary>
		public BatteryChargeStatus BatteryChargeStatus
		{
			get
			{
				Update();
				return (BatteryChargeStatus)mBatteryFlag;
			}
		}
		/// <summary>
		/// Gets the approximate percentage of full battery time remaining.
		/// </summary>
		/// <remarks>TThe approximate percentage, from 0 to 100, of full battery time remaining, or 255 if the percentage is unknown.</remarks>
		public byte BatteryLifePercent
		{
			get
			{
				Update();
				return mBatteryLifePercent;
			}
		}

		/// <summary>
		/// Gets the approximate number of seconds of battery time remaining.
		/// </summary>
		/// <value>The approximate number of seconds of battery life remaining, or -1 if the approximate remaining battery life is unknown.</value>
		public int BatteryLifeRemaining
		{
			get
			{
				Update();
				return mBatteryLifeTime;
			}
		}

		/// <summary>
		/// Gets the reported full charge lifetime of the primary battery power source in seconds.
		/// </summary>
		/// <value>The reported number of seconds of battery life available when the battery is fullly charged, or -1 if the battery life is unknown.</value>
		public int BatteryFullLifeTime
		{
			get
			{
				Update();
				return mBatteryFullLifeTime;
			}
		}

		/// <summary>
		/// Gets the backup battery charge status.
		/// </summary>
		public BatteryChargeStatus BackupBatteryChargeStatus
		{
			get
			{
				Update();
				return (BatteryChargeStatus)mBackupBatteryFlag;
			}
		}

		/// <summary>
		/// Percentage of full backup battery charge remaining. Must be in the range 0 to 100.
		/// </summary>
		public byte BackupBatteryLifePercent
		{
			get
			{
				Update();
				return mBackupBatteryLifePercent;
			}
		}
		
		/// <summary>
		/// Number of seconds of backup battery life remaining.
		/// </summary>
		public int BackupBatteryLifeRemaining
		{
			get
			{
				Update();
				return mBackupBatteryLifeTime;
			}
		}
		/// <summary>
		/// Number of seconds of backup battery life when at full charge. Or -1 If unknown.
		/// </summary>
		public int BackupBatteryFullLifeTime
		{
			get
			{
				Update();
				return mBackupBatteryFullLifeTime;
			}
		}

		private void Update()
		{
			bool success = GetSystemPowerStatusEx(this, true);
		}
		[DllImport("coredll.dll", EntryPoint="GetSystemPowerStatusEx", SetLastError=true)]
		private static extern bool GetSystemPowerStatusEx(PowerStatus pStatus, bool fUpdate);
		
	}

	
}
