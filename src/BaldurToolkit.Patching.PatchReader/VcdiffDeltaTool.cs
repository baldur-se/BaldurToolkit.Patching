using System.IO;

namespace BaldurToolkit.Patching.PatchReader
{
	/// <summary>
	/// Vcdiff delta file decoding tool.
	/// Uses NVcdiff to decode delta files.
	/// </summary>
	public class VcdiffDeltaTool : IDeltaTool
	{
		/// <summary>
		/// Gets code of current delta tool.
		/// </summary>
		public byte Code
		{
			get { return 1; }
		}

		/// <summary>
		/// Decode delta file.
		/// </summary>
		/// <param name="original">Original file stream.</param>
		/// <param name="delta">Delta file stream.</param>
		/// <param name="output">Output file stream.</param>
		public void Decode(Stream original, Stream delta, Stream output)
		{
			NVcdiff.VcdiffDecoder.Decode(original, delta, output);
		}
	}
}