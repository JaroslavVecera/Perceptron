using Microsoft.Win32;
using Perceptron.Core;
using Perceptron.Network;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Perceptron.MVVM.ViewModel
{
    class ImageInputViewModel : ObservableObject
    {
        public RelayCommand SetWidthCommand { get; set; }
        public RelayCommand GraphRedrawCommand { get; set; }
        public RelayCommand Step1Command { get; set; }
        public RelayCommand Step2Command { get; set; }
        public RelayCommand Step3Command { get; set; }

        public RelayCommand ClearCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand LoadCommand { get; set; }
        public RelayCommand RandomCommand { get; set; }
        public RelayCommand NextCommand { get; set; }
        Network.Network Network { get; set; }
        NetworkExecutionServiceImageInput ExecutionService { get; set; }
        public LongTrainingViewModel TrainingBox { get; set; } = new LongTrainingViewModel();
        ImageInputGraphBuilder Builder { get; set; }
        string Description { get; set; }

        ObservableCollection<PositionableViewModel> _graphItems = new ObservableCollection<PositionableViewModel>();
        public ObservableCollection<PositionableViewModel> GraphItems
        {
            get { return _graphItems; }
            set
            {
                _graphItems = value;
                OnPropertyChanged();
            }
        }
        public bool Exclusivity { set { Builder.Exclusivity = value; } }

        public bool Training
        {
            set 
            { 
                Builder.SetTraining(value);
                TrainingBox.SetVisibility(value);
                InputChanged();
            } 
        }

        public ImageInputViewModel()
        {
            Network = new ConsoleTest().Run(5);
            ExecutionService = new NetworkExecutionServiceImageInput(Network);
            CreateBuilder(false);
            ExecutionService.DesiredOutput = Network.Biases.Select(b => 0).ToList();
            RebuildGraph();
            InitializeCommands();
            TrainingBox.OnGetCoefficient += Builder.GetTrainingCoefficient;
            TrainingBox.OnSetCoefficient += Builder.SetTrainingCoefficient;
            TrainingBox.OnValueChanged();
            MnistLikeImageReader.OnModifyImage += ModifyImage;
            TrainingBox.OnInputChanged += InputChanged;
        }

        void ModifyImage()
        {
            Builder.ImageModified();
            ExecutionService.ResetProgress();
        }

        void CreateBuilder(bool t)
        {
            TrainingViewModel training = null;
            if (Builder != null)
            {
                Builder.OnRebuildGraph -= RebuildGraph;
                Builder.OnRedrawGraph -= RedrawGraph;
                Builder.OnInputChanged -= InputChanged;
                Builder.OnImageInputChanged += ImageInputChanged;
                //training = Builder.GetTrainingBox();
            }
            Builder = new ImageInputGraphBuilder(ExecutionService, Network);
            Builder.OnInputChanged += InputChanged;
            Builder.OnImageInputChanged += ImageInputChanged;
            //Builder.SetTrainingBox(training);
            Builder.OnRebuildGraph += RebuildGraph;
            Builder.OnRedrawGraph += RedrawGraph;
            Builder.SetTraining(t);
        }

        void RebuildGraph()
        {
            Builder.RebuildGraph(_graphItems);
            RedrawGraph();
        }

        void RedrawGraph()
        {
            Builder.RedrawGraph();
            OnPropertyChanged("GraphItems");
        }

        void InitializeCommands()
        {
            SetWidthCommand = new RelayCommand(o =>
            {
                if (o is not double)
                    return;
                Builder.Width = (double)o;
            });
            GraphRedrawCommand = new RelayCommand(o =>
            {
                if (o is not double)
                    return;
                Builder.Height = (double)o;
                RedrawGraph();
            });
            Step1Command = new RelayCommand(o =>
            {
                EnforceValidData(ExecutionService.Step1);
            },
            o =>
            {
                return AreValuesValid();
            });
            Step2Command = new RelayCommand(o =>
            {
                EnforceValidData(ExecutionService.Step2);
            },
            o =>
            {
                return AreValuesValid();
            });
            Step3Command = new RelayCommand(o =>
            {
                //EnforceValidData(ExecutionService.Step3);
            },
            o =>
            {
                return AreValuesValid();
            });
            ClearCommand = new RelayCommand(o =>
            {
                Network.Clear();
                Builder.NotifyBiasNodes();
                Builder.ResetProgress();
            });
            LoadCommand = new RelayCommand(o =>
            {
                var network = NetworkSerializer.Load();
                if (network == null)
                    return;
                if (network.InputLayer.Size != 28 * 28)
                {
                    MessageBox.Show("This network is not suitable for MNIST input.", "Data error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                Network = network;
                //bool oldTraining = ExecutionService.Training;
                bool t = ExecutionService.Training;
                ExecutionService = new NetworkExecutionServiceImageInput(Network);
                double width = Builder.Width;
                double height = Builder.Height;
                TrainingViewModel tb = null;
                CreateBuilder(t);
                Builder.Width = width;
                Builder.Height = height;
                Builder.ResetProgress();
                ExecutionService.DesiredOutput = Network.Biases.Select(b => 0).ToList();
                //ExecutionService.Training = oldTraining;
                RebuildGraph();
                //ExecutionService.Training = oldTraining;
            });
            SaveCommand = new RelayCommand(o =>
            {
                NetworkSerializer.Save(Network);
            });
            RandomCommand = new RelayCommand(o =>
            {
                Network.InitializeRandom();
                Builder.NotifyBiasNodes();
                Builder.ResetProgress();
            });
            NextCommand = new RelayCommand(o =>
            {
                Builder.NextImage();
                Builder.ResetProgress();
            },
            o =>
            {
                return Builder.Mnist;
            });
        }

        bool AreValuesValid()
        {
            return Builder.AreValuesValid() && (TrainingBox.IsNumeric || !ExecutionService.Training);
        }

        void InputChanged()
        {
            Step1Command?.RaiseCanExecuteChanged();
            Step2Command?.RaiseCanExecuteChanged();
            Step3Command?.RaiseCanExecuteChanged();
        }

        void ImageInputChanged()
        {
            NextCommand?.RaiseCanExecuteChanged();
        }

        void EnforceValidData(Func<string> action)
        {
            if (Builder.AreValuesValid() && (TrainingBox.IsNumeric || !ExecutionService.Training))
            {
                Description = action.Invoke();
                Builder.NotifyAll();
            }
            else
                ValuesErrorMessage();
        }

        void ResetDescription()
        {
            //Description = "";
        }

        void ValuesErrorMessage()
        {
            MessageBox.Show("Some values are invalid.", "Value error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
