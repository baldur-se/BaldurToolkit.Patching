using System;
using System.Collections.Generic;

namespace BaldurToolkit.Patching.PatchReader
{
	/// <summary>
	/// Delta tool container.
	/// </summary>
	public class DeltaToolContainer
	{
		private readonly Dictionary<byte, IDeltaTool> _DeltaTools = new Dictionary<byte, IDeltaTool>();

		/// <summary>
		/// Register delta tool in container.
		/// </summary>
		/// <param name="tool">Delta tool instance.</param>
		public void Register(IDeltaTool tool)
		{
			if (tool == null)
			{
				throw new ArgumentNullException("tool");
			}

			if (this._DeltaTools.ContainsKey(tool.Code))
			{
				this._DeltaTools.Remove(tool.Code);
			}

			this._DeltaTools.Add(tool.Code, tool);
		}

		/// <summary>
		/// Get delta tool by given delta tool code.
		/// </summary>
		/// <param name="code">Delta tool code.</param>
		/// <returns>Delta tool instance.</returns>
		public IDeltaTool GetDeltaToolByCode(byte code)
		{
			IDeltaTool tool;
			if (!this._DeltaTools.TryGetValue(code, out tool))
			{
				throw new KeyNotFoundException(String.Format("Can not found delta tool for code \"{0}\".", code));
			}

			return tool;
		}
	}
}