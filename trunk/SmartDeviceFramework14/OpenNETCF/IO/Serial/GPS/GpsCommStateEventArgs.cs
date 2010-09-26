using System;
using OpenNETCF.IO.Serial;
using System.Text;
using System.Collections;
using System.Threading;
using System.IO;
using System.Globalization;

namespace OpenNETCF.IO.Serial.GPS
{

	public class GpsCommStateEventArgs:EventArgs
	{

		private States state;


		public States State
		{
			get
			{
				return state;
			}

		}
		public GpsCommStateEventArgs(States state)
		{
			this.state=state;
		} 
	}
}