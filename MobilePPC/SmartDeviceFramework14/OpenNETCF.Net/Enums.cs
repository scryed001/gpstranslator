using System;

namespace OpenNETCF.Net
{
//	/// <summary>
//	/// Enumeration giving relative strength indication for
//	/// RF signal.
//	/// </summary>
//	public enum SignalQuality
//	{
//		/// <summary>
//		/// No signal is being received by the RF adapter or
//		/// the adapter is not an RF adapter at all.
//		/// </summary>
//		NoSignal,
//
//		/// <summary>
//		/// Signal strength is very low.
//		/// </summary>
//		VeryLow,
//
//		/// <summary>
//		/// Signal strength is low.
//		/// </summary>
//		Low,
//
//		/// <summary>
//		/// Signal strength is acceptable.
//		/// </summary>
//		Good,
//
//		/// <summary>
//		/// Signal strength is ample.
//		/// </summary>
//		VeryGood,
//
//		/// <summary>
//		/// Signal strength is at the highest level.
//		/// </summary>
//		Excellent,
//
//		/// <summary>
//		/// The adapter is definitely not an RF adapter.
//		/// </summary>
//		NotAnRFAdapter
//	}

	/// <summary>
	/// Enumeration returned in the NetworkTypeInUse property.
	/// Indicates the general type of radio network in use.
	/// </summary>
	public enum NetworkType
	{
		/// <summary>
		/// Indicates the physical layer of the frequency hopping spread-spectrum radio
		/// </summary>
		FH,
		/// <summary>
		/// Indicates the physical layer of the direct sequencing spread-spectrum radio
		/// </summary>
		DS,
		/// <summary>
		/// Indicates the physical layer for 5-GHz Orthagonal Frequency Division Multiplexing radios
		/// </summary>
		OFDM5,
		/// <summary>
		/// Indicates the physical layer for 24-GHz Orthagonal Frequency Division Multiplexing radios
		/// </summary>
		OFDM24
	}

	/// <summary>
	/// Define the general network infrastructure mode in
	/// which the selected network is presently operating.
	/// </summary>
	public enum InfrastructureMode
	{
		/// <summary>
		/// Specifies the independent basic service set (IBSS) mode. This mode is also known as ad hoc mode
		/// </summary>
		AdHoc,
		/// <summary>
		/// Specifies the infrastructure mode.
		/// </summary>
		Infrastructure,
		/// <summary>
		/// The infrastructure mode is either set to automatic or cannot be determined.
		/// </summary>
		AutoUnknown
	}

	/// <summary>
	/// Define authentication types for an adapter.
	/// </summary>
	public enum AuthenticationMode
	{
		Ndis802_11AuthModeOpen,
		Ndis802_11AuthModeShared,
		Ndis802_11AuthModeAutoSwitch,
		Ndis802_11AuthModeWPA,
		Ndis802_11AuthModeWPAPSK,
		Ndis802_11AuthModeWPANone,
		Ndis802_11AuthModeMax               // Not a real mode, defined as upper bound
	}

	/// <summary>
	/// Define WEP authentication state for the adapter.
	/// </summary>
	public enum WEPStatus
	{
		/// <summary>
		/// WEP encryption enabled
		/// </summary>
		Ndis802_11WEPEnabled,
		/// <summary>
		/// WEP encryption enabled
		/// </summary>
		Ndis802_11Encryption1Enabled = Ndis802_11WEPEnabled,    

		/// <summary>
		/// No WEP encryption
		/// </summary>
		Ndis802_11WEPDisabled,
		/// <summary>
		/// No WEP encryption
		/// </summary>
		Ndis802_11EncryptionDisabled = Ndis802_11WEPDisabled,    

		Ndis802_11WEPKeyAbsent,
		Ndis802_11Encryption1KeyAbsent = Ndis802_11WEPKeyAbsent,

		Ndis802_11WEPNotSupported,
		Ndis802_11EncryptionNotSupported = Ndis802_11WEPNotSupported,

		Ndis802_11Encryption2Enabled,

		Ndis802_11Encryption2KeyAbsent,

		Ndis802_11Encryption3Enabled,

		Ndis802_11Encryption3KeyAbsent
	}

	/// <summary>
	/// Control flags for Windows Zero Config
	/// </summary>
	[Flags]
	public enum WZCCTL
	{
		/// <summary>
		/// specifies whether the configuration includes or not a WEP key
		/// </summary>
		WZCCTL_WEPK_PRESENT        =0x0001, 
		/// <summary>
		/// the WEP Key material (if any) is entered as hexadecimal digits
		/// </summary>
		WZCCTL_WEPK_XFORMAT        =0x0002,  
		/// <summary>
		/// this configuration should not be stored
		/// </summary>
		WZCCTL_VOLATILE            =0x0004,  
		/// <summary>
		/// this configuration is enforced by the policy
		/// </summary>
		WZCCTL_POLICY              =0x0008,  
		/// <summary>
		/// for this configuration 802.1X should be enabled
		/// </summary>
		WZCCTL_ONEX_ENABLED        =0x0010,  
		/// <summary>
		/// Key is 40 bit
		/// </summary>
		WZCCTL_WEPK_40BLEN         =0x8000

	}
}
