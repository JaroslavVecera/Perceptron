using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.Network
{
    public class ConsoleTest
    {
        public Network Run(int num)
        {
            Network network = new Network(28 * 28, num, (float)0.5);
            if (!network.Run())
                Trace.WriteLine("fail");
            TestSet testSet = new TestSet();
            testSet.LoadTestMnist(@"C:\Users\Jarek\source\repos\Perceptron\data", true, num);
            Train(network, testSet.GetIterator());
            network.Stop();
            /*Test(network);
            while (true)
                OwnTest(network);*/
            return network;
        }
        void Train(Network network, TestIterator iterator)
        {
            int i = 0;
            int batch = 1;
            int all = 0;
            int good = 0;
            while (!iterator.IsEnd())
            {
                Test t = iterator.GetNext();
                network.InputLayer.InputArray = t.input;
                network.CalculateOutput();
                int iis = DecodeOutput(network.Output);
                int ishould = DecodeOutput(t.output);
                all++;
                if (iis == ishould)
                    good++;
                if (i % 500 == 0)
                {
                    Trace.WriteLine("batch " + batch + " learned, ERROR = " + (1 - ((float)good) / all));
                    batch++;
                    all = good = 0;
                }
                network.LearnNeurons(t.output);
                i++;
            }
        }

        void Test(Network network)
        {
            TestSet testSet = new TestSet();
            testSet.LoadMnist(@"C:\Users\Jarek\source\repos\Perceptron\data", false);
            TestIterator iterator = testSet.GetIterator();
            double good = 0;
            double all = 0;
            while (!iterator.IsEnd())
            {
                Test t = iterator.GetNext();
                network.InputLayer.InputArray = t.input;
                network.CalculateOutput();
                int iis = DecodeOutput(network.Output);
                int ishould = DecodeOutput(t.output);
                if (iis == ishould)
                    good++;
                all++;
            }
            Trace.WriteLine((good / all * 100) + "% accuracy");
        }

        void OwnTest(Network network)
        {
            network.InputLayer.InputArray = MnistLikeImageReader.ImageToArray(@"C:\Users\Jarek\source\repos\Perceptron\data\test.png");
            network.CalculateOutput();

            Trace.WriteLine("RECOGNIZED NUMBER IS: " + DecodeOutput(network.Output));
        }

        void PrintImage(float[] source)
        {
            for (int i = 0; i < 28; i++)
            {
                for (int j = 0; j < 28; j++)
                {
                    if (source[i * 28 + j] > 50)
                        Trace.Write(".");
                    else
                        Trace.Write("o");
                }
                Trace.Write("\n");
            }
        }

        int DecodeOutput(int[] arr)
        {
            return MnistOutputConvertor.DecodePositional(arr);
        }
    }
}
