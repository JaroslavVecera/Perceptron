using Microsoft.Win32;
using Perceptron.Core;
using Perceptron.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Perceptron.MVVM.ViewModel
{
    class ImageInputViewModel : ObservableObject
    {
        BitmapSource _bitmapSource;
        public BitmapSource Image
        { 
            get { return _bitmapSource; }
            set
            {
                _bitmapSource = value;
                OnPropertyChanged();
            }
        }
        int ImageIndex { get; set; }

        public RelayCommand NextCommand { get; set; }
        public RelayCommand PictureCommand { get; set; }
        TestSet TestSet { get; set; }


        public ImageInputViewModel()
        {
            TestSet = new TestSet();
            TestSet.LoadTestMnist(@"C:\Users\Jarek\source\repos\Perceptron\data", true);
            Next();
            NextCommand = new RelayCommand(o =>
            {
                Next();
            });
            PictureCommand = new RelayCommand(o =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.DefaultExt = ".png";
                openFileDialog.Filter = "Images (.png)|*.png";
                openFileDialog.CheckFileExists = true;
                openFileDialog.CheckPathExists = true;
                if (openFileDialog.ShowDialog() == true)
                {
                    float[] arr = MnistLikeImageReader.ImageToArray(openFileDialog.FileName);
                    SetPictureArray(arr);
                }
            });
        }

        void Next()
        {
            SetPictureArray(TestSet.Tests[ImageIndex++].input);
        }

        void SetPictureArray(float[] array)
        {
            byte[] arr = array.Select(f => (byte)f).ToArray();
            Image = BitmapSource.Create(28, 28, 300, 300, PixelFormats.Indexed8, BitmapPalettes.Gray256, arr, 28);
        }
    }
}
