using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Perceptron.Network
{
    static class MnistLoader
    {
        static TestSet _trainSet = new TestSet();
        static TestSet _testSet = new TestSet();

        public static bool Load(int train)
        {
            try
            {
                _trainSet = new TestSet();
                _trainSet.LoadMnist(@"", true, train);
                return true;
            }
            catch(Exception e)
            {
                MessageBox.Show("Files train-images-idx3-ubyte.gz and train-labels-idx1-ubyte.gz are missing. Mnist dataset is unavailable.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static TestSet TakeTrainSet(int numbers, int maxCount)
        {
            return new TestSet() { Tests = _trainSet.Tests?.Where(t => t.label < numbers).Take(maxCount).ToList() };
        }

        public static TestSet TakeTestSet(int numbers, int maxCount)
        {
            return new TestSet() { Tests = _testSet.Tests?.Where(t => t.label < numbers).Take(maxCount).ToList() };
        }
    }
}
