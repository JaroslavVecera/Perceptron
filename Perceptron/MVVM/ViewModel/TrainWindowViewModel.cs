using Perceptron.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.MVVM.ViewModel
{
    class TrainWindowViewModel : ObservableObject
    {
        static TrainWindowViewModel _instance = null;
        int _percent = 0;
        public int Percent
        {
            get { return _percent; }
            set
            {
                _percent = value;
                Info = _percent + "% learned";
                Compl = 100 - _percent;
                Working = _percent < 100;
                OnPropertyChanged();
            }
        }
        int _compl = 100;
        public int Compl
        {
            get { return _compl; }
            set
            {
                _compl = value;
                OnPropertyChanged();
            }
        }
        string _info = "";
        public string Info
        {
            get { return _info; }
            set { _info = value; OnPropertyChanged(); }
        }
        bool _working = true;
        public bool Working
        {
            get { return _working; }
            set
            {
                _working = value;
                OnPropertyChanged();
            }
        }

        private TrainWindowViewModel()
        {

        }

        public static TrainWindowViewModel GetInstance()
        {
            if (_instance == null)
                _instance = new TrainWindowViewModel();
            return _instance;
        }
    }
}
