//==============================================================================
// VirtualComPort.h
//------------------------------------------------------------------------------
// $Id: VirtualComPort.h,v 1.3 2006/05/24 21:32:08 andrew_klopper Exp $
//
// Copyright 2005 Andrew Rowland Klopper (http://gpsproxy.sourceforge.net/)
// 
// This file is part of GPSProxy.
// 
// GPSProxy is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// GPSProxy is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with GPSProxy; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//
//==============================================================================
// Revision History
//------------------------------------------------------------------------------
// $Log: VirtualComPort.h,v $
// Revision 1.3  2006/05/24 21:32:08  andrew_klopper
// Changed to support PPC2002.
//
// Revision 1.2  2005/09/15 23:24:17  andrew_klopper
// no message
//
//==============================================================================
// Functions
//------------------------------------------------------------------------------

#ifdef VIRTUALCOMPORT_EXPORTS
#define VIRTUALCOMPORT_API extern "C" __declspec(dllexport)
#else
#define VIRTUALCOMPORT_API extern "C" __declspec(dllimport)
#endif

#ifdef WCE4_DRIVER
VIRTUALCOMPORT_API DWORD COM_Init(LPCTSTR pContext, LPCVOID lpvBusContext);
#else
VIRTUALCOMPORT_API DWORD COM_Init(DWORD dwContext);
#endif

VIRTUALCOMPORT_API BOOL COM_Deinit(DWORD hDeviceContext);

VIRTUALCOMPORT_API BOOL COM_IOControl(DWORD hOpenContext, DWORD dwCode, PBYTE pBufIn, DWORD dwLenIn,
	PBYTE pBufOut, DWORD dwLenOut, PDWORD pdwActualOut);

VIRTUALCOMPORT_API DWORD COM_Open(DWORD hDeviceContext, DWORD AccessCode, DWORD ShareMode);
VIRTUALCOMPORT_API BOOL COM_Close(DWORD hOpenContext);
VIRTUALCOMPORT_API DWORD COM_Read(DWORD hOpenContext, LPVOID pBuffer, DWORD Count);
VIRTUALCOMPORT_API DWORD COM_Write(DWORD hOpenContext, LPCVOID pBuffer, DWORD Count);
VIRTUALCOMPORT_API DWORD COM_Seek(DWORD hOpenContext, long Amount, WORD Type);
