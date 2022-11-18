using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.Network
{
    class NetworkExecutionServiceImageInput
    {
        public List<int> DesiredOutput { set; get; } = new List<int>();
        public List<int?> Outputs { get; set; } = new List<int?>();
        ExecutionState State { get; set; } = new ExecutionState() { Group = ExecutionStateGroup.Normal };
        Network Network { get; set; }
        public bool Training { get; set; } = true;
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
            DesiredOutput.Add(0);

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
            DesiredOutput.RemoveAt(index);

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

        public string Step2()
        {
            string res = "";
            Network.Run();
            res = DoStep();
            while (State.Group != ExecutionStateGroup.Normal)
                res = DoStep();
            Network.Stop();
            return res;
        }

        string DoStep()
        {
            if (State.Group == ExecutionStateGroup.Normal || (State.Group == ExecutionStateGroup.Activation && !Training))
            {
                Network.CalculateOutput();
                Outputs = Network.Output.Select(o => (int?)o).ToList();
                if (Training)
                    State.Group = ExecutionStateGroup.Activation;
            }
            else
            {
                Network.CalculateOutput();
                Network.LearnNeurons(DesiredOutput.ToArray());
                State.Group = ExecutionStateGroup.Normal;
            }
            return null;
        }

        public void ResetProgress()
        {
            State.Group = ExecutionStateGroup.Normal;
        }
    }
}
