using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.Network
{
    static class MnistLoader
    {
        static TestSet _trainSet;
        static TestSet _testSet;

        public static void Load(int train, int test)
        {
            _trainSet = new TestSet();
            _trainSet.LoadMnist(@"", true, train);
            _testSet = new TestSet();
            _testSet.LoadMnist(@"", false, test);
        }

        public static TestSet TakeTrainSet(int numbers, int maxCount)
        {
            return new TestSet() { Tests = _trainSet.Tests.Where(t => t.label < numbers).Take(maxCount).ToList() };
        }

        public static TestSet TakeTestSet(int numbers, int maxCount)
        {
            return new TestSet() { Tests = _testSet.Tests.Where(t => t.label < numbers).Take(maxCount).ToList() };
        }
    }
}
