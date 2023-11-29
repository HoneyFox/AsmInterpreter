using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyInterpreter
{
    public class ListIndexerCommand : AsmCommand
    {
        public ValueStorageList source = null;
        public ValueStorage index = null;
        public ValueStorage target = null;

        public ListIndexerCommand(AsmInterpreter interpreter, string[] lineParts)
            : base(interpreter, lineParts)
        {
            string sourceName = lineParts[1];
            ValueStorageList vsl = null;
            string indexName = lineParts[2];
            ValueStorage vsi = null;
            string targetName = lineParts[3];
            ValueStorage vs = null;

            vsl = parent.GetNamedStorageByName(sourceName) as ValueStorageList;
            vsi = parent.GetVariableByName(indexName);
            if (vsi == null)
            {
                vsi = parent.GetRegisterByName(indexName);
                {
                    if (vsi == null)
                    {
                        vsi = parent.GetStackValueFromTop(indexName);
                    }
                }
            }
            vs = parent.GetVariableByName(targetName);
            if (vs == null)
            {
                vs = parent.GetRegisterByName(sourceName);
                {
                    if (vs == null)
                    {
                        vs = parent.GetStackValueFromTop(sourceName);
                    }
                }
            }
            if (vsl == null)
                throw new InvalidOperationException("Invalid indexer source.");
            if (vsi == null)
                throw new InvalidOperationException("Invalid indexer index.");
            if (vs == null)
                throw new InvalidOperationException("Invalid indexer target.");
            source = vsl;
            index = vsi;
            target = vs;
        }

        public override void Run()
        {
            target.SetValue(source.GetValue((int)index.GetValue()));
            base.Run();
        }
    }
}
