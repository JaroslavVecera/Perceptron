using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Perceptron.Network
{
    public static class MnistLikeImageReader
    {
        static FileSystemWatcher Watcher { get; set; }
        static bool _watcherDisabled = true;
        public static event Action OnModifyImage;
        static byte[] GetPixels(BitmapSource source)
        {
            if (source.Format != PixelFormats.Bgra32)
                source = new FormatConvertedBitmap(source, PixelFormats.Bgra32, null, 0);

            int width = source.PixelWidth;
            int height = source.PixelHeight;
            if (width != 28 || height != 28)
                throw new InvalidOperationException();
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
                buffer[index] = (float)(pixels[index * 4] + pixels[index * 4 + 1] + pixels[index * 4 + 2]) / 3;
            return buffer;
        }

        public static float[] LoadImage(string path)
        {
                try
                {
                    float[] arr = ImageToArray(path);
                    RestartWatcher(Path.GetDirectoryName(path));
                    return arr;
                }
                catch(Exception e)
                {
                    MessageBox.Show("Bad image.", "Image error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
        }

        static void RestartWatcher(string path)
        {
            if (_watcherDisabled)
            {
                Watcher = new FileSystemWatcher();
                Watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Attributes | NotifyFilters.Size | NotifyFilters.CreationTime
                                     | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                Watcher.Path = path;
                Watcher.Changed += new FileSystemEventHandler(OnChanged);
                Watcher.EnableRaisingEvents = true;
                _watcherDisabled = false;
            }
            Watcher.Path = path;
        }

        static void OnChanged(object sender, FileSystemEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                OnModifyImage?.Invoke();
            });
        }
    }
}
