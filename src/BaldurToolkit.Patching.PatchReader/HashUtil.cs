using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BaldurToolkit.Patching.PatchReader
{
	/// <summary>
	/// Hash utility class.
	/// </summary>
	public class HashUtil
	{
		/// <summary>
		/// Get MD5 hash string from file.
		/// </summary>
		/// <param name="fileInfo">File to hash.</param>
		/// <returns>Hash string (in lower case).</returns>
		public static string GetMd5HashFile(FileInfo fileInfo)
		{
			var sb = new StringBuilder();
			var md5 = MD5.Create();
			
			using (var stream = fileInfo.OpenRead())
			{
				foreach (var b in md5.ComputeHash(stream))
				{
					sb.Append(b.ToString("x2"));
				}
			}

			return sb.ToString();
		}
	}
}