using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Perceptron.MVVM.ViewModel
{
    class EdgeViewModel : PositionableViewModel
    {
        Point _source;
        Point _sink;
        
        public Point Source
        {
            get { return _source; }
            set
            {
                _source = value;
                OnPropertyChanged();
            }
        }

        public Point Sink
        {
            get { return _sink; }
            set
            {
                _sink = value;
                OnPropertyChanged();
            }
        }
    }
}
