using System;

namespace BaldurToolkit.Patching.PatchReader
{
	/// <summary>
	/// Represents errors that occur on output file hash mismatch.
	/// </summary>
	public class OutputFileHashMismatchException : Exception
	{
		/// <summary>
		/// File name.
		/// </summary>
		public string Filename { get; protected set; }

		/// <summary>
		/// Initializes a new instance of the OutputFileHashMismatchException class with a specified file name.
		/// </summary>
		/// <param name="filename">File name.</param>
		public OutputFileHashMismatchException(string filename)
			: base(String.Format("File hash mismatch for file \"{0}\".", filename))
		{
			this.Filename = filename;
		}
	}
}