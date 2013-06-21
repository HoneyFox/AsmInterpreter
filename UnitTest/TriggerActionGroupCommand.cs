using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using AssemblyInterpreter;

namespace UnitTest
{
	public class TriggerActionGroupCommand :  AsmCommand
	{
		//public static ActionGroupController actionGroupController = new ActionGroupController();

		public string actionGroupName = "None";

		public TriggerActionGroupCommand(AsmInterpreter interpreter, string[] lineParts)
			: base(interpreter, lineParts)
		{
			actionGroupName = lineParts[1];
		}

		public override void Run()
		{
			KSPActionGroup group = KSPActionGroup.None;
			if(Enum.TryParse<KSPActionGroup>(actionGroupName, out group))
			{
				//actionGroupController.FireAction(group, KSPActionType.Activate);
				//Debug.Log("Firing Action Group: " + actionGroupName);
				Console.WriteLine("Firing Action Group: " + actionGroupName);
			}
			else
			{
				//Debug.Log("Invalid Action Group Name.");
			}
			base.Run();
		}
	}
}
