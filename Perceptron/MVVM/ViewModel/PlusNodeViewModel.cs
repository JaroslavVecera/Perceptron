using Perceptron.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.MVVM.ViewModel
{
    class PlusNodeViewModel : PositionableViewModel
    {
        public event Action AddInputNode;
        public event Func<bool> GetIsPlusButtonEnabled;

        public RelayCommand AddCommand { get; set; }

        public bool IsEnabled
        { 
            get
            {
                return GetIsPlusButtonEnabled.Invoke();
            }
        }

        public PlusNodeViewModel()
        {
            InitializeCommands();
        }

        void InitializeCommands()
        {
            AddCommand = new RelayCommand(o =>
            {
                AddInputNode?.Invoke();
            });
        }

        public void OnEnabledChanged()
        {
            OnPropertyChanged("IsEnabled");
        }
    }
}
