//==============================================================================
// VirtualComPort.cpp
//------------------------------------------------------------------------------
// $Id: VirtualComPort.cpp,v 1.5 2006/05/24 21:32:08 andrew_klopper Exp $
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
// Notes
//------------------------------------------------------------------------------
// We cannot check whether a pOpenContext is still valid if, for example,
// COM_close and COM_read are called simultaneously, and the COM_close completes
// and frees the pOpenContext before the COM_read starts. We need to lock a
// critical section to do any checks, but this critical section is part of the
// structure which may be freed in between us checking that it is a valid
// pointer and between us trying to lock the critical section. In these cases,
// we just close our eyes and hope for the best (and that the device manager
// somehow tries to address some of these problems by appropriately 
// scheduling COM_xxx calls; haven't found any evidence of this, however).
//
// The sample drivers provided with Microsoft Platform Builder don't give any
// clues on how to do this better, unfortunately.
//
// TODO: Investigate the -1 returns in COM_Read and COM_Write to see if they
// should return 0 instead.
//
//==============================================================================
// Revision History
//------------------------------------------------------------------------------
// $Log: VirtualComPort.cpp,v $
// Revision 1.5  2006/05/24 21:32:08  andrew_klopper
// Changed to support PPC2002.
//
// Revision 1.4  2005/10/16 16:12:23  andrew_klopper
// Fixed debug macros.
//
// Revision 1.3  2005/10/16 16:05:25  andrew_klopper
// Fixed problem where Garmin Que would be unable to reconnect to
// GPSProxy after a disconnect. Added debug logging that can be
// enabled or disabled by a #define.
//
// Revision 1.2  2005/09/15 23:24:17  andrew_klopper
// no message
//
// Revision 1.1  2005/09/15 23:18:45  andrew_klopper
// no message
//
//==============================================================================
// Includes
//------------------------------------------------------------------------------

#include "stdafx.h"
#include <stdio.h>
#include "VirtualComPort.h"

//==============================================================================
// Platform Builder DDK Headers
//------------------------------------------------------------------------------

// Hijack a few values from the Platform Builder 4.2 DDK as these aren't in the
// Pocket PC 2003 SDK.

#define METHOD_BUFFERED					0
#define METHOD_IN_DIRECT				1
#define METHOD_OUT_DIRECT				2
#define METHOD_NEITHER					3

#define FILE_ANY_ACCESS					0
#define FILE_READ_ACCESS				1
#define FILE_WRITE_ACCESS				2

#define FILE_DEVICE_SERIAL_PORT			0x0000001b
#define FILE_DEVICE_PSL					0x00000103

#define CTL_CODE(DeviceType, Function, Method, Access) ( \
	((DeviceType) << 16) | ((Access) << 14) | ((Function) << 2) | (Method) \
)

#define IOCTL_SERIAL_SET_BREAK_ON		CTL_CODE(FILE_DEVICE_SERIAL_PORT, 1,METHOD_BUFFERED,FILE_ANY_ACCESS)
#define IOCTL_SERIAL_SET_BREAK_OFF		CTL_CODE(FILE_DEVICE_SERIAL_PORT, 2,METHOD_BUFFERED,FILE_ANY_ACCESS)
#define IOCTL_SERIAL_SET_DTR			CTL_CODE(FILE_DEVICE_SERIAL_PORT, 3,METHOD_BUFFERED,FILE_ANY_ACCESS)
#define IOCTL_SERIAL_CLR_DTR			CTL_CODE(FILE_DEVICE_SERIAL_PORT, 4,METHOD_BUFFERED,FILE_ANY_ACCESS)
#define IOCTL_SERIAL_SET_RTS			CTL_CODE(FILE_DEVICE_SERIAL_PORT, 5,METHOD_BUFFERED,FILE_ANY_ACCESS)
#define IOCTL_SERIAL_CLR_RTS			CTL_CODE(FILE_DEVICE_SERIAL_PORT, 6,METHOD_BUFFERED,FILE_ANY_ACCESS)
#define IOCTL_SERIAL_SET_XOFF			CTL_CODE(FILE_DEVICE_SERIAL_PORT, 7,METHOD_BUFFERED,FILE_ANY_ACCESS)
#define IOCTL_SERIAL_SET_XON			CTL_CODE(FILE_DEVICE_SERIAL_PORT, 8,METHOD_BUFFERED,FILE_ANY_ACCESS)
#define IOCTL_SERIAL_GET_WAIT_MASK		CTL_CODE(FILE_DEVICE_SERIAL_PORT, 9,METHOD_BUFFERED,FILE_ANY_ACCESS)
#define IOCTL_SERIAL_SET_WAIT_MASK		CTL_CODE(FILE_DEVICE_SERIAL_PORT,10,METHOD_BUFFERED,FILE_ANY_ACCESS)
#define IOCTL_SERIAL_WAIT_ON_MASK		CTL_CODE(FILE_DEVICE_SERIAL_PORT,11,METHOD_BUFFERED,FILE_ANY_ACCESS)
#define IOCTL_SERIAL_GET_COMMSTATUS		CTL_CODE(FILE_DEVICE_SERIAL_PORT,12,METHOD_BUFFERED,FILE_ANY_ACCESS)
#define IOCTL_SERIAL_GET_MODEMSTATUS	CTL_CODE(FILE_DEVICE_SERIAL_PORT,13,METHOD_BUFFERED,FILE_ANY_ACCESS)
#define IOCTL_SERIAL_GET_PROPERTIES		CTL_CODE(FILE_DEVICE_SERIAL_PORT,14,METHOD_BUFFERED,FILE_ANY_ACCESS)
#define IOCTL_SERIAL_SET_TIMEOUTS		CTL_CODE(FILE_DEVICE_SERIAL_PORT,15,METHOD_BUFFERED,FILE_ANY_ACCESS)
#define IOCTL_SERIAL_GET_TIMEOUTS		CTL_CODE(FILE_DEVICE_SERIAL_PORT,16,METHOD_BUFFERED,FILE_ANY_ACCESS)
#define IOCTL_SERIAL_PURGE				CTL_CODE(FILE_DEVICE_SERIAL_PORT,17,METHOD_BUFFERED,FILE_ANY_ACCESS)
#define IOCTL_SERIAL_SET_QUEUE_SIZE		CTL_CODE(FILE_DEVICE_SERIAL_PORT,18,METHOD_BUFFERED,FILE_ANY_ACCESS)
#define IOCTL_SERIAL_IMMEDIATE_CHAR		CTL_CODE(FILE_DEVICE_SERIAL_PORT,19,METHOD_BUFFERED,FILE_ANY_ACCESS)
#define IOCTL_SERIAL_GET_DCB			CTL_CODE(FILE_DEVICE_SERIAL_PORT,20,METHOD_BUFFERED,FILE_ANY_ACCESS)
#define IOCTL_SERIAL_SET_DCB			CTL_CODE(FILE_DEVICE_SERIAL_PORT,21,METHOD_BUFFERED,FILE_ANY_ACCESS)
#define IOCTL_SERIAL_ENABLE_IR			CTL_CODE(FILE_DEVICE_SERIAL_PORT,22,METHOD_BUFFERED,FILE_ANY_ACCESS)
#define IOCTL_SERIAL_DISABLE_IR			CTL_CODE(FILE_DEVICE_SERIAL_PORT,23,METHOD_BUFFERED,FILE_ANY_ACCESS)

#define IOCTL_PSL_NOTIFY				CTL_CODE(FILE_DEVICE_PSL, 255, METHOD_NEITHER, FILE_ANY_ACCESS)

typedef struct _SERIAL_DEV_STATUS {
	DWORD	Errors;
	COMSTAT	ComStat;
} SERIAL_DEV_STATUS, *PSERIAL_DEV_STATUS;

typedef struct _SERIAL_QUEUE_SIZES {
	DWORD	cbInQueue;
	DWORD	cbOutQueue;
} SERIAL_QUEUE_SIZES, *PSERIAL_QUEUE_SIZES;

//==============================================================================
// Constants
//------------------------------------------------------------------------------

#define DEFAULT_BUFFER_SIZE	512

//==============================================================================
// Macros
//------------------------------------------------------------------------------

#define MIN(x, y)	((x) < (y) ? (x) : (y))

//==============================================================================
// Types
//------------------------------------------------------------------------------

typedef struct _VIRTUALCOMPORT_DEVICE_CONTEXT VIRTUALCOMPORT_DEVICE_CONTEXT, *PVIRTUALCOMPORT_DEVICE_CONTEXT;

typedef struct _VIRTUALCOMPORT_OPEN_CONTEXT {
	PVIRTUALCOMPORT_DEVICE_CONTEXT		pDeviceContext;
	struct _VIRTUALCOMPORT_OPEN_CONTEXT *pPairedOpenContext;
	BOOL								BlockedReader;
	BOOL								BlockedWriter;
	DCB									DeviceControlBlock;
	SERIAL_QUEUE_SIZES					SerialQueueSizes;
	SERIAL_DEV_STATUS					SerialDeviceStatus;
	COMMTIMEOUTS						CommTimeouts;
	COMMPROP							CommProperties;
	DWORD								ModemStatus;
	DWORD								WaitMask;
	HANDLE								WaitReadEvent;
	HANDLE								WaitWriteEvent;
	HANDLE								AbortWaitEvent;
	HANDLE								ReadEvent;
	HANDLE								AbortReadEvent;
	HANDLE								WriteEvent;
	HANDLE								AbortWriteEvent;
	unsigned char						*pInQueue;
	DWORD								InQueueSize;
	DWORD								InQueueBytesUsed;
} VIRTUALCOMPORT_OPEN_CONTEXT, *PVIRTUALCOMPORT_OPEN_CONTEXT;

struct _VIRTUALCOMPORT_DEVICE_CONTEXT {
    CRITICAL_SECTION				CriticalSection;
    LONG							OpenCount;
	PVIRTUALCOMPORT_OPEN_CONTEXT	pOpenContext[2];
};

//==============================================================================
// Function: WriteDebugMsg
//------------------------------------------------------------------------------

// #define DEBUGLOG

#ifdef DEBUGLOG

	static FILE *DebugLog = NULL;
	static LONG last_dbg_id = 0;

	// You must enclose the parameters to LOGDEBUGMSGxxx inside two sets of
	// parentheses so that they are treated as a single parameter that is
	// pasted with its enclosing parentheses after the LogDebugMsg function
	// call.
#	define LOGDEBUGMSGSTART(printf_exp)		LONG DBGID = LogDebugMsgStart printf_exp
#	define LOGDEBUGMSG(params)				LogDebugMsg params
#	define LOGDEBUGWAITMASK(prefix, mask)	LogDebugWaitMask(prefix, mask)

	static void
	VLogDebugMsg(LONG dbg_id, const char *format, va_list args)
	{
		if (DebugLog) {
			DWORD ticks = GetTickCount();
			// TODO: this should be atomic
			fprintf(DebugLog, "%03d:%02d:%02d.%03d %08X ",
				ticks / 3600000, (ticks % 3600000) / 60000,
				(ticks % 60000) / 1000, ticks % 1000, dbg_id);
			vfprintf(DebugLog, format, args);
			fflush(DebugLog);
		}
	}

	static void
	LogDebugMsg(LONG dbg_id, const char *format, ...)
	{
		va_list args;
		va_start(args, format);
		VLogDebugMsg(dbg_id, format, args);
		va_end(args);
	}

	static LONG
	LogDebugMsgStart(const char *format, ...)
	{
		LONG dbg_id = InterlockedIncrement(&last_dbg_id);
		va_list args;
		va_start(args, format);
		VLogDebugMsg(dbg_id, format, args);
		va_end(args);
		return dbg_id;
	}

	static void
	LogDebugWaitMask(const char *prefix, DWORD mask)
	{
	}

	static const char *
	GetIOControlName(DWORD ioctl)
	{
		switch (ioctl) {
		case IOCTL_SERIAL_SET_BREAK_ON:
			return "IOCTL_SERIAL_SET_BREAK_ON";
		case IOCTL_SERIAL_SET_BREAK_OFF:
			return "IOCTL_SERIAL_SET_BREAK_OFF";
		case IOCTL_SERIAL_SET_DTR:
			return "IOCTL_SERIAL_SET_DTR";
		case IOCTL_SERIAL_CLR_DTR:
			return "IOCTL_SERIAL_CLR_DTR";
		case IOCTL_SERIAL_SET_RTS:
			return "IOCTL_SERIAL_SET_RTS";
		case IOCTL_SERIAL_CLR_RTS:
			return "IOCTL_SERIAL_CLR_RTS";
		case IOCTL_SERIAL_SET_XOFF:
			return "IOCTL_SERIAL_SET_XOFF";
		case IOCTL_SERIAL_SET_XON:
			return "IOCTL_SERIAL_SET_XON";
		case IOCTL_SERIAL_GET_WAIT_MASK:
			return "IOCTL_SERIAL_GET_WAIT_MASK";
		case IOCTL_SERIAL_SET_WAIT_MASK:
			return "IOCTL_SERIAL_SET_WAIT_MASK";
		case IOCTL_SERIAL_WAIT_ON_MASK:
			return "IOCTL_SERIAL_WAIT_ON_MASK";
		case IOCTL_SERIAL_GET_COMMSTATUS:
			return "IOCTL_SERIAL_GET_COMMSTATUS";
		case IOCTL_SERIAL_GET_MODEMSTATUS:
			return "IOCTL_SERIAL_GET_MODEMSTATUS";
		case IOCTL_SERIAL_GET_PROPERTIES:
			return "IOCTL_SERIAL_GET_PROPERTIES";
		case IOCTL_SERIAL_SET_TIMEOUTS:
			return "IOCTL_SERIAL_SET_TIMEOUTS";
		case IOCTL_SERIAL_GET_TIMEOUTS:
			return "IOCTL_SERIAL_GET_TIMEOUTS";
		case IOCTL_SERIAL_PURGE:
			return "IOCTL_SERIAL_PURGE";
		case IOCTL_SERIAL_SET_QUEUE_SIZE:
			return "IOCTL_SERIAL_SET_QUEUE_SIZE";
		case IOCTL_SERIAL_IMMEDIATE_CHAR:
			return "IOCTL_SERIAL_IMMEDIATE_CHAR";
		case IOCTL_SERIAL_GET_DCB:
			return "IOCTL_SERIAL_GET_DCB";
		case IOCTL_SERIAL_SET_DCB:
			return "IOCTL_SERIAL_SET_DCB";
		case IOCTL_SERIAL_ENABLE_IR:
			return "IOCTL_SERIAL_ENABLE_IR";
		case IOCTL_SERIAL_DISABLE_IR:
			return "IOCTL_SERIAL_DISABLE_IR";
		}
		return NULL;
	}

#else

#	define LOGDEBUGMSGSTART(printf_exp)
#	define LOGDEBUGMSG(params)
#	define LOGDEBUGWAITMASK(prefix, mask)

#endif

//==============================================================================
// Function: SetupCommProperties
//------------------------------------------------------------------------------

static void
SetupCommProperties(COMMPROP *pCommProperties)
{
	// Set up some dummy values for the COMM properties. We ignore most of the
	// settings we claim to support anyway, but we may as well appear to be a
	// reasonably functional driver...
	pCommProperties->wPacketLength = sizeof(COMMPROP);
	pCommProperties->wPacketVersion = 1; // FIXME???
	pCommProperties->dwServiceMask = SP_SERIALCOMM;
	pCommProperties->dwMaxTxQueue = 0;
	pCommProperties->dwMaxRxQueue = 0;
	pCommProperties->dwMaxBaud = BAUD_USER;
	pCommProperties->dwProvSubType = PST_RS232;
	pCommProperties->dwProvCapabilities = PCF_TOTALTIMEOUTS | PCF_PARITY_CHECK;
	pCommProperties->dwSettableParams = SP_BAUD | SP_DATABITS | SP_HANDSHAKING |
		SP_PARITY | SP_PARITY_CHECK | SP_STOPBITS;
	pCommProperties->dwSettableBaud = BAUD_USER;
	pCommProperties->wSettableData = DATABITS_7 | DATABITS_8;
	pCommProperties->wSettableStopParity = PARITY_EVEN | PARITY_MARK |
		PARITY_NONE | PARITY_ODD | PARITY_SPACE | STOPBITS_10 | STOPBITS_15 |
		STOPBITS_20;
	pCommProperties->dwCurrentTxQueue = 0;
	pCommProperties->dwCurrentRxQueue = 0;
	pCommProperties->dwProvSpec1 = 0;
	pCommProperties->dwProvSpec2 = 0;
	pCommProperties->wcProvChar[0] = 0;
}

//==============================================================================
// Function: SetupCommTimeouts
//------------------------------------------------------------------------------

static void
SetupCommTimeouts(COMMTIMEOUTS *pCommTimeouts)
{
	// Choose some timeout values.
	pCommTimeouts->ReadIntervalTimeout = MAXDWORD;
	pCommTimeouts->ReadTotalTimeoutMultiplier = MAXDWORD;
	pCommTimeouts->ReadTotalTimeoutConstant = 0;
	pCommTimeouts->WriteTotalTimeoutMultiplier = 0;
	pCommTimeouts->WriteTotalTimeoutConstant = 0;
}

//==============================================================================
// Function: SetupDCB
//------------------------------------------------------------------------------

static void
SetupDCB(DCB *pDeviceControlBlock)
{
	// Set up a reasonable-looking DCB, even if we ignore it completely from now
	// on...
	pDeviceControlBlock->DCBlength = sizeof(DCB);
	pDeviceControlBlock->BaudRate = CBR_115200;
	pDeviceControlBlock->fBinary = TRUE;
	pDeviceControlBlock->fParity = FALSE;
	pDeviceControlBlock->fOutxCtsFlow = FALSE;
	pDeviceControlBlock->fOutxDsrFlow = FALSE;
	pDeviceControlBlock->fDtrControl = DTR_CONTROL_DISABLE;
	pDeviceControlBlock->fDsrSensitivity = FALSE;
	pDeviceControlBlock->fTXContinueOnXoff = TRUE;
	pDeviceControlBlock->fOutX = FALSE;
	pDeviceControlBlock->fInX = FALSE;
	pDeviceControlBlock->fErrorChar = FALSE;
	pDeviceControlBlock->fNull = FALSE;
	pDeviceControlBlock->fRtsControl = FALSE;
	pDeviceControlBlock->fAbortOnError = FALSE;
	pDeviceControlBlock->fDummy2 = 0;
	pDeviceControlBlock->wReserved = 0;
	pDeviceControlBlock->XonLim = 0;
	pDeviceControlBlock->XoffLim = 0;
	pDeviceControlBlock->ByteSize = 8;
	pDeviceControlBlock->Parity = NOPARITY;
	pDeviceControlBlock->StopBits = ONESTOPBIT;
	pDeviceControlBlock->XonChar = '\x11';
	pDeviceControlBlock->XoffChar = '\x13';
	pDeviceControlBlock->ErrorChar = '\x15';
	pDeviceControlBlock->EofChar = '\x04';
	pDeviceControlBlock->EvtChar = '\0';
	pDeviceControlBlock->wReserved1 = 0;
}

//==============================================================================
// Function: FreeOpenContext
//------------------------------------------------------------------------------

static void
FreeOpenContext(PVIRTUALCOMPORT_OPEN_CONTEXT *pOpenContextRef)
{
	if (*pOpenContextRef) {
		if ((*pOpenContextRef)->WaitReadEvent)
			CloseHandle((*pOpenContextRef)->WaitReadEvent);
		if ((*pOpenContextRef)->WaitWriteEvent)
			CloseHandle((*pOpenContextRef)->WaitWriteEvent);
		if ((*pOpenContextRef)->AbortWaitEvent)
			CloseHandle((*pOpenContextRef)->AbortWaitEvent);
		if ((*pOpenContextRef)->ReadEvent)
			CloseHandle((*pOpenContextRef)->ReadEvent);
		if ((*pOpenContextRef)->AbortReadEvent)
			CloseHandle((*pOpenContextRef)->AbortReadEvent);
		if ((*pOpenContextRef)->WriteEvent)
			CloseHandle((*pOpenContextRef)->WriteEvent);
		if ((*pOpenContextRef)->AbortWriteEvent)
			CloseHandle((*pOpenContextRef)->AbortWriteEvent);
		if ((*pOpenContextRef)->pInQueue)
			LocalFree((*pOpenContextRef)->pInQueue);
		LocalFree(*pOpenContextRef);
		*pOpenContextRef = NULL;
	}
}

//==============================================================================
// Function: DllMain
//------------------------------------------------------------------------------

BOOL APIENTRY
DllMain(HANDLE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
    switch (ul_reason_for_call)
	{
		case DLL_PROCESS_ATTACH:
			DisableThreadLibraryCalls((HMODULE)hModule);
#ifdef DEBUGLOG
			// Open the debug log file.
			DebugLog = fopen("\\vcpdebug.txt", "w");
			LogDebugMsg(0, "Attached...\n");
#endif
			break;

		case DLL_PROCESS_DETACH:
#ifdef DEBUGLOG
			LogDebugMsg(0, "Detached...\n");
			if (DebugLog)
				fclose(DebugLog);
#endif
			break;

		case DLL_THREAD_ATTACH:
		case DLL_THREAD_DETACH:
			break;
    }
    return TRUE;
}

//==============================================================================
// Function: COM_Init
//------------------------------------------------------------------------------

#ifdef WCE4_DRIVER
VIRTUALCOMPORT_API DWORD
COM_Init(LPCTSTR pContext, LPCVOID lpvBusContext)
{
	LOGDEBUGMSGSTART(("COM_Init(%p, %p)\n", pContext, lpvBusContext));
#else
VIRTUALCOMPORT_API DWORD
COM_Init(DWORD dwContext)
{
	LOGDEBUGMSGSTART(("COM_Init(%p)\n", dwContext));
#endif

	// Allocate memory for the device context.
	PVIRTUALCOMPORT_DEVICE_CONTEXT pDeviceContext =
		(PVIRTUALCOMPORT_DEVICE_CONTEXT)LocalAlloc(LPTR,
			sizeof(VIRTUALCOMPORT_DEVICE_CONTEXT));
	if (pDeviceContext == NULL) {
		SetLastError(ERROR_NOT_ENOUGH_MEMORY);
		LOGDEBUGMSG((DBGID, "COM_Init -> 0\n"));
		return 0;
	}

	// Initialise the critical section.
	InitializeCriticalSection(&pDeviceContext->CriticalSection);

	LOGDEBUGMSG((DBGID, "COM_Init -> %p\n", pDeviceContext));
	return (DWORD)pDeviceContext;
}

//==============================================================================
// Function: COM_Deinit
//------------------------------------------------------------------------------

VIRTUALCOMPORT_API BOOL
COM_Deinit(DWORD hDeviceContext)
{
	LOGDEBUGMSGSTART(("COM_Deinit(%p)\n", hDeviceContext));

	PVIRTUALCOMPORT_DEVICE_CONTEXT pDeviceContext =
		(PVIRTUALCOMPORT_DEVICE_CONTEXT)hDeviceContext;
	if (pDeviceContext != NULL) {
		EnterCriticalSection(&pDeviceContext->CriticalSection);
		
		for (int i = 0; i < pDeviceContext->OpenCount; i++) {
			SetEvent(pDeviceContext->pOpenContext[i]->AbortWaitEvent);
			SetEvent(pDeviceContext->pOpenContext[i]->AbortReadEvent);
			SetEvent(pDeviceContext->pOpenContext[i]->AbortWriteEvent);
			FreeOpenContext(&pDeviceContext->pOpenContext[i]);
		}

		LeaveCriticalSection(&pDeviceContext->CriticalSection);

		DeleteCriticalSection(&pDeviceContext->CriticalSection);
		LocalFree(pDeviceContext);
	}

	LOGDEBUGMSG((DBGID, "COM_Deinit -> TRUE\n"));
	return TRUE;
}

//==============================================================================
// Function: COM_Open
//------------------------------------------------------------------------------

VIRTUALCOMPORT_API DWORD
COM_Open(DWORD hDeviceContext, DWORD AccessCode, DWORD ShareMode)
{
	LOGDEBUGMSGSTART(("COM_Open(%p, %x, %x)\n", hDeviceContext, AccessCode, ShareMode));

	PVIRTUALCOMPORT_DEVICE_CONTEXT pDeviceContext =
		(PVIRTUALCOMPORT_DEVICE_CONTEXT)hDeviceContext;
	if (! pDeviceContext) {
		SetLastError(ERROR_INVALID_HANDLE);
		LOGDEBUGMSG((DBGID, "COM_Open -> 0\n"));
		return 0;
	}

	PVIRTUALCOMPORT_OPEN_CONTEXT pOpenContext = NULL;

	EnterCriticalSection(&pDeviceContext->CriticalSection);

	if (pDeviceContext->OpenCount < 2) {
		pOpenContext = (PVIRTUALCOMPORT_OPEN_CONTEXT)LocalAlloc(LPTR,
				sizeof(VIRTUALCOMPORT_OPEN_CONTEXT));
		if (pOpenContext) {
			pOpenContext->SerialQueueSizes.cbInQueue = DEFAULT_BUFFER_SIZE;
			pOpenContext->SerialQueueSizes.cbOutQueue = DEFAULT_BUFFER_SIZE;
			
			pOpenContext->InQueueSize = pOpenContext->SerialQueueSizes.cbInQueue;
			pOpenContext->pInQueue = (unsigned char *)LocalAlloc(LPTR, pOpenContext->InQueueSize);
			
			pOpenContext->WaitReadEvent = CreateEvent(NULL, FALSE, FALSE, NULL);
			pOpenContext->WaitWriteEvent = CreateEvent(NULL, FALSE, FALSE, NULL);
			pOpenContext->AbortWaitEvent = CreateEvent(NULL, FALSE, FALSE, NULL);
			pOpenContext->ReadEvent = CreateEvent(NULL, FALSE, FALSE, NULL);
			pOpenContext->AbortReadEvent = CreateEvent(NULL, FALSE, FALSE, NULL);
			pOpenContext->WriteEvent = CreateEvent(NULL, FALSE, FALSE, NULL);
			pOpenContext->AbortWriteEvent = CreateEvent(NULL, FALSE, FALSE, NULL);

			SetupCommProperties(&pOpenContext->CommProperties);
			SetupCommTimeouts(&pOpenContext->CommTimeouts);
			SetupDCB(&pOpenContext->DeviceControlBlock);

			pOpenContext->pDeviceContext = pDeviceContext;

			if (pOpenContext->pInQueue && pOpenContext->WaitReadEvent &&
				pOpenContext->WaitWriteEvent && pOpenContext->AbortWaitEvent &&
				pOpenContext->ReadEvent && pOpenContext->AbortReadEvent &&
				pOpenContext->WriteEvent && pOpenContext->AbortWriteEvent)
			{
				if (pDeviceContext->OpenCount > 0) {
					unsigned char *buf;

					PVIRTUALCOMPORT_OPEN_CONTEXT pPairedOpenContext =
						pDeviceContext->pOpenContext[0];
					pOpenContext->pPairedOpenContext = pPairedOpenContext;

					buf = (unsigned char *)LocalReAlloc(pOpenContext->pInQueue,
						pOpenContext->SerialQueueSizes.cbInQueue +
						pPairedOpenContext->SerialQueueSizes.cbOutQueue,
						LMEM_MOVEABLE);
					if (buf) {
						pOpenContext->pInQueue = buf;
						pOpenContext->InQueueSize =
							pOpenContext->SerialQueueSizes.cbInQueue +
							pPairedOpenContext->SerialQueueSizes.cbOutQueue;
						buf = (unsigned char *)LocalReAlloc(pPairedOpenContext->pInQueue,
							pPairedOpenContext->SerialQueueSizes.cbInQueue +
							pOpenContext->SerialQueueSizes.cbOutQueue,
							LMEM_MOVEABLE);
						if (buf) {
							pPairedOpenContext->pInQueue = buf;
							pPairedOpenContext->InQueueSize =
								pPairedOpenContext->SerialQueueSizes.cbInQueue +
								pOpenContext->SerialQueueSizes.cbOutQueue;
							pPairedOpenContext->InQueueBytesUsed =
								MIN(pPairedOpenContext->InQueueBytesUsed,
									pPairedOpenContext->InQueueSize);
							pPairedOpenContext->pPairedOpenContext =
								pOpenContext;
						} else {
							// LocalReAlloc will set LastError if it fails.
							FreeOpenContext(&pOpenContext);
						}
					} else {
						// LocalReAlloc will set LastError if it fails.
						FreeOpenContext(&pOpenContext);
					}
				}

				if (pOpenContext) {
					pDeviceContext->pOpenContext[pDeviceContext->OpenCount] =
						pOpenContext;
					pDeviceContext->OpenCount++;
				}
			} else {
				FreeOpenContext(&pOpenContext);
				SetLastError(ERROR_NO_SYSTEM_RESOURCES);
			}
		} else {
			SetLastError(ERROR_NOT_ENOUGH_MEMORY);
		}
	} else {
		SetLastError(ERROR_NO_SYSTEM_RESOURCES);
	}
	
	LeaveCriticalSection(&pDeviceContext->CriticalSection);

	LOGDEBUGMSG((DBGID, "COM_Open -> %p\n", pOpenContext));
	return (DWORD)pOpenContext;
}

//==============================================================================
// Function: COM_Close
//------------------------------------------------------------------------------

VIRTUALCOMPORT_API BOOL
COM_Close(DWORD hOpenContext)
{
	LOGDEBUGMSGSTART(("COM_Close(%p)\n", hOpenContext));

	PVIRTUALCOMPORT_OPEN_CONTEXT pOpenContext =
		(PVIRTUALCOMPORT_OPEN_CONTEXT)hOpenContext;
	if (! pOpenContext) {
		SetLastError(ERROR_INVALID_HANDLE);
		LOGDEBUGMSG((DBGID, "COM_Close -> FALSE\n"));
		return FALSE;
	}
	PVIRTUALCOMPORT_DEVICE_CONTEXT pDeviceContext = pOpenContext->pDeviceContext;

	// Lock the device context to prevent any concurrent opens or closes.
	EnterCriticalSection(&pDeviceContext->CriticalSection);

	// Abort any active reads and writes.
	pOpenContext->BlockedReader = FALSE;
	pOpenContext->BlockedWriter = FALSE;
	SetEvent(pOpenContext->AbortWaitEvent);
	SetEvent(pOpenContext->AbortReadEvent);
	SetEvent(pOpenContext->AbortWriteEvent);

	// Unpair the paired open context, if any.
	if (pOpenContext->pPairedOpenContext) {
		unsigned char *buf;
		pOpenContext->pPairedOpenContext->pPairedOpenContext = NULL;
		buf = (unsigned char *)LocalReAlloc(pOpenContext->pPairedOpenContext->pInQueue,
			pOpenContext->pPairedOpenContext->SerialQueueSizes.cbInQueue,
			LMEM_MOVEABLE);
		if (buf) {
			pOpenContext->pPairedOpenContext->pInQueue = buf;
			pOpenContext->pPairedOpenContext->InQueueSize =
				pOpenContext->pPairedOpenContext->SerialQueueSizes.cbInQueue;
		} // else simply leave the buffer size as is
		pOpenContext->pPairedOpenContext->InQueueBytesUsed = 0;

		// Signal write events to the paired context so that all blocking writes
		// will complete. We leave blocked reads alone because they might complete
		// successfully at a later point if this end of the connection is opened
		// again.
		SetEvent(pOpenContext->pPairedOpenContext->WaitWriteEvent);
		SetEvent(pOpenContext->pPairedOpenContext->WriteEvent);
	}

	// Remove the reference to the open context from the device context.
	int i = 0;
	while ((i < pDeviceContext->OpenCount) &&
		(pDeviceContext->pOpenContext[i] != pOpenContext))
		i++;
	if (i < pDeviceContext->OpenCount) {
		for (int j = i + 1; j < pDeviceContext->OpenCount; j++)
			pDeviceContext->pOpenContext[j - 1] = pDeviceContext->pOpenContext[j];
		pDeviceContext->OpenCount--;
	}

	// Unlock the device context.
	LeaveCriticalSection(&pDeviceContext->CriticalSection);

	// Clean up. We rely on the application not to try a read or write during or
	// after a close.
	FreeOpenContext(&pOpenContext);

	LOGDEBUGMSG((DBGID, "COM_Close -> TRUE\n"));
	return TRUE;
}

//==============================================================================
// Function: COM_IOControl
//------------------------------------------------------------------------------

VIRTUALCOMPORT_API BOOL
COM_IOControl(DWORD hOpenContext, DWORD dwCode, PBYTE pBufIn, DWORD dwLenIn,
	PBYTE pBufOut, DWORD dwLenOut, PDWORD pdwActualOut)
{
#ifdef DEBUGLOG
	LONG DBGID = InterlockedIncrement(&last_dbg_id);
	if (GetIOControlName(dwCode))
		LOGDEBUGMSG((DBGID, "COM_IOControl(%p, %s, %p, %d, %p, %d, %p)\n",
			hOpenContext, GetIOControlName(dwCode), pBufIn, dwLenIn, pBufOut, dwLenOut, pdwActualOut));
	else
		LOGDEBUGMSG((DBGID, "COM_IOControl(%p, %x, %p, %d, %p, %d, %p)\n",
			hOpenContext, dwCode, pBufIn, dwLenIn, pBufOut, dwLenOut, pdwActualOut));
#endif

	PVIRTUALCOMPORT_OPEN_CONTEXT pOpenContext =
		(PVIRTUALCOMPORT_OPEN_CONTEXT)hOpenContext;
	if (! pOpenContext) {
		SetLastError(ERROR_INVALID_HANDLE);
		LOGDEBUGMSG((DBGID, "COM_IOControl -> FALSE\n"));
		return FALSE;
	}
	PVIRTUALCOMPORT_DEVICE_CONTEXT pDeviceContext = pOpenContext->pDeviceContext;

	BOOL ret = FALSE;

	EnterCriticalSection(&pDeviceContext->CriticalSection);

	switch (dwCode) {
	case IOCTL_PSL_NOTIFY:
		pOpenContext->BlockedReader = FALSE;
		pOpenContext->BlockedWriter = FALSE;
		SetEvent(pOpenContext->AbortWaitEvent);
		SetEvent(pOpenContext->AbortReadEvent);
		SetEvent(pOpenContext->AbortWriteEvent);
		break;
	
	case IOCTL_SERIAL_GET_WAIT_MASK:
		if (! pBufOut || ! pdwActualOut || (dwLenOut < sizeof(DWORD))) {
			SetLastError(ERROR_INVALID_PARAMETER);
			break;
		}
		*(DWORD *)pBufOut = pOpenContext->WaitMask;
		*pdwActualOut = sizeof(DWORD);
		LOGDEBUGWAITMASK("COM_IOControl: GetWaitMask = ", pOpenContext->WaitMask);
		ret = TRUE;
        break;
	
	case IOCTL_SERIAL_SET_WAIT_MASK:
		if (! pBufIn || (dwLenIn < sizeof(DWORD))) {
			SetLastError(ERROR_INVALID_PARAMETER);
			break;
		}
		pOpenContext->WaitMask = *(DWORD *)pBufIn;
		SetEvent(pOpenContext->AbortWaitEvent);
		LOGDEBUGWAITMASK("COM_IOControl: SetWaitMask = ", pOpenContext->WaitMask);
		ret = TRUE;
		break;
	
	case IOCTL_SERIAL_WAIT_ON_MASK:
	{
		// Check that we have an output buffer for the result.
		if (! pBufOut || ! pdwActualOut || (dwLenOut < sizeof(DWORD))) {
			SetLastError(ERROR_INVALID_PARAMETER);
			break;
		}

		// We arbitrarily decreee that this is not allowed in conjunction with
		// a read or a write.
		if (pOpenContext->BlockedReader || pOpenContext->BlockedWriter) {
			SetLastError(ERROR_INVALID_PARAMETER); // FIXME
			break;
		}

		// Check if we can immediately return a read event. We never immediately
		// return a TXEMPTY otherwise this call would never block with an empty
		// input buffer on the other end.
		DWORD events = 0;
		if ((pOpenContext->WaitMask & EV_RXCHAR) &&
			pOpenContext->InQueueBytesUsed)
			events |= EV_RXCHAR;
		if (events) {
			*(DWORD *)pBufOut = events;
			*pdwActualOut = sizeof(DWORD);
			ret = TRUE;
			break;
		}

		// We might need to wait multiple times for certain events (EV_TXEMPTY
		// for example).
		while (TRUE) {
			// Reset the events in which we may be interested, since we are only
			// interested in new events that occur after we have left the critical
			// section.
			ResetEvent(pOpenContext->WaitReadEvent);
			ResetEvent(pOpenContext->WaitWriteEvent);
			ResetEvent(pOpenContext->AbortWaitEvent);

			// Setup the wait handles.
			HANDLE waitHandles[3];
			waitHandles[0] = pOpenContext->WaitReadEvent;
			waitHandles[1] = pOpenContext->WaitWriteEvent;
			waitHandles[2] = pOpenContext->AbortWaitEvent;

			// Leave the critical section.
			LeaveCriticalSection(&pDeviceContext->CriticalSection);

			// Wait for an event. The only way to abort this is to change the
			// wait mask from another thread.
			DWORD waitResult = WaitForMultipleObjects(3, waitHandles, FALSE, INFINITE);

			// Check if we were aborted.
			if (waitResult == WAIT_OBJECT_0 + 2) {
				SetLastError(0);
				LOGDEBUGMSG((DBGID, "COM_IOControl -> FALSE\n"));
				return FALSE;
			}

			// Re-enter the critical section.
			EnterCriticalSection(&pDeviceContext->CriticalSection);

			// Check that the open context is still valid in case it was freed between
			// the time we got the event and the time we entered the critical section.
			// There is nothing we can do if it was freed after leaving the critical
			// section and before the wait.
			int i = 0;
			while (i < pDeviceContext->OpenCount) {
				if (pDeviceContext->pOpenContext[i] == pOpenContext)
					break;
				i++;
			}
			if (i >= pDeviceContext->OpenCount) {
				SetLastError(ERROR_INVALID_HANDLE);
				break;
			}

			// If we didn't get a read or a write event then an error occurred.
			// SetLastError will already have been called by the wait.
			if ((waitResult != WAIT_OBJECT_0) && (waitResult != WAIT_OBJECT_0 + 1))
				break;

			// Check for read events if required.
			if (pOpenContext->WaitMask & EV_RXCHAR && (waitResult == WAIT_OBJECT_0))
				events |= EV_RXCHAR;
			if (pOpenContext->WaitMask & EV_TXEMPTY && (waitResult == WAIT_OBJECT_0 + 1)) {
				if (pOpenContext->pPairedOpenContext) {
					// Check if the virtual 'output buffer' is empty.
					if (pOpenContext->pPairedOpenContext->InQueueBytesUsed <=
						pOpenContext->pPairedOpenContext->SerialQueueSizes.cbInQueue)
						events |= EV_TXEMPTY;
				} else {
					// If there is no paired context then assume the write
					// succeeded (there must have been one otherwise we wouldn't
					// have go the event.
					events |= EV_TXEMPTY;
				}
			}

			// If we got any events, then exit the loop.
			if (events) {
				*(DWORD *)pBufOut = events;
				*pdwActualOut = sizeof(DWORD);
				ret = TRUE;
				break;
			}
		}

		break;
	}
	
	case IOCTL_SERIAL_GET_COMMSTATUS:
		if (! pBufOut || ! pdwActualOut ||
			(dwLenOut < sizeof(SERIAL_DEV_STATUS))) {
			SetLastError(ERROR_INVALID_PARAMETER);
			break;
		}
		*(SERIAL_DEV_STATUS *)pBufOut = pOpenContext->SerialDeviceStatus;
		*pdwActualOut = sizeof(SERIAL_DEV_STATUS);
		ret = TRUE;
		break;
	
	case IOCTL_SERIAL_GET_MODEMSTATUS:
		if (! pBufOut || ! pdwActualOut || (dwLenOut < sizeof(DWORD))) {
			SetLastError(ERROR_INVALID_PARAMETER);
			break;
		}
		*(DWORD *)pBufOut = pOpenContext->ModemStatus;
		*pdwActualOut = sizeof(DWORD);
		ret = TRUE;
		break;
	
	case IOCTL_SERIAL_GET_PROPERTIES:
		if (! pBufOut || ! pdwActualOut || (dwLenOut < sizeof(COMMPROP))) {
			SetLastError(ERROR_INVALID_PARAMETER);
			break;
		}
		*(COMMPROP *)pBufOut = pOpenContext->CommProperties;
		*pdwActualOut = sizeof(COMMPROP);
		ret = TRUE;
		break;
	
	case IOCTL_SERIAL_SET_TIMEOUTS:
		if (! pBufIn || (dwLenIn < sizeof(COMMTIMEOUTS))) {
			SetLastError(ERROR_INVALID_PARAMETER);
			break;
		}
		pOpenContext->CommTimeouts = *(COMMTIMEOUTS *)pBufIn;
		pOpenContext->BlockedReader = FALSE;
		pOpenContext->BlockedWriter = FALSE;
		SetEvent(pOpenContext->AbortReadEvent);
		SetEvent(pOpenContext->AbortWriteEvent);
		ret = TRUE;
		break;
	
	case IOCTL_SERIAL_GET_TIMEOUTS:
		if (! pBufOut || ! pdwActualOut || (dwLenOut < sizeof(COMMTIMEOUTS))) {
			SetLastError(ERROR_INVALID_PARAMETER);
			break;
		}
		*(COMMTIMEOUTS *)pBufOut = pOpenContext->CommTimeouts;
		*pdwActualOut = sizeof(COMMTIMEOUTS);
		ret = TRUE;
		break;
	
	case IOCTL_SERIAL_PURGE:
		if (! pBufIn || (dwLenIn < sizeof(DWORD))) {
			SetLastError(ERROR_INVALID_PARAMETER);
			break;
		}
		// TODO
		ret = TRUE;
		break;
	
	case IOCTL_SERIAL_SET_QUEUE_SIZE:
		if (! pBufIn || (dwLenIn < sizeof(SERIAL_QUEUE_SIZES))) {
			SetLastError(ERROR_INVALID_PARAMETER);
			break;
		} else {
			SERIAL_QUEUE_SIZES *pNewSizes = (SERIAL_QUEUE_SIZES *)pBufIn;
			if (pNewSizes->cbInQueue !=
				pOpenContext->SerialQueueSizes.cbInQueue) {
				int size = pNewSizes->cbInQueue +
					(pOpenContext->pPairedOpenContext ?
					 pOpenContext->pPairedOpenContext->SerialQueueSizes.cbOutQueue : 0);
				unsigned char *buf = (unsigned char *)LocalReAlloc(pOpenContext->pInQueue, size,
					LMEM_MOVEABLE);
				if (buf) {
					pOpenContext->SerialQueueSizes.cbInQueue =
						pNewSizes->cbInQueue;
					pOpenContext->pInQueue = buf;
					pOpenContext->InQueueSize = size;
					pOpenContext->InQueueBytesUsed =
						MIN(pOpenContext->InQueueBytesUsed,
							pOpenContext->InQueueSize);
					// TODO: abort or check for output queue empty event on paired context
				}
			}
			if (pNewSizes->cbOutQueue != pOpenContext->SerialQueueSizes.cbOutQueue) {
				if (pOpenContext->pPairedOpenContext) {
					int size = pOpenContext->pPairedOpenContext->SerialQueueSizes.cbInQueue + pNewSizes->cbOutQueue;
					unsigned char *buf = (unsigned char *)LocalReAlloc(pOpenContext->pPairedOpenContext->pInQueue, size,
						LMEM_MOVEABLE);
					if (buf) {
						pOpenContext->SerialQueueSizes.cbOutQueue = pNewSizes->cbOutQueue;
						pOpenContext->pPairedOpenContext->pInQueue = buf;
						pOpenContext->pPairedOpenContext->InQueueSize = size;
						pOpenContext->pPairedOpenContext->InQueueBytesUsed =
							MIN(pOpenContext->pPairedOpenContext->InQueueBytesUsed, pOpenContext->pPairedOpenContext->InQueueSize);
					// TODO: abort or check for output queue empty event on this context
					}
				} else {
					pOpenContext->SerialQueueSizes.cbOutQueue = pNewSizes->cbOutQueue;
				}
			}
			if ((pNewSizes->cbInQueue == pOpenContext->SerialQueueSizes.cbInQueue) &&
				(pNewSizes->cbOutQueue == pOpenContext->SerialQueueSizes.cbOutQueue)) {
				ret = TRUE;
			} // else LocalReAlloc will already have set LastError
		}
		break;
	
	case IOCTL_SERIAL_GET_DCB:
		if (! pBufOut || ! pdwActualOut || (dwLenOut < sizeof(DCB))) {
			SetLastError(ERROR_INVALID_PARAMETER);
			break;
		}
		*(DCB *)pBufOut = pOpenContext->DeviceControlBlock;
		*pdwActualOut = sizeof(DCB);
		ret = TRUE;
		break;
 
	case IOCTL_SERIAL_SET_DCB:
		// We don't care about these values, but the application might.
		if (! pBufIn || (dwLenIn < sizeof(DCB))) {
			SetLastError(ERROR_INVALID_PARAMETER);
			break;
		}
		pOpenContext->DeviceControlBlock = *(DCB *)pBufIn;
		ret = TRUE;
		break;

	// Ignore these for the moment as we don't have to return anything.
	case IOCTL_SERIAL_SET_BREAK_ON:
	case IOCTL_SERIAL_SET_BREAK_OFF:
	case IOCTL_SERIAL_SET_DTR:
	case IOCTL_SERIAL_CLR_DTR:
	case IOCTL_SERIAL_SET_RTS:
	case IOCTL_SERIAL_CLR_RTS:
	case IOCTL_SERIAL_SET_XOFF:
	case IOCTL_SERIAL_SET_XON:
	case IOCTL_SERIAL_IMMEDIATE_CHAR:
	case IOCTL_SERIAL_ENABLE_IR:
	case IOCTL_SERIAL_DISABLE_IR:
		ret = TRUE;
		break;
	
	default:
		SetLastError(ERROR_INVALID_FUNCTION);
		break;
	}

	LeaveCriticalSection(&pDeviceContext->CriticalSection);

	LOGDEBUGMSG((DBGID, "COM_IOControl -> %s\n", ret ? "TRUE" : "FALSE"));
	return ret;
}

//==============================================================================
// Function: COM_Read
//------------------------------------------------------------------------------

VIRTUALCOMPORT_API DWORD
COM_Read(DWORD hOpenContext, LPVOID pBuffer, DWORD Count)
{
	LOGDEBUGMSGSTART(("COM_Read(%p, %p, %d)\n", hOpenContext, pBuffer, Count));

	if (Count == 0) {
		LOGDEBUGMSG((DBGID, "COM_Read -> 0\n"));
		return 0;
	}

	PVIRTUALCOMPORT_OPEN_CONTEXT pOpenContext = (PVIRTUALCOMPORT_OPEN_CONTEXT)hOpenContext;
	if (! pOpenContext) {
		SetLastError(ERROR_INVALID_HANDLE);
		LOGDEBUGMSG((DBGID, "COM_Read -> -1\n"));
		return -1;
	}
	PVIRTUALCOMPORT_DEVICE_CONTEXT pDeviceContext = pOpenContext->pDeviceContext;

	EnterCriticalSection(&pDeviceContext->CriticalSection);

	if (pOpenContext->BlockedReader) {
		// We only allow one simultaneous read, which makes perfect sense for a
		// COM port, plus it makes locking and signalling so much easier.
		LeaveCriticalSection(&pDeviceContext->CriticalSection);
		SetLastError(ERROR_INVALID_ACCESS);
		LOGDEBUGMSG((DBGID, "COM_Read -> -1\n"));
		return -1;
	}

	DWORD timeout = 0;
	if ((pOpenContext->CommTimeouts.ReadIntervalTimeout == MAXDWORD) &&
		(pOpenContext->CommTimeouts.ReadTotalTimeoutMultiplier == MAXDWORD)) {
		timeout = pOpenContext->CommTimeouts.ReadTotalTimeoutConstant;
	} else {
		timeout = pOpenContext->CommTimeouts.ReadTotalTimeoutMultiplier * Count +
			pOpenContext ->CommTimeouts.ReadTotalTimeoutConstant;
		if (timeout == 0)
			timeout = INFINITE;
	}

	DWORD size = MIN(Count, pOpenContext->InQueueBytesUsed);
	if ((size == 0) && (timeout > 0)) {
		HANDLE waitHandles[2] = {pOpenContext->ReadEvent, pOpenContext->AbortReadEvent};

		// Only interested in read and abort events that occur after leaving the critical
		// section, as none could have arrived while we are in the critical section, and
		// we have already checked for available data.
		ResetEvent(pOpenContext->ReadEvent);
		ResetEvent(pOpenContext->AbortReadEvent);

		// Indicate that we are blocking on a read and leave the critical
		// section.
		pOpenContext->BlockedReader = TRUE;
		LeaveCriticalSection(&pDeviceContext->CriticalSection);

		// Wait for an event.
		DWORD waitResult = WaitForMultipleObjects(2, waitHandles, FALSE, timeout);

		// If we were aborted, rely on the aborter to set BlockedReader to false
		// as the open context may have been freed due to a close.
		if (waitResult == WAIT_OBJECT_0 + 1) {
			SetLastError(0);
			LOGDEBUGMSG((DBGID, "COM_Read -> -1\n"));
			return -1;
		}

		// Re-enter the critical section.
		EnterCriticalSection(&pDeviceContext->CriticalSection);

		// Check that the open context is still valid in case it was freed between
		// the time we got the event and the time we entered the critical section.
		// There is nothing we can do if it was freed after leaving the critical
		// section and before the wait.
		int i = 0;
		while (i < pDeviceContext->OpenCount) {
			if (pDeviceContext->pOpenContext[i] == pOpenContext)
				break;
			i++;
		}
		if (i < pDeviceContext->OpenCount) {
			// Clear the blocked reader flag.
			pOpenContext->BlockedReader = FALSE;

			// Check that we got a read event, otherwise we timed out or a
			// wait error occurred. Also check that we didn't get an abort
			// in between the wait and re-entering the critical section.
			if ((waitResult == WAIT_OBJECT_0) &&
				(WaitForSingleObject(pOpenContext->AbortReadEvent, 0) == WAIT_TIMEOUT))
				size = MIN(Count, pOpenContext->InQueueBytesUsed);
		} else {
			SetLastError(ERROR_INVALID_HANDLE);
			size = -1;
		}
	}

	// If there is available data, copy it to the supplied buffer.
	if (size > 0) {
		memmove(pBuffer, pOpenContext->pInQueue, size);
		memmove(pOpenContext->pInQueue, &pOpenContext->pInQueue[size], pOpenContext->InQueueBytesUsed - size);
		pOpenContext->InQueueBytesUsed -= size;
		if (pOpenContext->pPairedOpenContext) {
			SetEvent(pOpenContext->pPairedOpenContext->WaitWriteEvent);
			SetEvent(pOpenContext->pPairedOpenContext->WriteEvent);
		}
	}

	LeaveCriticalSection(&pDeviceContext->CriticalSection);

	LOGDEBUGMSG((DBGID, "COM_Read -> %d\n", size));
	return size;
}

//==============================================================================
// Function: COM_Write
//------------------------------------------------------------------------------

VIRTUALCOMPORT_API DWORD
COM_Write(DWORD hOpenContext, LPCVOID pBuffer, DWORD Count)
{
	LOGDEBUGMSGSTART(("COM_Write(%p, %p, %d)\n", hOpenContext, pBuffer, Count));

	if (Count == 0) {
		LOGDEBUGMSG((DBGID, "COM_Write -> 0\n"));
		return 0;
	}

	PVIRTUALCOMPORT_OPEN_CONTEXT pOpenContext = (PVIRTUALCOMPORT_OPEN_CONTEXT)hOpenContext;
	if (! pOpenContext) {
		SetLastError(ERROR_INVALID_HANDLE);
		LOGDEBUGMSG((DBGID, "COM_Write: ERROR_INVALID_HANDLE -> -1\n"));
		return -1;
	}
	PVIRTUALCOMPORT_DEVICE_CONTEXT pDeviceContext = pOpenContext->pDeviceContext;

	EnterCriticalSection(&pDeviceContext->CriticalSection);

	// Only allow one simultaneous writer, which makes perfect sense for a COM
	// port. Plus it makes locking and signalling so much easier.
	if (pOpenContext->BlockedWriter) {
		LeaveCriticalSection(&pDeviceContext->CriticalSection);
		SetLastError(ERROR_INVALID_ACCESS);
		LOGDEBUGMSG((DBGID, "COM_Write: ERROR_INVALID_ACCESS -> -1\n"));
		return -1;
	}

	HANDLE waitHandles[2] = {pOpenContext->WriteEvent, pOpenContext->AbortWriteEvent};

	DWORD totalTimeout = pOpenContext->CommTimeouts.WriteTotalTimeoutMultiplier * Count +
		pOpenContext->CommTimeouts.WriteTotalTimeoutConstant;
	DWORD endTickCount = totalTimeout == 0 ? INFINITE : GetTickCount() + totalTimeout;

	DWORD bytesWritten = 0;
	while (bytesWritten < Count) {
		if (pOpenContext->pPairedOpenContext) {
			DWORD size = MIN(Count - bytesWritten,
				pOpenContext->pPairedOpenContext->InQueueSize - pOpenContext->pPairedOpenContext->InQueueBytesUsed);
			if (size > 0) {
				memmove(&pOpenContext->pPairedOpenContext->pInQueue[pOpenContext->pPairedOpenContext->InQueueBytesUsed],
					&((unsigned char *)pBuffer)[bytesWritten], size);
				pOpenContext->pPairedOpenContext->InQueueBytesUsed += size;
				bytesWritten += size;
				SetEvent(pOpenContext->pPairedOpenContext->WaitReadEvent);
				SetEvent(pOpenContext->pPairedOpenContext->ReadEvent);
			}

			DWORD tickCount = GetTickCount();
			DWORD timeout = endTickCount == INFINITE ? INFINITE : (endTickCount > tickCount ? endTickCount - tickCount : 0);
			if ((bytesWritten < Count) && (timeout > 0)) {
				// Only interested in write and abort events that occur after leaving the critical
				// section, as none could have arrived while we are in the critical section, and we
				// have already written as much as we could.
				ResetEvent(pOpenContext->WriteEvent);
				ResetEvent(pOpenContext->AbortWriteEvent);

				// Indicate that we are blocking on a write and leave the
				// critical section.
				pOpenContext->BlockedWriter = TRUE;
				LeaveCriticalSection(&pDeviceContext->CriticalSection);

				// Wait for an event.
				DWORD waitResult = WaitForMultipleObjects(2, waitHandles, FALSE, timeout);

				// If we were aborted, rely on the aborter to set BlockedWriter
				// to false as the open context may have been freed due to a close.
				if (waitResult == WAIT_OBJECT_0 + 1) {
					LOGDEBUGMSG((DBGID, "COM_Write -> %d\n", bytesWritten));
					return bytesWritten;
				}

				// Re-enter the critical section.
				EnterCriticalSection(&pDeviceContext->CriticalSection);

				// Check that the open context is still valid in case it was freed between
				// the time we got the event and the time we entered the critical section.
				// There is nothing we can do if it was freed after leaving the critical
				// section and before the wait.
				int i = 0;
				while (i < pDeviceContext->OpenCount) {
					if (pDeviceContext->pOpenContext[i] == pOpenContext)
						break;
					i++;
				}
				if (i >= pDeviceContext->OpenCount) {
					SetLastError(ERROR_INVALID_HANDLE);
					bytesWritten = -1;
					break;
				}

				// Clear the blocked writer flag.
				pOpenContext->BlockedWriter = FALSE;

				// If we didn't get a write event, then we timed out or a wait
				// error occurred. In either case LastError will have been set
				// by the wait.
				if (waitResult != WAIT_OBJECT_0) {
					bytesWritten = -1;
					break;
				}

				// Check that an abort event didn't arrive between the wait and
				// re-entering the critical section.
				if (WaitForSingleObject(pOpenContext->AbortWriteEvent, 0) !=
					WAIT_TIMEOUT)
					break;
			}
		} else {
			// Nobody listening, so fake a complete write.
			bytesWritten = Count;
		}
	}

	LeaveCriticalSection(&pDeviceContext->CriticalSection);

	LOGDEBUGMSG((DBGID, "COM_Write -> %d\n", bytesWritten));
	return bytesWritten;
}

//==============================================================================
// Function: COM_Seek
//------------------------------------------------------------------------------

VIRTUALCOMPORT_API DWORD
COM_Seek(DWORD hOpenContext, long Amount, WORD Type)
{
	return -1;
}