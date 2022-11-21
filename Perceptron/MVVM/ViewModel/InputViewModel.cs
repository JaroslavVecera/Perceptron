using Perceptron.Core;
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
        public RelayCommand<ArrowType> ArrowCommand { get; set; }
        public event Action<ArrowType, int> Arrow;
        public event Action OnInputChanged;
        bool _focused = false;
        public bool Focused { get { return _focused; } set { _focused = value; OnPropertyChanged(); } }

        protected bool _isNumeric;
        public bool IsNumeric
        {
            get { return _isNumeric; }
            protected set
            {
                _isNumeric = value;
                OnPropertyChanged();
            }
        }

        public float Value
        {
            get { return OnGetValue.Invoke(Index); }
            set { OnSetValue.Invoke(Index, value);  }
        }

        public virtual void OnValueChanged()
        {
            OnPropertyChanged("Input");
        }

        public InputViewModel(int index)
        {
            Index = index;
            ArrowCommand = new RelayCommand<ArrowType>(t =>
            {
                Arrow?.Invoke(t, Index);
            });
        }

        protected virtual void ParseValue(string value)
        {
            float parsed = 0;
            bool canParse = float.TryParse(value, out parsed);
            if (canParse != IsNumeric)
            {
                IsNumeric = canParse;
                OnInputChanged?.Invoke();
            }
            if (canParse)
                Value = parsed;
        }

        public void Focus()
        {
            Focused = false;
            Focused = true;
        }
    }
}
