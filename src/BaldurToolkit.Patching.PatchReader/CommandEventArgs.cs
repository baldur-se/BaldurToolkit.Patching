using System;

using BaldurToolkit.Patching.PatchReader.Commands;

namespace BaldurToolkit.Patching.PatchReader
{
	public class CommandEventArgs : EventArgs
	{
		/// <summary>
		/// Related command instance.
		/// </summary>
		public ICommand Command { get; protected set; }

		public CommandEventArgs(ICommand command)
		{
			this.Command = command;
		}
	}
}