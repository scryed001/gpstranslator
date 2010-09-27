using System;

namespace OpenNETCF.IO.Serial.GPS
{
	/// <summary>
	/// Summary description for OpenNETCF.
	/// </summary>
	public class GpsAutoDiscoveryEventArgs:EventArgs
	{
		public enum AutoDiscoverStates
		{
			Testing,
			FailedToOpen,
			Opened,
			Failed,
			NoGPSDetected
		}

		private States state;
		private string gpsport;
		private OpenNETCF.IO.Serial.BaudRates gpsbauds;


		public States State
		{
			get
			{
				return state;
			}

		}
		public string GpsPort
		{
			get
			{
				return gpsport;
			}

		}
		public OpenNETCF.IO.Serial.BaudRates GpsBauds
		{
			get
			{
				return gpsbauds;
			}

		}

		public GpsAutoDiscoveryEventArgs(States state,string gpsport,OpenNETCF.IO.Serial.BaudRates gpsbauds)
		{
			this.state=state;
			this.gpsport=gpsport;
			this.gpsbauds=gpsbauds;
		} 
	}
}
