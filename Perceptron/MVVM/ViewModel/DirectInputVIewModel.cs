using Perceptron.Core;
using Perceptron.Network;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.MVVM.ViewModel
{
    class DirectInputViewModel : ObservableObject
    {
        public RelayCommand SetWidthCommand { get; set; }
        public RelayCommand GraphRedrawCommand { get; set; }
        public RelayCommand Step3Command { get; set; }
        Network.Network Network { get; }
        NetworkExecutionService ExecutionService { get; }
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
            Network = new Network.Network(1, 1, (float)0.5);
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
                ExecutionService.Step3();
                Builder.NotifyOutput();
                Builder.NotifySumNode();
            });
        }
    }
}
