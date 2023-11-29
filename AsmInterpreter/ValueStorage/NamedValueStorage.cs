using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyInterpreter
{
	public class NamedValueStorage : ValueStorage, INamedStorage
	{
		public string name;
        string INamedStorage.Name => name;

		public NamedValueStorage(string name, float value)
		{
			this.name = name;
			this.SetValue(value);
		}
	}
}
