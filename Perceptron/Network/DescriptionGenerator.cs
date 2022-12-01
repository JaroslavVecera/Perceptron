using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.Network
{
    public static class DescriptionGenerator
    {
        public static string PartialSum(Network network, int index)
        {
            string res = @"\matrix{ \sum i_jw_j &= ";
            for (int i = 0; i < network.InputLayer.Size; i++)
            {
                res += i == 0 ? "" : "&{}+ ";
                res += i == index ? @"\underline{" : "";
                res += $"{network.InputLayer[i]}\\cdot{network.Weights[i, 0]}";
                res += i == index ? @"}" : "";
                res += i < network.InputLayer.Size - 1 ? " + \\\\" : "";
            }
            res += @"}\\ \text{partial sum: }";
            float ps = 0;
            for (int i = 0; i <= index; i++)
                ps += network.InputLayer[i] * network.Weights[i, 0];
            res += ps;
            return res;
        }

        public static string Sum(Network network)
        {
            string res = @"\matrix{\sum i_jw_j = ";
            float ps = 0;
            for (int i = 0; i < network.InputLayer.Size; i++)
                ps += network.InputLayer[i] * network.Weights[i, 0];
            res += ps + "\\\\}";
            return res;
        }

        public static string Output(Network network, float sum)
        {
            return "" + sum +
                   (network.Output[0] == 0 ? @"<" : @"\geq") +
                   network.Biases[0] +
                   @"\\" +
                   @"\text{Output: }" + network.Output[0];
        }

        public static string UpdateBias(Network network, float desiredOutput, int output)
        {
            string res = @"\text{bias}\;\;-\!=\;\;";
            res += network.LearningCoeficient + @"\cdot(";
            res += desiredOutput + "-" + output + ")";
            return res;
        }

        public static string UpdateWeight(Network network, float desiredOutput, int output, int index)
        {
            string res = @"\text{weight}_{" + index + @"}{\;\;+\!\!=\;\;}";
            res += network.LearningCoeficient + @"\cdot" + network.InputLayer[index] + @"\cdot(";
            res += desiredOutput + "-" + output + ")";
            return res;
        }
    }
}
