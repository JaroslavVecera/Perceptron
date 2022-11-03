using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Perceptron.MVVM.View
{
    /// <summary>
    /// Interakční logika pro NavigableTextBox.xaml
    /// </summary>
    public partial class NavigableTextBox : TextBox
    {

        public NavigableTextBox()
        {
            InitializeComponent();
        }

        private void PreviewKeyDownHandler(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    {
                        if (CaretIndex == 0)
                            ArrowCommand.Execute(ArrowType.Left);
                        break;
                    }
                case Key.Right:
                    {
                        if (CaretIndex == Text.Length)
                            ArrowCommand.Execute(ArrowType.Right);
                        break;
                    }
                case Key.Down:
                    {
                        ArrowCommand.Execute(ArrowType.Down);
                        break;
                    }
                case Key.Up:
                    {
                        ArrowCommand.Execute(ArrowType.Up);
                        break;
                    }
            }
        }
        public static readonly DependencyProperty ArrowCommandProperty =
            DependencyProperty.Register(
                name: "ArrowCommand",
                propertyType: typeof(ICommand),
                ownerType: typeof(NavigableTextBox),
                typeMetadata: new FrameworkPropertyMetadata(defaultValue: null));

        public ICommand ArrowCommand
        {
            get => (ICommand)GetValue(ArrowCommandProperty);
            set => SetValue(ArrowCommandProperty, value);
        }
        public bool Focused
        {
            get => (bool)GetValue(FocusedProperty);
            set => SetValue(FocusedProperty, value);
        }

        public static readonly DependencyProperty FocusedProperty =
            DependencyProperty.Register(
                name: "Focused",
                propertyType: typeof(bool),
                ownerType: typeof(NavigableTextBox),
                typeMetadata: new FrameworkPropertyMetadata(defaultValue: false, propertyChangedCallback: OnFocusedPropertyChanged));
        private static void OnFocusedPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        {
            NavigableTextBox b = (NavigableTextBox)d;
            if ((bool)e.NewValue)
                b.Focus();
        }
    }
}
