using SereniaBLPLib;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TextureCoordsCalculatorGUI.ViewModels;
using SWF = Microsoft.Win32;

namespace TextureCoordsCalculatorGUI
{
    public sealed record Coords(float Left, float Top, float Right, float Bottom);

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow(MainViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();
        }
    }
}