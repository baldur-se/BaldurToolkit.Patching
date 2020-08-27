using System;
using System.IO;

namespace BaldurToolkit.Patching.PatchReader.Commands
{
	/// <summary>
	/// Add file command.
	/// </summary>
	public class AddFileCommand : IFileCommand
	{
		protected BinaryReader _Reader;
		protected DeltaToolContainer _DeltaToolContainer { get; set; }

		/// <summary>
		/// File name.
		/// </summary>
		public string Filename { get; protected set; }

		/// <summary>
		/// Gets output file hash.
		/// </summary>
		public string Hash { get; protected set; }

		/// <summary>
		/// Gets of sets whether this command can overwrite existing file.
		/// </summary>
		public bool Overwrite { get; set; }

		/// <summary>
		/// Gets delta encoding tool code.
		/// </summary>
		public byte Encoding { get; protected set; }

		/// <summary>
		/// Gets encoded file length.
		/// </summary>
		public long Length { get; protected set; }

		/// <summary>
		/// Initializes a new instance of the AddFileCommand class with a specified binary reader and delta tool container.
		/// </summary>
		/// <param name="reader">Binary reader.</param>
		/// <param name="deltaToolContainer">Delta tool container.</param>
		public AddFileCommand(BinaryReader reader, DeltaToolContainer deltaToolContainer)
		{
			this._Reader = reader;
			this._DeltaToolContainer = deltaToolContainer;

			this.Filename = reader.ReadString();
			this.Hash = reader.ReadString();
			this.Overwrite = reader.ReadBoolean();
			this.Encoding = reader.ReadByte();
			this.Length = reader.ReadInt64();
		}

		/// <summary>
		/// Skip reading command data.
		/// </summary>
		public void Skip()
		{
			this._Reader.BaseStream.Seek(this.Length, SeekOrigin.Current);
		}

		/// <summary>
		/// Read command data and execute command.
		/// </summary>
		/// <param name="targetDirectory">Target directory.</param>
		/// <param name="tmpDirectory">Directory for temporary files.</param>
		public void Execute(DirectoryInfo targetDirectory, DirectoryInfo tmpDirectory)
		{
			if (this.Encoding != 1)
			{
				throw new Exception(String.Format("Unsupported file encoding: \"{0}\".", this.Filename));
			}

			var originalFile = new FileInfo(Path.Combine(targetDirectory.FullName, this.Filename));
			if (originalFile.Exists && !this.Overwrite)
			{
				throw new FileAlreadyExistsException(this.Filename);
			}

			var originalDirectory = originalFile.DirectoryName;
			if (originalDirectory != null && !Directory.Exists(originalDirectory))
			{
				Directory.CreateDirectory(originalDirectory);
			}

			var tmpFile = new FileInfo(Path.Combine(tmpDirectory.FullName, this.Filename + ".~tmp"));
			var tmpFileDir = tmpFile.DirectoryName;
			if (tmpFileDir != null && !Directory.Exists(tmpFileDir))
			{
				Directory.CreateDirectory(tmpFileDir);
			}

			using (var tmpFileStream = tmpFile.Open(FileMode.Create, FileAccess.ReadWrite))
			{
				var deltaTool = this._DeltaToolContainer.GetDeltaToolByCode(this.Encoding);
				deltaTool.Decode(null, this._Reader.BaseStream, tmpFileStream);
			}

			var hash = HashUtil.GetMd5HashFile(tmpFile);
			if (hash != this.Hash)
			{
				throw new OutputFileHashMismatchException(this.Filename);
			}

			if (originalFile.Exists)
			{
				originalFile.Delete();
			}

			tmpFile.MoveTo(originalFile.FullName);
		}
	}
}