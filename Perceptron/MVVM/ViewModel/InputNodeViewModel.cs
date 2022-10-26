using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.MVVM.ViewModel
{
    class InputNodeViewModel : InputViewModel
    {
        public InputNodeViewModel(int index) : base(index)
        {

        }

        public override void OnValueChanged()
        {
            Input = Value.ToString();
            base.OnValueChanged();
        }

        string _input;
        public string Input
        {
            get { return _input; }
            set
            {
                _input = value;
                ParseValue(value);
            }
        }
    }
}
