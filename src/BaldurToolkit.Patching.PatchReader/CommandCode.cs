
namespace BaldurToolkit.Patching.PatchReader
{
	/// <summary>
	/// Basic command codes.
	/// </summary>
	public enum CommandCode : int
	{
		None = 0,
		AddFile,
		RemoveFile,
		ModifyFile,
		Execute,

		Maximum
	}
}