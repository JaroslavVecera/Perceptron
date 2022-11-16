using Microsoft.Win32;
using Perceptron.Core;
using Perceptron.Network;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Perceptron.MVVM.ViewModel
{
    class ImageInputViewModel : ObservableObject
    {
        public RelayCommand SetWidthCommand { get; set; }
        public RelayCommand GraphRedrawCommand { get; set; }
        Network.Network Network { get; set; }
        NetworkExecutionServiceImageInput ExecutionService { get; set; }
        ImageInputGraphBuilder Builder { get; set; }

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
        }
    }
}
