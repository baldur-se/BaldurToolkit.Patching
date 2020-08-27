using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BaldurToolkit.Patching.PatchBuilder.Commands
{
	/// <summary>
	/// Add file command.
	/// </summary>
	public class AddFileCommand : BaseCommand, IFileCommand
	{
		/// <summary>
		/// Normalized file name.
		/// </summary>
		public string Filename { get; protected set; }

		/// <summary>
		/// FileInfo of added file.
		/// </summary>
		public FileInfo FileInfo { get; protected set; }

		/// <summary>
		/// Output file hash.
		/// </summary>
		public string Hash { get; protected set; }

		/// <summary>
		/// Gets of sets whether this command can overwrite existing file.
		/// </summary>
		public bool Overwrite { get; set; }

		/// <summary>
		/// Gets of sets delta encoding tool code.
		/// </summary>
		public bool IncludeFileContents { get; set; }

		/// <summary>
		/// Gets or sets delta encoding tool.
		/// </summary>
		public IDeltaTool DeltaTool { get; set; }

		private FileInfo _TmpFile { get; set; }

		/// <summary>
		/// Initializes a new instance of the AddFileCommand class.
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="fileInfo"></param>
		/// <param name="hash"></param>
		/// <param name="includeFileContents"></param>
		/// <param name="deltaTool"></param>
		public AddFileCommand(string filename, FileInfo fileInfo, string hash, bool includeFileContents = false, IDeltaTool deltaTool = null)
			: base((byte)CommandCode.AddFile)
		{
			if (includeFileContents && deltaTool == null)
			{
				throw new ArgumentNullException("deltaTool", "deltaTool argument can not be null if includeFileContents argument is set to true");
			}

			this.Filename = filename;
			this.FileInfo = fileInfo;
			this.Hash = hash;
			this.IncludeFileContents = includeFileContents;
			this.DeltaTool = deltaTool;
			this.Overwrite = true;
		}

		/// <summary>
		/// Compile command data to a temporary file(s).
		/// </summary>
		/// <param name="tmpDir">Directory for temporary files.</param>
		public override void Compile(DirectoryInfo tmpDir)
		{
			if (!this.IncludeFileContents) return;

			var dir = new DirectoryInfo(Path.Combine(tmpDir.FullName, this.Filename));
			dir.Create();

			this._TmpFile = new FileInfo(Path.Combine(dir.FullName, "patch.delta"));

			this.DeltaTool.CreateDelta(null, this.FileInfo, this._TmpFile);
		}

		/// <summary>
		/// Write compiled data to patch stream.
		/// </summary>
		/// <param name="writer">Patch stream binary writer.</param>
		public override void WriteData(BinaryWriter writer)
		{
			base.WriteData(writer);
			writer.Write(this.Filename);
			writer.Write(this.Hash);
			writer.Write(this.Overwrite);

			if (this.IncludeFileContents)
			{
				writer.Write(this.DeltaTool.Code); // Encoding
				writer.Write(this._TmpFile.Length);
				using (var fileStream = this._TmpFile.OpenRead())
				{
					fileStream.CopyTo(writer.BaseStream);
				}
			}
			else
			{
				writer.Write((byte)0); // Encoding
				writer.Write((long)0); // Length
			}
		}

		public override string ToString()
		{
			return String.Format("{0} {1}", CommandCode.AddFile, this.Filename);
		}
	}
}