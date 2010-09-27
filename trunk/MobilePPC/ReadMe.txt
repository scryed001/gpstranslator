
GPSProxy-2.2-Source
VirtualComPort-1.2-Source
Homepage: http://sourceforge.net/projects/gpsproxy/

Nini-1.1.0
HomePage: http://nini.sourceforge.net/

SmartDeviceFramework14
HomePage:http://www.opennetcf.com/


bin: Save all the release dlls.


Function description
The VirtualComPort.dll is the driver.
GPSProxy uses ActivateDevice to load the driver.
	Pass a register key to the function.
	The required information should be added under the key ahead, such as dll, prefix, friendly name.
	The <Prefix>_Init will be called.
	See more in http://msdn.microsoft.com/en-us/library/ms896106.aspx.
