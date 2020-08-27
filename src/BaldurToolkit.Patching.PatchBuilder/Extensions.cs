using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BaldurToolkit.Patching.PatchBuilder
{
	public static class Extensions
	{
		#region FileInfo extensions

		/// <summary>
		/// Get MD5 hash string from this file.
		/// </summary>
		/// <param name="fileInfo">File</param>
		/// <returns>MD5 hash string</returns>
		public static string GetMd5Hash(this FileInfo fileInfo)
		{
			var builder = new StringBuilder();
			using (var stream = fileInfo.OpenRead())
			{
				foreach (var b in MD5.Create().ComputeHash(stream))
				{
					builder.Append(b.ToString("x2"));
				}
			}

			return builder.ToString();
		}

		#endregion
	}
}