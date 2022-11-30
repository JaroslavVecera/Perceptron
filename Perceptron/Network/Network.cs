using Perceptron.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.Network
{
    public class Network
    {
        InputLayer _input;
        bool _stopLearning = false;
        Random Random { get; } = new Random();
        public float LearningCoeficient { get; set; }
        public float[] _biases;
        public float[] Biases
        {
            get { return _biases; }
            set
            {
                AssertStopped();
                _biases = value;
            }
        }
        public InputLayer InputLayer 
        {
        get { return _input; }
            set
            {
                AssertStopped();
                _input = value;
            }
        }
        int _neurons;
        bool _running = false;
        float[,] _weights;
        public float[,] Weights
        {
            get { return _weights; }
            set
            {
                AssertStopped();
                _weights = value;
            }
        }
        public int Neurons
        {
            get { return _neurons; }
            set
            {
                AssertStopped();
                if (value < 1)
                    throw new ArgumentOutOfRangeException();
                _neurons = value;
            }
        }
        int[] _output;
        public int[] Output { get { return _output; } }

        public Network()
        {

        }

        public void StopLearning()
        {
            _stopLearning = true;
        }

        public Network(int inputs, int neurons, float learningCoeficient)
        {
            InputLayer = new InputLayer(inputs);
            Neurons = neurons;
            Weights = new float[inputs, neurons];
            Biases = new float[neurons];
            LearningCoeficient = learningCoeficient;
            InitializeRandom();
        }

        public void Clear()
        {
            if (Biases == null || Weights == null)
                return;
            for (int i = 0; i < Neurons; i++)
            {
                Biases[i] = 0;
                for (int j = 0; j < InputLayer.Size; j++)
                {
                    Weights[j, i] = 0;
                }
            }
        }

        public void InitializeRandom()
        {
            if (Biases == null || Weights == null)
                return;
            for (int i = 0; i < Neurons; i++)
            {
                Biases[i] = GetNextRandom();
                for (int j = 0; j < InputLayer.Size; j++)
                {
                    Weights[j, i] = GetNextRandom();
                }
            }
        }

        public float GetNextRandom()
        {
            return (float)Math.Round((float)((Random.NextDouble() - 0.5) * 4), 6);
        }

        public bool Run()
        {
            if (_running)
                return true;
            _output = new int[Neurons];
            bool valid = ValidateProperties();
            if (valid)
                _running = true;
            return valid;
        }

        public void Stop()
        {
            _running = false;
        }

        public float GetWeightedSum(int neuron)
        {
            AssertRunning();
            if (Neurons <= neuron || neuron < 0)
                throw new ArgumentOutOfRangeException();
            return GetWeightedSumUnsafe(neuron);
        }

        float GetWeightedSumUnsafe(int neuron)
        {
            float sum = 0;
            for (int i = 0; i < InputLayer.Size; i++)
                sum += InputLayer[i] * Weights[i, neuron];
            return sum;
        }

        public int CalculateOutput(int neuron)
        {
            AssertRunning();
            if (Neurons <= neuron || neuron < 0)
                throw new ArgumentException();
            return CalculateOutputUnsafe(neuron);
        }

        int CalculateOutputUnsafe(int neuron)
        {
            _output[neuron] = GetWeightedSum(neuron) > _biases[neuron] ? 1 : 0;
            return _output[neuron];
        }


        public void CalculateOutput()
        {
            AssertRunning();
            for (int i = 0; i < Neurons; i++)
                CalculateOutputUnsafe(i);
        }

        public void LearnNeuron(int neuron, int expected)
        {
            AssertRunning();
            if (Neurons <= neuron || neuron < 0)
                throw new ArgumentException();
            LearnNeuronUnsafe(neuron, expected);
        }

        bool LearnNeuronUnsafe(int neuron, int expected)
        {
            bool e = expected != Output[neuron];
            for (int i = 0; i < InputLayer.Size; i++)
                Weights[i, neuron] += LearningCoeficient * InputLayer[i] * (expected - Output[neuron]);
            Biases[neuron] += -LearningCoeficient * (expected - Output[neuron]);
            return e;
        }

        public void LearnNeurons(int[] expected)
        {
            AssertRunning();
            if (expected.Length != Neurons)
                throw new ArgumentException();
            LearnNeuronsUnsafe(expected);
        }

        bool LearnNeuronsUnsafe(int[] expected)
        {
            bool e = false;
            for (int i = 0; i < Neurons; i++)
                e = e || LearnNeuronUnsafe(i, expected[i]);
            return e;
        }

        public void LearnSetOnline(TestSet set)
        {
            Run();
            TrainWindowViewModel.GetInstance().Percent = 0;
            _stopLearning = false;
            int e = 0;
            int a = 0;
            do
            {
                e = 0;
                a = 0;
                set.Tests.ForEach(test =>
                {
                    InputLayer.InputArray = test.input;
                    CalculateOutput();
                    if (LearnNeuronsUnsafe(MnistOutputConvertor.EncodePositional(test.label, Neurons)))
                        e++;
                    a++;
                });
                TrainWindowViewModel.GetInstance().Percent = (int)(100 - Math.Ceiling(100 * ((double)e / (double)a)));
            } while (e != 0 && !_stopLearning);
            _stopLearning = false;
            Stop();
        }

        public void LearnSetBatch(TestSet set)
        {
            TrainWindowViewModel.GetInstance().Percent = 0;
            _stopLearning = false;
            int e = 0;
            int a = 0;
            float[] deltaBiases = new float[Neurons];
            float[,] deltaWeights = new float[InputLayer.Size, Neurons];
            do
            {
                Run();
                e = 0;
                a = 0;
                set.Tests.ForEach(test =>
                {
                    InputLayer.InputArray = test.input;
                    int[] expected = MnistOutputConvertor.EncodePositional(test.label);
                    CalculateOutput();
                    bool err = false;
                    for (int i = 0; i < Neurons; i++)
                    {
                        if (Output[i] != expected[i])
                            err = true;
                        for (int j = 0; j < InputLayer.Size; j++)
                            deltaWeights[j, i] += LearningCoeficient * InputLayer[j] * (expected[i] - Output[i]);
                        deltaBiases[i] += -LearningCoeficient * (expected[i] - Output[i]);
                    }
                    if (err)
                        e++;
                    a++;
                });
                Stop();
                for (int i = 0; i < Neurons; i++)
                    Biases[i] += deltaBiases[i];
                for (int i = 0; i < InputLayer.Size; i++)
                    for (int j = 0; j < Neurons; j++)
                        Weights[i, j] += deltaWeights[i, j];
                TrainWindowViewModel.GetInstance().Percent = (int)(100 - Math.Ceiling(100 * ((double)e / (double)a)));
            } while (e != 0 && !_stopLearning);
            _stopLearning = false;
        }

        bool ValidateProperties()
        {
            int m = InputLayer.Size;
            int n = Neurons;
            return Weights.GetLength(0) == m && Weights.GetLength(1) == n;
        }

        void AssertStopped()
        {
            if (_running)
                throw new InvalidOperationException();
        }

        void AssertRunning()
        {
            if (!_running)
                throw new InvalidOperationException();
        }
    }
}
