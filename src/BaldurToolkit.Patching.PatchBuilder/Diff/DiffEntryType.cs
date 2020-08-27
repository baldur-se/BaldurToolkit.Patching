using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaldurToolkit.Patching.PatchBuilder.Diff
{
	/// <summary>
	/// Directory difference list entry type.
	/// </summary>
	public enum DiffEntryType : int
	{
		AddFile,
		RemoveFile,
		ModifyFile,
	}
}