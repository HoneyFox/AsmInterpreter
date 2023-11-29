using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyInterpreter
{
    public class ListLengthCommand : AsmCommand
    {
        public ValueStorageList source = null;
        public ValueStorage target = null;

        public ListLengthCommand(AsmInterpreter interpreter, string[] lineParts)
            : base(interpreter, lineParts)
        {
            string sourceName = lineParts[1];
            ValueStorageList vsl = null;
            string targetName = lineParts[2];
            ValueStorage vs = null;

            vsl = parent.GetNamedStorageByName(sourceName) as ValueStorageList;
            vs = parent.GetVariableByName(targetName);
            if (vs == null)
            {
                vs = parent.GetRegisterByName(sourceName);
                {
                    if (vs == null)
                    {
                        vs = parent.GetStackValueFromTop(sourceName);
                        if (vs == null)
                        {
                            float number = 0.0f;
                            if (float.TryParse(sourceName, out number))
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
                throw new InvalidOperationException("Invalid length source.");
            if (vs == null)
                throw new InvalidOperationException("Invalid length target.");
            source = vsl;
            target = vs;
        }

        public override void Run()
        {
            target.SetValue(source.GetLength());
            base.Run();
        }
    }
}
