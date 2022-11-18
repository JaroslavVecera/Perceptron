using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Perceptron.MVVM.ViewModel
{
    class DesiredOutputViewModel : InputViewModel
    {
        public DesiredOutputViewModel(int index) : base(index)
        {
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

        public override void OnValueChanged()
        {
            base.OnValueChanged();
            OnPropertyChanged("Visibility");
        }

        public bool Input
        {
            get { return Value == 1; }
            set { Value = value ? 1 : 0; }
        }
    }
}
