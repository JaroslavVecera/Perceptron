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
        public event Action<bool> OnImageInputChanged;
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

        public ImageInputBoxViewModel()
        {
            InitializeCommands();
        }

        public ImageInputBoxViewModel( float[] image)
        {
            SetPictureArray(image);
            InitializeCommands();
        }

        void InitializeCommands()
        { 
            PictureCommand = new RelayCommand(o =>
            {
                OnImageInputChanged?.Invoke(false);
            });
            MnistCommand = new RelayCommand(o =>
            {
                OnImageInputChanged?.Invoke(true);
            });
        }

        public void SetPictureArray(float[] array)
        {
            byte[] arr = array.Select(f => (byte)f).ToArray();
            Image = BitmapSource.Create(28, 28, 300, 300, PixelFormats.Indexed8, BitmapPalettes.Gray256, arr, 28);
            _array = array;
        }
    }
}
