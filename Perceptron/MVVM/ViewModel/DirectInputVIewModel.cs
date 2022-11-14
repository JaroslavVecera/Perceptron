using Perceptron.Core;
using Perceptron.Network;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Perceptron.MVVM.ViewModel
{
    class DirectInputViewModel : ObservableObject
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
        Network.Network Network { get; set; }
        NetworkExecutionService ExecutionService { get; set; }
        DirectInputGraphBuilder Builder { get; set; }
        string _description = "";
        public string Description { get { return _description; } set { _description = value; OnPropertyChanged(); } }

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

        public DirectInputViewModel()
        {
            Network = new Network.Network(2, 1, (float)0.5);
            ExecutionService = new NetworkExecutionService(Network);
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
                Builder.OnResetDescription -= ResetDescription;
                training = Builder.GetTrainingBox();
            }
            Builder = new DirectInputGraphBuilder(ExecutionService, Network);
            Builder.SetTrainingBox(training);
            Builder.OnRebuildGraph += RebuildGraph;
            Builder.OnRedrawGraph += RedrawGraph;
            Builder.OnResetDescription += ResetDescription;
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
            Step2Command = new RelayCommand(o =>
            {
                EnforceValidData(ExecutionService.Step2);
            });
            Step3Command = new RelayCommand(o =>
            {
                EnforceValidData(ExecutionService.Step3);
            });
            ClearCommand = new RelayCommand(o =>
            {
                Network.Clear();
                Builder.NotifyBiasNode();
                Builder.NotifyWeights();
                Builder.ResetProgress();
            });
            LoadCommand = new RelayCommand(o =>
            {
                var network = NetworkSerializer.Load();
                if (network == null)
                    return;
                Network = network;
                bool oldTraining = ExecutionService.Training;
                ExecutionService = new NetworkExecutionService(Network);
                double width = Builder.Width;
                double height = Builder.Height;
                TrainingViewModel tb = null;
                CreateBuilder();
                Builder.Width = width;
                Builder.Height = height;
                Builder.ResetProgress();
                ExecutionService.Training = oldTraining;
                RebuildGraph();
                ExecutionService.Training = oldTraining;
            });
            SaveCommand = new RelayCommand(o =>
            {
                NetworkSerializer.Save(Network);
            });
            RandomCommand = new RelayCommand(o =>
            {
                Network.InitializeRandom();
                Builder.NotifyBiasNode();
                Builder.NotifyWeights();
                Builder.ResetProgress();
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
            Description = "";
        }

        void ValuesErrorMessage()
        {
            MessageBox.Show("Some values are invalid.", "Value error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
