using System;

namespace OpenNETCF.Net
{
	/// <summary>
	/// The AdapterException class indicates an exception during
	/// an adapter query, modification, or other operation.
	/// </summary>
	public class AdapterException : System.Exception
	{
		private	uint	error;

		/// <summary>
		/// Basic constructor.  No message or error code number.
		/// </summary>
		public AdapterException() : base() {}

		/// <summary>
		/// Basic constructor using the message string of the base
		/// class.
		/// </summary>
		/// <param name="message">
		/// Message string for base class
		/// </param>
		public AdapterException(string message) : base(message) {}

		/// <summary>
		/// Constructor to which additional error code information,
		/// perhaps from a Windows Zero Config call, might be passed.
		/// </summary>
		/// <param name="errcode">
		/// Error code, available for return from HRESULT member.
		/// </param>
		[CLSCompliantAttribute(false)]
		public AdapterException(uint errcode) : base()
		{
			error = errcode;
		}

		/// <summary>
		/// Constructor to which additional error code information,
		/// perhaps from a Windows Zero Config call, might be passed.
		/// </summary>
		/// <param name="errcode">
		/// Error code, available for return from HRESULT member.
		/// </param>
		public AdapterException(int errcode) : base()
		{
			error = (uint)errcode;
		}

		/// <summary>
		/// Constructor which takes both string message (passed to
		/// base Exception class), and error code value.
		/// </summary>
		/// <param name="errcode">
		/// Error code, available for return from HRESULT member.
		/// </param>
		/// <param name="message">
		/// Message string for base class
		/// </param>
		[CLSCompliantAttribute(false)]
		public AdapterException(uint errcode, string message) : base(message)
		{
			error = errcode;
		}

		/// <summary>
		/// Constructor which takes both string message (passed to
		/// base Exception class), and error code value.
		/// </summary>
		/// <param name="errcode">
		/// Error code, available for return from HRESULT member.
		/// </param>
		/// <param name="message">
		/// Message string for base class
		/// </param>
		public AdapterException(int errcode, string message) : base(message)
		{
			error = (uint)errcode;
		}

		/// <summary>
		/// 32-bit error code value returned with exception
		/// </summary>
		public int HRESULT
		{
			get { return Convert.ToInt32(error); }
		}
	}
}
