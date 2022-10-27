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

namespace Perceptron.MVVM.ViewModel
{
    class DirectInputViewModel : ObservableObject
    {
        public RelayCommand SetWidthCommand { get; set; }
        public RelayCommand GraphRedrawCommand { get; set; }
        public RelayCommand Step3Command { get; set; }
        public RelayCommand ClearCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand LoadCommand { get; set; }
        Network.Network Network { get; set; }
        NetworkExecutionService ExecutionService { get; set; }
        GraphBuilder Builder { get; set; }

        List<PositionableViewModel> _graphItems;
        public List<PositionableViewModel> GraphItems
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
            Network = new Network.Network(5, 1, (float)0.5);
            ExecutionService = new NetworkExecutionService(Network);
            Builder = new GraphBuilder(ExecutionService, Network);
            RebuildGraph();
            InitializeCommands();
        }

        void RebuildGraph()
        {
            _graphItems = Builder.RebuildGraph();
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
            Step3Command = new RelayCommand(o =>
            {
                return;
                ExecutionService.Step3();
                Builder.NotifyOutput();
                Builder.NotifySumNode();
            });
            ClearCommand = new RelayCommand(o =>
            {
            });
            LoadCommand = new RelayCommand(o =>
            {
                var network = NetworkSerializer.Load();
                if (network == null)
                    return;
                Network = network;
                ExecutionService = new NetworkExecutionService(Network);
                double width = Builder.Width;
                double height = Builder.Height;
                Builder = new GraphBuilder(ExecutionService, Network) { Height = height, Width = width };
                RebuildGraph();
            });
            SaveCommand = new RelayCommand(o =>
            {
                NetworkSerializer.Save(Network);
            });
        }
    }
}
