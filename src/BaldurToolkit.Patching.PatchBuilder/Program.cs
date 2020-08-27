using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using NDesk.Options;
using BaldurToolkit.Patching.PatchBuilder.Commands;
using BaldurToolkit.Patching.PatchBuilder.Diff;

namespace BaldurToolkit.Patching.PatchBuilder
{
    static class Program
	{
		static int Main(string[] args)
		{
			string oldDir = null;
			string newDir = null;
			string patchFile = null;
			string logFilePath = null;
			string xdelta = null;
			var tmpDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			var excludes = new List<string>();
			var includes = new List<string>();
			var executables = new List<string>();
			var overwrite = false;
			var showHelp = false;

			var options = new OptionSet() {
				{
					"olddir=",
					"[REQUIRED] Old product version directory.",
					v => { oldDir = Path.GetFullPath(v); }
				},
				{
					"newdir=",
					"[REQUIRED] New product version directory.",
					v => { newDir = Path.GetFullPath(v); }
				},
				{
					"patchfile=",
					"[REQUIRED] Output patch file.",
					v => { patchFile = Path.GetFullPath(v); }
				},
				{
					"logfile=",
					"Patch creation log file.",
					v => { if (v != null) logFilePath = Path.GetFullPath(v); }
				},
				{
					"exclude=",
					"Exclude pattern.",
					excludes.Add
				},
				{
					"include=",
					"Include pattern.",
					includes.Add
				},
				{
					"overwrite",
					"Overwrite patch file.",
					v => { overwrite = true; }
				},
				{
					"execute=",
					"Execute command after all other patch commands.",
					executables.Add
				},
				{
					"tmp=",
					"Use specified path as a temporary folder.",
					v => { if (v != null) tmpDir = Path.GetFullPath(v); }
				},
				{
					"xdelta=",
					"Path to xdelta3 tool.",
					v => { if (v != null) xdelta = Path.GetFullPath(v); }
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

				if (oldDir == null || newDir == null || patchFile == null)
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

			Console.WriteLine("Collecting information...");

			var diff = new DiffGenerator(excludes, includes)
				.GenerateDiff(oldDir, newDir);

			var deltaTool = new Xdelta3DeltaTool();
			if (xdelta != null)
			{
				deltaTool.Xdelta3Path = xdelta;
			}

			var builder = new PatchBuilder(oldDir, newDir, tmpDir, deltaTool);
			var commands = builder.BuildCommandList(diff);

			// Add executables
			commands.AddRange(executables.Select(command => new ExecuteCommand(command)));


			// Counter
			int count = 0;
			builder.CommandCompiled += (sender, eventArgs) =>
				Console.Write("\rCommands compiled: {0}/{1}", System.Threading.Interlocked.Increment(ref count), commands.Count);

			Console.WriteLine("Compiling binary patch file...");
			builder.CompilePatchFile(commands, patchFile, overwrite);
			Console.WriteLine("\rDone. Total commands created: {0}", count);


			// Display command list
			Console.WriteLine();
			Console.WriteLine("Command list:");

			foreach (var command in commands)
			{
				var fileCommand = command as IFileCommand;
				if (fileCommand != null)
				{
					Console.WriteLine("{0,-15}{1}", (CommandCode)fileCommand.Code, fileCommand.Filename);
				}
				else
				{
					Console.WriteLine(command.ToString());
				}
			}

			if (logFilePath != null)
			{
				using (var logFile = File.OpenWrite(logFilePath))
				using (var writer = new StreamWriter(logFile))
				{
					writer.WriteLine();
					writer.WriteLine("Patch Info");
					writer.WriteLine();
					writer.WriteLine("Created: {0}", DateTime.Now);
					writer.WriteLine("Patch size: {0} bytes", new FileInfo(patchFile).Length);
					writer.WriteLine("Created commands: {0}", commands.Count);
					writer.WriteLine();
					writer.WriteLine("Commands:");

					foreach (var command in commands)
					{
						var fileCommand = command as IFileCommand;
						if (fileCommand != null)
						{
							writer.WriteLine("\t{0,-15}{1}", (CommandCode)fileCommand.Code, fileCommand.Filename);
						}
						else
						{
							writer.WriteLine("\t{0}", command);
						}
					}
				}
			}

			return 0;
		}

		static void ShowHelp(OptionSet options)
		{
			Console.WriteLine("Usage: BaldurToolkit.Patching.PatchBuilder {Arguments}");
			Console.WriteLine("Arguments:");
			options.WriteOptionDescriptions(Console.Out);
		}
	}
}
