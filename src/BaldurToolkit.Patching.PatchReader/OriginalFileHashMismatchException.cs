using System;

namespace BaldurToolkit.Patching.PatchReader
{
	/// <summary>
	/// Represents errors that occur on original file hash mismatch.
	/// </summary>
	public class OriginalFileHashMismatchException : Exception
	{
		/// <summary>
		/// File name.
		/// </summary>
		public string Filename { get; protected set; }

		/// <summary>
		/// Initializes a new instance of the OriginalFileHashMismatchException class with a specified file name.
		/// </summary>
		/// <param name="filename">File name.</param>
		public OriginalFileHashMismatchException(string filename)
			: base(String.Format("File hash mismatch for file \"{0}\".", filename))
		{
			this.Filename = filename;
		}
	}
}