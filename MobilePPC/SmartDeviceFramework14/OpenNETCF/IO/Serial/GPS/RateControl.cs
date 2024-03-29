using System;
using OpenNETCF.IO.Serial;
using System.Text;
using System.Collections;
using System.Threading;
using System.IO;
using System.Globalization;

/// <summary>
/// Produced Under the Terms Of The OpennetCF License
///
/// By Richard Jones - Richard@BinaryRefinery.com
///
/// </summary>
namespace OpenNETCF.IO.Serial.GPS
{

	public class RateControl
	{
		public enum Sentence
		{
			 GPGGA = 0,
			 GPGLL = 1,
			 GPGSA = 2,
			 GPGSV = 3,
			 GPRMC = 4,
			 GPVTG = 5
		}
		/// <summary>
		/// Caluation of checksum before send the message
		/// </summary>
		/// <param name="NMEAMessage"></param>
		/// <returns></returns>
		private static string CalculChecksum(string NMEAMessage)
		{
			int intor=0;
			// go from first character upto last *
			for(int i=0;(i<NMEAMessage.Length);i++)
				intor=intor^ (int) (NMEAMessage[i]);
			string ret;
			ret = Convert.ToString(intor,16).ToUpper();
			if (ret.Length < 2)
				ret="0"+ret;
			return ret;
		}
		/// <summary>
		/// Enable or disable and modify the rate of each NMEA message
		/// Rate = 0 disable the message
		/// Rate between 1 and 5 seconds
		/// </summary>
		/// <param name="GPSSentence"></param>
		/// <param name="Rate"></param>
		/// <returns></returns>
		public static string RateToSend(Sentence GPSSentence, int Rate)
		{
			string Result = "";
			if (Rate < 0 || Rate > 5) return Result;

			string Mode = "00"; // Set 00 = SetRate, 01 = Query
			string ChksumEnable = "01"; // 01 = Enable checksum, 00 = Disable checksum
			Result = "PSRF103" + "," + "0" + Convert.ToUInt32(GPSSentence) + "," + Mode + "," + "0" + Convert.ToString(Rate) + "," + ChksumEnable;
			Result = "$" + Result + "*" + CalculChecksum(Result) + Convert.ToChar(13) + Convert.ToChar(10);
			return Result;
		}
		/// <summary>
		/// Change the speed of the GPS receiver
		/// Min = 4800
		/// Max = 19200
		/// </summary>
		/// <param name="Speed"></param>
		/// <returns></returns>
		public static string SpeedToChange(int Speed)
		{
			string Result = "";
			if (Speed < 4800 || Speed > 19200) return Result;
			Result = "PSRF100" + ",1," + Speed + ",8,1,0";
			Result = "$" + Result + "*" + CalculChecksum(Result) + Convert.ToChar(13) + Convert.ToChar(10);
			return Result;
		}
	}
}

