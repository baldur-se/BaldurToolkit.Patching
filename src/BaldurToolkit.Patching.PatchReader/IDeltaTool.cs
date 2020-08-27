using System.IO;

namespace BaldurToolkit.Patching.PatchReader
{
	/// <summary>
	/// Delta file decoding tool.
	/// </summary>
	public interface IDeltaTool
	{
		/// <summary>
		/// Gets code of current delta tool.
		/// </summary>
		byte Code { get; }

		/// <summary>
		/// Decode delta file.
		/// </summary>
		/// <param name="original">Original file stream.</param>
		/// <param name="delta">Delta file stream.</param>
		/// <param name="output">Output file stream.</param>
		void Decode(Stream original, Stream delta, Stream output);
	}
}