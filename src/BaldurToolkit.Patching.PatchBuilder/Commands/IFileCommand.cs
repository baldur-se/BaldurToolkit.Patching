using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaldurToolkit.Patching.PatchBuilder.Commands
{
	/// <summary>
	/// File-related patch command.
	/// </summary>
	public interface IFileCommand : ICommand
	{
		/// <summary>
		/// File name.
		/// </summary>
		string Filename { get; }
	}
}