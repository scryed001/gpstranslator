/*=======================================================================================

    OpenNETCF.Net.FTP
    Copyright 2003, OpenNETCF.org

    This library is free software; you can redistribute it and/or modify it under 
    the terms of the OpenNETCF.org Shared Source License.

    This library is distributed in the hope that it will be useful, but 
    WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
    FITNESS FOR A PARTICULAR PURPOSE. See the OpenNETCF.org Shared Source License 
    for more details.

    You should have received a copy of the OpenNETCF.org Shared Source License 
    along with this library; if not, email licensing@opennetcf.org to request a copy.

    If you wish to contact the OpenNETCF Advisory Board to discuss licensing, please 
    email licensing@opennetcf.org.

    For general enquiries, email enquiries@opennetcf.org or visit our website at:
    http://www.opennetcf.org

=======================================================================================*/
using System;

namespace OpenNETCF.Net.Ftp
{
	/// <summary>
	/// Summary description for FtpStatusCode.
	/// </summary>
	public enum FtpStatusCode // TODO: Check up the actual number code to associate the different enum values with
	{
		ActionNotTaken,
		ArgumentSyntaxError,
		BadCommandSequence,
		CantOpenData,
		ClosingControl,
		ClosingData,
		CommandExtraneous,
		CommandNotImplemented,
		CommandOk,
		CommandSyntaxError,
		DataAlreadyOpen,
		DirectoryStatus,
		EnteringPassive,
		FileActionOk,
		FileCommandPending,
		FileStatus,
		LoggedInProceed,
		NeedLoginAccount,
		NotLoggedIn,
		OpeningData,
		PathnameCreated,
		RestartMarker,
		SendPasswordCommand,
		SendUserCommand
	}
}






















