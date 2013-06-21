using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyInterpreter
{
	public class CalculationCommand : AsmCommand
	{
		public delegate float CalculationDelegate(float a, float b);
		public static CalculationDelegate[] s_calculations = 
		{
			(float a, float b) => { return a+b; },
			(float a, float b) => { return a-b; },
			(float a, float b) => { return a*b; },
			(float a, float b) => { return a/b; },
			(float a, float b) => { return (float)Math.IEEERemainder(Convert.ToDouble(a), Convert.ToDouble(b)); },
			(float a, float b) => { return (float)Math.Pow(Convert.ToDouble(a), Convert.ToDouble(b)); },
		};

		public static CalculationDelegate GetCalculationDelegation(string operatorStr)
		{
			switch (operatorStr)
			{
				case "+=":
				case "add":
					return s_calculations[0];
				case "-=":
				case "sub": 
				return s_calculations[1];
				case "*=":
				case "mul": 
				return s_calculations[2];
				case "/=":
				case "div": 
				return s_calculations[3];
				case "%=":
				case "mod": 
				return s_calculations[4];
				case "^=":
				case "pow":
				return s_calculations[5];
				default:
					return null;
			}
		}
		
		public ValueStorage operand = null;
		public ValueStorage target = null;
		public CalculationDelegate calc = null;

		public CalculationCommand(AsmInterpreter interpreter, string[] lineParts)
			: base(interpreter, lineParts)
		{
			string operatorName = lineParts[0];
			string targetName = lineParts[1];
			string sourceName = lineParts[2];

			CalculationDelegate delegation = GetCalculationDelegation(operatorName);
			if (delegation == null)
				throw new InvalidOperationException("Invalid calculation operator.");
			calc = delegation;

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
				throw new InvalidOperationException("Invalid calculation operand.");
			operand = vs;

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
				throw new InvalidOperationException("Invalid calculation target.");
			target = vs;
		}

		public override void Run()
		{
			target.SetValue(calc(target.GetValue(), operand.GetValue()));
			base.Run();
		}
	}
}
