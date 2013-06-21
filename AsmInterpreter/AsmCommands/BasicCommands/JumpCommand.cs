using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyInterpreter
{
	public class JumpCommand : AsmCommand
	{
		public delegate bool ComparisonDelegation(float a, float b);
		public static ComparisonDelegation[] s_comparisons =
		{
			(float a, float b) => { return true; },
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
				case "jmp":
					return s_comparisons[0];
				case "je":
					return s_comparisons[1];
				case "jne": 
					return s_comparisons[2];
				case "jb": 
					return s_comparisons[3];
				case "jl": 
					return s_comparisons[4];
				case "jbe": 
					return s_comparisons[5];
				case "jle": 
					return s_comparisons[6];
				default:
					return null;
			}
		}

		public ValueStorage operandLeft = null;
		public ValueStorage operandRight = null;
		public JumpLabel target = null;
		public ComparisonDelegation comp = null;

		public JumpCommand(AsmInterpreter interpreter, string[] lineParts)
			: base(interpreter, lineParts)
		{
			string comparisonName = lineParts[0];
			string labelName;
			if (comparisonName == "jmp")
			{
				comp = GetComparisonDelegation(comparisonName);
				labelName = lineParts[1];
			}
			else
			{
				string operandLeftName = lineParts[1];
				string operandRightName = lineParts[2];
				labelName = lineParts[3];

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

			JumpLabel label = parent.GetJumpLabelByName(labelName);
			if (label == null)
				throw new InvalidOperationException("Invalid jump target.");
			target = label;
		}

		public override void Run()
		{
			if (operandLeft != null && operandRight != null)
			{
				if (comp(operandLeft.GetValue(), operandRight.GetValue()))
				{
					parent.instructionIndex = target.instructionIndexOfNextLine;
				}
				else
				{
					base.Run();
				}
			}
			else
			{
				parent.instructionIndex = target.instructionIndexOfNextLine;
			}
		}
	}
}
