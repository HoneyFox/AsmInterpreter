using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyInterpreter
{
	public class AsmCommand
	{
		public string line;

		public AsmCommand(AsmInterpreter parentInterpreter, string[] lineParts)
		{
			line = "";
			foreach (string part in lineParts)
			{
				line += part + " ";
			}
			line = line.Substring(0, line.Length - 1);
			parent = parentInterpreter;
		}

		public virtual void Run()
		{
			parent.instructionIndex++;
		}

		public AsmInterpreter parent = null;
	}
}
