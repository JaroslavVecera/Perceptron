using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.MVVM.ViewModel
{
    class OutputNodeViewModel : InputViewModel
    {
        public event Func<int, int?> GetOutput;

        public OutputNodeViewModel(int index) : base(index)
        {

        }

        public int? Output
        {
            get { return GetOutput?.Invoke(Index); }
        }
    }
}
