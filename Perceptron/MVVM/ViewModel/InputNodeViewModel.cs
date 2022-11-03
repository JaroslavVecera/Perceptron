using Perceptron.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.MVVM.ViewModel
{
    class InputNodeViewModel : InputViewModel
    {
        public event Action<int> OnRemove;
        public event Func<bool> OnGetCrossButtonEnabled;

        public RelayCommand RemoveCommand { get; set; }

        public bool CrossButtonEnabled { get { return OnGetCrossButtonEnabled.Invoke(); } }

        public InputNodeViewModel(int index) : base(index)
        {
            InitializeCommands();
        }

        void InitializeCommands()
        { 
            RemoveCommand = new RelayCommand(o =>
            {
                OnRemove.Invoke(Index);
            });
        }

        public override void OnValueChanged()
        {
            Input = Value.ToString();
            base.OnValueChanged();
        }

        public void OnCrossButtonEnabledChanged()
        {
            OnPropertyChanged("CrossButtonEnabled");
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
