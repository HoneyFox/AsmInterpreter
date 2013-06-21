using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyInterpreter
{
	public class AsmInterpreter
	{
		public AsmInterpreter()
		{
			for (int i = 0; i < m_registers.Length; i++)
				m_registers[i] = new ValueStorage();
		}

		public List<AsmCommand> m_commands = new List<AsmCommand>();
		public int instructionIndex = 0;
		public bool m_started = false;
		public bool m_running = false;

		public List<JumpLabel> m_labels = new List<JumpLabel>();
		public JumpLabel GetJumpLabelByName(string name)
		{
			foreach (JumpLabel obj in m_labels)
			{
				if (obj.labelName == name)
				{
					return obj;
				}
			}
			return null;
		}

		public List<NamedValueStorage> m_namedVariables = new List<NamedValueStorage>();
		public NamedValueStorage GetVariableByName(string name)
		{
			foreach (NamedValueStorage obj in m_namedVariables)
			{
				if (obj.name == name)
				{
					return obj;
				}
			}
			return null;
		}

		public List<ValueStorage> m_constants = new List<ValueStorage>();
		
		public ValueStorage[] m_registers = new ValueStorage[64];
		public ValueStorage GetRegisterByName(string name)
		{
			if (name[0] == 'r' || name[0] == 'R' && name[1] == '[' && name[name.Length - 1] == ']')
			{
				int index = Convert.ToInt32(name.Substring(2, name.Length - 3));
				if (index < 0 || index > 63)
					return null;
				return m_registers[index];
			}
			else
				return null;
		}

		public List<ValueStorage> m_stack = new List<ValueStorage>();
		public ValueStorage GetStackValueFromTop(string name)
		{
			if (name[0] == 's' || name[0] == 'S' && name[1] == '[' && name[name.Length - 1] == ']')
			{
				int index = Convert.ToInt32(name.Substring(2, name.Length - 3));
				if (index < 0 || index >= m_stack.Count)
					return null;
				return m_stack[m_stack.Count - 1 - index];
			}
			else
				return null;
		}

		public void LoadString(string codeStr)
		{
			Clear();
			string[] lines = codeStr.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			
			// First remove all comments.
			bool inCommentBody = false;
			for(int i = 0; i < lines.Length; i++)
			{
				if (inCommentBody == false)
				{
					int commentStartIdx = lines[i].IndexOf("/*");
					int commentLineIdx = lines[i].IndexOf("//");
					if (commentStartIdx >= 0 && commentLineIdx >= 0)
					{
						if (commentStartIdx > commentLineIdx)
						{
							// The line is commented.
							lines[i] = lines[i].Substring(0, commentLineIdx);
						}
						else
						{ 
							// The comment body starts.
							lines[i] = lines[i].Substring(0, commentStartIdx);
							inCommentBody = true;
							i--;
						}
					}
					else
					{
						if (commentStartIdx >= 0)
						{
							// The comment body start exists.
							lines[i] = lines[i].Substring(0, commentStartIdx);
							inCommentBody = true;
							i--;
						}
						else if (commentLineIdx >= 0)
						{ 
							// The comment line exists.
							lines[i] = lines[i].Substring(0, commentLineIdx);
						}
					}
				}
				else
				{
					// In comment body. Only focus on comment end.
					if (lines[i].Contains("*/"))
					{
						lines[i] = lines[i].Substring(lines[i].IndexOf("*/") + 2);
						inCommentBody = false;
						i--;
					}
					else
					{
						lines[i] = "";
					}
				}
			}

			// Second pick all labels.
			int lineIndex = 0;
			List<string> codeLines = new List<string>();
			foreach (string line in lines)
			{
				if (line.EndsWith(":"))
				{
					JumpLabel newLabel = new JumpLabel();
					newLabel.instructionIndexOfNextLine = lineIndex;
					newLabel.labelName = line.Substring(0, line.Length - 1).TrimStart(' ', '\t');
					m_labels.Add(newLabel);
				}
				else
				{
					if (line.Trim() == "")
						continue;

					lineIndex++;
					codeLines.Add(line.TrimStart(' ', '\t').TrimEnd(' ', '\t'));
				}
			}

			// Now analyze all code lines.
			lineIndex = 0;
			foreach(string codeLine in codeLines)
			{
				string[] lineParts = codeLine.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
				if (lineParts[0].Equals("var", StringComparison.CurrentCultureIgnoreCase))
				{
					string name = lineParts[1];
					if (m_namedVariables.Exists((NamedValueStorage nvs) => { return nvs.name == name; }) == false)
					{
						float value = 0.0f;
						if (lineParts.Length == 4 && lineParts[2] == "=")
						{
							value = Convert.ToSingle(lineParts[3]);
						}
						NamedValueStorage newNamedValueStorage = new NamedValueStorage(name, value);
						m_namedVariables.Add(newNamedValueStorage);
					}

					NoOperationCommand newCmd = new NoOperationCommand(this, lineParts);
					m_commands.Add(newCmd);
				}
				else if(lineParts[0].Equals("mov", StringComparison.CurrentCultureIgnoreCase))
				{
					AssignmentCommand newCmd = new AssignmentCommand(this, lineParts);
					m_commands.Add(newCmd);
				}
				else if (lineParts.Length == 3 && lineParts[1].Equals("=", StringComparison.CurrentCultureIgnoreCase))
				{
					AssignmentCommand newCmd = new AssignmentCommand(this, new string[] { "mov", lineParts[0], lineParts[2] });
					m_commands.Add(newCmd);
				}
				else if (lineParts[0].Equals("add", StringComparison.CurrentCultureIgnoreCase))
				{
					CalculationCommand newCmd = new CalculationCommand(this, lineParts);
					m_commands.Add(newCmd);
				}
				else if (lineParts.Length == 3 && lineParts[1].Equals("+=", StringComparison.CurrentCultureIgnoreCase))
				{
					CalculationCommand newCmd = new CalculationCommand(this, new string[] { "add", lineParts[0], lineParts[2] });
					m_commands.Add(newCmd);
				}
				else if (lineParts[0].Equals("sub", StringComparison.CurrentCultureIgnoreCase))
				{
					CalculationCommand newCmd = new CalculationCommand(this, lineParts);
					m_commands.Add(newCmd);
				}
				else if (lineParts.Length == 3 && lineParts[1].Equals("-=", StringComparison.CurrentCultureIgnoreCase))
				{
					CalculationCommand newCmd = new CalculationCommand(this, new string[] { "sub", lineParts[0], lineParts[2] });
					m_commands.Add(newCmd);
				}
				else if (lineParts[0].Equals("mul", StringComparison.CurrentCultureIgnoreCase))
				{
					CalculationCommand newCmd = new CalculationCommand(this, lineParts);
					m_commands.Add(newCmd);
				}
				else if (lineParts.Length == 3 && lineParts[1].Equals("*=", StringComparison.CurrentCultureIgnoreCase))
				{
					CalculationCommand newCmd = new CalculationCommand(this, new string[] { "mul", lineParts[0], lineParts[2] });
					m_commands.Add(newCmd);
				}
				else if (lineParts[0].Equals("div", StringComparison.CurrentCultureIgnoreCase))
				{
					CalculationCommand newCmd = new CalculationCommand(this, lineParts);
					m_commands.Add(newCmd);
				}
				else if (lineParts.Length == 3 && lineParts[1].Equals("/=", StringComparison.CurrentCultureIgnoreCase))
				{
					CalculationCommand newCmd = new CalculationCommand(this, new string[] { "div", lineParts[0], lineParts[2] });
					m_commands.Add(newCmd);
				}
				else if (lineParts[0].Equals("mod", StringComparison.CurrentCultureIgnoreCase))
				{
					CalculationCommand newCmd = new CalculationCommand(this, lineParts);
					m_commands.Add(newCmd);
				}
				else if (lineParts.Length == 3 && lineParts[1].Equals("%=", StringComparison.CurrentCultureIgnoreCase))
				{
					CalculationCommand newCmd = new CalculationCommand(this, new string[] { "mod", lineParts[0], lineParts[2] });
					m_commands.Add(newCmd);
				}
				else if (lineParts[0].Equals("pow", StringComparison.CurrentCultureIgnoreCase))
				{
					CalculationCommand newCmd = new CalculationCommand(this, lineParts);
					m_commands.Add(newCmd);
				}
				else if (lineParts.Length == 3 && lineParts[1].Equals("^=", StringComparison.CurrentCultureIgnoreCase))
				{
					CalculationCommand newCmd = new CalculationCommand(this, new string[] { "pow", lineParts[0], lineParts[2] });
					m_commands.Add(newCmd);
				}
				else if (lineParts[0].Equals("jmp", StringComparison.CurrentCultureIgnoreCase))
				{
					JumpCommand newCmd = new JumpCommand(this, lineParts);
					m_commands.Add(newCmd);
				}
				else if (lineParts[0].Equals("je", StringComparison.CurrentCultureIgnoreCase))
				{
					JumpCommand newCmd = new JumpCommand(this, lineParts);
					m_commands.Add(newCmd);
				}
				else if (lineParts[0].Equals("jne", StringComparison.CurrentCultureIgnoreCase))
				{
					JumpCommand newCmd = new JumpCommand(this, lineParts);
					m_commands.Add(newCmd);
				}
				else if (lineParts[0].Equals("jb", StringComparison.CurrentCultureIgnoreCase))
				{
					JumpCommand newCmd = new JumpCommand(this, lineParts);
					m_commands.Add(newCmd);
				}
				else if (lineParts[0].Equals("jl", StringComparison.CurrentCultureIgnoreCase))
				{
					JumpCommand newCmd = new JumpCommand(this, lineParts);
					m_commands.Add(newCmd);
				}
				else if (lineParts[0].Equals("jbe", StringComparison.CurrentCultureIgnoreCase))
				{
					JumpCommand newCmd = new JumpCommand(this, lineParts);
					m_commands.Add(newCmd);
				}
				else if (lineParts[0].Equals("jle", StringComparison.CurrentCultureIgnoreCase))
				{
					JumpCommand newCmd = new JumpCommand(this, lineParts);
					m_commands.Add(newCmd);
				}
				else if (lineParts[0].Equals("if", StringComparison.CurrentCultureIgnoreCase))
				{
					int correspondingNode = FindCorrespondingEndNode(codeLines, lineIndex);
					if (correspondingNode == -1)
						throw new InvalidOperationException("Invalid if block.");
					IfCommand newCmd = new IfCommand(this, lineParts);
					newCmd.targetInstructionIndex = correspondingNode;
					m_commands.Add(newCmd);
				}
				else if (lineParts[0].Equals("else", StringComparison.CurrentCultureIgnoreCase))
				{
					NoOperationCommand newCmd = new NoOperationCommand(this, lineParts);
					m_commands.Add(newCmd);
				}
				else if (lineParts[0].Equals("while", StringComparison.CurrentCultureIgnoreCase))
				{
					int correspondingNode = FindCorrespondingEndNode(codeLines, lineIndex);
					if (correspondingNode == -1)
						throw new InvalidOperationException("Invalid while block.");
					IfCommand newCmd = new IfCommand(this, lineParts);
					newCmd.targetInstructionIndex = correspondingNode + 1;
					m_commands.Add(newCmd);
				}
				else if (lineParts[0].Equals("end", StringComparison.CurrentCultureIgnoreCase))
				{
					string startNodeType = "";
					int correspondingNode = FindCorrespondingStartNode(codeLines, lineIndex, out startNodeType);
					if (correspondingNode == -1)
						throw new InvalidOperationException("Invalid end block.");
					if (startNodeType == "if")
					{
						EndCommand newCmd = new EndCommand(this, lineParts);
						m_commands.Add(newCmd);
					}
					else if (startNodeType == "while")
					{
						EndCommand newCmd = new EndCommand(this, lineParts);
						newCmd.loopInstructionIndex = correspondingNode;
						m_commands.Add(newCmd);
					}
				}
				else if (lineParts[0].Equals("push", StringComparison.CurrentCultureIgnoreCase))
				{
					PushCommand newCmd = new PushCommand(this, lineParts);
					m_commands.Add(newCmd);
				}
				else if (lineParts[0].Equals("pop", StringComparison.CurrentCultureIgnoreCase))
				{
					PopCommand newCmd = new PopCommand(this, lineParts);
					m_commands.Add(newCmd);
				}
				else if (lineParts[0].Equals("yield", StringComparison.CurrentCultureIgnoreCase))
				{
					YieldCommand newCmd = new YieldCommand(this, lineParts);
					m_commands.Add(newCmd);
				}
				else if (lineParts[0].Equals("nop", StringComparison.CurrentCultureIgnoreCase))
				{
					NoOperationCommand newCmd = new NoOperationCommand(this, lineParts);
					m_commands.Add(newCmd);
				}
				//else if (lineParts[0].Equals("trig", StringComparison.CurrentCultureIgnoreCase))
				//{
				//    // Need to implement this later.
				//    NoOperationCommand newCmd = new NoOperationCommand(this, lineParts);
				//    m_commands.Add(newCmd);
				//}
				else
				{
					if (UserCommandModuleHandler.HandleUserCommand(this, codeLines, lineIndex, lineParts) == false)
					{
						throw new InvalidOperationException("Unknown command.");
					}
				}
				
				lineIndex++;
			}
		}

		private int FindCorrespondingEndNode(List<string> codeLines, int lineIndex)
		{
			int blockLevel = 1;
			int currentLineIndex = lineIndex + 1;
			while (blockLevel != 0 && currentLineIndex < codeLines.Count)
			{
				string line = codeLines[currentLineIndex];
				string[] lineParts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				if (lineParts[0].Equals("if", StringComparison.CurrentCultureIgnoreCase))
				{
					blockLevel++;
				}
				if (lineParts[0].Equals("while", StringComparison.CurrentCultureIgnoreCase))
				{
					blockLevel++;
				}
				if (lineParts[0].Equals("else", StringComparison.CurrentCultureIgnoreCase))
				{
					if (blockLevel == 1)
					{
						return currentLineIndex;
					}
				}
				if (lineParts[0].Equals("end", StringComparison.CurrentCultureIgnoreCase))
				{
					blockLevel--;
					if (blockLevel == 0)
					{
						return currentLineIndex;
					}
				}

				currentLineIndex++;
			}

			return -1;
		}

		private int FindCorrespondingStartNode(List<string> codeLines, int lineIndex, out string startNodeType)
		{
			int blockLevel = 1;
			int currentLineIndex = lineIndex - 1;
			while (blockLevel != 0 && currentLineIndex > 0)
			{
				string line = codeLines[currentLineIndex];
				string[] lineParts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				if (lineParts[0].Equals("if", StringComparison.CurrentCultureIgnoreCase))
				{
					blockLevel--;
					if (blockLevel == 0)
					{
						startNodeType = "if";
						return currentLineIndex;
					}
				}
				if (lineParts[0].Equals("while", StringComparison.CurrentCultureIgnoreCase))
				{
					blockLevel--;
					if (blockLevel == 0)
					{
						startNodeType = "while";
						return currentLineIndex;
					}
				}
				if (lineParts[0].Equals("end", StringComparison.CurrentCultureIgnoreCase))
				{
					blockLevel++;
				}

				currentLineIndex--;
			}

			startNodeType = "";
			return -1;
		}


		public void Clear()
		{
			m_labels.Clear();
			m_commands.Clear();
			m_constants.Clear();
		}

		public void Reset()
		{
			for(int i = 0; i < m_registers.Length; i++)
				m_registers[i].SetValue(0.0f);
		}

		public void Start()
		{
			Reset();

			m_started = true;
			m_running = true;

			Run();
		}

		public void Run()
		{
			while(instructionIndex < m_commands.Count && m_running == true)
			{
#if DEBUG
				Console.WriteLine(m_commands[instructionIndex].line);
#else
				if (m_commands[instructionIndex] is NoOperationCommand) Console.WriteLine(m_commands[instructionIndex].line);
#endif
				m_commands[instructionIndex].Run();
			}
			if (m_running == true)
			{
				m_started = false;
				m_running = false;
			}
		}

		public void Resume()
		{
			if (m_started == true)
			{
				m_running = true;
				Run();
			}
			else
				return;
		}

		public void Pause()
		{
			if (m_running == true)
			{
				m_running = false;
			}
		}
	}
}
