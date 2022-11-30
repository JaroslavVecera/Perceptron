using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.Network
{
    public static class MnistOutputConvertor
    {
        public static int[] EncodePositional(int i)
        {
            int[] res = new int[10];
            res[i] = 1;
            return res;
        }

        public static int[] EncodePositional(int i, int num)
        {
            int[] res = new int[Math.Max(num, 2)];
            res[i] = 1;
            return res;
        }

        public static int DecodePositional(int[] arr)
        {
            return arr.ToList().FindIndex(x => x == 1);
        }

        public static int[] EncodeBinary(int i)
        {
            int[] res = new int[4];
            for (int j = 0; j < 4; j++)
            {
                int x = i;
                i /= 2;
                if (i * 2 != x)
                    res[j] = 1;
            }
            return res;
        }

        public static int DecodeBinary(int[] arr)
        {
            int res = 0;
            int x = 1;
            for (int i = 0; i < 4; i++)
            {
                if (arr[i] == 1)
                    res += x;
                x *= 2;
            }
            if (res > 9 || res < 0)
                return -1;
            return res;
        }

        public static int[] EncodeIsOne(int i)
        {
            return new int[] { i };
        }

        public static int DecodeIsOne(int[] arr)
        {
            return arr[0];
        }

    }
}
