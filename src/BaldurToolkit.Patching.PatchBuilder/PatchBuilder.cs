using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BaldurToolkit.Patching.PatchBuilder.Commands;
using BaldurToolkit.Patching.PatchBuilder.Diff;

namespace BaldurToolkit.Patching.PatchBuilder
{
	/// <summary>
	/// Basic BaldurToolkit patch builder.
	/// </summary>
	public class PatchBuilder
	{
		/// <summary>
		/// Gets or sets patch format version.
		/// </summary>
		public int FormatVersion { get; set; }

		/// <summary>
		/// Gets or sets directory with old product version.
		/// </summary>
		public string OldDir { get; set; }

		/// <summary>
		/// Gets or sets directory with new product version.
		/// </summary>
		public string NewDir { get; set; }

		/// <summary>
		/// Gets or sets directory for temporary files.
		/// </summary>
		public string TmpDir { get; set; }

		/// <summary>
		/// Gets or sets whether builder should include content of added files.
		/// </summary>
		public bool IncludeAddedFileContents { get; set; }

		/// <summary>
		/// Gets or sets current delta tool.
		/// </summary>
		public IDeltaTool DeltaTool { get; set; }


		/// <summary>
		/// Occurs when one command was compiled.
		/// </summary>
		public event EventHandler<CommandCompiledEventArgs> CommandCompiled;

		/// <summary>
		/// Initializes a new instance of the PatchBuilder class.
		/// </summary>
		/// <param name="oldDir">Directory with old product version.</param>
		/// <param name="newDir">Directory with new product version.</param>
		/// <param name="tmpDir">Directory for temporary files.</param>
		/// <param name="deltaTool">Current delta tool.</param>
		public PatchBuilder(string oldDir, string newDir, string tmpDir, IDeltaTool deltaTool = null)
		{
			this.FormatVersion = 1;
			this.OldDir = oldDir;
			this.NewDir = newDir;
			this.TmpDir = tmpDir;
			this.IncludeAddedFileContents = true;
			this.DeltaTool = deltaTool ?? new Xdelta3DeltaTool();
		}

		/// <summary>
		/// Build patch command list based on directory difference.
		/// </summary>
		/// <param name="diff">Directory difference entries.</param>
		/// <returns>Patch command list.</returns>
		public List<ICommand> BuildCommandList(IEnumerable<DiffEntry> diff)
		{
			var list = new List<ICommand>();

			foreach (var entry in diff)
			{
				switch (entry.Type)
				{
					case DiffEntryType.AddFile:
						list.Add(
							new AddFileCommand(
								entry.Filename,
								new FileInfo(Path.Combine(this.NewDir, entry.Filename)),
								entry.TargetHash,
								this.IncludeAddedFileContents,
								this.DeltaTool
							)
						);
						break;

					case DiffEntryType.RemoveFile:
						list.Add(
							new RemoveFileCommand(entry.Filename)
						);
						break;

					case DiffEntryType.ModifyFile:
						list.Add(
							new ModifyCommand(
								entry.Filename,
								new FileInfo(Path.Combine(this.OldDir, entry.Filename)),
								new FileInfo(Path.Combine(this.NewDir, entry.Filename)),
								entry.SourceHash,
								entry.TargetHash,
								this.DeltaTool
							)
						);
						break;
				}
			}

			return list;
		}

		/// <summary>
		/// Compile patch file with specified commands.
		/// </summary>
		/// <param name="commands">Patch commands.</param>
		/// <param name="patchFile">Output patch file.</param>
		/// <param name="overwrite">Overwrite patch file if it is already exists.</param>
		public void CompilePatchFile(List<ICommand> commands, string patchFile, bool overwrite)
		{
			var tmpDir = new DirectoryInfo(this.TmpDir);
			tmpDir.Create();

			try
			{
				var ev = this.CommandCompiled;

#if PARALLEL
				Parallel.ForEach(commands, command =>
#else
				foreach (var command in commands)
#endif
				{
					command.Compile(tmpDir);
					if (ev != null)
					{
						ev.Invoke(this, new CommandCompiledEventArgs(command));
					}
				}
#if PARALLEL
				);
#endif

				using (var stream = File.Open(patchFile, overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.Write))
				using (var writer = new BinaryWriter(stream, Encoding.Unicode))
				{
					writer.Write(new byte[] { 0x00, 0x19, 0x9A, 0xFF }); // Magic
					writer.Write(this.FormatVersion);

					var header = this.GetCustomHeaderBytes();

					writer.Write(header.Length); // Custom header size
					writer.Write(header);

					writer.Write(commands.Count);
					foreach (var command in commands)
					{
						command.WriteData(writer);
					}
				}
			}
			finally
			{
				tmpDir.Delete(true);
			}
		}

		/// <summary>
		/// Get custom header as raw bytes.
		/// </summary>
		/// <returns>Custom header as byte array.</returns>
		protected virtual byte[] GetCustomHeaderBytes()
		{
			return new byte[0];
		}
	}
}