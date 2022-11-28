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
        public bool Mnist { get; set; }

        TestSet TestSet { get; set; } = new TestSet();
        int TestIndex { get; set; }
        string _imagePath = "";

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
            Network = new Network.Network(28 * 28, 5, 0.25f);
            ExecutionService = new NetworkExecutionServiceImageInput(Network);
            CreateBuilder(false);
            ExecutionService.DesiredOutput = Network.Biases.Select(b => 0).ToList();
            RebuildGraph();
            SetData(new float[28 * 28].Select(f => 255f).ToArray());
            InitializeCommands();
            TrainingBox.OnGetCoefficient += Builder.GetTrainingCoefficient;
            TrainingBox.OnSetCoefficient += Builder.SetTrainingCoefficient;
            TrainingBox.OnValueChanged();
            MnistLikeImageReader.OnModifyImage += ModifyImage;
            TrainingBox.OnInputChanged += InputChanged;
        }

        void ModifyImage()
        {
            float[] arr = MnistLikeImageReader.LoadImage(_imagePath);
            if (arr != null)
                SetData(arr);
            ExecutionService.ResetProgress();
        }

        void LoadPicture()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".png";
            openFileDialog.Filter = "Images (.png)|*.png";
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            if (openFileDialog.ShowDialog() == true)
            {
                float[] arr = MnistLikeImageReader.LoadImage(openFileDialog.FileName);
                if (arr != null)
                {
                    _imagePath = openFileDialog.FileName;
                    SetData(arr);
                }
            }
        }

        void CreateBuilder(bool t)
        {
            TrainingViewModel training = null;
            if (Builder != null)
            {
                Builder.OnRebuildGraph -= RebuildGraph;
                Builder.OnRedrawGraph -= RedrawGraph;
                Builder.OnInputChanged -= InputChanged;
                Builder.OnImageInputChanged -= ImageInputChanged;
                //training = Builder.GetTrainingBox();
            }
            Builder = new ImageInputGraphBuilder(ExecutionService, Network);
            Builder.OnImageInputChanged += ImageInputChanged;
            //Builder.SetTrainingBox(training);
            Builder.OnRebuildGraph += RebuildGraph;
            Builder.OnInputChanged += InputChanged;
            Builder.OnRedrawGraph += RedrawGraph;
            Builder.SetTraining(t);
        }

        void RebuildGraph()
        {
            Builder.RebuildGraph(_graphItems);
            RedrawGraph();
            LoadMnist();
        }

        void LoadMnist()
        {
            TestSet = MnistLoader.TakeTrainSet(Network.Neurons, 10000);
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
                float[] input = Network.InputLayer.Output;
                if (Builder.AreValuesValid() && (TrainingBox.IsNumeric || !ExecutionService.Training))
                {
                    Description = ExecutionService.Step3(TestSet);
                    Builder.NotifyAll();
                }
                else
                    ValuesErrorMessage();
                Network.InputLayer.InputArray = input;
            },
            o =>
            {
                return Mnist && AreValuesValid();
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
                float[] oldInput = Network.InputLayer.Output;
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
                SetData(oldInput);
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
                if (TestIndex >= TestSet.Size)
                    TestIndex = 0;
                TestIndex++;
                SetData(TestSet.Tests[TestIndex].input, TestSet.Tests[TestIndex].label);
                Builder.ResetProgress();
            },
            o =>
            {
                return Mnist;
            });
        }

        void SetData(float[] input, int label)
        {
            SetData(input);
            ExecutionService.DesiredOutput = MnistOutputConvertor.EncodePositional(label, Network.Neurons).ToList();
            Builder.NotifyAll();
        }

        void SetData(float[] input)
        {
            Network.InputLayer.InputArray = input;
            Builder.SetImage(input);
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

        void ImageInputChanged(bool mnist)
        {
            if (mnist)
                SetData(TestSet.Tests[TestIndex].input, TestSet.Tests[TestIndex].label);
            else
                LoadPicture();
            Mnist = mnist;
            NextCommand?.RaiseCanExecuteChanged();
            Step3Command?.RaiseCanExecuteChanged();
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
