using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BaldurToolkit.Patching.PatchBuilder.Diff
{
	/// <summary>
	/// Directory difference generator.
	/// </summary>
	public class DiffGenerator
	{
		/// <summary>
		/// Gets current directory scanner instance.
		/// </summary>
		public readonly DirectoryScanner DirectoryScanner;

		/// <summary>
		/// Initializes a new instance of the DiffGenerator class with default DirectoryScanner.
		/// </summary>
		public DiffGenerator()
		{
			this.DirectoryScanner = new DirectoryScanner();
		}

		/// <summary>
		/// Initializes a new instance of the DiffGenerator class with specified excludes and includes for DirectoryScanner.
		/// </summary>
		/// <param name="excludes"></param>
		/// <param name="includes"></param>
		public DiffGenerator(IEnumerable<string> excludes = null, IEnumerable<string> includes = null)
		{
			this.DirectoryScanner = new DirectoryScanner(excludes, includes);
		}

		/// <summary>
		/// Generate directory difference.
		/// </summary>
		/// <param name="oldDir">Directory with old product version.</param>
		/// <param name="newDir">Directory with new product version.</param>
		/// <returns>Directory difference entries.</returns>
		public IList<DiffEntry> GenerateDiff(string oldDir, string newDir)
		{
			var list = new List<DiffEntry>();

			var oldFiles = this.DirectoryScanner.ScanDirectory(oldDir).ToArray();
			var newFiles = this.DirectoryScanner.ScanDirectory(newDir).ToArray();

			foreach (var filename in oldFiles)
			{
				if (!newFiles.Contains(filename))
				{
					var hash = new FileInfo(Path.Combine(oldDir, filename)).GetMd5Hash();
					list.Add(new DiffEntry(DiffEntryType.RemoveFile, filename, hash));
				}
			}

			foreach (var filename in newFiles)
			{
				var newFileInfo = new FileInfo(Path.Combine(newDir, filename));
				var newHash = newFileInfo.GetMd5Hash();
				if (!oldFiles.Contains(filename))
				{
					list.Add(new DiffEntry(DiffEntryType.AddFile, filename, null, newHash));
				}
				else
				{
					var oldFileInfo = new FileInfo(Path.Combine(oldDir, filename));
					var oldHash = oldFileInfo.GetMd5Hash();

					if (newHash != oldHash)
					{
						list.Add(new DiffEntry(DiffEntryType.ModifyFile, filename, oldHash, newHash));
					}
				}
			}

			return list;
		}
	}
}