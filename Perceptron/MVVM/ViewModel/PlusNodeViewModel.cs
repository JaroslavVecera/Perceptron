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

        public RelayCommand AddCommand { get; set; }

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
    }
}
