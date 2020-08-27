using System;

namespace BaldurToolkit.Patching.PatchReader
{
	/// <summary>
	/// Represents errors that occur when file is already exists.
	/// </summary>
	public class FileAlreadyExistsException : Exception
	{
		/// <summary>
		/// File name.
		/// </summary>
		public string Filename { get; protected set; }

		/// <summary>
		/// Initializes a new instance of the OriginalFileHashMismatchException class with a specified file name.
		/// </summary>
		/// <param name="filename">File name.</param>
		public FileAlreadyExistsException(string filename)
			: base(String.Format("File \"{0}\" is already exists.", filename))
		{
			this.Filename = filename;
		}
	}
}