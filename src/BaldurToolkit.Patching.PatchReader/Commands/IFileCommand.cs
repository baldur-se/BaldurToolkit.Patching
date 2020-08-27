using System;
using System.Collections.Generic;
using System.Text;

namespace BaldurToolkit.Patching.PatchReader.Commands
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