using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace BaldurToolkit.Patching.PatchBuilder
{
	/// <summary>
	/// xdelta3 delta tool.
	/// </summary>
	public class Xdelta3DeltaTool : IDeltaTool
	{
		/// <summary>
		/// Gets code of current delta tool.
		/// </summary>
		public virtual byte Code
		{
			get { return 1; }
		}

		/// <summary>
		/// Gets or sets path to xdelta3 executable.
		/// </summary>
		public string Xdelta3Path { get; set; }

		private int _CompressionLevel;

		/// <summary>
		/// Gets or sets compression level.
		/// </summary>
		public int CompressionLevel
		{
			get { return this._CompressionLevel; }
			set
			{
				if (value < 0 || value > 9)
				{
					throw new ArgumentOutOfRangeException("value", "CompressionLevel must be in range from 0 to 9.");
				}
				this._CompressionLevel = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the Xdelta3DeltaTool class.
		/// </summary>
		/// <param name="compressionLevel">Compression level.</param>
		/// <param name="xdelta3Path">Path to xdelta3 executable.</param>
		public Xdelta3DeltaTool(int compressionLevel = 9, string xdelta3Path = "xdelta3")
		{
			this.CompressionLevel = compressionLevel;
			this.Xdelta3Path = xdelta3Path;
		}

		/// <summary>
		/// Create delta file.
		/// </summary>
		/// <param name="sourceFile">Source file.</param>
		/// <param name="targetFile">Target file.</param>
		/// <param name="outputFile">Output delta file.</param>
		public virtual void CreateDelta(FileInfo sourceFile, FileInfo targetFile, FileInfo outputFile)
		{
			var arguments = String.Format("-e -n -A= -{0}", this.CompressionLevel);

			if (sourceFile != null)
			{
				arguments += String.Format(" -s \"{0}\"", sourceFile.FullName);
			}

			arguments += String.Format(" \"{0}\" \"{1}\"", targetFile.FullName, outputFile.FullName);

			var ps = new Process() {
				StartInfo = new ProcessStartInfo() {
					FileName = this.Xdelta3Path,
					Arguments = arguments,
					UseShellExecute = false,
					RedirectStandardOutput = true,
					CreateNoWindow = true
				}
			};
			ps.Start();
			var output = ps.StandardOutput.ReadToEnd();
			ps.WaitForExit();

			output = output.Replace("\n", " ").Replace("\r", "");

			if (ps.ExitCode != 0)
			{
				throw new Exception(String.Format("Delta tool exited with error code ({0}) for file \"{2}\". Program output: {1}", ps.ExitCode, output, targetFile.FullName));
			}

			using (var stream = outputFile.Open(FileMode.Append, FileAccess.Write))
			using (var writer = new BinaryWriter(stream))
			{
				writer.Write((byte)128); // EOF marker
			}
		}
	}
}