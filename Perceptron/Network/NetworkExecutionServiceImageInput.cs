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
        Network Network { get; set; }
        public NetworkExecutionServiceImageInput(Network network)
        {
            Network = network;
        }

        public int? GetOutput()
        {
            return null;
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
        }
    }
}
