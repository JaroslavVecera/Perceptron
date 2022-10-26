using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.MVVM.ViewModel
{
    class OutputNodeViewModel : PositionableViewModel
    {
        public event Func<int?> GetOutput;

        public int? Output
        {
            get { return GetOutput?.Invoke(); }
        }
    }
}
