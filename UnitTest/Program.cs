using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssemblyInterpreter;

namespace UnitTest
{
	class Program
	{
		private static bool HandleTriggerCommand(AsmInterpreter interpreter, List<string> codeLines, int lineIndex, string[] lineParts)
		{
			if (lineParts[0].Equals("trig", StringComparison.CurrentCultureIgnoreCase))
			{
			    // Need to implement this later.
				TriggerActionGroupCommand newCmd = new TriggerActionGroupCommand(interpreter, lineParts);
				interpreter.m_commands.Add(newCmd);
				return true;
			}
			return false;
		}
		static void Main(string[] args)
		{
			AsmInterpreter asmInterpreter = new AsmInterpreter();
			UserCommandModuleHandler.RegisterCodeHandler(HandleTriggerCommand);

			asmInterpreter.m_namedVariables.Add(new NamedValueStorage("MissionTime", 0.0f));
			asmInterpreter.m_namedVariables.Add(new NamedValueStorage("AirDensity", 0.0f));
			asmInterpreter.m_namedVariables.Add(new NamedValueStorage("TotalAvailableThrust", 0.0f));
			string testCode =
@"// Initialize variables
var boosterDecoupled = 0
var fairingsDecoupled = 0
var solarPanelsExpanded = 0

// Wait for update() calls.
yield

// Main loop goes here.
r[0] = boosterDecoupled
r[0] += fairingsDecoupled
r[0] += solarPanelsExpanded
while r[0] < 3
	// Don't start triggering action groups too early.
	if MissionTime >= 15
		// If boosters are exhausted, decouple them.
		if TotalAvailableThrust < 1500
			if boosterDecoupled != 1 // Check if we've already decoupled.
				trig Custom01
				boosterDecoupled = 1
			end
		end

		// If air density is low enough, we can decouple the fairings to reduce weight.
		if AirDensity <= 0.0001
			if fairingsDecoupled != 1 // Check if we've already decoupled.
				trig Custom02
				fairingsDecoupled = 1
			end
		end

		// If we've got to the last stage, we should expand our solar panels to gain electricity.
		if TotalAvailableThrust <= 100
			if solarPanelsExpanded != 1 // Check if we've already expanded.
				trig Custom03
				solarPanelsExpanded = 1
			end
		end
	end
	
	yield

	// A check-up to see if we've finished all three jobs.
	r[0] = boosterDecoupled
	r[0] += fairingsDecoupled
	r[0] += solarPanelsExpanded
end
";
			asmInterpreter.LoadString(testCode);
			asmInterpreter.Start();
			float MissionTime = 0.0f;
			float AirDensity = 1.0f;
			float TotalAvailableThrust = 1800.0f;
			while (asmInterpreter.m_started)
			{
				MissionTime += 0.0001f;
				AirDensity -= 0.000001f;
				if (AirDensity < 0.0f) AirDensity = 0.0f;

				if (MissionTime >= 25.0f) TotalAvailableThrust = 1200.0f;
				if (MissionTime >= 120.0f) TotalAvailableThrust = 450.0f;
				if (MissionTime >= 300.0f) TotalAvailableThrust = 10.0f;

				asmInterpreter.GetVariableByName("MissionTime").SetValue(MissionTime);
				asmInterpreter.GetVariableByName("AirDensity").SetValue(AirDensity);
				asmInterpreter.GetVariableByName("TotalAvailableThrust").SetValue(TotalAvailableThrust);
				asmInterpreter.Resume();
			}
		}
	}
}
