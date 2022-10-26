using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.MVVM.ViewModel
{
    class WeightViewModel : InputViewModel
    {
        public WeightViewModel(int index) : base(index)
        {

        }

        public override void OnValueChanged()
        {
            Weight = Value.ToString();
            base.OnValueChanged();
        }

        string _weight;
        public string Weight
        {
            get { return _weight; }
            set
            {
                _weight = value;
                ParseValue(value);
            }
        }
    }
}
