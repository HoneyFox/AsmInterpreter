using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyInterpreter
{
	public class IfCommand : AsmCommand
	{
		public delegate bool ComparisonDelegation(float a, float b);
		public static ComparisonDelegation[] s_comparisons =
		{
			(float a, float b) => { return a == b; },
			(float a, float b) => { return a != b; },
			(float a, float b) => { return a > b; },
			(float a, float b) => { return a < b; },
			(float a, float b) => { return a >= b; },
			(float a, float b) => { return a <= b; },
		};

		public static ComparisonDelegation GetComparisonDelegation(string operatorStr)
		{
			switch (operatorStr)
			{
				case "!=":
					return s_comparisons[0];
				case "==":
					return s_comparisons[1];
				case "<=":
					return s_comparisons[2];
				case ">=":
					return s_comparisons[3];
				case "<":
					return s_comparisons[4];
				case ">":
					return s_comparisons[5];
				default:
					return null;
			}
		}

		public ValueStorage operandLeft = null;
		public ValueStorage operandRight = null;
		public int targetInstructionIndex = -1;
		public ComparisonDelegation comp = null;

		public IfCommand(AsmInterpreter interpreter, string[] lineParts)
			: base(interpreter, lineParts)
		{
			string comparisonName = lineParts[2];
			string operandLeftName = lineParts[1];
			string operandRightName = lineParts[3];
			
			ComparisonDelegation delegation = GetComparisonDelegation(comparisonName);
			if (delegation == null)
				throw new InvalidOperationException("Invalid comparison operator.");
			comp = delegation;

			ValueStorage vs = null;
			vs = parent.GetVariableByName(operandLeftName);
			if (vs == null)
			{
				vs = parent.GetRegisterByName(operandLeftName);
				if (vs == null)
				{
					vs = parent.GetStackValueFromTop(operandLeftName);
					if (vs == null)
					{
						float number = 0.0f;
						if (float.TryParse(operandLeftName, out number))
						{
							vs = new ValueStorage();
							vs.SetValue(number);
							parent.m_constants.Add(vs);
						}
					}
				}
			}
			if (vs == null)
				throw new InvalidOperationException("Invalid comparison left operand.");
			operandLeft = vs;

			vs = null;
			vs = parent.GetVariableByName(operandRightName);
			if (vs == null)
			{
				vs = parent.GetRegisterByName(operandRightName);
				if (vs == null)
				{
					vs = parent.GetStackValueFromTop(operandRightName);
					if (vs == null)
					{
						float number = 0.0f;
						if (float.TryParse(operandRightName, out number))
						{
							vs = new ValueStorage();
							vs.SetValue(number);
							parent.m_constants.Add(vs);
						}
					}
				}
			}
			if (vs == null)
				throw new InvalidOperationException("Invalid comparison right operand.");
			operandRight = vs;
		}

		public override void Run()
		{
			if (comp(operandLeft.GetValue(), operandRight.GetValue()))
			{
				parent.instructionIndex = targetInstructionIndex;
			}
			else
			{
				base.Run();
			}
		}
	}
}
