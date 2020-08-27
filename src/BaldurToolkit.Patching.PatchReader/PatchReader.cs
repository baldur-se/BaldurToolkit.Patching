using System;
using System.IO;
using System.Text;

namespace BaldurToolkit.Patching.PatchReader
{
	/// <summary>
	/// Basic BaldurToolkit patch reader.
	/// </summary>
	public class PatchReader
	{
		/// <summary>
		/// Default header size.
		/// </summary>
		protected const int _DefaultHeaderSize = 16;

		/// <summary>
		/// Current patch file stream.
		/// </summary>
		protected Stream _Stream;

		/// <summary>
		/// Gets version of loaded patch file.
		/// </summary>
		public int Version { get; protected set; }

		/// <summary>
		/// Gets custom header size of loaded patch file.
		/// </summary>
		public int CustomHeadersSize { get; protected set; }

		/// <summary>
		/// Gets command count in loaded patch file.
		/// </summary>
		public int CommandsCount { get; protected set; }

		/// <summary>
		/// Occurs when command from patch file started.
		/// </summary>
		public event EventHandler<CommandEventArgs> CurrentCommandChanged;

		/// <summary>
		/// Occurs when command from patch file finishes.
		/// </summary>
		public event EventHandler<CommandEventArgs> CommandProcessed;

		/// <summary>
		/// Gets or sets current command factory.
		/// </summary>
		public ICommandFactory CommandFactory { get; set; }


		/// <summary>
		/// Initializes PatchReader with default command factory and starts to read information from patch file header from given stream.
		/// </summary>
		/// <param name="patchFileStream">Patch file stream.</param>
		public PatchReader(Stream patchFileStream) : this(patchFileStream, true) { }

		/// <summary>
		/// Initializes PatchReader with default command factory.
		/// </summary>
		protected PatchReader()
		{
			this.CommandFactory = new CommandFactory();
		}

		/// <summary>
		/// Initializes PatchReader with default command factory and with given patch file stream.
		/// Patch file header will be read from the stream if second parameter is True.
		/// </summary>
		/// <param name="patchFileStream">Patch file stream.</param>
		/// <param name="readHeader">True for start reading patch file header.</param>
		protected PatchReader(Stream patchFileStream, bool readHeader)
			: this()
		{
			if (patchFileStream == null)
			{
				throw new ArgumentNullException("patchFileStream");
			}
			if (!patchFileStream.CanRead || !patchFileStream.CanSeek)
			{
				throw new ArgumentException("Unable to read or seek patch file stream.");
			}

			this._Stream = patchFileStream;

			if (readHeader)
			{
				this.ReadHeader();
			}
		}

		/// <summary>
		/// Read patch file header.
		/// </summary>
		protected void ReadHeader()
		{
			if (this._Stream.Length < _DefaultHeaderSize)
			{
				throw new Exception("Patch file is too short.");
			}
			var reader = new BinaryReader(this._Stream, Encoding.Unicode);

			var magic = reader.ReadUInt32();
			if (magic != 0xFF9A1900)
			{
				throw new Exception("Incorrect patch file header.");
			}

			this.Version = reader.ReadInt32();
			if (this.Version != 1)
			{
				throw new Exception("Not supported patch file version.");
			}

			this.CustomHeadersSize = reader.ReadInt32();
			if (this.CustomHeadersSize > 0)
			{
				reader.BaseStream.Seek(this.CustomHeadersSize, SeekOrigin.Current);
			}

			this.CommandsCount = reader.ReadInt32();
		}

		/// <summary>
		/// Read raw custom header from the patch file.
		/// </summary>
		/// <returns>Custom header raw bytes.</returns>
		public byte[] ReadCustomHeaders()
		{
			var buffer = new byte[this.CustomHeadersSize];
			
			if (this._Stream.Read(buffer, _DefaultHeaderSize, this.CustomHeadersSize) < this.CustomHeadersSize)
			{
				throw new EndOfStreamException("Can not read custom headers.");
			}

			return buffer;
		}

		/// <summary>
		/// Apply patch file.
		/// </summary>
		/// <param name="targetDirectory">Target directory.</param>
		/// <param name="tmpDirectory">Directory for temporary files.</param>
		/// <param name="skipCommands">Number of commands to skip from the start.</param>
		/// <returns>Total number of commands executed.</returns>
		public int ApplyPatch(DirectoryInfo targetDirectory, DirectoryInfo tmpDirectory, int skipCommands = 0)
		{
			var totalCompleted = 0;
			var current = this.CurrentCommandChanged;
			var completed = this.CommandProcessed;
			var reader = new BinaryReader(this._Stream, Encoding.Unicode);

			this._Stream.Seek(_DefaultHeaderSize + this.CustomHeadersSize, SeekOrigin.Begin);

			for (int i = 0; i < this.CommandsCount; i++)
			{
				var commandCode = reader.ReadInt32();
				var command = this.CommandFactory.CreateCommandByCode(commandCode, reader);
				
				if (i < skipCommands)
				{
					command.Skip();
					continue;
				}

				// Notify command execution start
				if (current != null)
					current.Invoke(this, new CommandEventArgs(command));

				command.Execute(targetDirectory, tmpDirectory);

				// Notify command execution end
				if (completed != null)
					completed.Invoke(this, new CommandEventArgs(command));

				totalCompleted++;
			}

			return totalCompleted;
		}
	}
}