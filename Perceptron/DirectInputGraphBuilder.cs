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
    public class DirectInputGraphBuilder
    {
        List<InputNodeViewModel> InputNodes { get; set; } = new List<InputNodeViewModel>();
        List<EdgeViewModel> Edges { get; set; } = new List<EdgeViewModel>();
        List<WeightViewModel> Weights { get; set; } = new List<WeightViewModel>();
        SumNodeViewModel SumNode { get; set; }
        OutputNodeViewModel OutputNode { get; set; }
        PlusNodeViewModel PlusButton { get; set; }
        TrainingViewModel TrainingBox { get; set; }
        Network.NetworkExecutionService ExecutionService { get; set; }
        Network.Network Network { get; set; }
        public event Action OnRedrawGraph;
        public event Action OnRebuildGraph;
        public event Action OnResetDescription;
        int LastNodeIndex { get; set; } = 0;
        int LastTrainingIndex { get; set; } = 0;
        public int MaxInputNodes { get; set; } = 7;

        public double Width { get; set; } = 0;
        public double Height { get; set; } = 0;

        public DirectInputGraphBuilder(Network.NetworkExecutionService executionService, Network.Network network)
        {
            ExecutionService = executionService;
            Network = network;
        }

        public void RebuildGraph(ObservableCollection<PositionableViewModel> items)
        {
            RebuildInputNodes();
            RebuildEdges();
            RebuildWeights();
            RebuildPlusButton();
            RebuildSumNode();
            RebuildOutputNode();
            if (!items.Any())
            {
                RebuildTrainingBox();
            }
            RebuildGraphItems(items);
        }

        public void RedrawGraph()
        {
            RedrawInputNodes();
            RedrawSumNode();
            RedrawOutputNode();
            RedrawEdges();
            RedrawWeights();
            RedrawPlusButton();
            RedrawTrainingBox();
        }

        public void ResetProgress()
        {
            OnResetDescription.Invoke();
            ExecutionService.ResetProgress();
        }

        #region Rebuilding
        void RebuildInputNodes()
        {
            int count = Network.InputLayer.Size;
            InputNodes.ForEach(n =>
            {
                n.OnGetValue -= GetNodeInput;
                n.OnSetValue -= SetNodeInput;
                n.OnRemove -= RemoveInputNode;
                n.OnGetCrossButtonEnabled -= GetCrossButtonEnabled;
                n.Arrow -= InputNodeArrow;
            });
            InputNodes.Clear();
            for (int i = 0; i < count; i++)
                InputNodes.Add(new InputNodeViewModel(i));
            InputNodes.ForEach(n =>
            {
                n.OnGetValue += GetNodeInput;
                n.OnSetValue += SetNodeInput;
                n.OnRemove += RemoveInputNode;
                n.OnGetCrossButtonEnabled += GetCrossButtonEnabled;
                n.Arrow += InputNodeArrow;
            });
            NotifyInputNodes();
        }

        void RebuildEdges()
        {
            int count = InputNodes.Count;
            Edges = new List<EdgeViewModel>();
            for (int i = 0; i < count + 1; i++)
                Edges.Add(new EdgeViewModel());
        }

        void RebuildSumNode()
        {
            if (SumNode != null)
            {
                SumNode.GetSum -= GetSum;
                SumNode.OnGetBias -= GetBias;
                SumNode.OnSetBias -= SetBias;
                SumNode.Arrow -= SumNodeArrow;
            }
            SumNode = new SumNodeViewModel();
            SumNode.GetSum += GetSum;
            SumNode.OnGetBias += GetBias;
            SumNode.OnSetBias += SetBias;
            SumNode.Arrow += SumNodeArrow;
            NotifySumNode();
            NotifyBiasNode();
        }

        void RebuildWeights()
        {
            int count = InputNodes.Count;
            Weights.ForEach(n =>
            {
                n.OnGetValue -= GetWeightInput;
                n.OnSetValue -= SetWeightInput;
                n.Arrow -= WeightArrow;
            });
            Weights.Clear();
            for (int i = 0; i < count; i++)
                Weights.Add(new WeightViewModel(i));
            Weights.ForEach(n =>
            {
                n.OnGetValue += GetWeightInput;
                n.OnSetValue += SetWeightInput;
                n.Arrow += WeightArrow;
            });
            NotifyWeights();
        }

        void RebuildOutputNode()
        {
            if (OutputNode != null)
                OutputNode.GetOutput -= GetOutput;
            OutputNode = new OutputNodeViewModel();
            OutputNode.GetOutput += GetOutput;
        }

        void RebuildPlusButton()
        {
            if (PlusButton != null)
            {
                PlusButton.AddInputNode -= AddInputNode;
                PlusButton.GetIsPlusButtonEnabled -= GetIsPlusButtonEnabled;
            }
            PlusButton = new PlusNodeViewModel();
            PlusButton.AddInputNode += AddInputNode;
            PlusButton.GetIsPlusButtonEnabled += GetIsPlusButtonEnabled;
        }

        void RebuildTrainingBox()
        {
            if (TrainingBox != null)
            {
                TrainingBox.OnSetOutput -= SetDesiredOutput;
                TrainingBox.OnSetCoefficient -= SetTrainingCoefficient;
                TrainingBox.OnGetCoefficient -= GetTrainingCoefficient;
                TrainingBox.OnSetTraining -= SetTraining;
                TrainingBox.Arrow -= TrainingArrow;
            }
            TrainingBox = new TrainingViewModel();
            TrainingBox.OnSetOutput += SetDesiredOutput;
            TrainingBox.OnSetCoefficient += SetTrainingCoefficient;
            TrainingBox.OnGetCoefficient += GetTrainingCoefficient;
            TrainingBox.OnSetTraining += SetTraining;
            TrainingBox.Arrow += TrainingArrow;
            NotifyTrainingBox();
        }

        public TrainingViewModel GetTrainingBox()
        {
            TrainingBox.OnSetOutput -= SetDesiredOutput;
            TrainingBox.OnSetCoefficient -= SetTrainingCoefficient;
            TrainingBox.OnGetCoefficient -= GetTrainingCoefficient;
            TrainingBox.OnSetTraining -= SetTraining;
            return TrainingBox;
        }

        public void SetTrainingBox(TrainingViewModel trainingBox)
        {
            TrainingBox = trainingBox;
            if (TrainingBox == null)
                return;
            TrainingBox.OnSetOutput += SetDesiredOutput;
            TrainingBox.OnSetCoefficient += SetTrainingCoefficient;
            TrainingBox.OnGetCoefficient += GetTrainingCoefficient;
            TrainingBox.OnSetTraining += SetTraining;
            NotifyTrainingBox();
        }

        void RebuildGraphItems(ObservableCollection<PositionableViewModel> graphItems)
        {
            var it = graphItems.Where(i => i != TrainingBox).ToList();
            it.ForEach(i => graphItems.Remove(i));
            if (!graphItems.Any())
            {
                graphItems.Add(TrainingBox);
            }
            graphItems.Add(SumNode);
            graphItems.Add(OutputNode);
            Edges.ForEach(e => graphItems.Add(e));
            InputNodes.ForEach(n => graphItems.Add(n));
            Weights.ForEach(w => graphItems.Add(w));
            graphItems.Add(PlusButton);
        }
        #endregion

        #region Redrawing
        void RedrawInputNodes()
        {
            double left = GetColumnLeft(0);
            double top = Height / 2 - (InputNodes.Count - 1) * 60 / 2;
            for (int i = 0; i < InputNodes.Count; i++)
            {
                InputNodes[i].Left = left - 20;
                InputNodes[i].Top = top + i * 60 - 20;
            };
        }

        void RedrawEdges()
        {
            for (int i = 0; i < InputNodes.Count; i++)
            {
                Edges[i].Source = new System.Windows.Point(InputNodes[i].Left + 40, InputNodes[i].Top + 20);
                Edges[i].Sink = new System.Windows.Point(SumNode.Left, SumNode.Top + 75);
            };
            Edges[InputNodes.Count].Source = new System.Windows.Point(SumNode.Left + 100, SumNode.Top + 75);
            Edges[InputNodes.Count].Sink = new System.Windows.Point(OutputNode.Left, SumNode.Top + 75);
        }

        void RedrawSumNode()
        {
            double left = GetColumnLeft(2);
            SumNode.Left = left - 75;
            SumNode.Top = Height / 2 - 75;
        }

        void RedrawOutputNode()
        {
            double left = GetColumnLeft(3);
            OutputNode.Left = left - 20;
            OutputNode.Top = Height / 2 - 20;
        }

        void RedrawWeights()
        {
            double left = GetColumnLeft(1) - 80;
            for (int i = 0; i < Weights.Count; i++)
            {
                Weights[i].Left = left;
                Weights[i].Top = InputNodes[i].Top - 10;
            }
        }

        void RedrawPlusButton()
        {
            double left = InputNodes[0].Left;
            PlusButton.Left = left;
            PlusButton.Top = InputNodes.Last().Top + 60;
        }

        void RedrawTrainingBox()
        {
            double left = GetColumnLeft(4);
            TrainingBox.Left = left - 55;
            TrainingBox.Top = Height / 2 - 100;
        }

        double GetColumnLeft(double index)
        {
            return Width / 5 * (index + 0.5);
        }
        #endregion

        #region Getters
        float GetNodeInput(int index)
        {
            return Network.InputLayer[index];
        }

        float GetWeightInput(int index)
        {
            return Network.Weights[index, 0];
        }

        float? GetSum()
        {
            return ExecutionService.GetSum();
        }

        int? GetOutput()
        {
            return ExecutionService.GetOutput();
        }

        float GetBias()
        {
            return Network.Biases[0];
        }

        public float GetTrainingCoefficient()
        {
            return Network.LearningCoeficient;
        }

        public bool GetIsPlusButtonEnabled()
        {
            return Network.InputLayer.Size < MaxInputNodes;
        }

        public bool AreValuesValid()
        {
            bool inputs = InputNodes.All(i => i.IsNumeric);
            bool weights = Weights.All(w => w.IsNumeric);
            bool bias = SumNode.IsNumeric;
            bool training = !ExecutionService.Training || (TrainingBox.IsNumeric && TrainingBox.IsValidOutput);
            return inputs && weights && bias && training;
        }
        #endregion

        #region Setters
        void SetNodeInput(int index, float value)
        {
            Network.InputLayer[index] = value;
        }

        void SetWeightInput(int index, float value)
        {
            Network.Weights[index, 0] = value;
        }

        void SetBias(float value)
        {
            Network.Biases[0] = value;
        }

        void AddInputNode()
        {
            if (Network.InputLayer.Size == MaxInputNodes)
                return;
            ExecutionService.AddInputNode();
            OnRebuildGraph?.Invoke();
            if (Network.InputLayer.Size == MaxInputNodes)
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
            ExecutionService.Training = val;
        }

        void RemoveInputNode(int index)
        {
            if (Network.InputLayer.Size == 1)
                return;
            ExecutionService.RemoveInputNode(index);
            OnRebuildGraph?.Invoke();
            PlusButton.OnEnabledChanged();
            NotifyCrossButtons();
            ResetProgress();
        }

        bool GetCrossButtonEnabled()
        {
            return Network.InputLayer.Size > 1;
        }

        void InputNodeArrow(ArrowType type, int index)
        {
            LastNodeIndex = index;
            if (type == ArrowType.Down && index < InputNodes.Count - 1)
                InputNodes[index + 1].Focus();
            else if (type == ArrowType.Up && index > 0)
                InputNodes[index - 1].Focus();
            else if (type == ArrowType.Right)
                Weights[index].Focus();
        }

        void WeightArrow(ArrowType type, int index)
        {
            LastNodeIndex = index;
            if (type == ArrowType.Down && index < InputNodes.Count - 1)
                Weights[index + 1].Focus();
            else if (type == ArrowType.Up && index > 0)
                Weights[index - 1].Focus();
            else if (type == ArrowType.Left)
                InputNodes[index].Focus();
            else
                SumNode.Focus();
        }

        void SumNodeArrow(ArrowType type)
        {
            LastNodeIndex = Math.Min(LastNodeIndex, Weights.Count - 1);
            if (type == ArrowType.Left)
                Weights[LastNodeIndex].Focus();
            else if (type == ArrowType.Right && ExecutionService.Training)
                TrainingBox.Focus(LastTrainingIndex);
                return;
        }

        void TrainingArrow(ArrowType type, int index)
        {
            LastTrainingIndex = index;
            if (type == ArrowType.Left)
                SumNode.Focus();
            else if (type == ArrowType.Up)
                TrainingBox.Focus(0);
            else if (type == ArrowType.Down)
                TrainingBox.Focus(1);
        }
        #endregion

        #region Notifications
        public void NotifyAll()
        {
            NotifyOutput();
            NotifyInputNodes();
            NotifyWeights();
            NotifySumNode();
            NotifyBiasNode();
            NotifyTrainingBox();
        }

        public void NotifyInputNodes()
        {
            InputNodes.ForEach(n => n.OnValueChanged());
        }

        public void NotifyWeights()
        {
            Weights.ForEach(n => n.OnValueChanged());
        }

        public void NotifySumNode()
        {
            SumNode.ForceNotify("Sum");
        }

        public void NotifyBiasNode()
        {
            SumNode.OnValueChanged();
        }

        public void NotifyOutput()
        {
            OutputNode.ForceNotify("Output");
        }

        void NotifyCrossButtons()
        {
            InputNodes.ForEach(n => n.OnCrossButtonEnabledChanged());
        }

        void NotifyTrainingBox()
        {
            TrainingBox.OnValueChanged();
        }

        #endregion
    }
}
