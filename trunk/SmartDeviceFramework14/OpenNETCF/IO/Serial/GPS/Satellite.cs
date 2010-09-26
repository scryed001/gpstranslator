using System;
using OpenNETCF.IO.Serial;
using System.Text;
using System.Collections;
using System.Threading;
using System.IO;
using System.Globalization;

namespace OpenNETCF.IO.Serial.GPS
{

	public class Satellite
	{
		#region Initialization
		int id=0;
		int snr=0;
		int elevation=0;
		int azimuth=0;
		bool active=false;
		int channel=0;
		#endregion

		#region properties
		public int ID
		{
			get
			{
				return id;
			}
			set
			{
				id=value;
			}
		}

		public int SNR
		{
			get
			{
				return snr;
			}
			set
			{
				snr=value;
			}
		}

		public int Elevation
		{
			get
			{
				return elevation;
			}
			set
			{
				elevation=value;
			}
		}
		public int Azimuth
		{
			get
			{
				return azimuth;
			}
			set
			{
				azimuth=value;
			}
		}

		public bool Active
		{
			get
			{
				return active;
			}
			set
			{
				active=value;
			}
		}

		public int Channel
		{
			get
			{
				return channel;
			}
			set
			{
				channel=value;
			}
		}

		#endregion
	}
}