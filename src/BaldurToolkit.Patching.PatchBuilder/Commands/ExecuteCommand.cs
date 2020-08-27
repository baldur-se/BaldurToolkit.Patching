using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BaldurToolkit.Patching.PatchBuilder.Commands
{
	/// <summary>
	/// Execute statement command.
	/// </summary>
	public class ExecuteCommand : BaseCommand
	{
		/// <summary>
		/// Code of target interpreter.
		/// </summary>
		public byte InterpreterType { get; set; }

		/// <summary>
		/// Command statement.
		/// </summary>
		public string Command { get; protected set; }

		/// <summary>
		/// Gets or sets whether command should throw an errors on fail.
		/// </summary>
		public bool IgnoreErrors { get; set; }

		/// <summary>
		/// Initializes a new instance of the ExecuteCommand class.
		/// </summary>
		/// <param name="command"></param>
		public ExecuteCommand(string command)
			: base((byte)CommandCode.Execute)
		{
			this.InterpreterType = 1;
			this.Command = command;
			this.IgnoreErrors = true;
		}

		/// <summary>
		/// Write compiled data to patch stream.
		/// </summary>
		/// <param name="writer">Patch stream binary writer.</param>
		public override void WriteData(BinaryWriter writer)
		{
			base.WriteData(writer);
			writer.Write(this.InterpreterType);
			writer.Write(this.Command);
			writer.Write(this.IgnoreErrors);
		}

		public override string ToString()
		{
			return String.Format("{0} {1}", CommandCode.Execute, this.Command);
		}
	}
}