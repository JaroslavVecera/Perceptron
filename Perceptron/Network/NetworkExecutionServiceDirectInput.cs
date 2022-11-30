using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.Network
{
    public class NetworkExecutionServiceDirectInput
    {
        public Network Network { get; }
        float? Sum { get; set; }
        int? Output { get; set; }
        public int DesiredOutput { get; set; } = 0;
        public bool Training { get; set; } = true;
        ExecutionState State { get; set; } = new ExecutionState() { Group = ExecutionStateGroup.Normal };

        public NetworkExecutionServiceDirectInput(Network network)
        {
            Network = network;
        }

        public float? GetSum()
        {
            return Sum;
        }

        public void CountSum()
        {
            Sum = Network.GetWeightedSum(0);
        }

        public int? GetOutput()
        {
            return Output;
        }

        public void CountOutput()
        {
            Network.CalculateOutput(0);
            Output = Network.Output[0];
        }

        public void ResetProgress()
        {
            State.Group = ExecutionStateGroup.Normal;
            State.Index = 0;
        }

        public void AddInputNode()
        {
            float[] oldInput = Network.InputLayer.Output;
            float[,] oldWeights = Network.Weights;

            oldInput = oldInput.Append(0).ToArray();

            Network.InputLayer.InputArray = oldInput;
            Network.Weights = new float[Network.InputLayer.Size, Network.Neurons];
            for (int i = 0; i < oldWeights.GetLength(0); i++)
                for (int j = 0; j < oldWeights.GetLength(1); j++)
                    Network.Weights[i, j] = oldWeights[i, j];
        }

        public void RemoveInputNode(int index)
        {
            float[] oldInput = Network.InputLayer.Output;
            float[,] oldWeights = Network.Weights;
            var oi = oldInput.ToList();
            oi.RemoveAt(index);
            oldInput = oi.ToArray();

            Network.InputLayer.InputArray = oldInput;

            Network.Weights = new float[Network.InputLayer.Size, Network.Neurons];
            for (int i = 0; i < Network.Weights.GetLength(0); i++)
                for (int j = 0; j < Network.Weights.GetLength(1); j++)
                {
                    int idec = i >= index ? 1 : 0;
                    Network.Weights[i, j] = oldWeights[i + idec, j];
                }
        }

        public string Step1()
        {
            string res = "";
            Network.Run();
            res = DoStep();
            Network.Stop();
            return res;
        }

        public string Step2()
        {
            string res = "";
            Network.Run();
            if (State.Group == ExecutionStateGroup.Normal || State.Group == ExecutionStateGroup.UpdateBias)
                res = Step1();

            ExecutionStateGroup curr = State.Group;
            while (State.Group == curr)
                res = Step1();
            Network.Stop();
            return res;
        }

        public string Step3()
        {
            string res = "";
            Network.Run();
            res = Step1();
            while (State.Group != ExecutionStateGroup.Normal)
                res = Step1();
            Network.Stop();
            return res;
        }

        string DoStep()
        {
            if (State.Group == ExecutionStateGroup.Normal)
            {
                Sum = null;
                Output = null;
                State.Group = ExecutionStateGroup.SumMember;
                State.Index = 0;
                return DescriptionGenerator.PartialSum(Network, State.Index);
            }
            else if (State.Group == ExecutionStateGroup.SumMember)
            {
                State.Index++;
                if (State.Index == Network.InputLayer.Size)
                {
                    State.Group = ExecutionStateGroup.Sum;
                    State.Index = 0;
                    CountSum();
                    return DescriptionGenerator.Sum(Network);
                }
                else
                {
                    return DescriptionGenerator.PartialSum(Network, State.Index);
                }
            }
            else if (State.Group == ExecutionStateGroup.Sum)
            {
                State.Group = ExecutionStateGroup.Activation;
                State.Index = 0;
                CountOutput();
                return DescriptionGenerator.Output(Network, Sum.Value);
            }
            else if (State.Group == ExecutionStateGroup.Activation)
            {
                if (Training)
                {
                    State.Group = ExecutionStateGroup.UpdateBias;
                    Network.Biases[0] += -Network.LearningCoeficient * (DesiredOutput - (float)Output);
                    return DescriptionGenerator.UpdateBias(Network, DesiredOutput, (int)Output);
                }
                else
                {
                    Sum = null;
                    State.Group = ExecutionStateGroup.Normal;
                }
                return "";
            }
            else if (State.Group == ExecutionStateGroup.UpdateBias)
            {
                if (!Training)
                {
                    State.Group = ExecutionStateGroup.Normal;
                    return "";
                }
                State.Group = ExecutionStateGroup.UpdateWeight;
                State.Index = 0;
                Network.Weights[State.Index, 0] += Network.LearningCoeficient * Network.InputLayer[State.Index] * (DesiredOutput - (float)Output);
                return DescriptionGenerator.UpdateWeight(Network, DesiredOutput, (int)Output, State.Index);
            }
            else if (State.Group == ExecutionStateGroup.UpdateWeight)
            {
                if (!Training)
                {
                    State.Group = ExecutionStateGroup.Normal;
                    return "";
                }
                State.Index++;
                if (State.Index < Network.InputLayer.Size)
                {
                    Network.Weights[State.Index, 0] += Network.LearningCoeficient * Network.InputLayer[State.Index] * (DesiredOutput - (float)Output);
                    return DescriptionGenerator.UpdateWeight(Network, DesiredOutput, (int)Output, State.Index);
                }
                else
                {
                    State.Group = ExecutionStateGroup.Normal;
                    Sum = null;
                }
            }
            return "";
        }
    }
}
