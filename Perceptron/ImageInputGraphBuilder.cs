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
        PlusNodeViewModel PlusButton { get; set; }
        Network.NetworkExecutionServiceImageInput ExecutionService { get; set; }
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

        public void RebuildGraph(ObservableCollection<PositionableViewModel> items)
        {
            RebuildStatic();
            RebuildBiasNodes();
            RebuildEdges();
            RebuildPlusButton();
            RebuildOutputNodes();
            /*
            if (!items.Any())
            {
                RebuildTrainingBox();
            }*/
            RebuildGraphItems(items);
        }
        public void RedrawGraph()
        {
            RedrawStatic();
            RedrawBiasNodes();
            RedrawOutputNodes();
            RedrawEdges();
            RedrawPlusButton();
            //RedrawTrainingBox();
        }

        void RebuildStatic()
        {
            if (ImageInputBox == null)
                ImageInputBox = new ImageInputBoxViewModel();
            else
            {
                var image = ImageInputBox.Image;
                var testSet = ImageInputBox.TestSet;
                ImageInputBox = new ImageInputBoxViewModel(testSet, image);
            }
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
            for (int i = 0; i < count + 1; i++)
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
            //var it = graphItems.Where(i => i != TrainingBox).ToList();
            var it = graphItems.ToList();
            it.ForEach(i => graphItems.Remove(i));
            if (!graphItems.Any())
            {
               // graphItems.Add(TrainingBox);
            }
            OutputNodes.ForEach(n => graphItems.Add(n));
            Edges.ForEach(e => graphItems.Add(e));
            BiasNodes.ForEach(n => graphItems.Add(n));
            graphItems.Add(PlusButton);
            graphItems.Add(ImageInputBox);
            graphItems.Add(Brace);
        }

        void RedrawStatic()
        {
            ImageInputBox.Left = GetColumnLeft(0) - 100;
            ImageInputBox.Top = Height / 2 - 100;

            Brace.Left = GetColumnLeft(1) - 25;
            Brace.Top = Height / 2 - 50;
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
            };
        }

        void RedrawPlusButton()
        {
            double left = (GetColumnLeft(2) + GetColumnLeft(3)) / 2;
            PlusButton.Left = left;
            PlusButton.Top = BiasNodes.Last().Top + 60;
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
            //bool training = !ExecutionService.Training || (TrainingBox.IsNumeric && TrainingBox.IsValidOutput);
            // return inputs && training;
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

        public void SetDesiredOutput(int output)
        {
            ExecutionService.DesiredOutput = output;
        }

        public void SetTrainingCoefficient(float coeff)
        {
            Network.LearningCoeficient = coeff;
        }

        public void SetTraining(bool val)
        {
            //ExecutionService.Training = val;
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

        }

        public void NotifyAll()
        {
            NotifyOutputs();
            NotifyBiasNodes();
            NotifyTrainingBox();
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

        void NotifyTrainingBox()
        {
            //TrainingBox.OnValueChanged();
        }
    }
}
