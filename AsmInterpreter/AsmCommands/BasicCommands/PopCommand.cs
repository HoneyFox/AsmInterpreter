using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyInterpreter
{
    [Obsolete()]
    public class PopCommand : AsmCommand
	{
		public ValueStorage target = null;

		public PopCommand(AsmInterpreter interpreter, string[] lineParts)
			: base(interpreter, lineParts)
		{
			if (lineParts.Length == 2)
			{
				string targetName = lineParts[1];
				ValueStorage vs = null;

				vs = parent.GetVariableByName(targetName);
				if (vs == null)
				{
					vs = parent.GetRegisterByName(targetName);
				}
				if (vs == null)
					throw new InvalidOperationException("Invalid pop target.");
				target = vs;
			}
			else
			{
				target = null;
			}
		}

		public override void Run()
		{
			if (target != null)
			{
				target.SetValue(parent.m_stack[parent.m_stack.Count - 1].GetValue());
			}
			parent.m_stack.RemoveAt(parent.m_stack.Count - 1);
			base.Run();
		}
	}
}
