using Perceptron.MVVM;
using Perceptron.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron
{
    class ImageInputGraphBuilder
    {
        ImageInputBoxViewModel ImageInputBox { get; set; }
        BraceViewModel Brace { get; set; }
        List<InputNodeViewModel> BiasNodes { get; set; } = new List<InputNodeViewModel>();
        List<EdgeViewModel> Edges { get; set; } = new List<EdgeViewModel>();
        List<OutputNodeViewModel> OutputNodes { get; set; } = new List<OutputNodeViewModel>();
        TrainingViewModel TrainingBox { get; set; }
        PlusNodeViewModel PlusButton { get; set; }
        List<DesiredOutputViewModel> DesiredOutputNodes { get; set; } = new List<DesiredOutputViewModel>();
        Network.NetworkExecutionServiceImageInput ExecutionService { get; set; }
        public bool Exclusivity { get; set; }
        Network.Network Network { get; set; }
        public event Action OnRedrawGraph;
        public event Action OnRebuildGraph;
        public event Action OnResetDescription;
        public int MaxBiasNodes { get; set; } = 10;

        public double Width { get; set; } = 0;
        public double Height { get; set; } = 0;

        public ImageInputGraphBuilder(Network.NetworkExecutionServiceImageInput executionService, Network.Network network)
        {
            ExecutionService = executionService;
            Network = network;
        }

        public void NextImage()
        {
            ImageInputBox.NextImage();
        }

        public void ImageModified()
        {
            ImageInputBox.ImageModified();
        }

        public void RebuildGraph(ObservableCollection<PositionableViewModel> items)
        {
            RebuildStatic();
            RebuildBiasNodes();
            RebuildEdges();
            RebuildPlusButton();
            RebuildOutputNodes();
            RebuildDesiredOutputNodes();
            RebuildGraphItems(items);
        }
        public void RedrawGraph()
        {
            RedrawStatic();
            RedrawBiasNodes();
            RedrawOutputNodes();
            RedrawEdges();
            RedrawPlusButton();
            RedrawDesiredOutputNodes();
        }

        void RebuildStatic()
        {
            if (ImageInputBox == null)
                ImageInputBox = new ImageInputBoxViewModel();
            else
            {
                ImageInputBox.OnChangeInput -= ChangeInput;
                var image = ImageInputBox.Array;
                var testSet = ImageInputBox.TestSet;
                var path = ImageInputBox.Path;
                ImageInputBox = new ImageInputBoxViewModel(testSet, path, image);
            }
            ImageInputBox.OnChangeInput += ChangeInput;
            ImageInputBox.Notify();
            Brace = new BraceViewModel();
        }

        void RebuildBiasNodes()
        {
            int count = Network.Neurons;
            BiasNodes.ForEach(n =>
            {
                n.OnGetValue -= GetBias;
                n.OnSetValue -= SetBias;
                n.OnRemove -= RemoveBiasNode;
                n.OnGetCrossButtonEnabled -= GetCrossButtonEnabled;
                n.Arrow -= BiasNodeArrow;
            });
            BiasNodes.Clear();
            for (int i = 0; i < count; i++)
                BiasNodes.Add(new InputNodeViewModel(i));
            BiasNodes.ForEach(n =>
            {
                n.OnGetValue += GetBias;
                n.OnSetValue += SetBias;
                n.OnRemove += RemoveBiasNode;
                n.OnGetCrossButtonEnabled += GetCrossButtonEnabled;
                n.Arrow += BiasNodeArrow;
            });
            NotifyBiasNodes();
        }

        void RebuildEdges()
        {
            int count = BiasNodes.Count;
            Edges = new List<EdgeViewModel>();
            for (int i = 0; i < 2 * count + 1; i++)
                Edges.Add(new EdgeViewModel());
        }

        void RebuildOutputNodes()
        {
            OutputNodes.ForEach(n =>
            {
                n.GetOutput -= GetOutput;
                n.OnSetValue -= SetBias;
                n.OnGetValue -= GetBias;
            });
            OutputNodes.Clear();
            for (int i = 0; i < BiasNodes.Count; i++)
            {
                var o = new OutputNodeViewModel(i);
                o.GetOutput += GetOutput;
                o.OnSetValue += SetBias;
                o.OnGetValue += GetBias;
                OutputNodes.Add(o);
            }
        }

        void RebuildPlusButton()
        {
            if (PlusButton != null)
            {
                PlusButton.AddInputNode -= AddBiasNode;
                PlusButton.GetIsPlusButtonEnabled -= GetIsPlusButtonEnabled;
            }
            PlusButton = new PlusNodeViewModel();
            PlusButton.AddInputNode += AddBiasNode;
            PlusButton.GetIsPlusButtonEnabled += GetIsPlusButtonEnabled;
        }

        void RebuildGraphItems(ObservableCollection<PositionableViewModel> graphItems)
        {
            var it = graphItems.Where(i => i != TrainingBox).ToList();
            it.ForEach(i => graphItems.Remove(i));
            if (!graphItems.Any())
            {
               graphItems.Add(TrainingBox);
            }
            OutputNodes.ForEach(n => graphItems.Add(n));
            Edges.ForEach(e => graphItems.Add(e));
            BiasNodes.ForEach(n => graphItems.Add(n));
            graphItems.Add(PlusButton);
            graphItems.Add(ImageInputBox);
            graphItems.Add(Brace);
            DesiredOutputNodes.ForEach(n => graphItems.Add(n));
        }

        void RebuildDesiredOutputNodes()
        {
            DesiredOutputNodes.ForEach(n =>
            {
                n.OnSetValue -= SetDesiredOutput;
                n.OnGetValue -= GetDesiredOutput;
            });
            DesiredOutputNodes.Clear();
            for (int i = 0; i < BiasNodes.Count; i++)
            {
                var o = new DesiredOutputViewModel(i);
                o.OnSetValue += SetDesiredOutput;
                o.OnGetValue += GetDesiredOutput;
                o.Visibility = ExecutionService.Training ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                DesiredOutputNodes.Add(o);
            }
            NotifyDesiredOutputs();
        }

        void RedrawStatic()
        {
            ImageInputBox.Left = GetColumnLeft(0) - 100;
            ImageInputBox.Top = Height / 2 - 100;

            Brace.Left = GetColumnLeft(1) - 70;
            Brace.Top = Height / 2 - 80;
        }
        void RedrawBiasNodes()
        {
            double left = GetColumnLeft(2);
            double top = Height / 2 - (BiasNodes.Count - 1) * 60 / 2;
            for (int i = 0; i < BiasNodes.Count; i++)
            {
                BiasNodes[i].Left = left - 20;
                BiasNodes[i].Top = top + i * 60 - 20;
            };
        }

        void RedrawOutputNodes()
        {
            double left = GetColumnLeft(3);
            for (int i = 0; i < OutputNodes.Count; i++)
            {
                OutputNodes[i].Left = left - 20;
                OutputNodes[i].Top = BiasNodes[i].Top;
            }
        }

        void RedrawEdges()
        {
            for (int i = 0; i < BiasNodes.Count; i++)
            {
                Edges[i].Source = new System.Windows.Point(BiasNodes[i].Left + 40, BiasNodes[i].Top + 20);
                Edges[i].Sink = new System.Windows.Point(OutputNodes[i].Left, OutputNodes[i].Top + 20);
            }
            for (int i = 0; i < BiasNodes.Count; i++)
            {
                Edges[BiasNodes.Count + i].Source = new System.Windows.Point(GetColumnLeft(1) - 5, Height / 2);
                Edges[BiasNodes.Count + i].Sink = new System.Windows.Point(BiasNodes[i].Left, BiasNodes[i].Top + 20);
            }
        }

        void RedrawPlusButton()
        {
            double left = (GetColumnLeft(2) + GetColumnLeft(3)) / 2;
            PlusButton.Left = left;
            PlusButton.Top = BiasNodes.Last().Top + 60;
        }

        void RedrawDesiredOutputNodes()
        {
            double left = (GetColumnLeft(4) + GetColumnLeft(5)) / 2 - 15;
            for (int i = 0; i < OutputNodes.Count; i++)
            {
                DesiredOutputNodes[i].Left = left;
                DesiredOutputNodes[i].Top = OutputNodes[i].Top + 10;
            }
        }

        double GetColumnLeft(int i)
        {
            if (i == 0)
                return 150;
            else if (i == 1)
                return 325;
            else
                return (Width - 350) / 4 * (i - 2 + 0.5) + 350;
        }

        int? GetOutput(int index)
        {
            return ExecutionService.GetOutput(index);
        }

        float GetBias(int index)
        {
            return Network.Biases[index];
        }

        public float GetTrainingCoefficient()
        {
            return Network.LearningCoeficient;
        }

        public bool GetIsPlusButtonEnabled()
        {
            return Network.Neurons < MaxBiasNodes;
        }

        public bool AreValuesValid()
        {
            bool inputs = BiasNodes.All(i => i.IsNumeric);
            return inputs;
        }
        void SetBias(int index, float value)
        {
            Network.Biases[index] = value;
        }

        void AddBiasNode()
        {
            if (Network.Neurons == MaxBiasNodes)
                return;
            ExecutionService.AddBiasNode();
            OnRebuildGraph?.Invoke();
            if (Network.Neurons == MaxBiasNodes)
                PlusButton.OnEnabledChanged();
            NotifyCrossButtons();
            ResetProgress();
        }


        void RemoveBiasNode(int index)
        {
            if (Network.Neurons == 1)
                return;
            ExecutionService.RemoveBiasNode(index);
            OnRebuildGraph?.Invoke();
            PlusButton.OnEnabledChanged();
            NotifyCrossButtons();
            ResetProgress();
        }

        public void SetDesiredOutput(int index, float value)
        {
            if (value == 1 && Exclusivity)
                ExecutionService.DesiredOutput = DesiredOutputNodes.Select(n => 0).ToList();
            ExecutionService.DesiredOutput[index] = (int)value;
            NotifyDesiredOutputs();
        }

        public float GetDesiredOutput(int index)
        {
            return (int)ExecutionService.DesiredOutput[index];
        }

        public void SetTrainingCoefficient(float coeff)
        {
            Network.LearningCoeficient = coeff;
        }

        public void SetTraining(bool val)
        {
            ExecutionService.Training = val;
            DesiredOutputNodes.ForEach(n => n.Visibility = val ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed);
        }

        public void SetDesiredOutput(int index, int value)
        {
            ExecutionService.DesiredOutput[index] = value;
        }

        private void BiasNodeArrow(ArrowType arg1, int arg2)
        {

        }

        bool GetCrossButtonEnabled()
        {
            return Network.Neurons > 1;
        }

        public void ResetProgress()
        {
            ExecutionService.ResetProgress();
        }

        public void NotifyAll()
        {
            NotifyOutputs();
            NotifyBiasNodes();
            NotifyDesiredOutputs();
            NotifyCrossButtons();
        }

        public void NotifyBiasNodes()
        {
            for (int i = 0; i < BiasNodes.Count; i++)
                BiasNodes[i].OnValueChanged();
        }

        public void NotifyOutputs()
        {
            OutputNodes.ForEach(n => n.ForceNotify("Output"));
        }

        void NotifyCrossButtons()
        {
            BiasNodes.ForEach(n => n.OnCrossButtonEnabledChanged());
        }

        void NotifyDesiredOutputs()
        {
            DesiredOutputNodes.ForEach(n => n.OnValueChanged());
        }

        void ChangeInput(float[] input)
        {
            Network.InputLayer.InputArray = input;
        }
    }
}
