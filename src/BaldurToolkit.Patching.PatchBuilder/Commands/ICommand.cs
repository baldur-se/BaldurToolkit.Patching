using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaldurToolkit.Patching.PatchBuilder.Commands
{
	/// <summary>
	/// Patch command.
	/// </summary>
	public interface ICommand
	{
		/// <summary>
		/// Gets command code.
		/// </summary>
		int Code { get; }

		/// <summary>
		/// Compile command data to a temporary file(s).
		/// </summary>
		/// <param name="tmpDir">Directory for temporary files.</param>
		void Compile(System.IO.DirectoryInfo tmpDir);

		/// <summary>
		/// Write compiled data to patch stream.
		/// </summary>
		/// <param name="writer">Patch stream binary writer.</param>
		void WriteData(System.IO.BinaryWriter writer);
	}
}