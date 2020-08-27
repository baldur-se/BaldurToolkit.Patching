using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace BaldurToolkit.Patching.PatchReader.Commands
{
	/// <summary>
	/// Execute statement command.
	/// </summary>
	public class ExecuteCommand : ICommand
	{
		public static string CmdPath = "cmd.exe";

		protected BinaryReader _Reader;

		/// <summary>
		/// Code of target interpreter.
		/// </summary>
		public byte InterpreterType { get; protected set; }

		/// <summary>
		/// Command statement.
		/// </summary>
		public string Command { get; protected set; }

		/// <summary>
		/// Gets or sets whether command should throw an errors on fail (not used yet).
		/// </summary>
		public bool IgnoreErrors { get; set; }

		/// <summary>
		/// Initializes a new instance of the ExecuteCommand class with a specified binary reader.
		/// </summary>
		/// <param name="reader">Binary reader.</param>
		public ExecuteCommand(BinaryReader reader)
		{
			this._Reader = reader;

			this.InterpreterType = reader.ReadByte();
			this.Command = reader.ReadString();
			this.IgnoreErrors = reader.ReadBoolean();
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
			using (var process = new Process())
			{
				process.StartInfo = new ProcessStartInfo() {
					FileName = CmdPath,
					Arguments = this.Command,
					UseShellExecute = false,
					RedirectStandardOutput = true,
					CreateNoWindow = true
				};
				process.Start();
			}
		}
	}
}