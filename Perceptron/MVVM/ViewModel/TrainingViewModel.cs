using Perceptron.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.MVVM.ViewModel
{
    class TrainingViewModel : PositionableViewModel
    {
        public event Action<float> OnSetCoefficient;
        public event Func<float> OnGetCoefficient;
        public event Action<int> OnSetOutput;
        public event Action<bool> OnSetTraining;

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

        public bool IsEnabled
        {
            set { OnSetTraining.Invoke(value); }
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

        public int DesiredOutput
        {
            set { OnSetOutput.Invoke(value); }
        }

        public virtual void OnValueChanged()
        {
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

        string _outputInput;
        public string OutputInput
        {
            get { return _outputInput; }
            set
            {
                _outputInput = value;
                ParseOutput(value);
            }
        }

        protected void ParseOutput(string output)
        {
            bool isOutputValid = output == "0" || output == "1";
            if (isOutputValid != IsValidOutput)
                IsValidOutput = isOutputValid;
            if (isOutputValid)
                DesiredOutput = output == "0" ? 0 : 1;
        }
    }
}
