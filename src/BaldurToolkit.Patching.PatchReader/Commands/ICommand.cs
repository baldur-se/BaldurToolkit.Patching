using System.IO;

namespace BaldurToolkit.Patching.PatchReader.Commands
{
	/// <summary>
	/// Patch command.
	/// </summary>
	public interface ICommand
	{
		/// <summary>
		/// Skip reading command data.
		/// </summary>
		void Skip();

		/// <summary>
		/// Read command data and execute command.
		/// </summary>
		/// <param name="targetDirectory">Target directory.</param>
		/// <param name="tmpDirectory">Directory for temporary files.</param>
		void Execute(DirectoryInfo targetDirectory, DirectoryInfo tmpDirectory);
	}
}