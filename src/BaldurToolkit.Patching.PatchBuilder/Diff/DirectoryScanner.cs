using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BaldurToolkit.Patching.PatchBuilder.Diff
{
	/// <summary>
	/// Default directory scanner.
	/// </summary>
	public class DirectoryScanner
	{
		/// <summary>
		/// Exclude rules.
		/// </summary>
		public readonly List<string> Excludes = new List<string>();

		/// <summary>
		/// Include rules.
		/// </summary>
		public readonly List<string> Includes = new List<string>();

		/// <summary>
		/// Initializes a new instance of the DirectoryScanner class.
		/// </summary>
		/// <param name="excludes">Exclude rules.</param>
		/// <param name="includes">Include rules.</param>
		public DirectoryScanner(IEnumerable<string> excludes = null, IEnumerable<string> includes = null)
		{
			if (excludes != null)
			{
				this.Excludes.AddRange(excludes);
			}
			if (includes != null)
			{
				this.Includes.AddRange(includes);
			}
		}

		/// <summary>
		/// Scan directory for files.
		/// </summary>
		/// <param name="directory">Directory path.</param>
		/// <returns>Normalized relative file name list.</returns>
		public IEnumerable<string> ScanDirectory(string directory)
		{
			var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
			foreach (var file in files)
			{
				var filename = NormalizeRelativePath(file, directory);

				if (!this.IsIgnored(filename))
				{
					yield return filename;
				}
			}
		}

		/// <summary>
		/// Determines whether file is ignired.
		/// </summary>
		/// <param name="filename">Normalized relative file name.</param>
		/// <returns></returns>
		public bool IsIgnored(string filename)
		{
			//TODO: Exclude/Include patterns
			if (this.Excludes.Contains(filename) && !this.Includes.Contains(filename))
				return true;

			return false;
		}

		/// <summary>
		/// Normalize file relative path.
		/// </summary>
		/// <param name="fileFullName">File full path.</param>
		/// <param name="directoryFullName">Directory witch contains specified file or subfolders with specified file.</param>
		/// <returns>Normalized relative path.</returns>
		public static string NormalizeRelativePath(string fileFullName, string directoryFullName)
		{
			return fileFullName
				.Substring(directoryFullName.Length)
				.Replace(Path.DirectorySeparatorChar, '/')
				.Replace(Path.AltDirectorySeparatorChar, '/')
				.TrimStart('/');
		}
	}
}