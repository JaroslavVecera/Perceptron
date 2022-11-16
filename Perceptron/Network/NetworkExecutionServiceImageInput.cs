using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.Network
{
    class NetworkExecutionServiceImageInput
    {
        public int DesiredOutput { set; get; }
        public List<int?> Outputs { get; set; } = new List<int?>();
        ExecutionState State { get; set; } = new ExecutionState() { Group = ExecutionStateGroup.Normal };
        Network Network { get; set; }
        public NetworkExecutionServiceImageInput(Network network)
        {
            Network = network;
            Outputs = network.Biases.Select(b => (int?)null).ToList();
        }

        public int? GetOutput(int index)
        {
            return Outputs[index];
        }

        public void AddBiasNode()
        {
            float[] oldBiases = Network.Biases;
            float[,] oldWeights = Network.Weights;

            oldBiases = oldBiases.Append(0).ToArray();

            Network.Neurons++;
            Network.Biases = oldBiases;
            Network.Weights = new float[Network.InputLayer.Size, Network.Neurons];
            for (int i = 0; i < oldWeights.GetLength(0); i++)
                for (int j = 0; j < oldWeights.GetLength(1); j++)
                    Network.Weights[i, j] = oldWeights[i, j];
            Outputs.Add(null);
        }

        public void RemoveBiasNode(int index)
        {
            float[] oldBiases = Network.Biases;
            float[,] oldWeights = Network.Weights;
            var ob = oldBiases.ToList();
            ob.RemoveAt(index);
            oldBiases = ob.ToArray();

            Network.Biases = oldBiases;
            Network.Neurons--;

            Network.Weights = new float[Network.InputLayer.Size, Network.Neurons];
            for (int i = 0; i < Network.Weights.GetLength(0); i++)
                for (int j = 0; j < Network.Weights.GetLength(1); j++)
                {
                    int idec = j >= index ? 1 : 0;
                    Network.Weights[i, j] = oldWeights[i, j + idec];
                }
            Outputs.RemoveAt(index);
        }

        public string Step1()
        {
            string res = "";
            Network.Run();
            res = DoStep();
            Network.Stop();
            return res;
        }

        string DoStep()
        {
            if (State.Group == ExecutionStateGroup.Normal)
            {
                Network.CalculateOutput();
                Outputs = Network.Output.Select(o => (int?)o).ToList();
            }
            return null;
            /*else if (State.Group == ExecutionStateGroup.SumMember)
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
                    return DescriptionGenerator.UpdateBias(Network, DesiredOutput);
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
                return DescriptionGenerator.UpdateWeight(Network, DesiredOutput, State.Index);
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
                    return DescriptionGenerator.UpdateWeight(Network, DesiredOutput, State.Index);
                }
                else
                {
                    State.Group = ExecutionStateGroup.Normal;
                    Sum = null;
                }
            }
            return "";*/
        }
    }
}
