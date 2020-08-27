using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BaldurToolkit.Patching.PatchBuilder.Commands;

namespace BaldurToolkit.Patching.PatchBuilder
{
	public class CommandCompiledEventArgs : EventArgs
	{
		/// <summary>
		/// Related command instance.
		/// </summary>
		public ICommand Command { get; private set; }

		public CommandCompiledEventArgs(ICommand command)
		{
			this.Command = command;
		}
	}
}