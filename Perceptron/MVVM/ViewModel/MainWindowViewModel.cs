using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perceptron.Core;
using Perceptron.Network;

namespace Perceptron.MVVM.ViewModel
{
    class MainWindowViewModel : ObservableObject
    {
        ObservableObject _currentView;

        DirectInputViewModel DirectInputView { get; set; }
        ImageInputViewModel ImageInputView { get; set; }

        public ObservableObject CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }
        public RelayCommand DirectInputViewCommand { get; set; }
        public RelayCommand ImageInputViewCommand { get; set; }


        public MainWindowViewModel()
        {
            LoadMnist();
            InitializeViews();
            InitializeCommands();
            CurrentView = DirectInputView;
        }

        void LoadMnist()
        {
            MnistLoader.Load(10000, 100);
        }

        void InitializeViews()
        {
            DirectInputView = new DirectInputViewModel();
            ImageInputView = new ImageInputViewModel();
        }

        void InitializeCommands()
        {
            DirectInputViewCommand = new RelayCommand((x) =>
            {
                CurrentView = DirectInputView;
            });
            ImageInputViewCommand = new RelayCommand((x) =>
            {
                CurrentView = ImageInputView;
            });
        }
    }
}
