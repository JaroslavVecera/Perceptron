using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Perceptron.Network
{
    public static class MnistLikeImageReader
    {
        static byte[] GetPixels(BitmapSource source)
        {
            if (source.Format != PixelFormats.Bgra32)
                source = new FormatConvertedBitmap(source, PixelFormats.Bgra32, null, 0);

            int width = source.PixelWidth;
            int height = source.PixelHeight;
            Int32 stride = (width * source.Format.BitsPerPixel + 7) / 8;
            byte[] result = new byte[width * height * 4];

            source.CopyPixels(result, stride, 0);
            return result;
        }

        public static float[] ImageToArray(string path)
        {
            BitmapImage bms = new BitmapImage();
            bms.BeginInit();
            bms.CacheOption = BitmapCacheOption.OnLoad;
            bms.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bms.UriSource = new Uri(path, UriKind.Absolute);
            bms.EndInit();
            byte[] pixels = GetPixels(bms);
            bms.Freeze();
            var buffer = new float[pixels.Length / 4];

            for (int index = 0; index < buffer.Length; index++)
                buffer[index] = 255 - (float)(pixels[index * 4] + pixels[index * 4 + 1] + pixels[index * 4 + 2]) / 3;
            return buffer;
        }
    }
}
