using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BaldurToolkit.Patching.PatchBuilder.Commands
{
	/// <summary>
	/// Remove file command.
	/// </summary>
	public class RemoveFileCommand : BaseCommand, IFileCommand
	{
		/// <summary>
		/// File name.
		/// </summary>
		public string Filename { get; protected set; }

		/// <summary>
		/// Gets or sets whether this command should ignore missing files.
		/// </summary>
		public bool IgnoreMissing { get; set; }

		/// <summary>
		/// Initializes a new instance of the RemoveFileCommand class.
		/// </summary>
		/// <param name="filename">File name.</param>
		public RemoveFileCommand(string filename)
			: base((byte)CommandCode.RemoveFile)
		{
			this.Filename = filename;
		}

		/// <summary>
		/// Write compiled data to patch stream.
		/// </summary>
		/// <param name="writer">Patch stream binary writer.</param>
		public override void WriteData(BinaryWriter writer)
		{
			base.WriteData(writer);
			writer.Write(this.Filename);
			writer.Write(this.IgnoreMissing);
		}

		public override string ToString()
		{
			return String.Format("{0} {1}", CommandCode.RemoveFile, this.Filename);
		}
	}
}