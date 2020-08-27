using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BaldurToolkit.Patching.PatchBuilder.Commands
{
	/// <summary>
	/// Modify file command.
	/// </summary>
	public class ModifyCommand : BaseCommand, IFileCommand
	{
		/// <summary>
		/// File name.
		/// </summary>
		public string Filename { get; protected set; }

		/// <summary>
		/// Old file.
		/// </summary>
		public FileInfo OldFile { get; protected set; }

		/// <summary>
		/// New File.
		/// </summary>
		public FileInfo NewFile { get; protected set; }

		/// <summary>
		/// Hash of old file.
		/// </summary>
		public string OldHash { get; protected set; }

		/// <summary>
		/// Hash of new file.
		/// </summary>
		public string NewHash { get; protected set; }

		/// <summary>
		/// Gets of sets delta encoding tool code.
		/// </summary>
		public IDeltaTool DeltaTool { get; set; }

		private FileInfo _TmpFile { get; set; }

		/// <summary>
		/// Initializes a new instance of the ModifyCommand class.
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="oldFile"></param>
		/// <param name="newFile"></param>
		/// <param name="oldHash"></param>
		/// <param name="newHash"></param>
		/// <param name="deltaTool"></param>
		public ModifyCommand(string filename, FileInfo oldFile, FileInfo newFile, string oldHash, string newHash, IDeltaTool deltaTool)
			: base((byte)CommandCode.ModifyFile)
		{
			if (deltaTool == null)
			{
				throw new ArgumentNullException("deltaTool");
			}

			this.Filename = filename;
			this.OldFile = oldFile;
			this.NewFile = newFile;
			this.OldHash = oldHash;
			this.NewHash = newHash;
			this.DeltaTool = deltaTool;
		}

		/// <summary>
		/// Compile command data to a temporary file(s).
		/// </summary>
		/// <param name="tmpDir">Directory for temporary files.</param>
		public override void Compile(DirectoryInfo tmpDir)
		{
			var dir = new DirectoryInfo(Path.Combine(tmpDir.FullName, this.Filename));
			dir.Create();

			this._TmpFile = new FileInfo(Path.Combine(dir.FullName, "patch.delta"));

			this.DeltaTool.CreateDelta(this.OldFile, this.NewFile, this._TmpFile);
		}

		/// <summary>
		/// Write compiled data to patch stream.
		/// </summary>
		/// <param name="writer">Patch stream binary writer.</param>
		public override void WriteData(BinaryWriter writer)
		{
			base.WriteData(writer);
			writer.Write(this.Filename);
			writer.Write(this.OldHash);
			writer.Write(this.NewHash);

			writer.Write(this.DeltaTool.Code); // Encoding
			writer.Write(this._TmpFile.Length);
			using (var fileStream = this._TmpFile.OpenRead())
			{
				fileStream.CopyTo(writer.BaseStream);
			}
		}

		public override string ToString()
		{
			return String.Format("{0} {1}", CommandCode.ModifyFile, this.Filename);
		}
	}
}