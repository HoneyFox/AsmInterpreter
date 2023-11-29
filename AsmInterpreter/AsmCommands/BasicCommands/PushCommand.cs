using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyInterpreter
{
	[Obsolete()]
	public class PushCommand : AsmCommand
	{
		public ValueStorage source = null;

		public PushCommand(AsmInterpreter interpreter, string[] lineParts)
			: base(interpreter, lineParts)
		{
			string sourceName = lineParts[1];
			ValueStorage vs = null;

			vs = parent.GetVariableByName(sourceName);
			if (vs == null)
			{
				vs = parent.GetRegisterByName(sourceName);
				{
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
			}
			if (vs == null)
				throw new InvalidOperationException("Invalid push source.");
			source = vs;			
		}

		public override void Run()
		{
			ValueStorage itemPushed = new ValueStorage();
			itemPushed.SetValue(source.GetValue());
			parent.m_stack.Add(itemPushed);
			base.Run();
		}
	}
}
