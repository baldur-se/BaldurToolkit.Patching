using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaldurToolkit.Patching.PatchBuilder.Diff
{
	/// <summary>
	/// Directory difference list entry.
	/// </summary>
	public class DiffEntry
	{
		/// <summary>
		/// Gets type of this diff entry.
		/// </summary>
		public DiffEntryType Type { get; protected set; }

		/// <summary>
		/// Normalized relative file name.
		/// </summary>
		public string Filename { get; protected set; }

		/// <summary>
		/// Hash of the old file version.
		/// </summary>
		public string SourceHash;

		/// <summary>
		/// Hash of new file version.
		/// </summary>
		public string TargetHash;

		/// <summary>
		/// Initializes a new instance of the DiffEntry class.
		/// </summary>
		/// <param name="type">DiffEntry type.</param>
		/// <param name="filename">File name.</param>
		/// <param name="sourceHash">Old file version hash.</param>
		/// <param name="targetHash">New file version hash.</param>
		public DiffEntry(DiffEntryType type, string filename, string sourceHash = null, string targetHash = null)
		{
			this.Type = type;
			this.Filename = filename;
			this.SourceHash = sourceHash;
			this.TargetHash = targetHash;
		}
	}
}