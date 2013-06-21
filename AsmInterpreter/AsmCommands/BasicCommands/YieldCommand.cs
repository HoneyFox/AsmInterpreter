using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyInterpreter
{
	public class YieldCommand : AsmCommand
	{
		// This command will yield the interpreter until the interpreter call Resume() to continue the procedure.
		public YieldCommand(AsmInterpreter interpreter, string[] lineParts)
			: base(interpreter, lineParts)
		{ 
		
		}

		public override void Run()
		{
			base.Run();
			parent.Pause();
		}
	}
}
