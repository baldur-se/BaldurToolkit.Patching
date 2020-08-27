using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BaldurToolkit.Patching.PatchBuilder
{
	/// <summary>
	/// Delta file encoding tool.
	/// </summary>
	public interface IDeltaTool
	{
		/// <summary>
		/// Gets code of current delta tool.
		/// </summary>
		byte Code { get; }

		/// <summary>
		/// Create delta file.
		/// </summary>
		/// <param name="sourceFile">Source file.</param>
		/// <param name="targetFile">Target file.</param>
		/// <param name="outputFile">Output delta file.</param>
		void CreateDelta(FileInfo sourceFile, FileInfo targetFile, FileInfo outputFile);
	}
}