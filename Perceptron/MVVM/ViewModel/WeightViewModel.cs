using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.MVVM.ViewModel
{
    class WeightViewModel : InputViewModel
    {
        public string Latex { get; set; }

        public WeightViewModel(int index) : base(index)
        {
            Latex = "w_" + index + "=";
        }

        public override void OnValueChanged()
        {
            Weight = Value.ToString();
            OnPropertyChanged("Weight");
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
