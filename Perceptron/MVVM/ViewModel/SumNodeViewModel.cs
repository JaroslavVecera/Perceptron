using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.MVVM.ViewModel
{
    class SumNodeViewModel : PositionableViewModel
    {
        public event Func<float?> GetSum;
        public event Action< float> OnSetBias;
        public event Func<float> OnGetBias;

        protected bool _isNumeric;
        public bool IsNumeric
        {
            get { return _isNumeric; }
            private set
            {
                _isNumeric = value;
                OnPropertyChanged();
            }
        }

        public double Value
        {
            get { return (double)OnGetBias.Invoke(); }
            set { OnSetBias.Invoke((float)value); }
        }

        string _bias;
        public string Bias
        {
            get { return _bias; }
            set
            {
                _bias = value;
                ParseValue(value);
            }
        }

        public virtual void OnValueChanged()
        {
            Bias = Value.ToString();
            OnPropertyChanged("Bias");
        }

        protected void ParseValue(string value)
        {
            double parsed = 0;
            bool canParse = double.TryParse(value, out parsed);
            if (canParse != IsNumeric)
                IsNumeric = canParse;
            if (canParse)
                Value = parsed;
        }

        public double? Sum
        {
            get { return (double?)GetSum.Invoke(); }
        }
    }
}
