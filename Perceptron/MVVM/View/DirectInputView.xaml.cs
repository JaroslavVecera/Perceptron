using Perceptron.Core;
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
    /// Interakční logika pro DirectInputView.xaml
    /// </summary>
    public partial class DirectInputView : UserControl
    {
        public DirectInputView()
        {
            InitializeComponent();
        }

        private void test1_Changed(object sender, EventArgs e)
        {
            test1.Command?.Execute(Graph.ActualWidth);
            test2.Command?.Execute(Graph.ActualHeight);
        }
    }
}
