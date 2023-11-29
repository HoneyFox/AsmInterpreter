using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyInterpreter
{
    public class PrintCommand : AsmCommand
    {
        public ValueStorage[] values = null;

        public PrintCommand(AsmInterpreter interpreter, string[] lineParts)
            : base(interpreter, lineParts)
        {
            if (lineParts.Length > 1)
            {
                values = new ValueStorage[lineParts.Length - 1];
                for (int i = 1; i < lineParts.Length; i++)
                {
                    values[i - 1] = parent.GetVariableByName(lineParts[i]);
                }
            }
        }

        public override void Run()
        {
            StringBuilder sb = new StringBuilder();
            if (values.Length > 0)
            {
                sb.Append(values[0].GetValue().ToString());
                for (int i = 1; i < values.Length; i++)
                {
                    sb.Append(" ").Append(values[i].GetValue().ToString());
                }
            }
            Console.WriteLine(sb.ToString());
            base.Run();
        }
    }
}
