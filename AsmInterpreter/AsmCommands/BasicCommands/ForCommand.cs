using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyInterpreter
{
    public class ForCommand : AsmCommand
    {
        ValueStorage loopVariable = null;
        ValueStorage loopBegin = null;
        ValueStorage loopEnd = null;
        ValueStorage loopStep = null;

        private bool loopBeginExecuted = false;
        public int targetInstructionIndex = -1;

        public ForCommand(AsmInterpreter interpreter, string[] lineParts)
            : base(interpreter, lineParts)
        {
            // for i = b to e step s
            if (lineParts[2] == "=" && lineParts[4] == "to"
                && (lineParts.Length == 6 || (lineParts.Length == 8 && lineParts[6] == "step"))
            )
            {
                string loopVariableName = lineParts[1];
                string loopBeginName = lineParts[3];
                string loopEndName = lineParts[5];
                loopVariable = parent.GetVariableByName(loopVariableName);
                loopBegin = parent.GetVariableByName(loopBeginName);
                if (loopBegin == null)
                {
                    float number = 0.0f;
                    if (float.TryParse(loopBeginName, out number))
                    {
                        loopBegin = new ValueStorage();
                        loopBegin.SetValue(number);
                        parent.m_constants.Add(loopBegin);
                    }
                }
                loopEnd = parent.GetVariableByName(loopEndName);
                if (loopEnd == null)
                {
                    float number = 0.0f;
                    if (float.TryParse(loopEndName, out number))
                    {
                        loopEnd = new ValueStorage();
                        loopEnd.SetValue(number);
                        parent.m_constants.Add(loopEnd);
                    }
                }

                if (lineParts.Length == 6)
                {
                    loopStep = new ValueStorage();
                    loopStep.SetValue(1);
                    
                }
                else
                {
                    string loopStepName = lineParts[7];
                    loopStep = parent.GetVariableByName(loopStepName);
                    if (loopStep == null)
                    {
                        float number = 0.0f;
                        if (float.TryParse(loopStepName, out number))
                        {
                            loopStep = new ValueStorage();
                            loopStep.SetValue(number);
                            parent.m_constants.Add(loopStep);
                        }
                    }
                }

                if (loopVariable == null)
                {
                    throw new InvalidOperationException("Invalid for loop variable.");
                }
                if (loopBegin == null)
                {
                    throw new InvalidOperationException("Invalid for loop begin.");
                }
                if (loopEnd == null)
                {
                    throw new InvalidOperationException("Invalid for loop end.");
                }
                if (loopStep == null)
                {
                    throw new InvalidOperationException("Invalid for loop step.");
                }
            }
            else
                throw new InvalidOperationException("Invalid for command.");
        }

        public override void Run()
        {
            if (loopBeginExecuted == false)
            {
                loopVariable.SetValue(loopBegin.GetValue());
                loopBeginExecuted = true;
            }
            else
            {
                loopVariable.SetValue(loopVariable.GetValue() + loopStep.GetValue());
            }

            bool exitLoop = false;
            if (loopStep.GetValue() > 0)
            {
                if (loopVariable.GetValue() > loopEnd.GetValue())
                    exitLoop = true;
            }
            else if (loopStep.GetValue() < 0)
            {
                if (loopVariable.GetValue() < loopEnd.GetValue())
                    exitLoop = true;
            }
            if (!exitLoop)
            {
                base.Run();
            }
            else
            {
                parent.instructionIndex = targetInstructionIndex;
            }
        }

        public override void Reset()
        {
            loopBeginExecuted = false;
            base.Reset();
        }
    }
}
