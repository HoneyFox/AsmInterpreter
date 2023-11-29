using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyInterpreter
{
    public class ListAddCommand : AsmCommand
    {
        public ValueStorage source = null;
        public ValueStorageList target = null;

        public ListAddCommand(AsmInterpreter interpreter, string[] lineParts)
            : base(interpreter, lineParts)
        {
            string targetName = lineParts[1];
            ValueStorageList vsl = null;
            string sourceName = lineParts[2];
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
                throw new InvalidOperationException("Invalid add/push/enqueue target.");
            if (vs == null)
                throw new InvalidOperationException("Invalid add/push/enqueue source.");
            target = vsl;
            source = vs;
        }

        public override void Run()
        {
            ValueStorage itemPushed = new ValueStorage();
            itemPushed.SetValue(source.GetValue());
            target.Add(itemPushed);
            base.Run();
        }
    }

    public class ListEnqueueCommand : ListAddCommand
    {
        public ListEnqueueCommand(AsmInterpreter interpreter, string[] lineParts)
            : base(interpreter, lineParts) { }
    }

    public class ListPushCommand : ListAddCommand
    {
        public ListPushCommand(AsmInterpreter interpreter, string[] lineParts)
            : base(interpreter, lineParts) { }
    }
}
