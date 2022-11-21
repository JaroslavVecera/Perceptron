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
    class ImageInputBoxViewModel : PositionableViewModel
    {
        BitmapSource _bitmapSource;
        public event Action OnImageInputChanged;
        bool _mnist = true;
        public bool Mnist { get { return _mnist; } }
        public string Path { get; set; } = "";
        public event Action<float[]> OnChangeInput;
        float[] _array;
        public float[] Array { get { return _array; } }
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
        public RelayCommand PictureCommand { get; set; }
        public RelayCommand MnistCommand { get; set; }

        public TestSet TestSet { get; set; }

        public ImageInputBoxViewModel()
        {
            Test();
            InitializeCommands();
        }

        public ImageInputBoxViewModel(TestSet testSet, string path, float[] image)
        {
            Path = path;
            TestSet = testSet;
            SetPictureArray(image);
            InitializeCommands();
        }

        void Test()
        {
            TestSet = new TestSet();
            TestSet.LoadTestMnist(@"C:\Users\Jarek\source\repos\Perceptron\data", true, 3);
            SetPictureArray(TestSet.Tests[ImageIndex].input);
        }

        void InitializeCommands()
        { 
            PictureCommand = new RelayCommand(o =>
            {
                _mnist = false;
                LoadPicture();
                OnImageInputChanged?.Invoke();
            });
            MnistCommand = new RelayCommand(o =>
            {
                _mnist = true;
                SetPictureArray(TestSet.Tests[ImageIndex].input);
                OnImageInputChanged?.Invoke();
            });
        }

        public void ImageModified()
        {
            float[] arr = MnistLikeImageReader.LoadImage(Path);
            if (arr != null)
                SetPictureArray(arr);
        }

        public void NextImage()
        {
            if (_mnist)
            {
                if (ImageIndex == TestSet.Size)
                    ImageIndex = 0;
                SetPictureArray(TestSet.Tests[++ImageIndex].input);
            }
        }

        void LoadPicture()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".png";
            openFileDialog.Filter = "Images (.png)|*.png";
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            if (openFileDialog.ShowDialog() == true)
            {
                float[] arr = MnistLikeImageReader.LoadImage(openFileDialog.FileName);
                if (arr != null)
                {
                    Path = openFileDialog.FileName;
                    SetPictureArray(arr);
                }
            }
        }

        public void Notify()
        {
            OnChangeInput?.Invoke(_array);
        }

        void SetPictureArray(float[] array)
        {
            byte[] arr = array.Select(f => (byte)f).ToArray();
            Image = BitmapSource.Create(28, 28, 300, 300, PixelFormats.Indexed8, BitmapPalettes.Gray256, arr, 28);
            _array = array;
            Notify();
        }
    }
}
