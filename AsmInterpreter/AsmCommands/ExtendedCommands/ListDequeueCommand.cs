using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyInterpreter
{
    public class ListDequeueCommand : AsmCommand
    {
        public ValueStorageList source = null;
        public ValueStorage target = null;

        public ListDequeueCommand(AsmInterpreter interpreter, string[] lineParts)
            : base(interpreter, lineParts)
        {
            string sourceName = lineParts[1];
            ValueStorageList vsl = null;
            string targetName = lineParts[2];
            ValueStorage vs = null;

            vsl = parent.GetNamedStorageByName(targetName) as ValueStorageList;
            vs = parent.GetVariableByName(sourceName);
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
                throw new InvalidOperationException("Invalid dequeue source.");
            if (vs == null)
                throw new InvalidOperationException("Invalid dequeue target.");
            source = vsl;
            target = vs;
        }

        public override void Run()
        {
            target.SetValue(source.Dequeue().GetValue());
            base.Run();
        }
    }
}
