using Perceptron.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron
{
    public class GraphBuilder
    {
        List<InputViewModel> InputNodes { get; set; } = new List<InputViewModel>();
        List<EdgeViewModel> Edges { get; set; } = new List<EdgeViewModel>();
        List<WeightViewModel> Weights { get; set; } = new List<WeightViewModel>();
        SumNodeViewModel SumNode { get; set; }
        OutputNodeViewModel OutputNode { get; set; }
        PlusNodeViewModel PlusButton { get; set; }
        Network.NetworkExecutionService ExecutionService { get; set; }
        Network.Network Network { get; set; }

        public double Width { get; set; } = 0;
        public double Height { get; set; } = 0;

        public GraphBuilder(Network.NetworkExecutionService executionService, Network.Network network)
        {
            ExecutionService = executionService;
            Network = network;
        }

        public List<PositionableViewModel> RebuildGraph()
        {
            RebuildInputNodes();
            RebuildEdges();
            RebuildSumNode();
            RebuildWeights();
            RebuildPlusButton();
            RebuildOutputNode();
            return RebuildGraphItems();
        }

        public void RedrawGraph()
        {
            RedrawInputNodes();
            RedrawSumNode();
            RedrawOutputNode();
            RedrawEdges();
            RedrawWeights();
            RedrawPlusButton();
        }



        void RebuildInputNodes()
        {
            int count = Network.InputLayer.Size;
            InputNodes.ForEach(n =>
            {
                n.OnGetValue -= GetNodeInput;
                n.OnSetValue -= SetNodeInput;
            });
            InputNodes.Clear();
            for (int i = 0; i < count; i++)
                InputNodes.Add(new InputNodeViewModel(i));
            InputNodes.ForEach(n =>
            {
                n.OnGetValue += GetNodeInput;
                n.OnSetValue += SetNodeInput;
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
            }
            SumNode = new SumNodeViewModel();
            SumNode.GetSum += GetSum;
            SumNode.OnGetBias += GetBias;
            SumNode.OnSetBias += SetBias;
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
            });
            Weights.Clear();
            for (int i = 0; i < count; i++)
                Weights.Add(new WeightViewModel(i));
            Weights.ForEach(n =>
            {
                n.OnGetValue += GetWeightInput;
                n.OnSetValue += SetWeightInput;
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
                PlusButton.AddInputNode -= AddInputNode;
            PlusButton = new PlusNodeViewModel();
            PlusButton.AddInputNode += AddInputNode;
        }

        List<PositionableViewModel> RebuildGraphItems()
        {
            List<PositionableViewModel> graphItems = new List<PositionableViewModel>();
            graphItems = new List<PositionableViewModel>();
            graphItems.Add(SumNode);
            Edges.ForEach(e => graphItems.Add(e));
            InputNodes.ForEach(n => graphItems.Add(n));
            graphItems.Add(OutputNode);
            Weights.ForEach(w => graphItems.Add(w));
            graphItems.Add(PlusButton);
            return graphItems;
        }

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
            double left = GetColumnLeft(1) - 100;
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

        double GetColumnLeft(double index)
        {
            return Width / 5 * (index + 0.5);
        }

        void SetNodeInput(int index, float value)
        {
            Network.InputLayer[index] = value;
        }

        float GetNodeInput(int index)
        {
            return Network.InputLayer[index];
        }

        void SetWeightInput(int index, float value)
        {
            Network.Weights[index, 0] = value;
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

        void SetBias(float value)
        {
            Network.Biases[0] = value;
        }

        void AddInputNode()
        {
            // bacha na to, že při změně je třeba resetovat progres v executionService!!!
            throw new NotImplementedException();
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
    }
}
