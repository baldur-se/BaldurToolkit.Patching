using System.IO;

using BaldurToolkit.Patching.PatchReader.Commands;

namespace BaldurToolkit.Patching.PatchReader
{
	/// <summary>
	/// Patch command factory.
	/// </summary>
	public interface ICommandFactory
	{
		/// <summary>
		/// Create patch command class by given command code using given BinaryReader to read patch file.
		/// </summary>
		/// <param name="code">Command code.</param>
		/// <param name="reader">Patch file binary reader.</param>
		/// <returns>Patch command instance.</returns>
		ICommand CreateCommandByCode(int code, BinaryReader reader);
	}
}