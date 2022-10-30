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
        public int DesiredOutput { get; set; }
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

        public void Step3()
        {
            Network.Run();
            CountSum();
            CountOutput();
            Network.Stop();
        }
    }
}
