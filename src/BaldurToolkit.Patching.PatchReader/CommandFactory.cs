using System.IO;

using BaldurToolkit.Patching.PatchReader.Commands;

namespace BaldurToolkit.Patching.PatchReader
{
	/// <summary>
	/// Default patch command factory.
	/// </summary>
	public class CommandFactory : ICommandFactory
	{
		/// <summary>
		/// Gets current delta tool container.
		/// </summary>
		public readonly DeltaToolContainer DeltaToolContainer;

		/// <summary>
		/// Initializes a new instance of the CommandFactory class with default delta tool container with VcdiffDeltaTool in it.
		/// </summary>
		public CommandFactory()
		{
			this.DeltaToolContainer = new DeltaToolContainer();
			this.DeltaToolContainer.Register(new VcdiffDeltaTool());
		}

		/// <summary>
		/// Initializes a new instance of the CommandFactory class with given delta tool container.
		/// </summary>
		/// <param name="deltaToolContainer">Delta tool container.</param>
		public CommandFactory(DeltaToolContainer deltaToolContainer)
		{
			this.DeltaToolContainer = deltaToolContainer;
		}

		/// <summary>
		/// Create patch command class by given command code using given BinaryReader to read patch file.
		/// </summary>
		/// <param name="code">Command code.</param>
		/// <param name="reader">Patch file binary reader.</param>
		/// <returns>Patch command instance.</returns>
		public virtual ICommand CreateCommandByCode(int code, BinaryReader reader)
		{
			switch (code)
			{
				case (int)CommandCode.AddFile:
					return new AddFileCommand(reader, this.DeltaToolContainer);

				case (int)CommandCode.RemoveFile:
					return new RemoveFileCommand(reader);

				case (int)CommandCode.ModifyFile:
					return new ModifyFileCommand(reader, this.DeltaToolContainer);

				case (int)CommandCode.Execute:
					return new ExecuteCommand(reader);

				default:
					throw new InvalidCommandCodeException(code);
			}
		}
	}
}