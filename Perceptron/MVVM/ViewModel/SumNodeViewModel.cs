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
        public event Action<float> OnSetBias;
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

        public float Value
        {
            get { return (float)OnGetBias.Invoke(); }
            set { OnSetBias.Invoke(value); }
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
            float parsed = 0;
            bool canParse = float.TryParse(value, out parsed);
            if (canParse != IsNumeric)
                IsNumeric = canParse;
            if (canParse)
                Value = parsed;
        }

        public float? Sum
        {
            get
            {
                float? res = (float?)GetSum.Invoke();
                if (res.HasValue)
                    res = (float)Math.Round(res.Value, 4);
                return res;
            }
        }
    }
}
