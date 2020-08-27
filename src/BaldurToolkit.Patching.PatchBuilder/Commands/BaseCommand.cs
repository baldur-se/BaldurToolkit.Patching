using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BaldurToolkit.Patching.PatchBuilder.Commands
{
	/// <summary>
	/// Command base class.
	/// </summary>
	public abstract class BaseCommand : ICommand
	{
		/// <summary>
		/// Gets command code.
		/// </summary>
		public int Code { get; protected set; }

		/// <summary>
		/// Initializes a new instance of the BaseCommand class.
		/// </summary>
		/// <param name="code">Command code.</param>
		protected BaseCommand(int code)
		{
			this.Code = code;
		}

		/// <summary>
		/// Compile command data to a temporary file(s).
		/// </summary>
		/// <param name="tmpDir">Directory for temporary files.</param>
		public virtual void Compile(DirectoryInfo tmpDir) { }

		/// <summary>
		/// Write compiled data to patch stream.
		/// </summary>
		/// <param name="writer">Patch stream binary writer.</param>
		public virtual void WriteData(BinaryWriter writer)
		{
			writer.Write(this.Code);
		}
	}
}