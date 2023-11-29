using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyInterpreter
{
    public class ListRemoveCommand : AsmCommand
    {
        public ValueStorageList target = null;
        public ValueStorage index = null;

        public ListRemoveCommand(AsmInterpreter interpreter, string[] lineParts)
            : base(interpreter, lineParts)
        {
            string targetName = lineParts[1];
            ValueStorageList vsl = null;
            string indexName = lineParts[2];
            ValueStorage vs = null;

            vsl = parent.GetNamedStorageByName(targetName) as ValueStorageList;
            vs = parent.GetVariableByName(indexName);
            if (vs == null)
            {
                vs = parent.GetRegisterByName(indexName);
                {
                    if (vs == null)
                    {
                        vs = parent.GetStackValueFromTop(indexName);
                        if (vs == null)
                        {
                            float number = 0.0f;
                            if (float.TryParse(indexName, out number))
                            {
                                vs = new ValueStorage();
                                vs.SetValue(number);
                                parent.m_constants.Add(vs);
                            }
                        }
                    }
                }
            }
            if (vsl == null)
                throw new InvalidOperationException("Invalid target.");
            if (vs == null)
                throw new InvalidOperationException("Invalid index.");
            target = vsl;
            index = vs;
        }

        public override void Run()
        {
            target.Remove((int)index.GetValue());
            base.Run();
        }
    }
}
