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


        public ImageInputViewModel()
        {
            Network = new Network.Network(28 * 28, 5, (float)0.5);
            ExecutionService = new NetworkExecutionServiceImageInput(Network);
            CreateBuilder();
            RebuildGraph();
            InitializeCommands();
        }

        void CreateBuilder()
        {
            TrainingViewModel training = null;
            if (Builder != null)
            {
                Builder.OnRebuildGraph -= RebuildGraph;
                Builder.OnRedrawGraph -= RedrawGraph;
                //training = Builder.GetTrainingBox();
            }
            Builder = new ImageInputGraphBuilder(ExecutionService, Network);
            //Builder.SetTrainingBox(training);
            Builder.OnRebuildGraph += RebuildGraph;
            Builder.OnRedrawGraph += RedrawGraph;
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
                ExecutionService = new NetworkExecutionServiceImageInput(Network);
                double width = Builder.Width;
                double height = Builder.Height;
                TrainingViewModel tb = null;
                CreateBuilder();
                Builder.Width = width;
                Builder.Height = height;
                Builder.ResetProgress();
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
            });
        }

        void EnforceValidData(Func<string> action)
        {
            if (Builder.AreValuesValid())
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
