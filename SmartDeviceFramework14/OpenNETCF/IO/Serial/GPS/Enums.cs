using System;
using OpenNETCF.IO.Serial;
using System.Text;
using System.Collections;
using System.Threading;
using System.IO;
using System.Globalization;

namespace OpenNETCF.IO.Serial.GPS
{

	public enum CardinalDirection
	{
		North = 0,
		East = 1,
		South = 2,
		West = 4,
		NorthWest = 5,
		NorthEast = 6,
		SouthWest = 7,
		SouthEast = 8,
		Stationary = 9
	}

	public enum States
	{
		AutoDiscovery,
		Opening,
		Running,
		Stopping,
		Stopped
	}

	public enum StatusType
	{
		NotSet,
		OK, //A
		Warning //V
	}

	public enum AutoDiscoverStates
	{
		Testing,
		FailedToOpen,
		Opened,
		Failed,
		NoGPSDetected
	}
	public enum Fix_Mode
	{
		Auto,
		Manual
	}
	public enum Fix_Indicator
	{
		NotSet,
		Mode2D,
		Mode3D
	}
	public enum Fix_Type
	{
		NotSet,
		NoAltitude,
		WithAltitude
	}
	public enum Fix_TypeMode
	{
		NotSet,
		SPS,
		DSPS,
		PPS,
		RTK
	}
	public enum Units
	{
		Kilometers,
		Miles,
		Knots
	}


}	

