using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblyInterpreter
{
    public class ValueStorageList : INamedStorage
    {
        public string name;
        public string Name => name;

        private List<ValueStorage> valueObjs;

        public void SetValue(int index, ValueStorage newValue)
        {
            valueObjs[index] = newValue;
        }

        public ValueStorage GetValueStorage(int index)
        {
            if (index >= 0 && index < valueObjs.Count)
                return valueObjs[index];
            else
                return null;
        }

        public float GetValue(int index)
        {
            if (index >= 0 && index < valueObjs.Count)
                return valueObjs[index].GetValue();
            else
                throw new IndexOutOfRangeException();
        }

        public void Add(ValueStorage newValue)
        {
            valueObjs.Add(newValue);
        }

        public void Enqueue(ValueStorage newValue)
        {
            Add(newValue);
        }

        public void Push(ValueStorage newValue)
        {
            Add(newValue);
        }

        public ValueStorage Dequeue()
        {
            ValueStorage result = valueObjs[0];
            valueObjs.RemoveAt(0);
            return result;
        }

        public ValueStorage Pop()
        {
            ValueStorage result = valueObjs[valueObjs.Count - 1];
            valueObjs.RemoveAt(valueObjs.Count - 1);
            return result;
        }

        public ValueStorage Remove(int index)
        {
            ValueStorage result = valueObjs[index];
            valueObjs.RemoveAt(index);
            return result;
        }

        public int GetLength()
        {
            return valueObjs.Count;
        }

        public ValueStorageList(string name, params ValueStorage[] values)
        {
            this.name = name;
            if (values != null)
            {
                this.valueObjs = new List<ValueStorage>(values);
            }
            else
            {
                this.valueObjs = new List<ValueStorage>();
            }
        }
    }
}
