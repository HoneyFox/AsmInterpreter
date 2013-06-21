using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyInterpreter
{
	public class ValueStorage
	{
		float valueObj = 0.0f;

		public void SetValue(float newValue)
		{
			valueObj = newValue;
		}

		public float GetValue()
		{
			return valueObj;
		}
	}
}
