using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.Network
{
    public class NetworkExecutionService
    {
        public Network Network { get; }
        float? Sum { get; set; }
        int? Output { get; set; }
        public int DesiredOutput { get; set; } = 0;
        public bool Training { get; set; }

        public NetworkExecutionService(Network network)
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

        public void Step3()
        {
            Network.Run();
            CountSum();
            CountOutput();
            Network.Stop();
        }
    }
}
