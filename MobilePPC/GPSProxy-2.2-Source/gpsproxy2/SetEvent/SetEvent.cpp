// SetEvent.cpp : Defines the entry point for the application.
//

#include "stdafx.h"

int WINAPI WinMain(	HINSTANCE hInstance,
					HINSTANCE hPrevInstance,
					LPTSTR    lpCmdLine,
					int       nCmdShow)
{
	HANDLE event = CreateEvent(NULL, false, false, TEXT("GPSPROXY_WAKEUP"));
	if (event != NULL)
	{
		if (GetLastError() == ERROR_ALREADY_EXISTS)
			SetEvent(event);
		CloseHandle(event);
	}
	return 0;
}

