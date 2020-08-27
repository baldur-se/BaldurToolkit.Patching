using System;

namespace BaldurToolkit.Patching.PatchReader
{
	/// <summary>
	/// Represents errors that occur when invalid command code read.
	/// </summary>
	public class InvalidCommandCodeException : Exception
	{
		/// <summary>
		/// Command code.
		/// </summary>
		public int Code { get; private set; }

		/// <summary>
		/// Initializes a new instance of the InvalidCommandCodeException class with a specified command code.
		/// </summary>
		/// <param name="code">Command code.</param>
		public InvalidCommandCodeException(int code)
			: base(String.Format("Invalid command code: {0}.", code))
		{
			this.Code = code;
		}
	}
}