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
    class ImageInputBoxViewModel : PositionableViewModel
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
        public TestSet TestSet { get; set; }

        public ImageInputBoxViewModel()
        {
            Test();
            InitializeCommands();
        }

        public ImageInputBoxViewModel(TestSet testSet, BitmapSource image)
        {
            TestSet = testSet;
            Image = image;
            InitializeCommands();
        }

        void Test()
        {
            TestSet = new TestSet();
            TestSet.LoadTestMnist(@"C:\Users\Jarek\source\repos\Perceptron\data", true);
            Next();
        }

        void InitializeCommands()
        { 
            NextCommand = new RelayCommand(o =>
            {
                Next();
            });
            PictureCommand = new RelayCommand(o =>
            {
                LoadPicture();
            });
        }

        void Next()
        {
            if (ImageIndex == TestSet.Size)
                ImageIndex = 0;
            SetPictureArray(TestSet.Tests[ImageIndex++].input);
        }

        void LoadPicture()
        {
            float[] arr = MnistLikeImageReader.LoadImage();
            if (arr != null)
                SetPictureArray(arr);
        }

        void SetPictureArray(float[] array)
        {
            byte[] arr = array.Select(f => (byte)f).ToArray();
            Image = BitmapSource.Create(28, 28, 300, 300, PixelFormats.Indexed8, BitmapPalettes.Gray256, arr, 28);
        }
    }
}
