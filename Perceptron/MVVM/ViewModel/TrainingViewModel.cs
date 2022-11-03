using Perceptron.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.MVVM.ViewModel
{
    public class TrainingViewModel : PositionableViewModel
    {
        public event Action<float> OnSetCoefficient;
        public event Func<float> OnGetCoefficient;
        public event Action<int> OnSetOutput;
        public event Action<bool> OnSetTraining;
        public RelayCommand<ArrowType> ArrowCommand1 { get; set; }
        public RelayCommand<ArrowType> ArrowCommand2 { get; set; }
        public event Action<ArrowType, int> Arrow;
        bool _focusedCoefficient = false;
        public bool FocusedCoefficient { get { return _focusedCoefficient; } set { _focusedCoefficient = value; OnPropertyChanged(); } }
        bool _focusedOutput = false;
        public bool FocusedOutput { get { return _focusedOutput; } set { _focusedOutput = value; OnPropertyChanged(); } }

        public TrainingViewModel()
        {
            ParseOutput("0");
            ArrowCommand1 = new RelayCommand<ArrowType>(t =>
            {
                Arrow?.Invoke(t, 0);
            });
            ArrowCommand2 = new RelayCommand<ArrowType>(t =>
            {
                Arrow?.Invoke(t, 1);
            });
        }

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
            set { OnSetOutput?.Invoke(value); }
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

        string _outputInput = "0";
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

        public void Focus(int index)
        {
            if (index == 0)
            {
                FocusedCoefficient = false;
                FocusedCoefficient = true;
            }
            else
            {
                FocusedOutput = false;
                FocusedOutput = true;
            }
        }
    }
}
