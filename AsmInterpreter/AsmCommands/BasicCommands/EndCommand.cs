using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyInterpreter
{
	public class EndCommand : AsmCommand
	{
		public int loopInstructionIndex = -1;

		public EndCommand(AsmInterpreter interpreter, string[] lineParts)
			: base(interpreter, lineParts)
		{ 
		
		}

		public override void Run()
		{
			if (loopInstructionIndex == -1)
				base.Run();
			else
				parent.instructionIndex = loopInstructionIndex;
		}
	}
}
