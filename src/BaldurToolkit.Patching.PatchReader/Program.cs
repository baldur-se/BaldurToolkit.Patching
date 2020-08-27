using System;
using System.IO;

using NDesk.Options;
using BaldurToolkit.Patching.PatchReader.Commands;

namespace BaldurToolkit.Patching.PatchReader
{
    static class Program
	{
		static int Main(string[] args)
		{
			string target = null;
			string patch = null;
			var tmpDirName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			var showHelp = false;

			var options = new OptionSet() {
				{
					"target=",
					"[REQUIRED] Target product directory.",
					v => { target = Path.GetFullPath(v); }
				},
				{
					"patch=",
					"[REQUIRED] Patch file.",
					v => { patch = Path.GetFullPath(v); }
				},
				{
					"tmp=",
					"Use specified path as a temporary folder.",
					v => { if (v != null) tmpDirName = Path.GetFullPath(v); }
				},
				{
					"h|help",
					"Show help and exit.",
					v => { showHelp = true; }
				},
			};
			try
			{
				options.Parse(args);

				if (showHelp)
				{
					ShowHelp(options);
					return 0;
				}

				if (target == null || patch == null)
				{
					throw new Exception("Missing required arguments.");
				}
			}
			catch (Exception exception)
			{
				Console.WriteLine("ERROR: {0}", exception.Message);
				Console.WriteLine();
				ShowHelp(options);
				return 1;
			}


			if (!Directory.Exists(target))
			{
				throw new DirectoryNotFoundException("Target product directory not found.");
			}
			if (!File.Exists(patch))
			{
				throw new FileNotFoundException("Patch file not found.");
			}

			var tmpDir = new DirectoryInfo(tmpDirName);
			tmpDir.Create();

			using (var patchStream = File.OpenRead(patch))
			{
				var patchReader = new PatchReader(patchStream);

				patchReader.CurrentCommandChanged += (sender, eventArgs) =>
				{
					var fileCommand = eventArgs.Command as IFileCommand;
					if (fileCommand != null)
					{
						if (fileCommand is AddFileCommand)
						{
							Console.WriteLine("Adding file \"{0}\"...", fileCommand.Filename);
						}
						else if (fileCommand is ModifyFileCommand)
						{
							Console.WriteLine("Modifying file \"{0}\"...", fileCommand.Filename);
						}
						else if (fileCommand is RemoveFileCommand)
						{
							Console.WriteLine("Deleting file \"{0}\"...", fileCommand.Filename);
						}
						else
						{
							Console.WriteLine("Processing file \"{0}\"...", fileCommand.Filename);
						}
					}
					else
					{
						Console.WriteLine("Processing {0}...", eventArgs.Command.ToString());
					}
				};

				patchReader.ApplyPatch(new DirectoryInfo(target), tmpDir);
			}

			tmpDir.Delete(true);

			return 0;
		}

		static void ShowHelp(OptionSet options)
		{
			Console.WriteLine("Usage: BaldurToolkit.Patching.PatchReader {Arguments}");
			Console.WriteLine("Arguments:");
			options.WriteOptionDescriptions(Console.Out);
		}
	}
}
