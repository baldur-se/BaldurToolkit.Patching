using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaldurToolkit.Patching.PatchBuilder.Commands
{
	/// <summary>
	/// Patch command code.
	/// </summary>
	public enum CommandCode : int
	{
		None = 0,
		AddFile,
		RemoveFile,
		ModifyFile,
		Execute,

		Maximum
	}
}