using System.IO;

namespace BaldurToolkit.Patching.PatchReader.Commands
{
	/// <summary>
	/// Modify file patch command.
	/// </summary>
	public class ModifyFileCommand : IFileCommand
	{
		protected BinaryReader _Reader;
		protected DeltaToolContainer _DeltaToolContainer { get; set; }

		/// <summary>
		/// File name.
		/// </summary>
		public string Filename { get; protected set; }

		/// <summary>
		/// Gets hash required for input file.
		/// </summary>
		public string OldHash { get; protected set; }

		/// <summary>
		/// Gets hash required for output file.
		/// </summary>
		public string NewHash { get; protected set; }

		/// <summary>
		/// Gets delta encoding tool code.
		/// </summary>
		public byte Encoding { get; protected set; }

		/// <summary>
		/// Gets encoded file length.
		/// </summary>
		public long Length { get; protected set; }

		/// <summary>
		/// Initializes a new instance of the ModifyFileCommand class with a specified binary reader and delta tool container.
		/// </summary>
		/// <param name="reader">Binary reader.</param>
		/// <param name="deltaToolContainer">Delta tool container.</param>
		public ModifyFileCommand(BinaryReader reader, DeltaToolContainer deltaToolContainer)
		{
			this._Reader = reader;
			this._DeltaToolContainer = deltaToolContainer;

			this.Filename = reader.ReadString();
			this.OldHash = reader.ReadString();
			this.NewHash = reader.ReadString();
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
			var originalFile = new FileInfo(Path.Combine(targetDirectory.FullName, this.Filename));
			if (!originalFile.Exists)
			{
				throw new FileNotFoundException("Can not find file to modify.", this.Filename);
			}

			if (HashUtil.GetMd5HashFile(originalFile) != this.OldHash)
			{
				throw new OriginalFileHashMismatchException(this.Filename);
			}

			var tmpFile = new FileInfo(Path.Combine(tmpDirectory.FullName, this.Filename + ".~tmp"));
			var tmpFileDir = tmpFile.DirectoryName;
			if (tmpFileDir != null && !Directory.Exists(tmpFileDir))
			{
				Directory.CreateDirectory(tmpFileDir);
			}

			using (var originalFileStream = originalFile.OpenRead())
			using (var tmpFileStream = tmpFile.Open(FileMode.Create, FileAccess.ReadWrite))
			{
				var deltaTool = this._DeltaToolContainer.GetDeltaToolByCode(this.Encoding);
				deltaTool.Decode(originalFileStream, this._Reader.BaseStream, tmpFileStream);
			}

			if (HashUtil.GetMd5HashFile(tmpFile) != this.NewHash)
			{
				throw new OutputFileHashMismatchException(this.Filename);
			}

			originalFile.Delete();
			tmpFile.MoveTo(originalFile.FullName);
		}
	}
}