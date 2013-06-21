using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyInterpreter
{
	public class NoOperationCommand : AsmCommand
	{
		public NoOperationCommand(AsmInterpreter interpreter, string[] lineParts)
			: base(interpreter, lineParts)
		{ 
		
		}

		public override void Run()
		{
			base.Run();
		}
	}
}
