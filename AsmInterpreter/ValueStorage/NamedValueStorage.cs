using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyInterpreter
{
	public class NamedValueStorage : ValueStorage
	{
		public string name;

		public NamedValueStorage(string name, float value)
		{
			this.name = name;
			this.SetValue(value);
		}
	}
}
