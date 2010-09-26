//==========================================================================================
//
//		OpenNETCF.Phone.Sms.MessageProtocol
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

namespace OpenNETCF.Phone.Sms
{
	/// <summary>
	/// SMS Protocol Identifier (PID) constants.
	/// <para><b>New in v1.1</b></para>
	/// </summary>
	public enum MessageProtocol : int
	{
		Unknown  = 0x00000000 ,
		SmeToSme  = 0x00000001 ,
		Implicit  = 0x00000002 ,
		Telex  = 0x00000003 ,
		TeleFaxGroup3  = 0x00000004 ,
		TeleFaxGroup4  = 0x00000005 ,
		VoicePhone  = 0x00000006 ,
		Ermes  = 0x00000007 ,
		Paging  = 0x00000008 ,
		VideoTex  = 0x00000009 ,
		TeleTex  = 0x0000000a ,
		TeleTexPspdn  = 0x0000000b ,
		TeleTexCspdn  = 0x0000000c ,
		TeleTexPstn  = 0x0000000d ,
		TeleTexIsdn  = 0x0000000e ,
		Uci  = 0x0000000f ,
		MsgHandling  = 0x00000010 ,
		X400  = 0x00000011 ,
		EMail  = 0x00000012 ,
		SCSpecific1  = 0x00000013 ,
		SCSpecific2  = 0x00000014 ,
		SCSpecific3  = 0x00000015 ,
		SCSpecific4  = 0x00000016 ,
		SCSpecific5  = 0x00000017 ,
		SCSpecific6  = 0x00000018 ,
		SCSpecific7  = 0x00000019 ,
		GsmStation  = 0x0000001a ,
		SM_TYPE0  = 0x0000001b ,
		RSM_TYPE1  = 0x0000001c ,
		RSM_TYPE2  = 0x0000001d ,
		RSM_TYPE3  = 0x0000001e ,
		RSM_TYPE4  = 0x0000001f ,
		RSM_TYPE5  = 0x00000020 ,
		RSM_TYPE6  = 0x00000021 ,
		RSM_TYPE7  = 0x00000022 ,
		ReturnCall  = 0x00000023 ,
		MeDownload  = 0x00000024 ,
		Depersonalization  = 0x00000025 ,
		SimDownload  = 0x00000026 ,
	}
}
