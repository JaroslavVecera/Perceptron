using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MNIST.IO;

namespace Perceptron.Network
{
    public struct Test
    {
        public float[] input;
        public int[] output;
    }

    public class TestSet
    {
        public List<Test> Tests { get; set; }
        public int Size { get { return Tests.Count();  } }
        public TestIterator GetIterator()
        {
            return new TestIterator(this);
        }

        public void LoadMnist(string path, bool train)
        {
            IEnumerable<TestCase> data;
            if (train)
                data = FileReaderMNIST.LoadImagesAndLables(Path.Join(path, "train-labels-idx1-ubyte.gz"), Path.Join(path, "train-images-idx3-ubyte.gz"));
            else
                data = FileReaderMNIST.LoadImagesAndLables(Path.Join(path, "t10k-labels-idx1-ubyte.gz"), Path.Join(path, "t10k-images-idx3-ubyte.gz"));
            Tests = data.ToList().Select((TestCase tc) =>
            {
                Test t = new Test() { input = FlattenArray(tc.Image) };
                t.output = MnistOutputConvertor.EncodePositional(tc.Label);
                return t;
            }).ToList();
        }

        public void LoadTestMnist(string path, bool train)
        {
            IEnumerable<TestCase> data;
            if (train)
                data = FileReaderMNIST.LoadImagesAndLables(Path.Join(path, "train-labels-idx1-ubyte.gz"), Path.Join(path, "train-images-idx3-ubyte.gz"));
            else
                data = FileReaderMNIST.LoadImagesAndLables(Path.Join(path, "t10k-labels-idx1-ubyte.gz"), Path.Join(path, "t10k-images-idx3-ubyte.gz"));
            Tests = data.Take(500).ToList().Select((TestCase tc) =>
            {
                Test t = new Test() { input = FlattenArray(tc.Image) };
                t.output = MnistOutputConvertor.EncodePositional(tc.Label);
                return t;
            }).ToList();
        }

        float[] FlattenArray(byte[,] matrix)
        {
            float[] arr = new float[matrix.Length];
            int width = matrix.GetLength(0);
            int height = matrix.GetLength(1);
            int k = 0;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    arr[k++] = 255 - matrix[i, j];
                }
            }
            return arr;
        }
    }

    public class TestIterator
    {
        TestSet _testSet;
        int pos = 0;

        public TestIterator(TestSet testSet)
        {
            _testSet = testSet;
        }

        public Test GetNext()
        {
            return _testSet.Tests[pos++];
        }

        public bool IsEnd()
        {
            return pos >= _testSet.Size;
        }
    }
}
