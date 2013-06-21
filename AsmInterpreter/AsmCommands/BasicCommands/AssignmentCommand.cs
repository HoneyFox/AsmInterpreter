using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyInterpreter
{
	public class AssignmentCommand : AsmCommand
	{
		public ValueStorage source;
		public ValueStorage target;

		public AssignmentCommand(AsmInterpreter interpreter, string[] lineParts)
			: base(interpreter, lineParts)
		{
			string targetName = lineParts[1];
			string sourceName = lineParts[2];
			ValueStorage vs = null;
			
			vs = parent.GetVariableByName(sourceName);
			if (vs == null)
			{
				vs = parent.GetRegisterByName(sourceName);
				if (vs == null)
				{
					vs = parent.GetStackValueFromTop(sourceName);
					if (vs == null)
					{ 
						float number = 0.0f;
						if (float.TryParse(sourceName, out number))
						{
							vs = new ValueStorage();
							vs.SetValue(number);
							parent.m_constants.Add(vs);
						}
					}
				}
			}
			if (vs == null)
				throw new InvalidOperationException("Invalid assignment source.");
			source = vs;

			vs = parent.GetVariableByName(targetName);
			if (vs == null)
			{
				vs = parent.GetRegisterByName(targetName);
				if (vs == null)
				{
					vs = parent.GetStackValueFromTop(targetName);
				}
			}
			if (vs == null)
				throw new InvalidOperationException("Invalid assignment target.");
			target = vs;
		}

		public override void Run()
		{
			target.SetValue(source.GetValue());
			base.Run();
		}
	}
}
