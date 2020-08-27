using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BaldurToolkit.Patching.PatchReader.Commands
{
	/// <summary>
	/// Remove file command.
	/// </summary>
	public class RemoveFileCommand : IFileCommand
	{
		protected BinaryReader _Reader;

		/// <summary>
		/// File name.
		/// </summary>
		public string Filename { get; protected set; }

		/// <summary>
		/// Gets or sets whether this command should ignore missing files.
		/// </summary>
		public bool IgnoreMissing { get; set; }

		/// <summary>
		/// Initializes a new instance of the RemoveFileCommand class with a specified binary reader.
		/// </summary>
		/// <param name="reader">Binary reader.</param>
		public RemoveFileCommand(BinaryReader reader)
		{
			this._Reader = reader;

			this.Filename = reader.ReadString();
			this.IgnoreMissing = reader.ReadBoolean();
		}

		/// <summary>
		/// Skip reading command data.
		/// </summary>
		public void Skip()
		{
			// Do nothing
		}

		/// <summary>
		/// Read command data and execute command.
		/// </summary>
		/// <param name="targetDirectory">Target directory.</param>
		/// <param name="tmpDirectory">Directory for temporary files.</param>
		public void Execute(DirectoryInfo targetDirectory, DirectoryInfo tmpDirectory)
		{
			var file = new FileInfo(Path.Combine(targetDirectory.FullName, this.Filename));
			if (!file.Exists && !this.IgnoreMissing)
			{
				throw new FileNotFoundException("Can not find file to delete.", this.Filename);
			}

			file.Delete();
		}
	}
}