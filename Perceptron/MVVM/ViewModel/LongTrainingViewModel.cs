using Perceptron.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Perceptron.MVVM.ViewModel
{
    class LongTrainingViewModel : ObservableObject
    {
        public void SetVisibility(bool val)
        {
            Visibility = val ? Visibility.Visible : Visibility.Collapsed;
        }

        Visibility _visibility = Visibility.Visible;
        public Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                OnPropertyChanged();
            }
        }

        public event Action<float> OnSetCoefficient;
        public event Func<float> OnGetCoefficient;

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

        protected bool _isValidOutput;
        public bool IsValidOutput
        {
            get { return _isValidOutput; }
            private set
            {
                _isValidOutput = value;
                OnPropertyChanged();
            }
        }

        public float Coefficient
        {
            get { return OnGetCoefficient.Invoke(); }
            set { OnSetCoefficient.Invoke(value); }
        }

        public virtual void OnValueChanged()
        {
            CoefficientInput = Coefficient.ToString();
            OnPropertyChanged("CoefficientInput");
        }

        string _coefficientInput;
        public string CoefficientInput
        {
            get { return _coefficientInput; }
            set
            {
                _coefficientInput = value;
                ParseValue(value);
            }
        }

        protected void ParseValue(string value)
        {
            float parsed = 0;
            bool canParse = float.TryParse(value, out parsed);
            if (canParse != IsNumeric)
                IsNumeric = canParse;
            if (canParse)
                Coefficient = parsed;
        }
    }
}
