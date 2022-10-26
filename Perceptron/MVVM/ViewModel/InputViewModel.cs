using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.MVVM.ViewModel
{
    abstract class InputViewModel : PositionableViewModel
    {
        protected int Index { get; } = 0;
        public event Action<int, float> OnSetValue;
        public event Func<int, float> OnGetValue;

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
            get { return (double)OnGetValue.Invoke(Index); }
            set { OnSetValue.Invoke(Index, (float)value);  }
        }

        public virtual void OnValueChanged()
        {
            OnPropertyChanged("Input");
        }

        public InputViewModel(int index)
        {
            Index = index;
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
    }
}
