using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.Network
{
    public class InputLayer
    {
        protected int _size;
        public int Size { get { return _size; } }
        float[] _inputArray;
        public float[] InputArray { set { if (value.Count() == Size) _inputArray = value; else throw new InvalidOperationException(); } }
        public float[] Output { get { return _inputArray; } }

        public InputLayer(int size)
        {
            _size = size; _inputArray = new float[size];
        }

        public float this[int key]
        {
            get { return _inputArray[key]; }
            set { _inputArray[key] = value; }
        }
    }
}
