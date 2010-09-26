//==========================================================================================
//
//		OpenNETCF.Windows.Forms.DateRangeEventArgs
//		Copyright (c) 2004, OpenNETCF.org
//
//		This library is free software; you can redistribute it and/or modify it under 
//		the terms of the OpenNETCF.org Shared Source License.
//
//		This library is distributed in the hope that it will be useful, but 
//		WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
//		FITNESS FOR A PARTICULAR PURPOSE. See the OpenNETCF.org Shared Source License 
//		for more details.
//
//		You should have received a copy of the OpenNETCF.org Shared Source License 
//		along with this library; if not, email licensing@opennetcf.org to request a copy.
//
//		If you wish to contact the OpenNETCF Advisory Board to discuss licensing, please 
//		email licensing@opennetcf.org.
//
//		For general enquiries, email enquiries@opennetcf.org or visit our website at:
//		http://www.opennetcf.org
//
//==========================================================================================
using System;

namespace OpenNETCF.Windows.Forms
{
	/// <summary>
	/// Provides data for the <see cref="MonthCalendar.DateChanged"/> or <see cref="MonthCalendar.DateSelected"/> events of the <see cref="MonthCalendar"/> control.
	/// <para><b>New in v1.1</b></para>
	/// </summary>
	public class DateRangeEventArgs : EventArgs
	{
		private DateTime m_start;
		private DateTime m_end;

		/// <summary>
		/// Initializes a new instance of the <see cref="DateRangeEventArgs"/> class.
		/// </summary>
		/// <param name="start">The first date/time value in the range that the user has selected.</param>
		/// <param name="end">The last date/time value in the range that the user has selected.</param>
		public DateRangeEventArgs(DateTime start, DateTime end)
		{
			m_start = start;
			m_end = end;
		}

		/// <summary>
		/// Gets the first date/time value in the range that the user has selected.
		/// </summary>
		public DateTime Start
		{
			get
			{
				return m_start;
			}
		}

		/// <summary>
		/// Gets the last date/time value in the range that the user has selected.
		/// </summary>
		public DateTime End
		{
			get
			{
				return m_end;
			}
		}
	}

	/// <summary>
	/// Provides data for the <see cref="MonthCalendar.DateChanged"/> or <see cref="MonthCalendar.DateSelected"/> events of the <see cref="MonthCalendar"/> control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">A <see cref="DateRangeEventArgs"/> that contains the event data.</param>
	public delegate void DateRangeEventHandler(object sender, DateRangeEventArgs e);
	
}
