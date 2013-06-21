using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyInterpreter
{
	public class UserCommandModuleHandler
	{
		public static List<Func<AsmInterpreter, List<string>, int, string[], bool>> RegisterCallbacks = new List<Func<AsmInterpreter, List<string>, int, string[], bool>>();
		public static void RegisterCodeHandler(Func<AsmInterpreter, List<string>, int, string[], bool> registerCallback)
		{
			RegisterCallbacks.Add(registerCallback);
		}

		public static bool HandleUserCommand(AsmInterpreter interpreter, List<string> codeLines, int lineIndex, string[] lineParts)
		{
			foreach (var callback in RegisterCallbacks)
			{
				if (callback(interpreter, codeLines, lineIndex, lineParts) == true)
					return true;
			}

			return false;
		}
	}
}
