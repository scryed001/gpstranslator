using System;
using OpenNETCF.IO.Serial;
using System.Text;
using System.Collections;
using System.Threading;
using System.IO;
using System.Globalization;

namespace OpenNETCF.IO.Serial.GPS
{

	public class GpsSentenceEventArgs:EventArgs
	{
		private string sentence="";
		private int counter=0;

		public string Sentence
		{
			get
			{
				return sentence;
			}
		}

		public int Counter
		{
			get
			{
				return counter;
			}
		}
		public GpsSentenceEventArgs(string sentence,int counter)
		{
			this.counter=counter;
			this.sentence=sentence;
		}
	}

}